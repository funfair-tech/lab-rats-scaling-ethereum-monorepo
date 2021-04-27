import './App.scss';
import AccountBar from './containers/accountBar/accountBar';
import Faucet from './containers/faucet/faucet';
import Game from './containers/game/gameContainer';

function App() {
  return (
    <div className='app'>
      <AccountBar title='Lab Rats' />
      <Game />
      <Faucet />
    </div>
  );
}

export default App;
