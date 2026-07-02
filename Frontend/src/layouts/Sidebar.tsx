import AdminPanelSettingsOutlinedIcon from '@mui/icons-material/AdminPanelSettingsOutlined';
import AssessmentOutlinedIcon from '@mui/icons-material/AssessmentOutlined';
import BadgeOutlinedIcon from '@mui/icons-material/BadgeOutlined';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';

import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import HubOutlinedIcon from '@mui/icons-material/HubOutlined';
import ListAltOutlinedIcon from '@mui/icons-material/ListAltOutlined';
import PsychologyOutlinedIcon from '@mui/icons-material/PsychologyOutlined';
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import {
  Box,
  Collapse,
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
import { useState } from 'react';
import { NavLink, useLocation } from 'react-router-dom';

export const DRAWER_WIDTH = 280;
export const COLLAPSED_DRAWER_WIDTH = 72;

interface MenuItem {
  icon: React.ElementType;
  label: string;
  path?: string;
  children?: { icon: React.ElementType; label: string; path: string }[];
}

const menuItems: MenuItem[] = [
  { icon: DashboardOutlinedIcon, label: 'Dashboard', path: '/dashboard' },
  {
    icon: GroupsOutlinedIcon, label: 'Employee Management',
    children: [
      { icon: ListAltOutlinedIcon, label: 'Employee List', path: '/employees' },
      { icon: AdminPanelSettingsOutlinedIcon, label: 'Excel Column Mappings', path: '/admin/column-mappings' },
      { icon: SettingsOutlinedIcon, label: 'Excel Value Mappings', path: '/admin/column-value-mappings' },
    ],
  },
  { icon: HubOutlinedIcon, label: 'Resource Allocation', path: '/rmg' },
  { icon: AssessmentOutlinedIcon, label: 'Reports', path: '/reports' },
  { icon: BadgeOutlinedIcon, label: 'Designation Master', path: '/designations' },
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
  const [expandedMenus, setExpandedMenus] = useState<Record<string, boolean>>({
    'Employee Management': true,
  });

  const isActive = (path?: string) => {
    if (!path) return false;
    if (path === '/projects' || path === '/skills' || path === '/rmg') {
      return location.pathname.startsWith(path);
    }
    return location.pathname === path;
  };

  const isParentActive = (item: MenuItem) => {
    if (item.path && isActive(item.path)) return true;
    if (item.children) return item.children.some((child) => isActive(child.path));
    return false;
  };

  const toggleExpand = (label: string) => {
    setExpandedMenus((prev) => ({ ...prev, [label]: !prev[label] }));
  };

  const drawerContent = (
    <Box sx={{ height: '100%', overflowX: 'hidden', px: 1.5, py: 2 }}>
      <Stack alignItems="center" direction="row" spacing={1.5} sx={{ minHeight: 56, px: 1 }}>
        <Box
          component="img"
          src="/logo.png"
          alt="HRMS Logo"
          sx={{
            height: 64,
            width: 64,
            objectFit: 'contain',
            borderRadius: 1,
          }}
        />
        {!collapsed ? (
          <Typography color="primary" fontWeight={800} noWrap variant="subtitle1">
            HRMS
          </Typography>
        ) : null}
      </Stack>
      <List sx={{ mt: 2 }}>
        {menuItems.map((item) => {
          const Icon = item.icon;
          const hasChildren = !!item.children?.length;
          const parentActive = isParentActive(item);

          return (
            <Box key={item.label}>
              <ListItemButton
                onClick={() => {
                  if (hasChildren) {
                    toggleExpand(item.label);
                  }
                  if (!isDesktop) onMobileClose();
                }}
                component={hasChildren ? 'div' : NavLink}
                {...(hasChildren ? {} : { to: item.path! })}
                selected={parentActive && !hasChildren}
                sx={{
                  borderRadius: 2,
                  minHeight: 48,
                  my: 0.5,
                  px: collapsed ? 1.5 : 2,
                  bgcolor: parentActive && hasChildren ? 'action.selected' : 'transparent',
                  '&.active, &.Mui-selected, &.Mui-selected:hover': {
                    bgcolor: 'primary.main',
                    color: 'primary.contrastText',
                    '& .MuiListItemIcon-root': {
                      color: 'primary.contrastText',
                    },
                  },
                }}
              >
                <ListItemIcon sx={{ color: 'text.secondary', minWidth: collapsed ? 0 : 40 }}>
                  <Icon />
                </ListItemIcon>
                {!collapsed ? (
                  <>
                    <ListItemText primary={item.label} primaryTypographyProps={{ fontWeight: 700 }} />
                    {hasChildren ? (
                      expandedMenus[item.label] ? <ExpandLess /> : <ExpandMore />
                    ) : null}
                  </>
                ) : null}
              </ListItemButton>
              {hasChildren && !collapsed ? (
                <Collapse in={expandedMenus[item.label]} timeout="auto" unmountOnExit>
                  <List disablePadding>
                    {item.children!.map((child) => {
                      const ChildIcon = child.icon;
                      return (
                        <ListItemButton
                          component={NavLink}
                          key={child.path}
                          onClick={isDesktop ? undefined : onMobileClose}
                          selected={isActive(child.path)}
                          sx={{
                            borderRadius: 2,
                            minHeight: 40,
                            my: 0.25,
                            pl: 5,
                            '&.active, &.Mui-selected, &.Mui-selected:hover': {
                              bgcolor: 'primary.main',
                              color: 'primary.contrastText',
                              '& .MuiListItemIcon-root': {
                                color: 'primary.contrastText',
                              },
                            },
                          }}
                          to={child.path}
                        >
                          <ListItemIcon sx={{ color: 'text.secondary', minWidth: 32 }}>
                            <ChildIcon fontSize="small" />
                          </ListItemIcon>
                          <ListItemText primary={child.label} primaryTypographyProps={{ fontSize: '0.9rem', fontWeight: 600 }} />
                        </ListItemButton>
                      );
                    })}
                  </List>
                </Collapse>
              ) : null}
            </Box>
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
