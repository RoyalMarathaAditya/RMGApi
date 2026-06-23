import { useEffect } from 'react';
// Redux: useAppDispatch dispatches actions to the Redux store
import { useAppDispatch } from './redux/hooks';
import { AppRoutes } from './app/routes';
// Redux: initializeAuth restores auth state on page refresh; logout clears it
import { initializeAuth, logout } from './redux/slices/authSlice';

export default function App() {
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(initializeAuth());

    const handleExpired = () => dispatch(logout());
    window.addEventListener('auth:expired', handleExpired);
    return () => window.removeEventListener('auth:expired', handleExpired);
  }, [dispatch]);

  return <AppRoutes />;
}
