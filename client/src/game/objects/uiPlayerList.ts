import { FFEngine } from '@funfair/engine';
import { UIPlayerListRow } from './uiPlayerListRow';

export class UIPlayerData {

    public bet: number = 0;
    public win: number = 0;

    constructor (
        public playerID: string = '',
        public playerName: string = '',
        public localPlayer: boolean = false
    ) {}
}

export enum UIPlayerDisplayMode {
    SIMPLE,
    BET,
    WIN,
}

/**
 * Component which manages a List of UIPlayerListRow objects to form a list of players for display on the UI
 */
export class UIPlayerList extends FFEngine.Component {

    private static readonly TITLE_MARGIN_TOP = 34;
    private static readonly ROW_MARGIN_TOP = 80;
    private static readonly MARGIN_SIDE = 32;

    private width: number = 350;
    private maxRows: number = 30;
    private fontData!: any;
    private fontSize: number = 28;
    private background!: FFEngine.Sprite;
    private players: UIPlayerListRow[] = [];
    private titleLeftString!: FFEngine.BitmapString;
    private titleRightString!: FFEngine.BitmapString;
    private displayMode: UIPlayerDisplayMode = UIPlayerDisplayMode.SIMPLE;
    private hideZeroWin: boolean = false;

    public Create(params?: any): void {
        super.Create(params);
        this.container = new FFEngine.THREE.Object3D();
    }

    public SetWidth(width: number): number {
        return this.width = width;
    }

    public GetWidth(): number {
        return this.width;
    }

    public SetMaximumRows(max: number): void {
        this.maxRows = max;
        this.SortPlayers();
        this.UpdateList();
    }

    public SetFont(fontData: any, fontSize?: number): void {
        this.fontData = fontData;

        if (fontSize) {
            this.fontSize = fontSize;
        }

        for (let i=0;i<this.players.length;i++) {
            if (this.players[i]) {
                this.players[i].SetFont(this.fontData, this.fontSize);
            }
        }
    }

