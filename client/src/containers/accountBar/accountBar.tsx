import './accountBar.scss';
import React, { FunctionComponent } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import { Button } from '../../components/button/button';
import window from '@funfair-tech/wallet-sdk/window';
import { clearUserState } from '../../store/actions/user.actions';
import { gameService } from '../../services/game.service';

interface Props extends ReduxProps {
  title: string;
}

export const AccountBar: FunctionComponent<Props> = (props) => {

  const loginToWallet = () => {
    window.funwallet.sdk.auth.login();
  };

  const logOutOfWallet = async() => {
    await window.funwallet.sdk.auth.logout();
    props.clearUserState();
  };

  return <div className='accountBar'>
    <section className='accountBar__content'>
      <section>{props.title}</section>
      <section>{props.user.tokenBalance}</section>

      <section><Button disabled={!props.game.canPlay} onClick={()=>{gameService.handlePlayWithAbiCoder({
        roundId: '',
        address: '',
        amount: 10000000000,
        data: '0x02'
      })}}>play</Button></section>

      <section>{
        props.user.authenticated ? <Button onClick={logOutOfWallet}>Sign out</Button> :
        <Button onClick={loginToWallet} disabled={props.user.loading}>{props.user.loading ? 'Loading...': 'Sign in'}</Button>
      }</section>
    </section>
  </div>
}

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