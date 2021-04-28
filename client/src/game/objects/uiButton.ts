import { FFEngine } from '@funfair/engine';

/**
 * Button States
 */
export enum ButtonState {
    INIT,
    IDLE,
    OVER,
    DOWN,
    LOCKED,
    DISABLED
}

/**
 * Types of button
 */
export enum ButtonType {
    STANDARD,
    TOGGLE
}

/**
 * Base class for a button object
 * Handles collisions and state changes, display is left to child classes
 */
export abstract class UIButton extends FFEngine.Component {

    protected state: ButtonState = ButtonState.INIT;
    protected camera!: FFEngine.THREE.Camera;
    protected raycastObject!: FFEngine.THREE.Object3D;
    protected onClickCallback!: () => void;
    protected onStateChangeCallback!: (oldState: ButtonState, newState: ButtonState) => void;
    protected active: boolean = true;
    protected buttonType: ButtonType = ButtonType.STANDARD;
    protected toggleDown: boolean = false;
    protected mouseDown: boolean = false;

    public Create(params: any): void {
        this.container = new FFEngine.THREE.Object3D();
    }

    public OnMouseMove(params: any) : void {
        
        if (this.active === true && this.state !== ButtonState.LOCKED && this.state !== ButtonState.DISABLED) { 
            if (this.buttonType === ButtonType.TOGGLE && this.toggleDown === true) {
                this.SetButtonState(ButtonState.DOWN);
            }
            else {
                if (this.CheckCollision(new FFEngine.THREE.Vector2(params.point.x, params.point.y))) {
                    if (this.mouseDown === true) {
                        this.SetButtonState(ButtonState.DOWN);
                    }
                    else {
                        this.SetButtonState(ButtonState.OVER);
                    }
                }
                else {
                    this.SetButtonState(ButtonState.IDLE);
                }
            }
        }
    }

    public OnMouseUp(params: any): void {
        this.mouseDown = false;
        if (this.active === true && this.state !== ButtonState.LOCKED && this.state !== ButtonState.DISABLED) {
            if (this.CheckCollision(new FFEngine.THREE.Vector2(params.point.x, params.point.y))) {

                if (this.buttonType === ButtonType.TOGGLE) {
                    this.toggleDown = !this.toggleDown;
                }

                if (this.buttonType === ButtonType.TOGGLE && this.toggleDown === true) {
                    this.SetButtonState(ButtonState.DOWN);
                }
                else {
                    this.SetButtonState(ButtonState.IDLE);
                }
                    

                if (this.onClickCallback)
                    this.onClickCallback();
            }
        }
    }

    public OnMouseDown(params: any): void {
        this.mouseDown = true;
        if (this.active === true && this.state !== ButtonState.LOCKED && this.state !== ButtonState.DISABLED) {
            if (this.CheckCollision(new FFEngine.THREE.Vector2(params.point.x, params.point.y))) {
                this.SetButtonState(ButtonState.DOWN);
            }
        }
    }

    /**
     * Sets a callback function which is called when the button is clicked
     */
    public SetOnClicked(callback: () => void): void {
        this.onClickCallback = callback;
    }

    /**
     * Sets a callback function which is called when the button changes state
     */
    public SetOnStateChanged(callback: (oldState: ButtonState, newState: ButtonState) => void): void {
        this.onStateChangeCallback = callback;
    }

    /**
     * Sets the camera to use for raycasting and detecting collisions. This is required for the button to work.
     */
    public SetCamera(camera: FFEngine.THREE.Camera): void {
        this.camera = camera;
    }

    /**
     * Activates/Deactivates the button. 
     * An inactive button is completely invisible and does nothing. Used for hiding button objects.
     */
    public SetActive(active: boolean): void {
        this.active = active;
    }

    /**
     * Returns the current state of the button
     */
    public GetState(): ButtonState {
        return this.state;
    }

    /**
     * Sets the locked state of the button. A locked button will stay in the locked state and not check or enter other states until unlocked.
     */
    public SetLocked(locked: boolean): void {
        if (locked === true) {
            this.SetButtonState(ButtonState.LOCKED);
        }
        else {
            this.SetButtonState(ButtonState.IDLE);
        }
    }

    /**
     * Sets the disabled state of the button. A disabled button will stay in the disabled state and not check or enter other states until enabled.
     */
    public SetDisabled(disabled: boolean): void {
        if (disabled === true) {
            this.SetButtonState(ButtonState.DISABLED);
        }
        else {
            this.SetButtonState(ButtonState.IDLE);
        }
    }

    /**
     * Sets the button type of the button. By default the button is of type Standard which uses all mouse interactions.
     */
    public SetButtonType(type: ButtonType): void {
        if(this.buttonType !== type) {
            this.buttonType = type;
        }
    }

    /**
     * Returns true if this is a toggle button and is currently down
     */
    public GetToggleState(): boolean {
        return this.toggleDown;
    }

    /**
     * Sets the toggle state value for the button.
     */
    public SetToggleState(value: boolean): void {
        this.toggleDown = value;
    }

    protected CheckCollision(point: FFEngine.THREE.Vector2): boolean {

        if (this.active === false) {
            return false;
        }

        if (!this.raycastObject) {
            console.error('UIButton: No Raycast object set for this button');
            return false;
        }

        if (!this.camera) {
            console.error('UIButton: No Raycast camera set for this button, set one with SetCamera()');
            return false;
        }

        let raycaster = new FFEngine.THREE.Raycaster();
        raycaster.setFromCamera(point, this.camera);
        return (raycaster.intersectObject(this.raycastObject, false).length > 0);   
    }

    protected SetRaycastObject(raycastObject: FFEngine.THREE.Object3D): void {
        this.raycastObject = raycastObject;
    }

    protected SetButtonState(newState: ButtonState): boolean {

        if (newState === this.state) {
            return false;
        }

        if (this.onStateChangeCallback) {
            this.onStateChangeCallback(this.state, newState);
        }

        this.state = newState;
        return true;
    }
}
