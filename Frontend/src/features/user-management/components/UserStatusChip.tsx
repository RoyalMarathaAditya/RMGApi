import { Chip } from '@mui/material';

const statusConfig: Record<string, { color: 'success' | 'error' | 'warning' | 'default' | 'info'; label: string }> = {
  active: { color: 'success', label: 'Active' },
  inactive: { color: 'error', label: 'Inactive' },
  locked: { color: 'warning', label: 'Locked' },
  deleted: { color: 'default', label: 'Deleted' },
};

interface UserStatusChipProps {
  isActive: boolean;
  isLocked: boolean;
  isDeleted?: boolean;
}

export default function UserStatusChip({ isActive, isLocked, isDeleted }: UserStatusChipProps) {
  const key = isDeleted ? 'deleted' : isLocked ? 'locked' : isActive ? 'active' : 'inactive';
  const { color, label } = statusConfig[key];
  return <Chip color={color} label={label} size="small" variant="outlined" />;
}
