import './accountBar.scss';
import { FunctionComponent } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { Button } from '../../components/button/button';
import window from '@funfair-tech/wallet-sdk/window';
import {
  clearUserState,
} from '../../store/actions/user.actions';
import { gameService } from '../../services/game.service';
import { Coin } from '../../components/coin/coin';
// import store from '../../store/store';
interface Props extends ReduxProps {
  title: string;
}

export const AccountBar: FunctionComponent<Props> = (props) => {
  // const loginToWallet = () => {
  //   window.funwallet.sdk.auth.login();
  // };

  const logOutOfWallet = async () => {
    await window.funwallet.sdk.auth.logout();
    props.clearUserState();
  };

  return (
    <div className='accountBar'>
      <section className='accountBar__content'>
        {/* <section>{props.title}</section> */}
        <section className='accountBar__logo'>
          <img src={process.env.PUBLIC_URL + '/logo.png'} alt='Lab Rats' />
        </section>

        <section className='accountBar__balance'>
          <Coin visible={!!props.user.displayBalance} />
          {props.user.displayBalance}
        </section>
{/* 
        <section>
          <Button
            disabled={
              !props.game.canPlay ||
              (!!props.game.round && props.game.round.time < 0)
            }
            onClick={() => {
              gameService.handlePlay(10000000000, 2);
            }}
          >
            play
          </Button>
        </section> */}

        {/* <section>
          <Button
            onClick={() => {
              store.dispatch(setTokenBalance(0));
            }}
          >
            zero balance
          </Button>
        </section> */}

        <section>
          {/* {props.user.authenticated ? (
            <Button onClick={logOutOfWallet}>Sign out</Button>
          ) : (
            <Button onClick={loginToWallet} disabled={props.user.loading}>
              {props.user.loading ? 'Loading...' : 'Sign in'}
            </Button>
          )} */}
          {props.user.authenticated ? (
            <Button onClick={logOutOfWallet}>Sign out</Button>
          ) : null}
        </section>
      </section>
    </div>
  );
};

const mapStateToProps = (state: RootState) => {
  return {
    user: state.user,
    game: state.game,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {
    clearUserState: () => dispatch(clearUserState()),
  };
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(AccountBar);
