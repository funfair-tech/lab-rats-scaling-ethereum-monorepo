import { FFEngine } from '@funfair/engine';
import { Logic_Bet } from '../logic/logic_defines';
import { GLUI } from '../multiTraderUI';
import { UIPlayerData } from '../objects/uiPlayerList';

export class Player {
    constructor (public playerID: string) {}
}

/**
 * Manages the list of players in the game, and drives the UI to represent them
 */
export class PlayerManager extends FFEngine.Component {

    private players: Player[] = [];

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Player Manager';
        PLAYER_MANAGER = this;

    }

    /**
     * Update bets and players in the game
     */
    public UpdateBets(bets: Logic_Bet[]): void {

        let betsUpdated: boolean = false;
        let i: number = 0;
        for (i=0;i<bets.length;i++) {
            let bet = bets[i];
            let player = this.players[i];

            //destroy this player if its not the expected one
            if (player && player.playerID !== bet.address) {
                console.log('Player ID has changed. ' + player.playerID + ' but should be ' + bet.address);
                betsUpdated = true;
                this.DestroyPlayer(i);
            }

            //spawn a new player if required at this index
            if (!player) {
                console.log('Adding player: ' + bet.address)
                player = this.CreatePlayer(i, bet);
                betsUpdated = true;
            }

            //update player state on the UI if its changed
            let playerListUI = GLUI.GetPlayerList().GetPlayerByIndex(i);
            
            if (playerListUI) {
                let displayData = playerListUI.GetDisplayData();
                displayData.bet = bet.amount;
                displayData.win = bet.winnings;
                playerListUI.UpdateDisplay();
            }
        }

        //delete any remaining players that exist as they should not be there
        for (let j=i;j<this.players.length;j++) {
            if (this.players[j]) {
                this.DestroyPlayer(j);
                betsUpdated = true;
            }
        }

        if (betsUpdated === true) {
            //update UI
        }
    }

    private CreatePlayer(index: number, bet: Logic_Bet): Player {
        this.players[index] = new Player(bet.address);

        //add player to player list
        GLUI.GetPlayerList().SetPlayer(index, new UIPlayerData(bet.address, bet.name, bet.isLocalPlayer));

        return this.players[index];
    }

    private DestroyPlayer(index: number): void {
        delete this.players[index];

        //remove player from player list
        GLUI.GetPlayerList().RemovePlayer(index);
    }
}


/**
 * Global Singleton reference
 */
 let PLAYER_MANAGER!: PlayerManager;
 export { PLAYER_MANAGER };