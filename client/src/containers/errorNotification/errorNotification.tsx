import { FunctionComponent, useEffect, useState } from 'react';
import { Notification, NotificationType } from '../../components/notification/notification';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { LRError } from '../../model/errorCodes';
import { setGameError } from '../../store/actions/game.actions';
import { setNetworkError } from '../../store/actions/network.actions';
import { setUserError } from '../../store/actions/user.actions';

interface Props extends ReduxProps {
  visible: boolean;
}

export const ErrorNotification: FunctionComponent<Props> = (props) => {
  const NOTIFICATION_TIME = 3000;  
  const [error, setError] = useState<LRError|null>(null);
  
  useEffect(() => {
    if(!!props.game.error) {
      setError(props.game.error);
      setTimeout(() => {
        props.clearGameError();
      }, NOTIFICATION_TIME);
    } else {
      setError(null)
    }
    // eslint-disable-next-line
  }, [props.game.error]);

  useEffect(() => {
    if(!!props.network.error) {
      setError(props.network.error);
      setTimeout(() => {
        props.clearNetworkError();
      }, NOTIFICATION_TIME);
    } else {
      setError(null)
    }
    // eslint-disable-next-line
  }, [props.network.error]);

  useEffect(() => {
    if(!!props.user.error) {
      setError(props.user.error);
      setTimeout(() => {
        props.clearUserError();
      }, NOTIFICATION_TIME);
    } else {
      setError(null)
    }
    // eslint-disable-next-line
  }, [props.user.error]);
  
  return error ? <Notification label={error.msg} visible={props.visible} category={NotificationType.ERROR}/> : null;
};

const mapStateToProps = (state: RootState) => {
  return {
    game: state.game,
    network: state.network,
    user: state.user,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {
    clearGameError: () => dispatch(setGameError(null)),
    clearNetworkError: () => dispatch(setNetworkError(null)),
    clearUserError: () => dispatch(setUserError(null)),
  };
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(ErrorNotification);
