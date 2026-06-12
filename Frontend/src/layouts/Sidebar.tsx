import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import FolderSharedOutlinedIcon from '@mui/icons-material/FolderSharedOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import HubOutlinedIcon from '@mui/icons-material/HubOutlined';
import PsychologyOutlinedIcon from '@mui/icons-material/PsychologyOutlined';
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import {
  Box,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
  Typography,
  useMediaQuery,
} from '@mui/material';
import type { Theme } from '@mui/material/styles';
import { NavLink, useLocation } from 'react-router-dom';

export const DRAWER_WIDTH = 280;
export const COLLAPSED_DRAWER_WIDTH = 72;

const menuItems = [
  //{ icon: DashboardOutlinedIcon, label: 'Home', path: '/home' },
  { icon: DashboardOutlinedIcon, label: 'Dashboard', path: '/dashboard' },
  { icon: GroupsOutlinedIcon, label: 'Employee Management', path: '/employees' },
  { icon: WorkOutlineOutlinedIcon, label: 'Project Management', path: '/projects' },
  { icon: PsychologyOutlinedIcon, label: 'Skills Management', path: '/skills' },
  { icon: HubOutlinedIcon, label: 'Resource Allocation', path: '/allocations' },
  { icon: AssessmentOutlinedIcon, label: 'Reports', path: '/reports' },
  { icon: SettingsOutlinedIcon, label: 'Settings', path: '/settings' },
];

interface SidebarProps {
  collapsed: boolean;
  mobileOpen: boolean;
  onMobileClose: () => void;
}

export default function Sidebar({ collapsed, mobileOpen, onMobileClose }: SidebarProps) {
  const isDesktop = useMediaQuery((theme: Theme) => theme.breakpoints.up('md'));
  const drawerWidth = collapsed ? COLLAPSED_DRAWER_WIDTH : DRAWER_WIDTH;
  const location = useLocation();

  const drawerContent = (
    <Box sx={{ height: '100%', overflowX: 'hidden', px: 1.5, py: 2 }}>
      <Stack alignItems="center" direction="row" spacing={1.5} sx={{ minHeight: 56, px: 1 }}>
        <Box
          alignItems="center"
          bgcolor="primary.main"
          borderRadius={2}
          color="primary.contrastText"
          display="flex"
          height={40}
          justifyContent="center"
          width={40}
        >
          <FolderSharedOutlinedIcon />
        </Box>
        {!collapsed ? (
          <Typography color="primary" fontWeight={800} noWrap variant="subtitle1">
            HRMS
          </Typography>
        ) : null}
      </Stack>
      <List sx={{ mt: 2 }}>
        {menuItems.map((item) => {
          const Icon = item.icon;

          return (
            <ListItemButton
              component={NavLink}
              key={item.path}
              onClick={isDesktop ? undefined : onMobileClose}
              selected={
                item.path === '/projects' || item.path === '/skills' ? location.pathname.startsWith(item.path) : location.pathname === item.path
              }
              sx={{
                borderRadius: 2,
                minHeight: 48,
                my: 0.5,
                px: collapsed ? 1.5 : 2,
                '&.active, &.Mui-selected, &.Mui-selected:hover': {
                  bgcolor: 'primary.main',
                  color: 'primary.contrastText',
                  '& .MuiListItemIcon-root': {
                    color: 'primary.contrastText',
                  },
                },
              }}
              to={item.path}
            >
              <ListItemIcon sx={{ color: 'text.secondary', minWidth: collapsed ? 0 : 40 }}>
                <Icon />
              </ListItemIcon>
              {!collapsed ? <ListItemText primary={item.label} primaryTypographyProps={{ fontWeight: 700 }} /> : null}
            </ListItemButton>
          );
        })}
      </List>
    </Box>
  );

  return (
    <>
      <Drawer
        ModalProps={{ keepMounted: true }}
        onClose={onMobileClose}
        open={mobileOpen}
        sx={{
          display: { xs: 'block', md: 'none' },
          '& .MuiDrawer-paper': {
            boxSizing: 'border-box',
            width: DRAWER_WIDTH,
          },
        }}
        variant="temporary"
      >
        {drawerContent}
      </Drawer>
      <Drawer
        open
        sx={{
          display: { xs: 'none', md: 'block' },
          flexShrink: 0,
          width: drawerWidth,
          '& .MuiDrawer-paper': {
            borderRight: '1px solid',
            borderColor: 'divider',
            boxSizing: 'border-box',
            transition: (theme) => theme.transitions.create('width'),
            width: drawerWidth,
          },
        }}
        variant="permanent"
      >
        {drawerContent}
      </Drawer>
    </>
  );
}
