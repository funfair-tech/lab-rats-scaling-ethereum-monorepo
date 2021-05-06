import { FFEngine } from '@funfair/engine';
import { Logic_BetType } from '../logic/logic_defines';
import { ENVIRONMENT_MANAGER } from '../objectManagers/environmentManager';
import { GRAPH_MANAGER } from '../objectManagers/graphManager';
import { ASSETPACK, TextureAssetType } from '../assetPack';

/**
 * A display object representing a single cell on the graph grid
 */
export class GraphCell extends FFEngine.Component {

    private static readonly CELL_BORDER: number = 0;//0.05;
    private static readonly CELL_IDLE_ALPHA: number = 0.8;
    private static readonly CELL_ACTIVE_ALPHA: number = 0.5;

    private sprite!: FFEngine.Sprite;
    private coords: FFEngine.THREE.Vector2 = new FFEngine.THREE.Vector2();
    private betType: Logic_BetType = Logic_BetType.NONE;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();

        //custom material which accepts lights
        let material = new FFEngine.THREE.MeshLambertMaterial();
        material.map = ASSETPACK.GetTextureAsset(TextureAssetType.CELL);
        material.depthWrite = false;

        this.sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite, { material: material });
        this.SetBetType(Logic_BetType.NONE);
    }

    public OnMouseUp(params: any): void {
        if (this.CheckCollision(new FFEngine.THREE.Vector2(params.point.x, params.point.y))) {
            console.log('CELL CLICKED');
        }
    }

    public SetCoordinates(coords: FFEngine.THREE.Vector2): void {
        this.coords.copy(coords);
        this.container.position.copy(GRAPH_MANAGER.GridToWorld(coords));

    }

    public GetCoordinates(): FFEngine.THREE.Vector2 {
        return this.coords;
    }

    public SetSize(width: number, height: number): void {
        if (this.sprite) {
            this.sprite.SetSize(width - GraphCell.CELL_BORDER, height - GraphCell.CELL_BORDER);
            this.sprite.GetContainer().position.set(width/2, height/2, 0);
        }
    }

    public SetBetType(type: Logic_BetType): void {
        this.betType = type;

        if (this.betType === Logic_BetType.NONE) {
            this.sprite.SetAlpha(GraphCell.CELL_IDLE_ALPHA);
        }
        else {
            this.sprite.SetAlpha(GraphCell.CELL_ACTIVE_ALPHA);
        }
    }

    private CheckCollision(point: FFEngine.THREE.Vector2): boolean {
        if (this.betType !== Logic_BetType.NONE) {
            let raycaster = new FFEngine.THREE.Raycaster();
            raycaster.setFromCamera(point, ENVIRONMENT_MANAGER.GetCamera());
            return (raycaster.intersectObject(this.sprite.GetContainer(), false).length > 0);
        }
        return false;
    }
}
