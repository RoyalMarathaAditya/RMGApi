import { Box, useTheme } from '@mui/material';
import type { ReactNode } from 'react';

interface InfoGridProps {
  children: ReactNode;
}

export default function InfoGrid({ children }: InfoGridProps) {
  const theme = useTheme();
  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: { xs: '1fr', sm: 'repeat(2,1fr)', md: 'repeat(4, 1fr)' },
        gap: 0,
        '& > *': {
          px: '16px',
          py: 0,
          borderBottom: `1px solid ${theme.palette.divider}`,
          borderRight: { xs: 'none', md: `1px solid ${theme.palette.divider}` },
          '&:nth-of-type(4n)': { borderRight: 'none' },
        },
        '& > :last-child': { borderBottom: 'none' },
      }}
    >
      {children}
    </Box>
  );
}
