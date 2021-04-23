import { FFEngine } from '@funfair/engine';
import { ASSETPACK, TextureAssetType } from '../assetPack';
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
    }

    public AssetLoadingFinished(): void {
        this.CreateScene();
    }

    private CreateScene(): void {

        //create components
        FFEngine.instance.CreateChildObjectWithComponent(this.container, GridManager);
        
        //test line
        let line = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Line, { map:  ASSETPACK.GetTextureAsset(TextureAssetType.LINE), numPoints: 1024});
        line.GetContainer().position.set(0, 0, 0.5);
        line.SetWidth(0.1);
        line.SetFacingDirection(new FFEngine.THREE.Vector3(0, 0, 1));
        line.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        line.SetColor(0xffffff);

        let points: FFEngine.THREE.Vector3[] = [
            new FFEngine.THREE.Vector3(-2, 0, 0),
            new FFEngine.THREE.Vector3(-1, 1, 0),
            new FFEngine.THREE.Vector3(1, -1, 0),
            new FFEngine.THREE.Vector3(2, 0, 0),
        ];
        line.SetShape(points);

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
