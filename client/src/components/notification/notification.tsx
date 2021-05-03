import './notification.scss';
import { FunctionComponent } from 'react';

export enum NotificationType {
  DEFAULT = 'default',
  SUCCESS = 'success',
  INFO = 'info',
  WARN = 'warn',
  ERROR = 'error',
}

interface Props {
  label: string;
  visible: boolean;
  category: NotificationType;
}

export const Notification: FunctionComponent<Props> = (props) => {
  return props.visible ? <div className={`notification ${props.category}`}>
    <div className="notification__title">{props.label}</div>
  </div> : null;
};
