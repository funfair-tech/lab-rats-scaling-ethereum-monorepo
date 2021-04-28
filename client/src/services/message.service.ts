import * as signalR from '@microsoft/signalr';
import window from '@funfair-tech/wallet-sdk/window';
import { apiRequest } from './api-request.service';
import { Bet, SafeBet } from '../model/bet';
import { MessageId } from '../model/messageId';
import store from '../store/store';
import { addBet, setCanPlay, setPlayersOnline, setRoundId } from '../store/actions/game.actions';

class MessageService {
  private connection: signalR.HubConnection | undefined;

  private async getAccessToken(): Promise<string> {
    return await window.funwallet.sdk.auth.app.jwt();
  }

  private sendInitMessage(networkName: string): void {
    this.connection!.invoke('Subscribe', networkName).catch((err) => {
      return console.error(err.toString());
    });
  }

  private handlePlayersOnline = (playerCount: number): void => {
    store.dispatch(setPlayersOnline(playerCount));
  };

  private handleGameStarting = (roundId: string, potId: string, transactionhash: string): void => {
    console.log(`GameRoundStarting: ${roundId} ${potId} ${transactionhash}`);

  };

  private handleGameStarted = (
    gameRoundId: string,
    progressivePotId: string,
    timeLeftInSeconds: number,
    startRoundBlockNumber: number,
    interRoundPause: number,
  ): void => {
    console.log(
      `GameRoundStarted: ${gameRoundId} ${progressivePotId} ${timeLeftInSeconds} ${startRoundBlockNumber} ${interRoundPause}`,
    );
    store.dispatch(setRoundId(gameRoundId));
    store.dispatch(setCanPlay(true));
  };

  private handleBettingEnding = (): void => {
    console.log(
      `BettingEnding: `,
    );
    store.dispatch(setCanPlay(false));
  };

  private handleGameEnding = (gameId: string, potId: string, transactionHash: string, entropyReveal: string) => {
    console.log(
      `GameEnding: GameId: ${gameId} PotId: ${potId} TransactionHash: ${transactionHash} EntropyReveal: ${entropyReveal}`,
    );
  };

  private handleGameEnded = (gameId: string, potId: string, blockNumber: number, interGameDelay: number) => {
    console.log(`GameEnded: ${gameId} ${potId} ${blockNumber} ${interGameDelay}`);
  };

  private handleBroadcast = (address: string, message: string): void => {
    try {
      const decoded = JSON.parse(message);
      switch (decoded.action) {
        case MessageId.BET:
          const bet: Bet = new SafeBet(decoded.roundID, address, decoded.betAmount, decoded.betData, false);
          store.dispatch(addBet(bet));
          break;
      }
    } catch (error) {
      console.error(`Error parsing broadcast message ${error}`);
    }
  };


  public async connectToServer(authenticate: boolean, networkName: string): Promise<void> {
    var options = {};
    const connectionUrl = apiRequest.buildEndpoint('hub/authenticated');

    if (authenticate) {
      options = {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
        accessTokenFactory: this.getAccessToken.bind(this),
      };
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(connectionUrl, options)
      .withAutomaticReconnect()
      .build();

    this.connection.on('PlayersOnline', this.handlePlayersOnline);
    this.connection.on('GameRoundStarting', this.handleGameStarting);
    this.connection.on('GameRoundStarted', this.handleGameStarted);
    this.connection.on('BettingEnding', this.handleBettingEnding);
    this.connection.on('GameRoundEnding', this.handleGameEnding);
    this.connection.on('GameRoundEnded', this.handleGameEnded);
    this.connection.on('NewMessage', this.handleBroadcast);

    this.connection.onclose(function () {
      console.log('signalr disconnected');
    });

    this.connection.onreconnecting((err) => {
      console.log('err reconnecting  ', err);
    });

    this.connection.onreconnected((connectionId) => {
      this.sendInitMessage(networkName);
    });

    this.connection
      .start()
      .then((res) => {
        if (authenticate) {
          this.sendInitMessage(networkName);
        }
      })
      .catch(console.error);
  }

  public async play(bet: Bet): Promise<void> {
    const toSend = JSON.stringify({
      action: MessageId.BET,
    });
    this.connection!.invoke('SendMessage', toSend).catch((err) => {
      return console.error(err.toString());
    });
  }
}

export const messageService = new MessageService();