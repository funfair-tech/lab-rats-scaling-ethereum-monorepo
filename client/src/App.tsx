import window from '@funfair-tech/wallet-sdk/window';
import './App.scss';
import { Button } from './components/button/button';
import { Canvas } from './components/canvas/canvas';
import { Game } from './game/game';

function App() {
  const game = new Game();

  const loginToWallet = () => {
    window.funwallet.sdk.auth.login();
  };

  return (
    <div className="app">
      <header className="app__header">
        <Canvas />

        <span>Engine version: {game.getEngineVersion()}</span>

        <section className="app__debugButtons">
          <Button onClick={() => game.initEngine()}>init engine</Button>
        </section>

        <section className="app__loginButtons">
          <Button onClick={() => loginToWallet()}>Login to wallet</Button>
        </section>
      </header>
    </div>
  );
}

export default App;
