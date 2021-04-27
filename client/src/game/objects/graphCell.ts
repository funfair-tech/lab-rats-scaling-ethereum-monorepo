import { FFEngine } from '@funfair/engine';

/**
 * A display object representing a single cell on the graph grid
 */
export class GraphCell extends FFEngine.Component {

    private static readonly CELL_BORDER: number = 0.05;

    private sprite!: FFEngine.Sprite;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();

        this.sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.sprite.SetAlpha(0.4);
        this.sprite.SetColor(new FFEngine.THREE.Color(0x80ff80));
        this.sprite.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
    }

    public SetSize(width: number, height: number): void {
        if (this.sprite) {
            this.sprite.SetSize(width - GraphCell.CELL_BORDER, height - GraphCell.CELL_BORDER);
            this.sprite.GetContainer().position.set(width/2, height/2, 0);
        }
    }
}
