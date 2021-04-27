import { FFEngine } from '@funfair/engine';
import { ASSETPACK, TextureAssetType } from '../assetPack';

/**
 * A display object representing a the graph line
 */
export class GraphLine extends FFEngine.Component {

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();

        //test line
        let line = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Line, { map:  ASSETPACK.GetTextureAsset(TextureAssetType.LINE), numPoints: 1024});
        line.GetContainer().position.set(0, 0, 0.1);
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
    }

}
