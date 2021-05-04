import './splashScreen.scss';
import { FunctionComponent } from 'react';
import { Button } from '../../components/button/button';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';
import window from '@funfair-tech/wallet-sdk/window';

interface Props extends ReduxProps {}

const SplashScreen: FunctionComponent<Props> = (props) => {

  const handleLogin = () => {
    window.funwallet.sdk.auth.login();
  };

  return !props.user.authenticated ? (
    <div className='splash'>
      {/* <section className='splash__header'>
        <section className='splash__logo'>
          <img
            src={process.env.PUBLIC_URL + '/logo.png'}
            className=''
            alt='Lab Rats'
          />
        </section>
      </section> */}
      <section className='splash__hero'>
        <section className='splash__cta'>
          <section className='splash__cta__title'>
            <h1>Rat Trace</h1>
          </section>
          <section className='splash__cta__description'>
            <p>
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus
              dapibus auctor leo a hendrerit. Sed dui dui, accumsan a libero
              nec, tempus molestie augue. Cras sed arcu a diam fringilla aliquet
              quis eget nulla.
            </p>
          </section>
          <section className='splash__cta__signin'>
            <Button onClick={handleLogin} disabled={props.user.loading}>
              {props.user.loading ? 'Loading...' : 'Sign in'}
            </Button>
          </section>
        </section>
        <section className='splash__media'>
          <section className='splash__media__image'>
            <img
              src={process.env.PUBLIC_URL + '/media.png'}
              className=''
              alt='Lab Rats'
            />
          </section>
        </section>
      </section>
    </div>
  ) : null;
};


const mapStateToProps = (state: RootState) => {
  return {
    user: state.user,
    game: state.game,
  };
};

const mapDispatchToProps = (dispatch: Function) => {
  return {};
};

const connector = connect(mapStateToProps, mapDispatchToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(SplashScreen);