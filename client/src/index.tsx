// @ts-ignore sort types
import { WalletLeader } from '@funfair-tech/wallet-react';
import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import App from './App';
import './index.scss';
import reportWebVitals from './reportWebVitals';
import store from './store/store';

const registerEventListeners = () => {
  //TODO
};

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <WalletLeader registerEventListeners={registerEventListeners} />
      <App />
    </Provider>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
