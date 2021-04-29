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
    if(!!props.user.address) {
      gameInstance.setAddress(props.user.address);
    }
  }, [gameInstance, props.user.address]);

  useEffect(() => {
    if(!!props.game.result) {
      gameInstance.handleRoundResult(props.game.result);
    }
  }, [gameInstance, props.game.result]);

  useEffect(() => {
    if(!!props.game.round) {
      gameInstance.handleNextRound(props.game.round);
    }
  }, [gameInstance, props.game.round]);

  useEffect(() => {
    if(!!props.game.bets) {
      gameInstance.handleBets(props.game.bets);
    }
  }, [gameInstance, props.game.bets]);

  useEffect(() => {
    if(props.game.canPlay === false && !!props.game.round) {
      gameInstance.handleNoMoreBets(props.game.round.id);
    }
  }, [gameInstance, props.game.canPlay, props.game.round]);

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
