import TrendingUpOutlinedIcon from '@mui/icons-material/TrendingUpOutlined';
import { Card, CardContent, Stack, Typography } from '@mui/material';
import type { ReactNode } from 'react';

interface SkillCardProps {
  icon?: ReactNode;
  label: string;
  value: string | number;
  helper?: string;
}

export default function SkillCard({ helper, icon, label, value }: SkillCardProps) {
  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: '100%' }}>
      <CardContent>
        <Stack direction="row" justifyContent="space-between" spacing={2}>
          <Stack spacing={0.5}>
            <Typography color="text.secondary" fontWeight={700} variant="body2">
              {label}
            </Typography>
            <Typography component="p" fontWeight={900} variant="h4">
              {value}
            </Typography>
            {helper ? (
              <Typography color="text.secondary" variant="body2">
                {helper}
              </Typography>
            ) : null}
          </Stack>
          {icon ?? <TrendingUpOutlinedIcon color="primary" />}
        </Stack>
      </CardContent>
    </Card>
  );
}
