import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
import FilterListOutlinedIcon from '@mui/icons-material/FilterListOutlined';
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Paper,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
// Redux: dispatches deleteSkill, reads skills list from store
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import SkillFilterPanel from '../components/SkillFilterPanel';
import SkillTable from '../components/SkillTable';
import { deleteSkill } from '../../../redux/slices/skillSlice';
import type { Skill } from '../types';

export default function SkillList() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const skills = useAppSelector((state) => state.skills.skills);
  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState('All');
  const [categoryFilter, setCategoryFilter] = useState('All');
  const [filterOpen, setFilterOpen] = useState(false);
  const [skillToDelete, setSkillToDelete] = useState<Skill | null>(null);
  const [snackbarOpen, setSnackbarOpen] = useState(false);

  const filteredSkills = useMemo(() => {
    const normalized = searchText.trim().toLowerCase();
    return skills.filter((skill) => {
      const matchesSearch = !normalized || [skill.skillCode, skill.skillName, skill.category].join(' ').toLowerCase().includes(normalized);
      const matchesStatus = statusFilter === 'All' || skill.status === statusFilter;
      const matchesCategory = categoryFilter === 'All' || skill.category === categoryFilter;
      return matchesSearch && matchesStatus && matchesCategory;
    });
  }, [categoryFilter, searchText, skills, statusFilter]);

  const confirmDelete = () => {
    if (!skillToDelete) {
      return;
    }
    dispatch(deleteSkill(skillToDelete.id));
    setSkillToDelete(null);
    setSnackbarOpen(true);
  };

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
        <Box>
          <Typography component="h1" fontWeight={900} variant="h4">
            Skills Management
          </Typography>
          <Typography color="text.secondary" mt={0.75}>
            Maintain skill catalog, employee coverage, and search-ready resource capability.
          </Typography>
        </Box>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <Button onClick={() => navigate('/skills/dashboard')} startIcon={<DashboardOutlinedIcon />} variant="outlined">
            Dashboard
          </Button>
          <Button onClick={() => navigate('/skills/create')} startIcon={<AddOutlinedIcon />} variant="contained">
            Add Skill
          </Button>
        </Stack>
      </Stack>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 2 }}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
          <TextField
            InputProps={{ startAdornment: <SearchOutlinedIcon color="action" sx={{ mr: 1 }} /> }}
            fullWidth
            label="Search skills"
            onChange={(event) => setSearchText(event.target.value)}
            value={searchText}
          />
          <Button onClick={() => setFilterOpen(true)} startIcon={<FilterListOutlinedIcon />} sx={{ minWidth: 150 }} variant="outlined">
            Filters
          </Button>
        </Stack>
      </Paper>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
        {filteredSkills.length > 0 ? (
          <SkillTable
            onDelete={setSkillToDelete}
            onEdit={(skill) => navigate(`/skills/edit/${skill.id}`)}
            onView={(skill) => navigate(`/skills/${skill.id}`)}
            skills={filteredSkills}
          />
        ) : (
          <Box alignItems="center" display="flex" flexDirection="column" justifyContent="center" minHeight={320} p={3}>
            <Typography fontWeight={800} variant="h6">
              No skills found
            </Typography>
            <Typography color="text.secondary" mt={1} textAlign="center">
              Adjust search, status, or category filters to find matching skills.
            </Typography>
          </Box>
        )}
      </Paper>

      <SkillFilterPanel
        category={categoryFilter}
        onCategoryChange={setCategoryFilter}
        onClose={() => setFilterOpen(false)}
        onReset={() => {
          setCategoryFilter('All');
          setStatusFilter('All');
        }}
        onStatusChange={setStatusFilter}
        open={filterOpen}
        status={statusFilter}
      />

      <Dialog onClose={() => setSkillToDelete(null)} open={Boolean(skillToDelete)}>
        <DialogTitle>Delete skill?</DialogTitle>
        <DialogContent>
          <DialogContentText>
            This will remove {skillToDelete?.skillName} from the local skill catalog. This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setSkillToDelete(null)}>Cancel</Button>
          <Button color="error" onClick={confirmDelete} variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar autoHideDuration={3000} onClose={() => setSnackbarOpen(false)} open={snackbarOpen}>
        <Alert severity="success" variant="filled">
          Skill deleted successfully.
        </Alert>
      </Snackbar>
    </Stack>
  );
}
