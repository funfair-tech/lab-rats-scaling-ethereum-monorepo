import { FFEngine } from '@funfair/engine';
import { GridCell } from '../objects/gridCell';

/**
 * Manages the display of the grid of graph cells
 */
export class GridManager extends FFEngine.Component {

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Grid';
        GRID_MANAGER = this;

        //create grid
        this.CreateGrid();
    }

    private CreateGrid(): void {

        //test grid cells
        let width = 2;
        let height = 1.5;
        for (let i=-10;i<10;i++) {
            for (let j=-10;j<10;j++) {
                let cell = FFEngine.instance.CreateChildObjectWithComponent(this.container, GridCell);
                cell.GetContainer().position.set(i * width, j * height, 0);
                cell.SetSize(width, height);
            }
        }

    }
}

/**
 * Global Singleton reference
 */
 let GRID_MANAGER!: GridManager;
 export { GRID_MANAGER };
