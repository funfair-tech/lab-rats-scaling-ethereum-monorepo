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

    /**
     * returns the width of a grid cell in world units
     */
    public GetCellWidth(): number {
        return GridManager.CELL_WIDTH;
    }

    /**
     * returns the height of a grid cell in world units
     */
    public GetCellHeight(): number {
        return GridManager.CELL_HEIGHT;
    }

    /**
     * Converts a grid x,y coordinate to world position
     */
    public GridToWorld(x: number, y: number): FFEngine.THREE.Vector3 {
        return new FFEngine.THREE.Vector3(x * GridManager.CELL_WIDTH, y * GridManager.CELL_HEIGHT, 0);
    }

    /**
     * Adds a result to the end of the graph line
     */
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
                cell.GetContainer().position.copy(this.GridToWorld(i, j));
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
