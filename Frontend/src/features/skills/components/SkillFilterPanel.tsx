import FilterListOutlinedIcon from '@mui/icons-material/FilterListOutlined';
import { Button, Drawer, MenuItem, Stack, TextField, Typography } from '@mui/material';
import { skillCategories, skillStatuses } from '../types';

interface SkillFilterPanelProps {
  category: string;
  onCategoryChange: (value: string) => void;
  onClose: () => void;
  onReset: () => void;
  onStatusChange: (value: string) => void;
  open: boolean;
  status: string;
}

export default function SkillFilterPanel({
  category,
  onCategoryChange,
  onClose,
  onReset,
  onStatusChange,
  open,
  status,
}: SkillFilterPanelProps) {
  return (
    <Drawer anchor="right" onClose={onClose} open={open}>
      <Stack spacing={2.5} sx={{ p: 3, width: { xs: 300, sm: 360 } }}>
        <Stack alignItems="center" direction="row" spacing={1}>
          <FilterListOutlinedIcon color="primary" />
          <Typography fontWeight={800} variant="h6">
            Filters
          </Typography>
        </Stack>
        <TextField label="Status" onChange={(event) => onStatusChange(event.target.value)} select value={status}>
          <MenuItem value="All">All Statuses</MenuItem>
          {skillStatuses.map((item) => (
            <MenuItem key={item} value={item}>
              {item}
            </MenuItem>
          ))}
        </TextField>
        <TextField label="Category" onChange={(event) => onCategoryChange(event.target.value)} select value={category}>
          <MenuItem value="All">All Categories</MenuItem>
          {skillCategories.map((item) => (
            <MenuItem key={item} value={item}>
              {item}
            </MenuItem>
          ))}
        </TextField>
        <Button onClick={onReset} variant="outlined">
          Reset Filters
        </Button>
      </Stack>
    </Drawer>
  );
}
