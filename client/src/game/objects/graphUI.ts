import { FFEngine } from '@funfair/engine';

/**
 * Component which manages the UI which is overlaid on the graph in world space
 */
export class GraphUI extends FFEngine.Component {

    private priceLine!: FFEngine.Sprite;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();

        this.priceLine = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.priceLine.SetColor(new FFEngine.THREE.Color(0xee60ee));
        this.priceLine.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        this.priceLine.SetSize(100, 0.05);
    }

    public SetVisibility(visible: boolean): void {
        this.container = visible;
    }

    public SetPosition(position: FFEngine.THREE.Vector3): void {
        this.container.position.copy(position);
    }
}
