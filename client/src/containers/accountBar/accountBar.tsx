import './accountBar.scss';
import { FunctionComponent } from 'react';
import { RootState } from '../../store/reducers';
import { connect, ConnectedProps } from 'react-redux';

interface Props extends ReduxProps {
  title: string;
}

export const AccountBar: FunctionComponent<Props> = (props) => {
  return <div className='accountBar'>
    <section>{props.title}</section>
    <section>{props.user.address}</section>
  </div>
}

const mapStateToProps = (state: RootState) => {
  return {
    user: state.user,
  };
};


const connector = connect(mapStateToProps);

type ReduxProps = ConnectedProps<typeof connector>;

export default connector(AccountBar);