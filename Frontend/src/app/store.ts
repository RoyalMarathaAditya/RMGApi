import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../features/auth/authSlice';
import employeeReducer from '../features/employees/store/employeeSlice';
import projectReducer from '../features/projects/store/projectSlice';
import skillReducer from '../features/skills/skillSlice';
import allocationReducer from '../features/allocations/store/allocationSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    employees: employeeReducer,
    projects: projectReducer,
    skills: skillReducer,
    allocations: allocationReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
