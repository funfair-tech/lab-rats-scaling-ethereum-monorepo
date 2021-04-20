import './App.scss';
import { Game } from './game/game';
import { Canvas } from './components/canvas/canvas';
import { Button } from './components/button/button';

function App() {
  const game = new Game();

  return (
    <div className="app">
      <header className="app__header">

        <Canvas/>

        <span>Engine version: {game.getEngineVersion()}</span>

        <section className='app__debugButtons'>
          <Button onClick={() => game.initEngine()}>init engine</Button> 
        </section>
               
        
      </header>
    </div>
  );
}

export default App;
