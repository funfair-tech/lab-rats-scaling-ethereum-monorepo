import './App.scss';
import AccountBar from './containers/accountBar/accountBar';
import ErrorNotification from './containers/errorNotification/errorNotification';
import Faucet from './containers/faucet/faucet';
import Game from './containers/game/gameContainer';

function App() {
  return (
    <div className='app'>
      <AccountBar title='Lab Rats' />
      <Game />
      <Faucet />
      <ErrorNotification visible={true}/>
    </div>
  );
}

export default App;
