import { FFEngine } from '@funfair/engine';

/**
 * Manages the 3D game environment including world camera and gameplay objects
 */
export class EnvironmentManager extends FFEngine.Component {

    private camera!: FFEngine.THREE.PerspectiveCamera;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Environment';
        ENVIRONMENT_MANAGER = this;

        //create world camera
        let pos = new FFEngine.THREE.Vector3(0, 0, 7);
        this.camera = FFEngine.instance.cameras['WORLD'] = FFEngine.instance.CreatePerspectiveCamera([pos.x, pos.y, pos.z]);

        //set initial properties
        this.camera.lookAt(new FFEngine.THREE.Vector3(0, 0, 0));
        this.container.add(this.camera);
    }

    public OnRendererResize(params: FFEngine.IRendererResizeParams): void {
        super.OnRendererResize(params);
        console.log('Game Resized to: ' + params.width + ',' + params.height);
    }

    public AssetLoadingFinished(): void {
        this.CreateScene();
    }

    private CreateScene(): void {
        //test
        let sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        sprite.SetSize(5, 5);
    }
}

/**
 * Global Singleton reference
 */
 let ENVIRONMENT_MANAGER!: EnvironmentManager;
 export { ENVIRONMENT_MANAGER };
