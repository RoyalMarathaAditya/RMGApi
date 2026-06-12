import { Box } from '@mui/material';
import { useState } from 'react';
import { Outlet } from 'react-router-dom';
import Header from './Header';
import Sidebar, { COLLAPSED_DRAWER_WIDTH, DRAWER_WIDTH } from './Sidebar';

export default function MainLayout() {
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileOpen, setIsMobileOpen] = useState(false);
  const drawerWidth = isSidebarCollapsed ? COLLAPSED_DRAWER_WIDTH : DRAWER_WIDTH;

  const handleSidebarToggle = () => {
    if (window.matchMedia('(min-width: 900px)').matches) {
      setIsSidebarCollapsed((current) => !current);
      return;
    }

    setIsMobileOpen((current) => !current);
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <Header
        drawerWidth={DRAWER_WIDTH}
        isSidebarCollapsed={isSidebarCollapsed}
        onSidebarToggle={handleSidebarToggle}
      />
      <Sidebar
        collapsed={isSidebarCollapsed}
        mobileOpen={isMobileOpen}
        onMobileClose={() => setIsMobileOpen(false)}
      />
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: { xs: 2, md: 3 },
          pt: { xs: 11, md: 12 },
          transition: (theme) => theme.transitions.create('width'),
          width: { md: `calc(100% - ${drawerWidth}px)` },
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}
