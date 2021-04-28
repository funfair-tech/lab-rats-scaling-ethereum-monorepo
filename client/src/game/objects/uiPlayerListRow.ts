import { FFEngine } from '@funfair/engine';
import { UIPlayerData, UIPlayerDisplayMode } from './uiPlayerList';

/**
 * Component which manages single row in a UIPlayerList object
 */
export class UIPlayerListRow extends FFEngine.Component {

    private static readonly NAME_LENGTH_MAX: number = 22;
    private static readonly DEFAULT_COLOUR: number = 0xFFFFFF;
    private static readonly LOCAL_PLAYER_COLOUR: number = 0xFFCC88;
    
    private playerData!: UIPlayerData;
    private nameString?: FFEngine.BitmapString;
    private betString!: FFEngine.BitmapString;
    private winString!: FFEngine.BitmapString;
    private displayMode: UIPlayerDisplayMode = UIPlayerDisplayMode.BET;
    private sortedIndex: number = -1;

    public Create(params?: any): void {
        super.Create(params);
        this.container = new FFEngine.THREE.Object3D();
    }

    public SetFont(fontData: any, fontSize: number): void {
        this.Destroy();
        this.nameString = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.BitmapString, { text: '', font: fontData, size: fontSize, justification: 'left', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        this.betString = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.BitmapString, { text: '', font: fontData, size: fontSize, justification: 'right', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        this.winString = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.BitmapString, { text: '', font: fontData, size: fontSize, justification: 'right', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        this.UpdateDisplay();
    }

    public SetWidth(width: number): void {
        if (this.betString) {
            this.betString.GetContainer().position.x = width;
        }

        if (this.winString) {
            this.winString.GetContainer().position.x = width;
        }
    }

    public SetPlayerData(playerData: UIPlayerData): void {
        this.playerData = playerData;
        this.UpdateDisplay();
    }

    public GetDisplayData(): UIPlayerData {
        return this.playerData;
    }

    public SetDisplayMode(mode: UIPlayerDisplayMode): void {
        this.displayMode = mode;
        this.UpdateDisplay();
    }

    public UpdateDisplay(): void {
        if (this.playerData) {
            if (this.nameString) {
                let name = this.playerData.playerName;
                if (name.length > UIPlayerListRow.NAME_LENGTH_MAX) {
                    name = name.slice(0, UIPlayerListRow.NAME_LENGTH_MAX);
                    name += '...';
                }
                this.nameString.SetText(name);
                this.nameString.SetColour(this.GetTextColour());
            }

            if (this.betString) {
                this.betString.SetText(this.playerData.bet.toString());
                this.betString.SetVisible(this.displayMode === UIPlayerDisplayMode.BET);
                this.betString.SetColour(this.GetTextColour());
            }

            if (this.winString) {
                this.winString.SetText(this.playerData.win.toString());
                this.winString.SetVisible(this.displayMode === UIPlayerDisplayMode.WIN);
                this.winString.SetColour(this.GetTextColour());
            }
        }
    }

    public SetSortedIndex(index: number): void {
        this.sortedIndex = index;
    }

    public GetSortedIndex(): number {
        return this.sortedIndex;
    }

    private Destroy(): void {
        if (this.nameString) {
            FFEngine.instance.Destroy(this.nameString.GetContainer());
        }

        if (this.betString) {
            FFEngine.instance.Destroy(this.betString.GetContainer());
        }

        if (this.winString) {
            FFEngine.instance.Destroy(this.winString.GetContainer());
        }
    }

    private GetTextColour(): FFEngine.THREE.Color {
        return new FFEngine.THREE.Color(this.playerData.localPlayer === true ? UIPlayerListRow.LOCAL_PLAYER_COLOUR : UIPlayerListRow.DEFAULT_COLOUR);
    }
}
