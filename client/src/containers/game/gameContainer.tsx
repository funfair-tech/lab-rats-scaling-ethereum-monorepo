import { FunctionComponent, useEffect, useMemo } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { Canvas } from '../../components/canvas/canvas';
import { Game } from '../../game/game';
import { gameService } from '../../services/game.service';

interface Props extends ReduxProps {}

export const GameContainer: FunctionComponent<Props> = (props) => {

  const gameInstance = useMemo(() => new Game(gameService.handlePlay), []);

  useEffect(() => {
    console.log('initialising game...');
    gameInstance.initEngine();
  }, [gameInstance]);

  useEffect(() => {
    if(!!props.game.result) {
      gameInstance.handleRoundResult(props.game.result);
    }
  }, [gameInstance, props.game]);

  useEffect(() => {
    if(!!props.user.address) {
      gameInstance.setAddress(props.user.address);
    }
  }, [props.user.address]);

  return (
    <div>
        <Canvas />
    </div>
  );
}

const mapStateToProps = (state: RootState) => {
  return {
    game: state.game,
    user: state.user,
  };
};

const connector = connect(mapStateToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(GameContainer);
