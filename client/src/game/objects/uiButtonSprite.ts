import { FFEngine } from '@funfair/engine';
import { UIButton, ButtonState } from './uiButton';

/**
 * Button object which uses a Sprite for display
 */
export class UIButtonSprite extends UIButton {

    private sprite!: FFEngine.Sprite;
    private highlight!: FFEngine.Sprite;
    private stateConfig: ButtonSpriteStateConfig[] = [];
    private highlightGlowActive: boolean = false;
    private highlightGlowTimer: number = 0;

    public Create(params: any): void {
        super.Create(params);
        
        this.sprite = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.SetRaycastObject(this.sprite.GetContainer());
    }

    public Update(): void {
        if (this.highlight && this.highlightGlowActive) {
            //this.highlightGlowTimer += FFEngine.instance.GetDeltaTime() * 3;
            //this.highlight.SetAlpha((Math.sin(this.highlightGlowTimer) * 0.5) + 0.5);
        }
    }

    /**
     * Returns the sprite object used for button display
     */
    public GetSprite(): FFEngine.Sprite {
        return this.sprite;
    }

    public GetHighlightSprite(): FFEngine.Sprite {
        return this.highlight;
    }

    public SetHighlightGlow(active: boolean): void {
        if (this.highlight) {
            this.highlight.GetContainer().visible = active;
            this.highlightGlowActive = active;
        }
    }

    public SetHighlight(active: boolean): void {
        if (this.highlight) {
            this.highlight.SetAlpha(active ? 1 : 0);
        }
    }

    public StartLerpAlpha(targetAlpha: number, time: number) {
        if (this.sprite) {
            this.sprite.StartLerpAlpha(targetAlpha, time);
        }

        if (this.highlight) {
            this.highlight.StartLerpAlpha(targetAlpha, time);
        }
    }

    /**
     * Sets a display config for a particular button state.
     * Only ButtonState.IDLE is required to have a config, and will be used as a fallback for other states.
     */
    public SetupState(state: ButtonState, config: ButtonSpriteStateConfig): void {
        this.stateConfig[state] = config;

        if (state === ButtonState.IDLE) {
            this.SetButtonState(ButtonState.IDLE);
        }
    }

    public SetupHighlight(texture: FFEngine.THREE.Texture): void {
        this.highlight = FFEngine.instance.CreateChildObjectWithComponent(this.container, FFEngine.Sprite);
        this.highlight.SetBlendingMode(FFEngine.THREE.AdditiveBlending);
        this.highlight.SetTexture(texture);
    }

    /**
     * Activates/Deactivates the button
     */
    public SetActive(active: boolean): void {
        super.SetActive(active);
        this.sprite.GetContainer().visible = active;
    }

    protected SetButtonState(newState: ButtonState): boolean {
        if (super.SetButtonState(newState) === false) {
            return false;
        }
        let config = this.GetConfigForState(this.state);
        if (config.texture) {
            this.sprite.SetTexture(config.texture, false);
        }
        if (config.colour) {
            this.sprite.SetColor(config.colour);
        }
        return true;
    }

    private GetConfigForState(state: ButtonState): ButtonSpriteStateConfig {
        return this.stateConfig[state] !== undefined ? this.stateConfig[state] : this.stateConfig[ButtonState.IDLE];
    }

}

/**
 * A config object used to define display for a particular state of the sprite button.
 * Call SetupState() on the button for each state you wish to attach config for.
 */
export class ButtonSpriteStateConfig {
    constructor (
        public texture: FFEngine.THREE.Texture,
        public colour: FFEngine.THREE.Color) { }
}
