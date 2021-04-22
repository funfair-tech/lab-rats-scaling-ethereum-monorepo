import './App.scss';
import AccountBar from './containers/accountBar/accountBar';
import Game from './containers/game/gameContainer';

function App() {
  return (
    <div className="app">
        <AccountBar title='Lab Rats'/>
        <Game />
    </div>
  );
}

export default App;
