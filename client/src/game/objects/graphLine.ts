import { FFEngine } from '@funfair/engine';
import { ASSETPACK, TextureAssetType } from '../assetPack';
import { GRAPH_MANAGER } from '../objectManagers/graphManager';
import { GraphGlow } from './graphGlow';

/**
 * A display object representing a the graph line
 */
export class GraphLine extends FFEngine.Component {

    private static readonly INTERMEDIATE_POINTS_NUM = 3;
    private static readonly INTERMEDIATE_POINTS_VARIANCE = 1;

    private points: FFEngine.THREE.Vector3[] = [];
    private dataPoints: FFEngine.THREE.Vector3[] = [];
    private line!: FFEngine.Line;
    private glow!: GraphGlow;

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

        //create glow object
        this.glow = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphGlow);

        //add initial point
        this.AddResult(0);
    }

    /**
     * Adds a result to the graph line and advances the line along the grid
     */
    public AddResult(price: number, instant: boolean = true): void {
        let newIndex = this.dataPoints.length;
        let dataPoint = new FFEngine.THREE.Vector3(newIndex * GRAPH_MANAGER.GetCellWidth(), price * GRAPH_MANAGER.GetCellHeight(), 0);
        this.dataPoints.push(dataPoint);

        if (newIndex === 0) {
            this.points.push(dataPoint);
        }
        else {
            let previousPoint = this.dataPoints[newIndex-1];
            let dif = new FFEngine.THREE.Vector3().subVectors(dataPoint, previousPoint).multiplyScalar(1/GraphLine.INTERMEDIATE_POINTS_NUM);

            for (let i=1;i<=GraphLine.INTERMEDIATE_POINTS_NUM;i++) {
                let point = new FFEngine.THREE.Vector3().addVectors(previousPoint, new FFEngine.THREE.Vector3().copy(dif).multiplyScalar(i));
                point.y += FFEngine.MathHelper.GetRandomRange(-GraphLine.INTERMEDIATE_POINTS_VARIANCE, GraphLine.INTERMEDIATE_POINTS_VARIANCE);
                this.points.push(point);
            }
        }
        this.line.SetShape(this.points);

        this.glow.GetContainer().position.copy(this.points[this.points.length-1]);
    }
}
