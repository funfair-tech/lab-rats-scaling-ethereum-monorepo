import { FFEngine } from '@funfair/engine';
import { GraphManager, GRAPH_MANAGER } from './graphManager';
import { PlayerManager } from './playerManager';

/**
 * Manages the 3D game environment including world camera and gameplay objects
 */
export class EnvironmentManager extends FFEngine.Component {

    private static readonly CAM_X_OFFSET: number = 2;
    private static readonly CAM_Z: number = 7;

    private camera!: FFEngine.THREE.PerspectiveCamera;
    private cameraInterpolator!: FFEngine.Interpolator;
    private cameraZoomLerpTimer: number = 0;
    private cameraZoomLerpTime: number = 0;
    private cameraZoomLerpStart: number = 0;
    private cameraZoomLerpEnd: number = 0;

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
        worldScene.fog = new FFEngine.THREE.Fog(0x220144, 8, 16);

        //add world ambient light
        this.container.add(new FFEngine.THREE.AmbientLight(0x606060));
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

    public Update(): void {

        //update camera lerp
        if (this.cameraZoomLerpTimer > 0) {
            this.cameraZoomLerpTimer -= FFEngine.instance.GetDeltaTime();

            if (this.cameraZoomLerpTimer < 0) {
                this.cameraZoomLerpTimer = 0;
            }

            this.camera.zoom = this.cameraZoomLerpStart + (((this.cameraZoomLerpTime - this.cameraZoomLerpTimer) / this.cameraZoomLerpTime) * (this.cameraZoomLerpEnd - this.cameraZoomLerpStart));
            this.camera.updateProjectionMatrix();
        }
    }

    /**
     * Moves the camera so the provided graph coordinate is centered on the screen
     */
    public SetCameraToGraphCoordinate(coord: FFEngine.THREE.Vector2, instant: boolean): void {
        let camPos = GRAPH_MANAGER.GridToWorld(coord);
        camPos.x += EnvironmentManager.CAM_X_OFFSET + 2.5;
        camPos.z = EnvironmentManager.CAM_Z;
        this.MoveCamera(camPos, instant);
    }

    public GetCamera(): FFEngine.THREE.PerspectiveCamera {
        return this.camera;
    }

    private CreateScene(): void {
        //create components
        FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphManager);
        FFEngine.instance.CreateChildObjectWithComponent(this.container, PlayerManager);
    }

    private MoveCamera(targetPosition: FFEngine.THREE.Vector3, instant: boolean): void {
        
        this.cameraInterpolator.CancelAll();

        if (instant) {
            this.camera.position.copy(targetPosition);
        }
        else {
            let startPosition = new FFEngine.THREE.Vector3().copy(this.camera.position);

            let controlPosition = new FFEngine.THREE.Vector3(
                startPosition.x + ((targetPosition.x - startPosition.x) / 2 ),
                startPosition.y + ((targetPosition.y - startPosition.y) / 2 ),
                startPosition.z + ((targetPosition.z - startPosition.z) / 2 ) );

            
            this.cameraInterpolator.QueueCurvePositionChange(targetPosition, controlPosition, 2, 0);
            this.StartZoomLerp(1.4, 1.5);

            FFEngine.instance.DelayedCallback(2.3, () => {
                this.StartZoomLerp(1, 1.5);
            });
        }
    }

    private StartZoomLerp(target: number, time: number): void {
        this.cameraZoomLerpTimer = this.cameraZoomLerpTime = time;
        this.cameraZoomLerpStart = this.camera.zoom;
        this.cameraZoomLerpEnd = target;
    }
}

/**
 * Global Singleton reference
 */
 let ENVIRONMENT_MANAGER!: EnvironmentManager;
 export { ENVIRONMENT_MANAGER };
