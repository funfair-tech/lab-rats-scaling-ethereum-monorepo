import { FFEngine } from '@funfair/engine';
import { GridManager } from './gridManager';

/**
 * Manages the 3D game environment including world camera and gameplay objects
 */
export class EnvironmentManager extends FFEngine.Component {

    private camera!: FFEngine.THREE.PerspectiveCamera;
    private cameraInterpolator!: FFEngine.Interpolator;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Environment';
        ENVIRONMENT_MANAGER = this;

        //create world camera
        let pos = new FFEngine.THREE.Vector3(2, 0, 7);
        this.camera = FFEngine.instance.cameras['WORLD'] = FFEngine.instance.CreatePerspectiveCamera([pos.x, pos.y, pos.z]);
        this.camera.lookAt(new FFEngine.THREE.Vector3(0, 0, 0));
        this.container.add(this.camera);

        //camera interpolator
        this.cameraInterpolator = FFEngine.instance.AddComponent(this.camera, FFEngine.Interpolator);
    }

    public OnRendererResize(params: FFEngine.IRendererResizeParams): void {
        super.OnRendererResize(params);
        console.log('Game Resized to: ' + params.width + ',' + params.height);

        if (this.camera) {
            this.camera.aspect = params.width / params.height;
            this.camera.updateProjectionMatrix();
        }
    }

    public AssetLoadingFinished(): void {
        this.CreateScene();
    }

    private CreateScene(): void {

        //create components
        let grid = FFEngine.instance.CreateChildObjectWithComponent(this.container, GridManager);

        //create some dummy graph data
        grid.AddResult(0);
        grid.AddResult(1);
        grid.AddResult(-1);
        grid.AddResult(2);
        grid.AddResult(3);

        //test camera
        this.MoveCamera();
    }

    private MoveCamera(): void {
        
        let startPosition = new FFEngine.THREE.Vector3().copy(this.camera.position);
        let targetPosition = new FFEngine.THREE.Vector3().set(1, 0, 6);

        let controlPosition = new FFEngine.THREE.Vector3(
            startPosition.x + ((targetPosition.x - startPosition.x) / 2 ),
            startPosition.y + ((targetPosition.y - startPosition.y) / 2 ),
            startPosition.z + ((targetPosition.z - startPosition.z) / 2 ) );

        this.cameraInterpolator.CancelAll();
        this.cameraInterpolator.QueueCurvePositionChange(targetPosition, controlPosition, 4, 0);
    }
}

/**
 * Global Singleton reference
 */
 let ENVIRONMENT_MANAGER!: EnvironmentManager;
 export { ENVIRONMENT_MANAGER };
