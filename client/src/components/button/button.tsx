import './button.scss';
import { FunctionComponent } from 'react';

interface Props {
  onClick: () => void;
}

export const Button: FunctionComponent<Props> = (props) => {
  return <button className='btn' onClick={props.onClick}>{props.children}</button>
}