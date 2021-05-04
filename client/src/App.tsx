import './App.scss';
import AccountBar from './containers/accountBar/accountBar';
import ErrorNotification from './containers/errorNotification/errorNotification';
import Faucet from './containers/faucet/faucet';
import Game from './containers/game/gameContainer';
import SplashScreen from './containers/splash/splashScreen';

function App() {
  const queryString = window.location.search;
  const urlParams = new URLSearchParams(queryString);
  const localGame = urlParams.get('localGame') === 'true';

  return (
    <div className='app'>
      {localGame ? null : <SplashScreen />}
      <AccountBar title='Lab Rats' />
      <Game />
      <Faucet />
      <ErrorNotification visible={true}/>
    </div>
  );
}

export default App;
