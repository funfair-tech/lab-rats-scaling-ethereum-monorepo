import { FFEngine } from '@funfair/engine';
import { ASSETPACK, TextureAssetType } from '../assetPack';

/**
 * Glow FX object to cover the end of the graph line
 */
export class GraphGlow extends FFEngine.Component {

    private static readonly GLOW_SIZE_MIN = 1;
    private static readonly GLOW_SIZE_MAX = 1.5;

    private sprite!: FFEngine.Sprite;
    private sprite2!: FFEngine.Sprite;


    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();

        this.sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.sprite.SetTexture(ASSETPACK.GetTextureAsset(TextureAssetType.GLOW));
        this.sprite.SetColor(new FFEngine.THREE.Color(0xffff80));
        this.sprite.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        this.sprite.GetContainer().position.set(0, 0, 0.1);
        this.sprite.GetContainer().rotateZ(Math.random());

        this.sprite2 = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.sprite2.SetTexture(ASSETPACK.GetTextureAsset(TextureAssetType.GLOW));
        this.sprite2.SetColor(new FFEngine.THREE.Color(0xffff80));
        this.sprite2.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        this.sprite2.GetContainer().position.set(0, 0, 0.1);
        this.sprite2.GetContainer().rotateZ(Math.random());

        let light = new FFEngine.THREE.PointLight(0xffffff, 1, 10);
        light.position.z = 2;
        this.container.add(light);
    }

    public Update(): void {
        let size = FFEngine.MathHelper.GetRandomRange(GraphGlow.GLOW_SIZE_MIN, GraphGlow.GLOW_SIZE_MAX);
        this.sprite.SetSize(size, size);
        size = FFEngine.MathHelper.GetRandomRange(GraphGlow.GLOW_SIZE_MIN, GraphGlow.GLOW_SIZE_MAX);
        this.sprite2.SetSize(size, size);
    }
}
