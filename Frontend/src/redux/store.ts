import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import clientReducer from './slices/clientSlice';
import employeeReducer from './slices/employeeSlice';
import projectReducer from './slices/projectSlice';
import projectManagementReducer from './slices/projectManagementSlice';
import skillReducer from './slices/skillSlice';
import designationReducer from './slices/designationSlice';

// Redux store configuration combining all feature slices
export const store = configureStore({
  reducer: {
    auth: authReducer,
    clients: clientReducer,
    employees: employeeReducer,
    projects: projectReducer,
    projectManagement: projectManagementReducer,
    skills: skillReducer,
    designations: designationReducer,
  },
});

// TypeScript types for dispatch and state used by typed hooks
export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
