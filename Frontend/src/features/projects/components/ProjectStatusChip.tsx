import { Chip } from '@mui/material';
import type { ChipProps } from '@mui/material';
import type { ProjectStatus } from '../types/project.types';

const statusColorMap: Record<ProjectStatus, ChipProps['color']> = {
  Planned: 'info',
  Active: 'success',
  Completed: 'primary',
  'On Hold': 'warning',
  Cancelled: 'error',
};

export default function ProjectStatusChip({ status }: { status: ProjectStatus }) {
  return <Chip color={statusColorMap[status]} label={status} size="small" variant="outlined" />;
}
