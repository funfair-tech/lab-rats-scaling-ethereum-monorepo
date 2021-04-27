import { FFEngine } from '@funfair/engine';
import { ASSETPACK, FontAssetType } from './assetPack';

/**
 * Main UI Scene component, containing the orthographic camera and webGL UI components
 */
export class MultiTraderUI extends FFEngine.Component {

    private scene: FFEngine.THREE.Scene = null;
    private camera: FFEngine.THREE.OrthographicCamera = null;

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
        let anchor = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.UIAnchor);
        anchor.SetCamera(this.camera);
        anchor.SetAnchors(FFEngine.UIAnchorType.CENTER, FFEngine.UIAnchorType.MAX);
        FFEngine.instance.CreateChildObjectWithComponent(anchor.GetContainer(), FFEngine.BitmapString, { text: 'RatTrace', font: ASSETPACK.GetFontAsset(FontAssetType.STANDARD), size: 40, justification: 'center', noMipMaps: false, colour: 0xFFFFFF, pos:[0, -80, 0]});
    }

}


/**
 * Global Singleton reference to the GL UI Class
 */
 let GLUI!: MultiTraderUI;
 export { GLUI };
