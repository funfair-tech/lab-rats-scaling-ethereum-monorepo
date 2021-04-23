import { FunctionComponent, useEffect } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { apiRequest } from '../../services/api-request.service';
import { setTransactionHash } from '../../store/actions/network.actions';
import { FaucetRequest } from '../../model/faucetRequst';
import { FaucetResponse } from '../../model/faucetResponse';
import { setUserError } from '../../store/actions/user.actions';

interface Props extends ReduxProps {}

export const Faucet: FunctionComponent<Props> = (props) => {

  useEffect(() => {
    const tokenBalance = props.user.tokenBalance;
    if(props.user.authenticated && !!tokenBalance && tokenBalance < 100) {
      openFaucet();
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.user.authenticated, props.user.tokenBalance]);

  const openFaucet = async () => {
    const request: FaucetRequest = {
      network: props.network.name as string,
      address: props.user.address,
    } 
    const response = await apiRequest.post<FaucetRequest, FaucetResponse>('api/faucet/open', request);

    if(response.message) {
      props.setUserError(response.message)
    } else {
      props.setTransactionHash(response.transaction.transactionHash);
    }
  }

  return null;
}

const mapStateToProps = (state: RootState) => {
  return {
    user: state.user,
    network: state.network,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {
    setTransactionHash: (transactionHash: string) => dispatch(setTransactionHash(transactionHash)),
    setUserError: (error: string) => dispatch(setUserError(error)),
  };
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(Faucet);