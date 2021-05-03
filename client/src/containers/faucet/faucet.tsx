import { FunctionComponent, useEffect } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { apiRequest } from '../../services/api-request.service';
import { setTransactionHash } from '../../store/actions/network.actions';
import { FaucetRequest } from '../../model/faucetRequst';
import { FaucetResponse } from '../../model/faucetResponse';
import { setUserError } from '../../store/actions/user.actions';
import { Notification, NotificationType } from '../../components/notification/notification';
import { ethers } from '../../services/ether.service';
import { ErrorCode, LRError } from '../../model/errorCodes';

interface Props extends ReduxProps {}

export const Faucet: FunctionComponent<Props> = (props) => {
  useEffect(() => {
    const tokenBalance = props.user.tokenBalance;
    if (
      !props.user.loading &&
      !!props.user.address &&
      !!props.network.id &&
      tokenBalance !== null &&
      tokenBalance < 100
    ) {
      openFaucet();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [
    props.user.loading,
    props.user.address,
    props.user.tokenBalance,
    props.network.id,
  ]);

  const openFaucet = async () => {

    const request: FaucetRequest = {
      network: props.network.name as string,
      address: props.user.address,
    };
    const response = await apiRequest.post<FaucetRequest, FaucetResponse>(
      'api/faucet/open',
      request
    );

    if (response.message) {
      props.setUserError({code: ErrorCode.FAUCET_ERROR, msg:response.message});
      // TODO: possibly retry the call here ... (add some sort of exit)
      // props.setTransactionHash('0x');
      // setTimeout(() => {
      //   openFaucet();
      // }, 10000);
    } else {
      props.setTransactionHash(response.transaction.transactionHash);
      await ethers.waitForTransactionReceipt(
        response.transaction.transactionHash
      );
      props.setTransactionHash(null);
    }
  };

  return (
    <Notification
      label='Funding your account'
      visible={!!props.network.transactionHash}
      category={NotificationType.INFO}
    />
  );
};

const mapStateToProps = (state: RootState) => {
  return {
    user: state.user,
    network: state.network,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {
    setTransactionHash: (transactionHash: string | null) =>
      dispatch(setTransactionHash(transactionHash)),
    setUserError: (error: LRError) => dispatch(setUserError(error)),
  };
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(Faucet);
