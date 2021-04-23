import { FunctionComponent, useEffect } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { apiRequest } from '../../services/api-request.service';
import { setTransactionHash } from '../../store/actions/network.actions';

interface Props extends ReduxProps {}

export const Faucet: FunctionComponent<Props> = (props) => {

  useEffect(() => {
    const tokenBalance = props.user.tokenBalance;
    if(props.user.authenticated && !!tokenBalance && tokenBalance < 100) {
      openFaucet();
    }
  }, [props.user.authenticated, props.user.tokenBalance]);

  const openFaucet = async () => {
    const response = await apiRequest.get<string>(apiRequest.buildEndpoint('faucet'));
    props.setTransactionHash(response);
  }

  return null;
}

const mapStateToProps = (state: RootState) => {
  return {
    user: state.user,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {
    setTransactionHash: (transactionHash: string) => dispatch(setTransactionHash(transactionHash)),
  };
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(Faucet);