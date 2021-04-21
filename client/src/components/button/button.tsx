import './button.scss';
import { FunctionComponent } from 'react';

interface Props {
  onClick: () => void;
  disabled?: boolean;
}

export const Button: FunctionComponent<Props> = (props) => {
  return <button className='btn' onClick={props.onClick} disabled={props.disabled}>{props.children}</button>
}