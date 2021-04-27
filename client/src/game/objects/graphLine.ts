import { FFEngine } from '@funfair/engine';
import { ASSETPACK, TextureAssetType } from '../assetPack';
import { GRID_MANAGER } from '../objectManagers/graphManager';

/**
 * A display object representing a the graph line
 */
export class GraphLine extends FFEngine.Component {

    private points: FFEngine.THREE.Vector3[] = [];
    private line!: FFEngine.Line;

    public Create(params: any): void {
        super.Create(params);
        this.container = new FFEngine.THREE.Object3D();

        //create line object
        this.line = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Line, { map:  ASSETPACK.GetTextureAsset(TextureAssetType.LINE), numPoints: 1024});
        this.line.GetContainer().position.set(0, 0, 0.01);
        this.line.SetWidth(0.1);
        this.line.SetFacingDirection(new FFEngine.THREE.Vector3(0, 0, 1));
        this.line.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        this.line.SetColor(0xffffff);
    }

    /**
     * Adds a result to the graph line and advances the line along the grid
     */
    public AddResult(price: number): void {
        let newIndex = this.points.length;
        this.points.push(new FFEngine.THREE.Vector3(newIndex * GRID_MANAGER.GetCellWidth(), price * GRID_MANAGER.GetCellHeight(), 0));
        this.line.SetShape(this.points);
    }
}
