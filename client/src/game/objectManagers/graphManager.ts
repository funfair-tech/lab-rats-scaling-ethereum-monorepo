import { FFEngine } from '@funfair/engine';
import { GraphLine } from '../objects/graphLine';
import { GraphCell } from '../objects/graphCell';
import { ENVIRONMENT_MANAGER } from './environmentManager';

/**
 * Manages the display of the grid of graph cells and line
 */
export class GraphManager extends FFEngine.Component {

    private static readonly CELL_WIDTH: number = 3;
    private static readonly CELL_HEIGHT: number = 1.5;

    private graphLine!: GraphLine;
    private cells: GraphCell[] = [];
    private graphCoord: FFEngine.THREE.Vector2 = new FFEngine.THREE.Vector2();

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Grid';
        GRAPH_MANAGER = this;

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
    public GridToWorld(coord: FFEngine.THREE.Vector2): FFEngine.THREE.Vector3 {
        return new FFEngine.THREE.Vector3(coord.x * GraphManager.CELL_WIDTH, coord.y * GraphManager.CELL_HEIGHT, 0);
    }

    /**
     * Adds a result to the end of the graph line
     */
    public AddResult(price: number): void {
        if (this.graphLine) {
            this.graphLine.AddResult(price);
        }

        //advance graph
        this.graphCoord.x++;
        this.graphCoord.y = price;
        ENVIRONMENT_MANAGER.SetCameraToGraphCoordinate(this.graphCoord);
    }

    public GetCellAtCoordinate(coord: FFEngine.THREE.Vector2): GraphCell | undefined {
        for (let i=0;i<this.cells.length;i++) {
            if (this.cells[i]) {
                if (this.cells[i].GetCoordinates().equals(coord)) {
                    return this.cells[i];
                }
            }
        }
        return undefined;
    }

    private CreateGrid(): void {
        //test grid cells
        for (let i=-10;i<10;i++) {
            for (let j=-10;j<10;j++) {
                let cell = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphCell);
                cell.GetContainer().position.copy(this.GridToWorld(new FFEngine.THREE.Vector2(i, j)));
                cell.SetSize(GraphManager.CELL_WIDTH, GraphManager.CELL_HEIGHT);
                cell.SetCoordinates(new FFEngine.THREE.Vector2(i, j));
                this.cells.push(cell);
            }
        }

        //test grid bet highlighting
        /*
        let cell = this.GetCellAtCoordinate(new FFEngine.THREE.Vector2(4, 3));
        cell?.SetBetType(Logic_BetType.HIGHER);
        cell = this.GetCellAtCoordinate(new FFEngine.THREE.Vector2(4, 4));
        cell?.SetBetType(Logic_BetType.HIGHER);
        cell = this.GetCellAtCoordinate(new FFEngine.THREE.Vector2(4, 1));
        cell?.SetBetType(Logic_BetType.HIGHER);
        cell = this.GetCellAtCoordinate(new FFEngine.THREE.Vector2(4, 2));
        cell?.SetBetType(Logic_BetType.HIGHER);
        */
    }
}

/**
 * Global Singleton reference
 */
 let GRAPH_MANAGER!: GraphManager;
 export { GRAPH_MANAGER };
