import { Navigate, Outlet, useLocation } from 'react-router-dom';
// Redux: reads auth state to determine if user is authenticated; shows loader during init
import { useAppSelector } from '../../redux/hooks';
import Loader from './Loader';

export default function ProtectedRoute() {
  const { isAuthenticated, loading } = useAppSelector((state) => state.auth);
  const location = useLocation();

  if (loading) {
    return <Loader />;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
}
