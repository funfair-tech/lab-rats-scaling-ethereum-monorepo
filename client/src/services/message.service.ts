import * as signalR from '@microsoft/signalr';
import window from '@funfair-tech/wallet-sdk/window';
import { apiRequest } from './api-request.service';
import { Bet, SafeBet } from '../model/bet';
import { MessageId } from '../model/messageId';
import store from '../store/store';
import { addBet, setCanPlay, setPlayersOnline, setRound } from '../store/actions/game.actions';
import { Round } from '../model/round';

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

  private handleGameStarting = (roundId: string, transactionhash: string): void => {
    console.log(`GameRoundStarting: ${roundId} ${transactionhash}`);

  };

  private handleGameStarted = (
    gameRoundId: string,
    timeLeftInSeconds: number,
    startRoundBlockNumber: number,
    interRoundPause: number,
  ): void => {
    console.log(
      `GameRoundStarted: ${gameRoundId} ${timeLeftInSeconds} ${startRoundBlockNumber} ${interRoundPause}`,
    );
    const round: Round = {
      id: gameRoundId,
      block: startRoundBlockNumber,
      time: timeLeftInSeconds,
      timeToNextRound: interRoundPause
    }
    store.dispatch(setRound(round));
    store.dispatch(setCanPlay(true));
  };

  private handleBettingEnding = (): void => {
    console.log(
      `BettingEnding: `,
    );
    store.dispatch(setCanPlay(false));
  };

  private handleGameEnding = (gameId: string, transactionHash: string, entropyReveal: string) => {
    console.log(
      `GameEnding: GameId: ${gameId}  TransactionHash: ${transactionHash} EntropyReveal: ${entropyReveal}`,
    );
  };

  private handleGameEnded = (gameId: string, blockNumber: number, interGameDelay: number) => {
    console.log(`GameEnded: ${gameId} ${blockNumber} ${interGameDelay}`);
  };

  private handleBroadcast = (address: string, message: string): void => {
    console.log(`handleBroadcast: ${address} ${message}`);

    try {
      const decoded = JSON.parse(message);
      switch (decoded.action) {
        case MessageId.BET:
          const decodedBet = decoded as Bet;
          const bet: Bet = new SafeBet(decodedBet.roundId, decodedBet.address, decodedBet.amount, decodedBet.data, false);
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

  public async broadcastBet(bet: Bet): Promise<void> {
    const toSend = JSON.stringify({
      action: MessageId.BET,
      ...bet,
    });
    this.connection!.invoke('SendMessage', toSend).catch((err) => {
      return console.error(err.toString());
    });
  }
}

export const messageService = new MessageService();