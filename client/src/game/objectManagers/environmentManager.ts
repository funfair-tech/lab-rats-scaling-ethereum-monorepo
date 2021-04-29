import { FFEngine } from '@funfair/engine';
import { GraphManager, GRAPH_MANAGER } from './graphManager';

/**
 * Manages the 3D game environment including world camera and gameplay objects
 */
export class EnvironmentManager extends FFEngine.Component {

    private static readonly CAM_X_OFFSET: number = 2;
    private static readonly CAM_Z: number = 7;

    private camera!: FFEngine.THREE.PerspectiveCamera;
    private cameraInterpolator!: FFEngine.Interpolator;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Environment';
        ENVIRONMENT_MANAGER = this;

        //create world camera
        let pos = new FFEngine.THREE.Vector3(EnvironmentManager.CAM_X_OFFSET, 0, EnvironmentManager.CAM_Z);
        this.camera = FFEngine.instance.cameras['WORLD'] = FFEngine.instance.CreatePerspectiveCamera([pos.x, pos.y, pos.z]);
        this.camera.lookAt(new FFEngine.THREE.Vector3(0, 0, 0));
        this.container.add(this.camera);

        //camera interpolator
        this.cameraInterpolator = FFEngine.instance.AddComponent(this.camera, FFEngine.Interpolator);

        //add world fog
        let worldScene = FFEngine.instance.scenes['WORLD'];
        worldScene.fog = new FFEngine.THREE.Fog(0x000000, 5, 16);
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

    /**
     * Moves the camera so the provided graph coordinate is centered on the screen
     */
    public SetCameraToGraphCoordinate(coord: FFEngine.THREE.Vector2): void {
        let camPos = GRAPH_MANAGER.GridToWorld(coord);
        camPos.x += EnvironmentManager.CAM_X_OFFSET;
        camPos.z = EnvironmentManager.CAM_Z;
        this.MoveCamera(camPos);
    }

    public GetCamera(): FFEngine.THREE.PerspectiveCamera {
        return this.camera;
    }

    private CreateScene(): void {
        //create components
        FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphManager);
    }

    private MoveCamera(targetPosition: FFEngine.THREE.Vector3): void {
        
        let startPosition = new FFEngine.THREE.Vector3().copy(this.camera.position);

        let controlPosition = new FFEngine.THREE.Vector3(
            startPosition.x + ((targetPosition.x - startPosition.x) / 2 ),
            startPosition.y + ((targetPosition.y - startPosition.y) / 2 ),
            startPosition.z + ((targetPosition.z - startPosition.z) / 2 ) );

        this.cameraInterpolator.CancelAll();
        this.cameraInterpolator.QueueCurvePositionChange(targetPosition, controlPosition, 2, 0);
    }
}

/**
 * Global Singleton reference
 */
 let ENVIRONMENT_MANAGER!: EnvironmentManager;
 export { ENVIRONMENT_MANAGER };
