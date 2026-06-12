import { Chip } from '@mui/material';
import type { SkillCategory } from '../types';

const colorByCategory: Record<SkillCategory, string> = {
  Backend: '#e8f5e9',
  Business: '#fff3e0',
  Cloud: '#e3f2fd',
  Data: '#ede7f6',
  Design: '#fce4ec',
  DevOps: '#e0f2f1',
  Frontend: '#e8eaf6',
  Mobile: '#f1f8e9',
  Security: '#ffebee',
  Testing: '#f3e5f5',
};

export default function SkillCategoryChip({ category }: { category: SkillCategory }) {
  return <Chip label={category} size="small" sx={{ bgcolor: colorByCategory[category], color: 'text.primary', fontWeight: 700 }} />;
}
