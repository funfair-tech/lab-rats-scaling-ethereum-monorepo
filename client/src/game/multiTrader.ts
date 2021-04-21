import { FFEngine } from '@funfair/engine';

/**
 * Main game scene for the multiplayer trader game
 */
export class MultiTrader extends FFEngine.Component {

    private sprite: any;
    private camera: any = null;

    public Create(params: any): void {
        //create scene container
        this.container = new FFEngine.THREE.Object3D();

        //create world camera
        let pos = new FFEngine.THREE.Vector3(0, 0, 7);
        this.camera = FFEngine.instance.cameras['WORLD'] = FFEngine.instance.CreatePerspectiveCamera([pos.x, pos.y, pos.z]);

        //set initial properties
        this.camera.lookAt(new FFEngine.THREE.Vector3(0, 0, 0));
        this.container.add(this.camera);

        this.sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.sprite.SetSize(5, 5);
    }

    public OnRendererResize(params: FFEngine.IRendererResizeParams): void {
        super.OnRendererResize(params);

        console.log('Game Resized to: ' + params.width + ',' + params.height);
    }

}
