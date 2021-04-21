import './App.scss';
import { Canvas } from './components/canvas/canvas';
import AccountBar from './containers/accountBar/accountBar';
import { Game } from './game/game';
import { useEffect, useMemo } from 'react';

function App() {

  const game = useMemo(() => new Game(), []);

  useEffect(() => {
    console.log('initialising game...');
    game.initEngine();
  }, [game]);

  return (
    <div className="app">
        <AccountBar title='Lab Rats'/>
        <Canvas />
    </div>
  );
}

export default App;
