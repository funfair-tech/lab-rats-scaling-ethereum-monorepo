import { FFEngine } from '@funfair/engine';
import { GraphLine } from '../objects/graphLine';
import { GraphCell } from '../objects/graphCell';

/**
 * Manages the display of the grid of graph cells and line
 */
export class GraphManager extends FFEngine.Component {

    private static readonly CELL_WIDTH: number = 3;
    private static readonly CELL_HEIGHT: number = 1.5;

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
        return GraphManager.CELL_WIDTH;
    }

    /**
     * returns the height of a grid cell in world units
     */
    public GetCellHeight(): number {
        return GraphManager.CELL_HEIGHT;
    }

    /**
     * Converts a grid x,y coordinate to world position
     */
    public GridToWorld(x: number, y: number): FFEngine.THREE.Vector3 {
        return new FFEngine.THREE.Vector3(x * GraphManager.CELL_WIDTH, y * GraphManager.CELL_HEIGHT, 0);
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
                let cell = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphCell);
                cell.GetContainer().position.copy(this.GridToWorld(i, j));
                cell.SetSize(GraphManager.CELL_WIDTH, GraphManager.CELL_HEIGHT);
            }
        }
    }
}

/**
 * Global Singleton reference
 */
 let GRID_MANAGER!: GraphManager;
 export { GRID_MANAGER };
