import { combineReducers } from 'redux';
import user from './user.reducer';
import network from './network.reducer';
import game from './game.reducer';

const rootReducer = combineReducers({ network, user, game });

export default rootReducer;

export type RootState = ReturnType<typeof rootReducer>;
