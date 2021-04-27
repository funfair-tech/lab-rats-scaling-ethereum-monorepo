import { FFEngine } from '@funfair/engine';
import { GraphLine } from '../objects/graphLine';
import { GridCell } from '../objects/gridCell';

/**
 * Manages the display of the grid of graph cells
 */
export class GridManager extends FFEngine.Component {

    private static CELL_WIDTH: number = 3;
    private static CELL_HEIGHT: number = 1.5;

    private graphLine!: GraphLine;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Grid';
        GRID_MANAGER = this;

        //create grid
        this.CreateGrid();

        //create graph line
        this.graphLine = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphLine);
    }

    public GetCellWidth(): number {
        return GridManager.CELL_WIDTH;
    }

    public GetCellHeight(): number {
        return GridManager.CELL_HEIGHT;
    }

    public AddResult(price: number): void {
        if (this.graphLine) {
            this.graphLine.AddResult(price);
        }
    }

    private CreateGrid(): void {

        //test grid cells
        for (let i=-10;i<10;i++) {
            for (let j=-10;j<10;j++) {
                let cell = FFEngine.instance.CreateChildObjectWithComponent(this.container, GridCell);
                cell.GetContainer().position.set(i * GridManager.CELL_WIDTH, j * GridManager.CELL_HEIGHT, 0);
                cell.SetSize(GridManager.CELL_WIDTH, GridManager.CELL_HEIGHT);
            }
        }
    }
}

/**
 * Global Singleton reference
 */
 let GRID_MANAGER!: GridManager;
 export { GRID_MANAGER };
