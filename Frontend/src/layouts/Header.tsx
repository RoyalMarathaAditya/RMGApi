import LogoutIcon from '@mui/icons-material/Logout';
import MenuIcon from '@mui/icons-material/Menu';
import NotificationsOutlinedIcon from '@mui/icons-material/NotificationsOutlined';
import {
  AppBar,
  Avatar,
  Box,
  IconButton,
  Stack,
  Toolbar,
  Tooltip,
  Typography,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
// Redux: dispatch reads user from state for display, dispatches logout on click
import { useAppDispatch, useAppSelector } from '../redux/hooks';
import { logout } from '../redux/slices/authSlice';

interface HeaderProps {
  drawerWidth: number;
  isSidebarCollapsed: boolean;
  onSidebarToggle: () => void;
}

export default function Header({ drawerWidth, isSidebarCollapsed, onSidebarToggle }: HeaderProps) {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const user = useAppSelector((state) => state.auth.user);
  const appBarWidth = { md: `calc(100% - ${isSidebarCollapsed ? 72 : drawerWidth}px)` };
  const appBarMargin = { md: `${isSidebarCollapsed ? 72 : drawerWidth}px` };

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login', { replace: true });
  };

  return (
    <AppBar
      color="inherit"
      elevation={0}
      position="fixed"
      sx={{
        borderBottom: '1px solid',
        borderColor: 'divider',
        ml: appBarMargin,
        width: appBarWidth,
        zIndex: (theme) => theme.zIndex.drawer + 1,
      }}
    >
      <Toolbar sx={{ minHeight: 72, px: { xs: 2, md: 3 } }}>
        <IconButton aria-label="Toggle navigation" edge="start" onClick={onSidebarToggle} sx={{ mr: 2 }}>
          <MenuIcon />
        </IconButton>
        <Box sx={{ flexGrow: 1, minWidth: 0 }}>
          <Typography color="primary" fontWeight={800} noWrap variant="h6">
            HRMS Resource Management
          </Typography>
        </Box>
        <Stack alignItems="center" direction="row" spacing={{ xs: 1, sm: 2 }}>
          <Tooltip title="Notifications">
            <IconButton aria-label="Notifications">
              <NotificationsOutlinedIcon />
            </IconButton>
          </Tooltip>
          <Stack alignItems="center" direction="row" spacing={1} sx={{ display: { xs: 'none', sm: 'flex' } }}>
            <Avatar sx={{ bgcolor: 'secondary.main', height: 36, width: 36 }}>
              {user?.name.charAt(0).toUpperCase() ?? 'A'}
            </Avatar>
            <Typography fontWeight={700} noWrap variant="body2">
              {user?.name ?? 'Admin'}
            </Typography>
          </Stack>
          <Tooltip title="Logout">
            <IconButton aria-label="Logout" color="primary" onClick={handleLogout}>
              <LogoutIcon />
            </IconButton>
          </Tooltip>
        </Stack>
      </Toolbar>
    </AppBar>
  );
}
