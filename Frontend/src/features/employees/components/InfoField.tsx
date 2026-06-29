import { Box, Typography } from '@mui/material';
import type { ReactNode } from 'react';

interface InfoFieldProps {
  icon: ReactNode;
  label: string;
  value: ReactNode;
  colSpan?: number;
}

export default function InfoField({ icon, label, value, colSpan }: InfoFieldProps) {
  return (
    <Box
      sx={{
        gridColumn: colSpan ? `span ${colSpan}` : undefined,
        py: '3px',
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: '2px' }}>
        <Box sx={{ color: '#6B7280', display: 'flex', fontSize: '0.85rem', lineHeight: 0 }}>
          {icon}
        </Box>
        <Typography
          component="span"
          sx={{
            fontSize: 11,
            fontWeight: 500,
            color: '#4B5563',
            textTransform: 'uppercase',
            letterSpacing: '0.4px',
            lineHeight: 1,
          }}
        >
          {label}
        </Typography>
      </Box>
      <Typography
        sx={{
          fontSize: 14,
          fontWeight: 500,
          color: '#111827',
          lineHeight: 1.4,
          wordBreak: 'break-word',
        }}
      >
        {value ?? '—'}
      </Typography>
    </Box>
  );
}
