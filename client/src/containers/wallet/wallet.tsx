// @ts-ignore sort types
import { WalletLeader } from '@funfair-tech/wallet-react';
import { MessageListeners } from '@funfair-tech/wallet-sdk/dist/src/app/core/services/message/enums';
import {
  AuthenticationCompletedResponse,
  ERC20TokenBalanceChangedResponse,
  NewBlockResponse,
} from '@funfair-tech/wallet-sdk/dist/src/app/core/services/message/models';
import window from '@funfair-tech/wallet-sdk/window';
import BigNumber from 'bignumber.js';
import { Component } from 'react';
import { connect, ConnectedProps } from 'react-redux';
import { BlockHeader } from '../../model/blockHeader';
import { setEtherContext } from '../../services/ether.service';
import { gameService } from '../../services/game.service';
import { messageService } from '../../services/message.service';
import {
  clearNetworkState,
  setBlockHeader,
  setNetworkId,
  setNetworkName,
  setTokenSymbol,
} from '../../store/actions/network.actions';
import {
  clearUserState,
  setAddress,
  setAuthenticated,
  setEthBalance,
  setLoading,
  setTokenBalance,
} from '../../store/actions/user.actions';
import { RootState } from '../../store/reducers';

interface Props extends ReduxProps {}

class Wallet extends Component<Props> {
  get walletUrl(): string {
    const url = window.funwallet.url;
    return url.endsWith('/') ? url.slice(0, -1) : url;
  }

  registerEventListeners = () => {
    window.funwallet.sdk.on(
      MessageListeners.authenticationCompleted,
      (result: AuthenticationCompletedResponse) => {
        if (result.origin === this.walletUrl) {
          const data = result.data.authenticationCompleted;

          this.props.setAuthenticated(true);
          this.props.setAddress(data.ethereumAddress);
          this.props.setNetworkName(data.currentNetwork.name);
          this.props.setNetworkId(data.currentNetwork.id.valueOf());

          messageService.connectToServer(true, data.currentNetwork.name);
        }
      }
    );

    window.funwallet.sdk.on(
      MessageListeners.restoreAuthenticationCompleted,
      (result) => {
        if (result.origin === this.walletUrl) {
          setEtherContext();
          this.props.setLoading(false);
          gameService.subscribeToContractEvents();

        }
      }
    );

    window.funwallet.sdk.on(
      MessageListeners.walletInactivityLoggedOut,
      (result) => {
        if (result.origin === this.walletUrl) {
          this.props.clearNetworkState();
          this.props.clearUserState();
        }
      }
    );

    window.funwallet.sdk.on(
      MessageListeners.walletDeviceDeletedLoggedOut,
      (result) => {
        if (result.origin === this.walletUrl) {
          this.props.clearNetworkState();
          this.props.clearUserState();
        }
      }
    );

    window.funwallet.sdk.on<ERC20TokenBalanceChangedResponse>(
      MessageListeners.erc20TokenBalanceChanged,
      (result: ERC20TokenBalanceChangedResponse) => {
        if (result.origin === this.walletUrl) {
          const balance = new BigNumber(result.data.tokenBalance);
          const symbol = result.data.symbol;

          if (symbol.toLowerCase() === 'weth') {
            this.props.setEthBalance(balance.toNumber());
          } else {
            this.props.setTokenBalance(balance.toNumber());
            this.props.setTokenSymbol(symbol);
          }
        }
      }
    );

    window.funwallet.sdk.on<NewBlockResponse>(
      MessageListeners.newBlock,
      (result: NewBlockResponse) => {
        if (result.origin === this.walletUrl) {
          const networkId = this.props.network.id;
          if (!!networkId && networkId === result.data.networkId) {
            this.props.setBlockHeader(result.data);
            gameService.readBlockHeader(result.data);
          }
        }
      }
    );
  };

  render() {
    return (
      <WalletLeader registerEventListeners={this.registerEventListeners} />
    );
  }
}

const mapStateToProps = (state: RootState) => {
  return {
    network: state.network,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {
    setAddress: (address: string) => dispatch(setAddress(address)),
    setAuthenticated: (isAuthenticated: boolean) =>
      dispatch(setAuthenticated(isAuthenticated)),
    setEthBalance: (ethBalance: number) => dispatch(setEthBalance(ethBalance)),
    setTokenBalance: (tokenBalance: number) =>
      dispatch(setTokenBalance(tokenBalance)),
    setNetworkName: (name: string) => dispatch(setNetworkName(name)),
    setNetworkId: (id: number) => dispatch(setNetworkId(id)),
    setTokenSymbol: (tokenSymbol: string) =>
      dispatch(setTokenSymbol(tokenSymbol)),
    setLoading: (isloading: boolean) => dispatch(setLoading(isloading)),
    clearUserState: () => dispatch(clearUserState()),
    setBlockHeader: (blockHeader: BlockHeader) =>
      dispatch(setBlockHeader(blockHeader)),
    clearNetworkState: () => dispatch(clearNetworkState()),
  };
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(Wallet);
