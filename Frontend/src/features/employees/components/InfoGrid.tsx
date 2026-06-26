import { Box } from '@mui/material';
import type { ReactNode } from 'react';

interface InfoGridProps {
  children: ReactNode;
}

export default function InfoGrid({ children }: InfoGridProps) {
  return (
    <Box
      sx={{
        display: 'grid',
        gridTemplateColumns: { xs: '1fr', sm: 'repeat(2,1fr)', md: 'repeat(4, 1fr)' },
        gap: 0,
        '& > *': {
          px: '16px',
          py: 0,
          borderBottom: '1px solid #F1F5F9',
          borderRight: { xs: 'none', md: '1px solid #EEF2F7' },
          '&:nth-of-type(4n)': { borderRight: 'none' },
        },
        '& > :last-child': { borderBottom: 'none' },
      }}
    >
      {children}
    </Box>
  );
}
