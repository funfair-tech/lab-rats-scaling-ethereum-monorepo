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

        this.betHigh = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betHigh.GetContainer().position.set(-1.5, 1.5, 0.05);
        this.betHigh.GetSprite().SetSize(2.9, 2.8);
        this.betHigh.GetSprite().SetAlpha(0.5);

        this.betHigh.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGH), new FFEngine.THREE.Color(0xffffff)));
        this.betHigh.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGH), new FFEngine.THREE.Color(0x888888)));
        this.betHigh.SetCamera(ENVIRONMENT_MANAGER.GetCamera());

        this.betHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.HIGHER);
        });

        let text = FFEngine.instance.CreateChildObjectWithComponent(this.betHigh.GetContainer(), FFEngine.BitmapString, { text: 'High', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 1, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        text.SetRenderOrder(1);

        this.betLow = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betLow.GetContainer().position.set(-1.5, -1.5, 0.05);
        this.betLow.GetSprite().SetSize(2.9, 2.8);
        this.betLow.GetSprite().SetAlpha(0.5);

        this.betLow.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOW), new FFEngine.THREE.Color(0xffffff)));
        this.betLow.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOW), new FFEngine.THREE.Color(0x888888)));
        this.betLow.SetCamera(ENVIRONMENT_MANAGER.GetCamera());

        this.betLow.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.LOWER);
        });

        text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 1, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        text.SetRenderOrder(1);

        this.betSmallHigh = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betSmallHigh.GetContainer().position.set(4.5, 1.5/2, 0.05);
        this.betSmallHigh.GetSprite().SetSize(2.9, 1.48);
        this.betSmallHigh.GetSprite().SetAlpha(0.5);

        this.betSmallHigh.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGHSMALL), new FFEngine.THREE.Color(0xffffff)));
        this.betSmallHigh.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGHSMALL), new FFEngine.THREE.Color(0x888888)));
        this.betSmallHigh.SetCamera(ENVIRONMENT_MANAGER.GetCamera());

        this.betSmallHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.SMALLHIGHER);
        });

        //text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.5, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        //text.SetRenderOrder(1);

        this.betBigHigh = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betBigHigh.GetContainer().position.set(4.5, 1.5 + (1.5/2), 0.05);
        this.betBigHigh.GetSprite().SetSize(2.9, 1.48);
        this.betBigHigh.GetSprite().SetAlpha(0.5);

        this.betBigHigh.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGHSMALL), new FFEngine.THREE.Color(0xffffff)));
        this.betBigHigh.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETHIGHSMALL), new FFEngine.THREE.Color(0x888888)));
        this.betBigHigh.SetCamera(ENVIRONMENT_MANAGER.GetCamera());

        this.betBigHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.LARGEHIGHER);
        });

        this.betSmallLow = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betSmallLow.GetContainer().position.set(4.5, -1.5/2, 0.05);
        this.betSmallLow.GetSprite().SetSize(2.9, 1.48);
        this.betSmallLow.GetSprite().SetAlpha(0.5);

        this.betSmallLow.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOWSMALL), new FFEngine.THREE.Color(0xffffff)));
        this.betSmallLow.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOWSMALL), new FFEngine.THREE.Color(0x888888)));
        this.betSmallLow.SetCamera(ENVIRONMENT_MANAGER.GetCamera());

        this.betSmallLow.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.SMALLLOWER);
        });

        //text = FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 0.5, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});
        //text.SetRenderOrder(1);

        this.betBigLow = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betBigLow.GetContainer().position.set(4.5, -1.5 - (1.5/2), 0.05);
        this.betBigLow.GetSprite().SetSize(2.9, 1.48);
        this.betBigLow.GetSprite().SetAlpha(0.5);

        this.betBigLow.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOWSMALL), new FFEngine.THREE.Color(0xffffff)));
        this.betBigLow.SetupState(FFEngine.ButtonState.LOCKED, new ButtonSpriteStateConfig(ASSETPACK.GetTextureAsset(TextureAssetType.BUTTONBETLOWSMALL), new FFEngine.THREE.Color(0x888888)));
        this.betBigLow.SetCamera(ENVIRONMENT_MANAGER.GetCamera());

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

        //spawn new bets
        for (let i=0;i<players.length;i++) {
            if (players[i]) {
                let sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
                sprite.SetTexture(ASSETPACK.GetTextureAsset(TextureAssetType.PLAYERICON));
                sprite.SetSize(0.5, 0.5);
                sprite.GetContainer().position.copy(this.GetPlayerPositionForBet(players[i].betType));
                sprite.SetColor(new FFEngine.THREE.Color(players[i].localPlayer === true ? 0xffffff : 0x808080));
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

        return position;
    }
}
