import { FFEngine } from '@funfair/engine';
import { GraphLine } from '../objects/graphLine';
import { GraphCell } from '../objects/graphCell';
import { ENVIRONMENT_MANAGER } from './environmentManager';
import { GraphUI } from '../objects/graphUI';
import { Player } from './playerManager';

/**
 * Manages the display of the grid of graph cells and line
 */
export class GraphManager extends FFEngine.Component {

    private static readonly CELL_WIDTH: number = 3;
    private static readonly CELL_HEIGHT: number = 1.5;
    private static readonly GRAPH_CELLS_WIDTH = 20;
    private static readonly GRAPH_CELLS_HEIGHT = 30;

    private graphLine!: GraphLine;
    private cells: GraphCell[] = [];
    private graphUI!: GraphUI;
    private graphCoord: FFEngine.THREE.Vector2 = new FFEngine.THREE.Vector2();
    private graphCenter!: FFEngine.THREE.Vector2;

    public Create(params: any): void {
        super.Create(params);

        this.container = new FFEngine.THREE.Object3D();
        this.container.name = 'Grid';
        GRAPH_MANAGER = this;

        //create graph grid
        this.CreateCells();
        this.UpdateCells(this.graphCoord);

        //create graph line
        this.graphLine = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphLine);

        //create graph UI
        this.graphUI = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphUI);
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
    public AddResult(price: number, instant: boolean = true): void {

        //temporary scaling until the graph sizing is sorted
        price -= 1000;
        price /= 5;

        if (this.graphLine) {
            this.graphLine.AddResult(price, instant);
        }

        //advance graph
        if (instant === false) {
            this.graphCoord.x++;
        }

        this.graphCoord.y = price;
        ENVIRONMENT_MANAGER.SetCameraToGraphCoordinate(this.graphCoord);
        this.UpdateCells(this.graphCoord);
    }

    public OffsetLine(numCells: number): void {
        this.graphLine.OffsetLine(numCells);
    }

    public AddWinningBetTypes(betWinFlags: boolean[]): void {
        this.graphUI.SetBetWinFlags(betWinFlags);
    }

    public HighlightWinningButtons(active: boolean): void {
        this.graphUI.HighlightWinningButtons(active);
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

    public UpdateGraphUI(): void {
        this.graphUI.SetPosition(this.GridToWorld(this.graphCoord));
    }

    public GraphUIShowBetButtons(visible: boolean): void {
        this.graphUI.ShowBetButtons(visible);
    }

    public GraphUILockBetButtons(locked: boolean): void {
        this.graphUI.LockBetButtons(locked);
    }

    public GraphUIUpdatePlayers(players: Player[]): void {
        this.graphUI.UpdatePlayers(players);
    }

    /**
     * Dynamically create cells around the current graph position
     */
    private UpdateCells(coord: FFEngine.THREE.Vector2): void {

        //round to closest whole grid coordinate
        let centerCoord = new FFEngine.THREE.Vector3().copy(coord);
        centerCoord.x = Math.round(centerCoord.x);
        centerCoord.y = Math.round(centerCoord.y);

        if (this.graphCenter === undefined || centerCoord.equals(this.graphCenter) === false) {
            this.graphCenter = new FFEngine.THREE.Vector2(centerCoord);

            //create cell coordinate bounds
            let bounds = new FFEngine.THREE.Box2(   new FFEngine.THREE.Vector2(centerCoord.x - GraphManager.GRAPH_CELLS_WIDTH, centerCoord.y - GraphManager.GRAPH_CELLS_HEIGHT),
                                                    new FFEngine.THREE.Vector2(centerCoord.x + GraphManager.GRAPH_CELLS_WIDTH - 1, centerCoord.y + GraphManager.GRAPH_CELLS_HEIGHT - 1));

            //create list of spare cells that are out of bounds to subsequently be moved
            let spareCells: GraphCell[] = [];
            for (let i=0;i<this.cells.length;i++) {
                if (bounds.containsPoint(this.cells[i].GetCoordinates()) === false) {
                    spareCells.push(this.cells[i]);
                }
            }

            //make sure all required cells are filled
            let spareCellIndex = 0;
            for (let i=bounds.min.x;i<=bounds.max.x;i++) {
                for (let j=bounds.min.y;j<=bounds.max.y;j++) {
                    let coord = new FFEngine.THREE.Vector2(i, j);
                    if (this.GetCellAtCoordinate(coord) === undefined) {
                        spareCells[spareCellIndex].SetCoordinates(coord);
                        spareCellIndex++;
                    }
                }
            }
        }
    }

    /**
     * create enough grid cell objects for the required visible width/height
     */
    private CreateCells(): void {
        for (let i=0;i<GraphManager.GRAPH_CELLS_WIDTH * GraphManager.GRAPH_CELLS_HEIGHT * 4;i++) {
            let cell = FFEngine.instance.CreateChildObjectWithComponent(this.container, GraphCell);
            cell.SetSize(GraphManager.CELL_WIDTH, GraphManager.CELL_HEIGHT);
            cell.SetCoordinates(new FFEngine.THREE.Vector2(Math.floor(i / GraphManager.GRAPH_CELLS_HEIGHT), i % GraphManager.GRAPH_CELLS_HEIGHT));
            this.cells.push(cell);
        }
    }
}

/**
 * Global Singleton reference
 */
 let GRAPH_MANAGER!: GraphManager;
 export { GRAPH_MANAGER };
