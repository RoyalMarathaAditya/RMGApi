import { Box, Paper, Typography } from '@mui/material';
import type { ReactNode } from 'react';

interface PageContainerProps {
  children: ReactNode;
  title?: string;
}

export default function PageContainer({ children, title }: PageContainerProps) {
  return (
    <Box component="section" sx={{ width: '100%' }}>
      {title ? (
        <Typography component="h1" fontWeight={700} gutterBottom variant="h4">
          {title}
        </Typography>
      ) : null}
      <Paper
        elevation={0}
        sx={{
          border: '1px solid',
          borderColor: 'divider',
          borderRadius: 2,
          color: 'text.secondary',
          p: { xs: 2, md: 3 },
        }}
      >
        {children}
      </Paper>
    </Box>
  );
}
