import './coin.scss';
import { FunctionComponent } from 'react';

interface Props {
  visible: boolean;
}

export const Coin: FunctionComponent<Props> = (props) => {
  return props.visible ? <div className="coin gold"><p>&#128000;</p></div> : null;

}