import type { SvgIconComponent } from '@mui/icons-material';
import { Box, Card, Typography } from '@mui/material';

interface KpiCardProps {
  icon: SvgIconComponent;
  label: string;
  value: string | number;
  subtitle?: string;
  color?: string;
}

export default function KpiCard({
  icon: Icon,
  label,
  value,
  subtitle,
  color = 'primary.main',
}: KpiCardProps) {
  return (
    <Card
      elevation={0}
      sx={{
        border: '1px solid',
        borderColor: 'divider',
        borderRadius: 2,
        minHeight: 110,
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        gap: 0.25,
        py: 1.5,
        px: 1.5,
        textAlign: 'center',
        transition: 'all 0.25s ease',
        '&:hover': {
          borderColor: color,
          boxShadow: 4,
          transform: 'translateY(-1px)',
          cursor: 'pointer',
        },
      }}
    >
      <Box
        sx={{
          width: 36,
          height: 36,
          borderRadius: '50%',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          bgcolor: `${color}14`,
          color,
          mb: 0.25,
        }}
      >
        <Icon sx={{ fontSize: 18 }} />
      </Box>
      <Typography variant="caption" color="text.secondary" fontWeight={600} fontSize={11}>
        {label}
      </Typography>
      <Typography variant="h5" fontWeight={800} color={color}>
        {value}
      </Typography>
      {subtitle && (
        <Typography variant="caption" color="text.secondary" fontSize={10}>
          {subtitle}
        </Typography>
      )}
    </Card>
  );
}
