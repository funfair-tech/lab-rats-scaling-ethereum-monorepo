import { FFEngine } from '@funfair/engine';
import { ASSETPACK, FontAssetType } from './assetPack';
import { Logic_BetType } from './logic/logic_defines';
import { MULTITRADER } from './multiTrader';
import { ButtonSpriteStateConfig, UIButtonSprite } from './objects/uiButtonSprite';
import { UIPlayerDisplayMode, UIPlayerList } from './objects/uiPlayerList';

/**
 * Main UI Scene component, containing the orthographic camera and webGL UI components
 */
export class MultiTraderUI extends FFEngine.Component {

    private scene!: FFEngine.THREE.Scene;
    private camera!: FFEngine.THREE.OrthographicCamera;

    private betUI!: FFEngine.THREE.Object3D;
    private betHigh!: UIButtonSprite;
    private betLow!: UIButtonSprite;
    private playerList!: UIPlayerList;
    private prizePoolText!: FFEngine.BitmapString;
    private winUI!: FFEngine.THREE.Object3D;
    private winText!: FFEngine.BitmapString;

    /**
     * Creates the WebGL UI Scene and main component and registers it with the engine
     */
    public static SetupScene(): void {
        let scene = FFEngine.instance.scenes['GLUI'] = new FFEngine.THREE.Scene();
        FFEngine.instance.CreateChildObjectWithComponent(scene, MultiTraderUI);
    }
    
    public Create(params: any): void {

        //Global Singleton
        GLUI = this;

        //main container
        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'WebGL UI';
        this.scene = FFEngine.instance.scenes['GLUI'];

        this.CreateCamera();
    }

    /**
     * WebGL UI is rendered after the rest of the world
     */
    public AfterRender(): void {
        super.AfterRender();

        if (this.scene) {
            FFEngine.instance.GetRenderer().render(this.scene, this.camera);
        }
    }

    public OnRendererResize(params: FFEngine.IRendererResizeParams): void {
        super.OnRendererResize(params);

        if (this.camera) {
            this.camera.left = -params.width/2;
            this.camera.right = params.width/2;
            this.camera.bottom = -params.height/2;
            this.camera.top = params.height/2;
            this.camera.updateProjectionMatrix();
        }

        //scale components if required

    }

    public SetPrizePool(value: number): void {
        if (this.prizePoolText) {
            this.prizePoolText.SetText('Prize Pool: ' + value.toString());
        }
    }

    /**
     * Show/hide betting UI
     */
    public ShowBetUI(visible: boolean): void {
        if (this.betUI) {
            this.betUI.visible = visible;
        }
    }

    /**
     * Show/hide betting UI
     */
     public ShowWinUI(visible: boolean, winnings: number): void {
        if (this.winUI) {
            this.winUI.visible = visible;

            if (visible) {
                if (winnings > 0) {
                    this.winText.SetText('You Win: ' + winnings);
                }
                else {
                    this.winText.SetText('Better Luck Next Time!');
                }
            }
        }
    }

    /**
     * Show/hide the player list
     */
    public SetPlayerList(visible: boolean): void {
        if (this.playerList) {
            this.playerList.GetContainer().visible = visible;
        }
    }

    public GetPlayerList(): UIPlayerList {
        return this.playerList;
    }

    public SetPlayerListMode(mode: UIPlayerDisplayMode): void {
        if (this.playerList) {
            this.playerList.SetDisplayMode(mode);

            if (mode === UIPlayerDisplayMode.BET) {
                this.playerList.SetRightTitle('Bet');
            }
            else {
                this.playerList.SetRightTitle('Win');
            }
        }
    }

    public AssetLoadingFinished(): void {
        this.CreateUIElements();
    }

    private CreateCamera(): void {
        this.camera = FFEngine.instance.CreateOrthographicCamera(1);
        this.camera.position.z = 10;
        this.camera.updateProjectionMatrix();
        this.container.add(this.camera);
    }

    private CreateUIElements(): void {

        //create title
        let anchor = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.UIAnchor);
        anchor.SetCamera(this.camera);
        anchor.SetAnchors(FFEngine.UIAnchorType.MIN, FFEngine.UIAnchorType.MAX);
        FFEngine.instance.CreateChildObjectWithComponent(anchor.GetContainer(), FFEngine.BitmapString, { text: 'RatTrace', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 40, justification: 'left', noMipMaps: false, colour: 0xFFFFFF, pos:[15, -80, 0]});

