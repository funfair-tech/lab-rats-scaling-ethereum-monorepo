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
    private particleSystem!: FFEngine.ParticleSystem;
    private particleTimer: number = 0;
    private particleActive: boolean = false;

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

        this.particleSystem = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.ParticleSystem, { map: ASSETPACK.GetTextureAsset(TextureAssetType.GLOW), numParticles: 1000 });
        this.particleSystem.SetSystemGravity(new FFEngine.THREE.Vector3(0, -4, 0));
        this.particleSystem.GetContainer().position.set(0, 0, 5);

        let light = new FFEngine.THREE.PointLight(0xffffff, 1, 10);
        light.position.z = 2;
        this.container.add(light);
    }

    public Update(): void {
        let size = FFEngine.MathHelper.GetRandomRange(GraphGlow.GLOW_SIZE_MIN, GraphGlow.GLOW_SIZE_MAX);
        this.sprite.SetSize(size, size);
        size = FFEngine.MathHelper.GetRandomRange(GraphGlow.GLOW_SIZE_MIN, GraphGlow.GLOW_SIZE_MAX);
        this.sprite2.SetSize(size, size);

        //update particle spawns
        if (this.particleActive) {
            this.particleTimer += FFEngine.instance.GetDeltaTime();
            while (this.particleTimer >= 0.01) {
                this.particleTimer -= 0.01;
                this.SpawnParticle();
            }
        }
    }

    public SetParticlesActive(active: boolean): void {
        this.particleActive = active;
    }

    private SpawnParticle(): void {
        this.particleSystem.SpawnParticle(new FFEngine.THREE.Vector3(0, 0, -5), //pos
            new FFEngine.THREE.Vector3((Math.random()-0.5) * 3, (Math.random()-0.3) * 3, 0),    //vel
            0.2 + (Math.random()*0.2),    //Size
            0.3 + (Math.random()*0.3),    //Lifetime
            [1, 0.7, 0.7, 0.8],   //Colour
            (Math.random() - 0.5) * 4,   //Spin
            0.001,  //Size end
            0.0, // fade in coef
            0.2, // fade out coef
            [1, 0.7, 0.7, 0.3],   //Colour end
            true    // random initial rotation
        );
    }
}
