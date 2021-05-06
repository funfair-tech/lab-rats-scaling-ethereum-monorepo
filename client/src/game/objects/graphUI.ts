import { FFEngine } from '@funfair/engine';
import { ASSETPACK, FontAssetType, TextureAssetType } from '../assetPack';
import { Logic_BetType } from '../logic/logic_defines';
import { MULTITRADER } from '../multiTrader';
import { ENVIRONMENT_MANAGER } from '../objectManagers/environmentManager';
import { Player } from '../objectManagers/playerManager';
import { ButtonSpriteStateConfig, UIButtonSprite } from './uiButtonSprite';

/**
 * Component which manages the UI which is overlaid on the graph in world space
 */
export class GraphUI extends FFEngine.Component {

    private priceLine!: FFEngine.Sprite;
    private betUI!: FFEngine.THREE.Object3D;
    private betHigh!: UIButtonSprite;
    private betLow!: UIButtonSprite;
    private betSmallHigh!: UIButtonSprite;
    private betBigHigh!: UIButtonSprite;
    private betSmallLow!: UIButtonSprite;
    private betBigLow!: UIButtonSprite;
    private playerBets: FFEngine.Sprite[] = [];
    private playerBetCounters: number[] = [];

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();

        //price line
        this.priceLine = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.priceLine.SetColor(new FFEngine.THREE.Color(0xccccff));
        this.priceLine.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        this.priceLine.SetSize(100, 0.05);
        this.priceLine.GetContainer().position.set(0, 0, 0.05);

        //betting UI
        this.betUI = new FFEngine.THREE.Object3D();
        this.container.add(this.betUI);

        this.betHigh = this.CreateButton(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGH), -1.5, 1.5, 2.9, 2.8);

        this.betHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.HIGHER);
        });

        let text = FFEngine.instance.CreateChildObjectWithComponent(this.betHigh.GetContainer(), FFEngine.BitmapString, { text: 'High', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.6, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        text.SetRenderOrder(1);

        this.betLow = this.CreateButton(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOW), -1.5, -1.5, 2.9, 2.8);
       
        this.betLow.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.LOWER);
        });

        text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.6, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        text.SetRenderOrder(1);

        this.betSmallHigh = this.CreateButton(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGHSMALL), 4.5, 1.5/2, 2.9, 1.48);

        this.betSmallHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.SMALLHIGHER);
        });

        //text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.5, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        //text.SetRenderOrder(1);

        this.betBigHigh = this.CreateButton(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGHSMALL), 4.5, 1.5 + (1.5/2), 2.9, 1.48);
        

        this.betBigHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.LARGEHIGHER);
        });

        this.betSmallLow = this.CreateButton(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOWSMALL), 4.5, -1.5/2, 2.9, 1.48);

        this.betSmallLow.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.SMALLLOWER);
        });

        //text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.5, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        //text.SetRenderOrder(1);

        this.betBigLow = this.CreateButton(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOWSMALL), 4.5, -1.5 - (1.5/2), 2.9, 1.48);

        this.betBigLow.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.LARGELOWER);
        });

        //text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.5, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        //text.SetRenderOrder(1);

        this.ShowBetButtons(false);
    }

    public SetVisibility(visible: boolean): void {
        this.container = visible;
    }

    public ShowBetButtons(visible: boolean): void {
        this.betUI.visible = visible
    }

    public LockBetButtons(locked: boolean): void {
        this.betHigh.SetLocked(locked);
        this.betLow.SetLocked(locked);
        this.betSmallHigh.SetLocked(locked);
        this.betSmallLow.SetLocked(locked);
        this.betBigHigh.SetLocked(locked);
        this.betBigLow.SetLocked(locked);
    }

    public UpdatePlayers(players: Player[]): void {

        //remove current bets
        for (let i=0;i<this.playerBets.length;i++) {
            FFEngine.instance.Destroy(this.playerBets[i].GetContainer());
        }
        this.playerBets = [];
        let playerBetCounters = [];

        //spawn new bets
        for (let i=0;i<players.length;i++) {
            if (players[i]) {
                let sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
                sprite.SetTexture(ASSETPACK.GetTextureAsset(TextureAssetType.PLAYERICON));
                sprite.SetSize(0.5, 0.5);
                sprite.SetColor(new FFEngine.THREE.Color(players[i].localPlayer === true ? 0xffffff : 0x808080));
                
                //position
                let pos = this.GetPlayerPositionForBet(players[i].betType);
                if (playerBetCounters[players[i].betType] === undefined) {
                    playerBetCounters[players[i].betType] = 0;
                }
                playerBetCounters[players[i].betType]++;

                pos.x -= playerBetCounters[players[i].betType] * 0.15;
                sprite.GetContainer().position.copy(pos);
                this.playerBets.push(sprite);
            }
        }
    }

    public SetPosition(position: FFEngine.THREE.Vector3): void {
        this.container.position.copy(position);
    }

    private GetPlayerPositionForBet(betType: Logic_BetType): FFEngine.THREE.Vector3 {
        let position = new FFEngine.THREE.Vector3();

        switch (betType) {
            case Logic_BetType.HIGHER:
                position.copy(this.betHigh.GetContainer().position);
                position.y -= 0.8;
            break;
            case Logic_BetType.LOWER:
                position.copy(this.betLow.GetContainer().position);
                position.y -= 0.8;
            break;
            case Logic_BetType.SMALLHIGHER:
                position.copy(this.betSmallHigh.GetContainer().position);
                position.y -= 0.4;
            break;
            case Logic_BetType.LARGEHIGHER:
                position.copy(this.betBigHigh.GetContainer().position);
                position.y -= 0.4;
            break;
            
            case Logic_BetType.SMALLLOWER:
                position.copy(this.betSmallLow.GetContainer().position);
                position.y -= 0.4;
            break;
            case Logic_BetType.LARGELOWER:
                position.copy(this.betBigLow.GetContainer().position);
                position.y -= 0.4;
            break;
        }
        position.x += 1.2;
        return position;
    }

    private CreateButton(texture: FFEngine.THREE.Texture, posX: number, posY: number, sizeX: number, sizeY: number): UIButtonSprite {
        let button = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        button.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(texture, new FFEngine.THREE.Color(0xffffff)));
        button.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(texture, new FFEngine.THREE.Color(0x888888)));
        button.SetCamera(ENVIRONMENT_MANAGER.GetCamera());
        button.SetupHighlight(texture);

        button.GetContainer().position.set(posX, posY, 0.05);
        button.GetSprite().SetSize(sizeX, sizeY);
        button.GetSprite().SetAlpha(0.5);
        button.GetHighlightSprite().SetSize(sizeX, sizeY);
        button.GetHighlightSprite().GetContainer().visible = false;
        return button;
    }
}