        //prize pool UI
        anchor = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.UIAnchor);
        anchor.SetCamera(this.camera);
        anchor.SetAnchors(FFEngine.UIAnchorType.CENTER, FFEngine.UIAnchorType.MIN);
        this.prizePoolText = FFEngine.instance.CreateChildObjectWithComponent(anchor.GetContainer(), FFEngine.BitmapString, { text: '', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 40, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 40, 0]});
        this.SetPrizePool(0);

        //create bet UI
        this.betUI = new FFEngine.THREE.Object3D();
        this.container.add(this.betUI);

        anchor = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, FFEngine.UIAnchor);
        anchor.SetCamera(this.camera);
        anchor.SetAnchors(FFEngine.UIAnchorType.CENTER, FFEngine.UIAnchorType.MAX);
        FFEngine.instance.CreateChildObjectWithComponent(anchor.GetContainer(), FFEngine.BitmapString, { text: 'Place your tokens', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 80, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, -160, 0]});

        this.betHigh = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betHigh.GetContainer().position.set(-140, 190, 0);
        this.betHigh.GetSprite().SetSize(200, 300);
        this.betHigh.GetSprite().SetAlpha(0.5);

        this.betHigh.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(undefined, new FFEngine.THREE.Color(0xff50ff)));
        this.betHigh.SetCamera(this.camera);

        this.betHigh.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.HIGHER);
        });

        FFEngine.instance.CreateChildObjectWithComponent(this.betHigh.GetContainer(), FFEngine.BitmapString, { text: 'High', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 40, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});

        this.betLow = FFEngine.instance.CreateChildObjectWithComponent(this.betUI, UIButtonSprite);
        this.betLow.GetContainer().position.set(-140, -190, 0);
        this.betLow.GetSprite().SetSize(200, 300);
        this.betLow.GetSprite().SetAlpha(0.5);

        this.betLow.SetupState(FFEngine.ButtonState.IDLE, new ButtonSpriteStateConfig(undefined, new FFEngine.THREE.Color(0xff50ff)));
        this.betLow.SetCamera(this.camera);

        this.betLow.SetOnClicked(() => {
            MULTITRADER.InitiatePlayerBet(Logic_BetType.LOWER);
        });

        FFEngine.instance.CreateChildObjectWithComponent(this.betLow.GetContainer(), FFEngine.BitmapString, { text: 'Low', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 40, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, 0, 0]});

        this.ShowBetUI(false);

        //create win UI
        this.winUI = new FFEngine.THREE.Object3D();
        this.container.add(this.winUI);

        anchor = FFEngine.instance.CreateChildObjectWithComponent(this.winUI, FFEngine.UIAnchor);
        anchor.SetCamera(this.camera);
        anchor.SetAnchors(FFEngine.UIAnchorType.CENTER, FFEngine.UIAnchorType.MAX);

        this.winText = FFEngine.instance.CreateChildObjectWithComponent(anchor.GetContainer(), FFEngine.BitmapString, { text: '', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 120, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, -260, 0]});
        this.ShowWinUI(false, 0);

        //create player list
        anchor = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.UIAnchor);
        anchor.SetCamera(this.camera);
        anchor.SetAnchors(FFEngine.UIAnchorType.MAX, FFEngine.UIAnchorType.MAX);
        this.playerList = FFEngine.instance.CreateChildObjectWithComponent(anchor.GetContainer(), UIPlayerList);
        this.playerList.SetWidth(400);
        this.playerList.GetContainer().position.set(-320, -180, 0);
        this.playerList.SetBackground(undefined, new FFEngine.THREE.Color(0xaaaaaa), 0.4);
        this.playerList.SetFont(ASSETPACK.GetFontAsset(FontAssetType.STANDARD));
        this.playerList.SetLeftTitle('Player');
        this.playerList.SetRightTitle('Win');
        this.playerList.SetDisplayMode(UIPlayerDisplayMode.WIN);
        this.SetPlayerList(true);
    }

}


/**
 * Global Singleton reference to the GL UI Class
 */
 let GLUI!: MultiTraderUI;
 export { GLUI };
