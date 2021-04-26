import './notification.scss';
import { FunctionComponent } from 'react';
interface Props {
  label: string;
  visible: boolean;
}

export const Notification: FunctionComponent<Props> = (props) => {
  return props.visible ? <div className='notification'>
    <div className="notification__title">{props.label}</div>
  </div> : null;
};
