import { Chip } from '@mui/material';
import type { SkillStatus } from '../types';

const colorByStatus: Record<SkillStatus, 'success' | 'warning' | 'default'> = {
  Active: 'success',
  Deprecated: 'warning',
  Inactive: 'default',
};

export default function SkillStatusChip({ status }: { status: SkillStatus }) {
  return <Chip color={colorByStatus[status]} label={status} size="small" variant={status === 'Inactive' ? 'outlined' : 'filled'} />;
}
