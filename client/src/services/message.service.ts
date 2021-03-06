import * as signalR from '@microsoft/signalr';
import window from '@funfair-tech/wallet-sdk/window';
import { apiRequest } from './api-request.service';
import store from '../store/store';
import { setHistory, setPlayersOnline } from '../store/actions/game.actions';

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

  private handleHistory = (history: string[]): void => {
    if(history.length > 0){
      store.dispatch(setHistory(history[0]));
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
    this.connection.on('History', this.handleHistory);

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
}

export const messageService = new MessageService();