    /**
     * set the left-justified title for the player list
     */
    public SetLeftTitle(text: string): void {
        if (!this.titleLeftString) {
            this.titleLeftString = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.BitmapString, { text: text, font: this.fontData, size: this.fontSize, justification: 'left', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        }
        this.titleLeftString.SetText(text);
        this.UpdateList();
    }

    /**
     * set the right-justified title for the right hand column of the player list
     */
    public SetRightTitle(text: string): void {
        if (!this.titleRightString) {
            this.titleRightString = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.BitmapString, { text: text, font: this.fontData, size: this.fontSize, justification: 'right', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        }
        this.titleRightString.SetText(text);
        this.UpdateList();
    }

    /**
     * Sets the texture to create the background object for the player list
     */
    public SetBackground(texture?: FFEngine.THREE.Texture, colour?: FFEngine.THREE.Color, alpha?: number): void {

        if (this.background) {
            FFEngine.instance.Destroy(this.background.GetContainer());
        }

        this.background = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite, { map: texture });

        if (colour) {
            this.background.SetColor(colour);
        }

        if (alpha) {
            this.background.SetAlpha(alpha);
        }

        this.UpdateList();
    }

    public SetDisplayMode(mode: UIPlayerDisplayMode): void {
        this.displayMode = mode;
        for (let i=0;i<this.players.length;i++) {
            if (this.players[i]) {
                this.players[i].SetDisplayMode(mode);
            }
        }
        this.UpdateList();
    }

    public SetPlayer(index: number, playerData: UIPlayerData): void {

        this.RemovePlayer(index);

        let row = FFEngine.instance.CreateChildObjectWithComponent(this.container, UIPlayerListRow);
        if (this.fontData) {
            row.SetFont(this.fontData, this.fontSize);
            row.SetPlayerData(playerData);
            row.SetDisplayMode(this.displayMode);
            row.SetWidth(this.width - (UIPlayerList.MARGIN_SIDE * 2));
        }
        this.players[index] = row;
        this.SortPlayers();
        this.UpdateList();
    }

    public RemovePlayer(index: number): void {
        if (this.players[index]) {
            FFEngine.instance.Destroy(this.players[index].GetContainer());
            delete this.players[index];
            this.SortPlayers();
            this.UpdateList();
        }
    }

    public ClearAllPlayers(): void {
        for (let i=0;i<this.players.length;i++) {
            if (this.players[i]) {
                FFEngine.instance.Destroy(this.players[i].GetContainer());
            }
        }
        this.players = [];
        this.UpdateList();
    }

    public GetPlayerByIndex(index: number): UIPlayerListRow | undefined {
        return (this.players[index] ? this.players[index] : undefined);
    }

    public GetPlayerBySortedIndex(index: number): UIPlayerListRow | undefined {
        for (let i=0;i<this.players.length;i++) {
            if (this.players[i] && this.players[i].GetSortedIndex() === index) {
                return this.players[i];
            }
        }
        return undefined;
    }

    public GetLocalPlayer(): UIPlayerListRow | undefined {
        for (let i=0;i<this.players.length;i++) {
            if (this.players[i] && this.players[i].GetDisplayData().localPlayer === true) {
                return this.players[i];
            }
        }
        return undefined;
    }

    public GetPlayers(): UIPlayerListRow[] {
        return this.players;
    }

    /**
     * Sorts player rows by win amount decending
     */
    public SortPlayers(): void {
        let sorted: boolean[] = [];
        let currentRow: number = 0;
        let val = -1;
        let index = -1;
        let playerData: UIPlayerData;

        for (let i=0;i<this.players.length;i++) {
            val = -1;
            index = -1;
            for (let j=0;j<this.players.length;j++) {
                if (!sorted[j] && this.players[j]) {
                    playerData = this.players[j].GetDisplayData();
                    if (playerData.win > val) {
                        val = playerData.win;
                        index = j;
                    }
                }
            }

            if (index >= 0) {
                this.SetRowToListPosition(this.players[index], currentRow);
                sorted[index] = true;
            }
            currentRow++;
        }

        this.UpdateList();
    }

    public GetRowHeight(): number {
        return (this.fontSize * 1.1);
    }

    private UpdateList(): void {
        if (this.background) {
            let height = this.GetVisibleRowsHeight() + (UIPlayerList.ROW_MARGIN_TOP);
            this.background.SetSize(this.width, height);
            this.background.GetContainer().position.y = -height/2;
        }

        if (this.titleLeftString) {
            this.titleLeftString.GetContainer().position.set(-(this.width/2) + UIPlayerList.MARGIN_SIDE, -UIPlayerList.TITLE_MARGIN_TOP, 0);
        }

        if (this.titleRightString) {
            this.titleRightString.GetContainer().position.set((this.width/2) -UIPlayerList.MARGIN_SIDE, -UIPlayerList.TITLE_MARGIN_TOP, 0);
        }
    }

    private SetRowToListPosition(row: UIPlayerListRow, rowIndex: number): void {

        //check visibility first
        if ((this.displayMode === UIPlayerDisplayMode.WIN && this.hideZeroWin === true && row.GetDisplayData().win === 0) || rowIndex >= this.maxRows) {
            row.GetContainer().visible = false;
        }
        else {
            row.GetContainer().visible = true;
            row.GetContainer().position.set(-(this.width/2) + UIPlayerList.MARGIN_SIDE, -UIPlayerList.ROW_MARGIN_TOP - (this.GetRowHeight() * rowIndex), 0);
        }

        //set sorted index
        row.SetSortedIndex(rowIndex);
    }

    private GetVisibleRowsHeight(): number {
        let numRows = 0;
        for (let i=0;i<this.players.length;i++) {
            if (numRows < this.maxRows && this.players[i] && this.players[i].GetContainer().visible === true) {
                numRows++;
            }
        }

        return (numRows * this.GetRowHeight());
    }
}
