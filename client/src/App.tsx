import './App.scss';
import { Button } from './components/button/button';
import { Canvas } from './components/canvas/canvas';
import AccountBar from './containers/accountBar/accountBar';
import { Game } from './game/game';
import { connect } from 'react-redux';
import { setAddress } from './store/actions/user.actions';

function App() {
  const game = new Game();

  return (
    <div className="app">
      <header className="app__header">
        <AccountBar title='Lab Rats'/>

        <Canvas />

        <span>Engine version: {game.getEngineVersion()}</span>

        <section className="app__debugButtons">
          <Button onClick={() => game.initEngine()}>init engine</Button>
        </section>

      </header>

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
