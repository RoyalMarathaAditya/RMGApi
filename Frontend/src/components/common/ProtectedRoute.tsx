import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAppSelector } from '../../redux/hooks';
import Loader from './Loader';

export default function ProtectedRoute() {
  const { isAuthenticated, loading, forcePasswordChange } = useAppSelector((state) => state.auth);
  const location = useLocation();

  if (loading) {
    return <Loader />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (forcePasswordChange) {
    return <Navigate to="/change-password" replace />;
  }

  return <Outlet />;
}
