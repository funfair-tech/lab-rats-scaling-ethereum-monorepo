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
    private static readonly RESULT_LERP_SPEED = 0.5;

    private points: FFEngine.THREE.Vector3[] = [];
    private dataPoints: FFEngine.THREE.Vector3[] = [];
    private resultLerpCoef: number = 1;
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

    public Update(): void {
        if (this.resultLerpCoef < 1) {
            this.resultLerpCoef += FFEngine.instance.GetDeltaTime() * GraphLine.RESULT_LERP_SPEED;

            if (this.resultLerpCoef > 1) {
                this.resultLerpCoef = 1;
            }

            this.UpdateGraphLerp();
        }
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

                if (i < GraphLine.INTERMEDIATE_POINTS_NUM) {
                    point.y += FFEngine.MathHelper.GetRandomRange(-GraphLine.INTERMEDIATE_POINTS_VARIANCE, GraphLine.INTERMEDIATE_POINTS_VARIANCE);
                }
                
                this.points.push(point);
            }
        }
        
        //setup lerp and update graph
        if (this.points.length > 2 && instant === false) {
            this.resultLerpCoef = 0;
        }
        else {
            this.resultLerpCoef = 1;
        }
        this.UpdateGraphLerp();
    }

    private UpdateGraphLerp(): void {
        let drawPoints: FFEngine.THREE.Vector3[] = [];
        let lerpPointCoef: number = (1 / GraphLine.INTERMEDIATE_POINTS_NUM);
        let startLerpIndex: number = (this.points.length - GraphLine.INTERMEDIATE_POINTS_NUM);
        let completeLerpPoints = Math.floor(this.resultLerpCoef / lerpPointCoef);
        let remainingScaledLerpCoef = (this.resultLerpCoef - (completeLerpPoints * lerpPointCoef)) * GraphLine.INTERMEDIATE_POINTS_NUM;

        for (let i=0;i<this.points.length;i++) {
            if (i < startLerpIndex + completeLerpPoints) {
                drawPoints.push(this.points[i]);
            }
            else if (i === startLerpIndex + completeLerpPoints) {
                let dif = new FFEngine.THREE.Vector3().subVectors(this.points[i], this.points[i-1]).multiplyScalar(remainingScaledLerpCoef);
                let lerpedPoint = new FFEngine.THREE.Vector3().copy(this.points[i-1]).add(dif);
                drawPoints.push(lerpedPoint);
            }
        }

        this.line.SetShape(drawPoints);
        this.glow.GetContainer().position.copy(drawPoints[drawPoints.length-1]);
    }
}
