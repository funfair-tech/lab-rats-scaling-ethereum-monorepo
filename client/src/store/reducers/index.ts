import { combineReducers } from 'redux';
import user from './user.reducer';
import network from './network.reducer';

const rootReducer = combineReducers({ network, user });

export default rootReducer;

export type RootState = ReturnType<typeof rootReducer>;
