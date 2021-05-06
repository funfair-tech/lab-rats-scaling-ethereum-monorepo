import './playerCount.scss';
import { FunctionComponent } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';

interface Props extends ReduxProps {}

export const PlayerCount: FunctionComponent<Props> = (props) => {
  return props.user.authenticated ? <div className='playerCount'>
    <div key={props.game.playersOnline} className='playerCount__summary'>	
      <div className='playerCount__icon'>&#128101;</div> <div>{props.game.playersOnline}</div>
    </div>
    <div className='playerCount__details'>players online</div>
  </div> : null;
};

const mapStateToProps = (state: RootState) => {
  return {
    game: state.game,
    user: state.user,
  };
};

const connector = connect(mapStateToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(PlayerCount);
