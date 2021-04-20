import window from '@funfair-tech/wallet-sdk/window';
import './App.scss';
import { Button } from './components/button/button';
import { Canvas } from './components/canvas/canvas';
import AccountBar from './containers/accountBar/accountBar';
import { Game } from './game/game';
import { RootState } from './store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { setAddress } from './store/actions/user.actions';

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
      <AccountBar title='Lab Rats'/>

    </div>
  );
}


const mapDispatchToProps = (dispatch: Function) => {
  return {
    setAddress: (address: string) => dispatch(setAddress(address)),
  };
};

const connector = connect(null, mapDispatchToProps);


export default connector(App);
