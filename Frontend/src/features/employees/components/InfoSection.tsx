import { Box, Typography } from '@mui/material';
import type { ReactNode } from 'react';

interface InfoSectionProps {
  icon: ReactNode;
  title: string;
  subtitle: string;
  headerBg: string;
  accentColor: string;
  children: ReactNode;
}

export default function InfoSection({ icon, title, subtitle, headerBg, accentColor, children }: InfoSectionProps) {
  return (
    <Box
      sx={{
        borderRadius: '14px',
        border: '1px solid #E5E7EB',
        overflow: 'hidden',
        bgcolor: '#FFF',
        boxShadow: '0 1px 3px rgba(0,0,0,.04)',
        position: 'relative',
      }}
    >
      {/* Left accent bar */}
      <Box
        sx={{
          position: 'absolute',
          left: 0,
          top: 0,
          width: 4,
          height: '100%',
          bgcolor: accentColor,
          borderTopLeftRadius: 14,
          borderBottomLeftRadius: 14,
        }}
      />
      <Box
        sx={{
          height: 52,
          px: '20px',
          pl: '32px',
          bgcolor: headerBg,
          borderBottom: '1px solid #E5E7EB',
          display: 'flex',
          alignItems: 'center',
          gap: 1.25,
        }}
      >
        <Box sx={{ color: accentColor, display: 'flex', lineHeight: 0, '& .MuiSvgIcon-root': { fontSize: '1.1rem' } }}>
          {icon}
        </Box>
        <Box>
          <Typography sx={{ fontSize: 18, fontWeight: 600, color: '#111827', lineHeight: 1.25 }}>
            {title}
          </Typography>
          <Typography sx={{ fontSize: 10, fontWeight: 400, color: '#6B7280', lineHeight: 1.3 }}>
            {subtitle}
          </Typography>
        </Box>
      </Box>
      <Box sx={{ py: 0.75 }}>
        {children}
      </Box>
    </Box>
  );
}
