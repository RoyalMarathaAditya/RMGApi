import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import DashboardOutlinedIcon from '@mui/icons-material/DashboardOutlined';
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
  MenuItem,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
// Redux: dispatches deleteProject, reads projects list from store
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import ProjectTable from '../components/ProjectTable';
import { projectStatuses } from '../mock/projects';
import { deleteProject } from '../../../redux/slices/projectSlice';
import type { Project, ProjectStatus } from '../types/project.types';
import { toastService } from '../../../services/toastService';

export default function ProjectList() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const projects = useAppSelector((state) => state.projects.projects);
  const [searchText, setSearchText] = useState('');
  const [statusFilter, setStatusFilter] = useState<ProjectStatus | 'All'>('All');
  const [projectToDelete, setProjectToDelete] = useState<Project | null>(null);

  const filteredProjects = useMemo(() => {
    const normalizedSearch = searchText.trim().toLowerCase();

    return projects.filter((project) => {
      const matchesStatus = statusFilter === 'All' || project.status === statusFilter;
      const matchesSearch =
        !normalizedSearch ||
        [project.projectCode, project.projectName, project.clientName, project.projectManager]
          .join(' ')
          .toLowerCase()
          .includes(normalizedSearch);

      return matchesStatus && matchesSearch;
    });
  }, [projects, searchText, statusFilter]);

  const handleDeleteConfirm = () => {
    if (!projectToDelete) {
      return;
    }

    dispatch(deleteProject(projectToDelete.id));
    setProjectToDelete(null);
    toastService.success('Project deleted successfully.');
  };

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
        <Box>
          <Typography component="h1" fontWeight={800} variant="h4">
            Project Management
          </Typography>
          <Typography color="text.secondary" mt={0.75}>
            Manage delivery portfolio, clients, status, and resource allocation.
          </Typography>
        </Box>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
          <Button onClick={() => navigate('/projects/dashboard')} startIcon={<DashboardOutlinedIcon />} variant="outlined">
            Dashboard
          </Button>
          <Button onClick={() => navigate('/projects/create')} startIcon={<AddOutlinedIcon />} variant="contained">
            Create Project
          </Button>
        </Stack>
      </Stack>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 2 }}>
        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
          <TextField
            InputProps={{ startAdornment: <SearchOutlinedIcon color="action" sx={{ mr: 1 }} /> }}
            label="Search projects"
            onChange={(event) => setSearchText(event.target.value)}
            value={searchText}
            fullWidth
          />
          <TextField
            label="Filter by status"
            onChange={(event) => setStatusFilter(event.target.value as ProjectStatus | 'All')}
            select
            sx={{ minWidth: { md: 220 } }}
            value={statusFilter}
          >
            <MenuItem value="All">All Statuses</MenuItem>
            {projectStatuses.map((status) => (
              <MenuItem key={status} value={status}>
                {status}
              </MenuItem>
            ))}
          </TextField>
        </Stack>
      </Paper>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
        {filteredProjects.length > 0 ? (
          <ProjectTable
            onDelete={setProjectToDelete}
            onEdit={(project) => navigate(`/projects/edit/${project.id}`)}
            onView={(project) => navigate(`/projects/${project.id}`)}
            projects={filteredProjects}
          />
        ) : (
          <Box alignItems="center" display="flex" flexDirection="column" justifyContent="center" minHeight={320} p={3}>
            <Typography fontWeight={800} variant="h6">
              No projects found
            </Typography>
            <Typography color="text.secondary" mt={1} textAlign="center">
              Adjust search or status filters to find matching projects.
            </Typography>
          </Box>
        )}
      </Paper>

      <Dialog onClose={() => setProjectToDelete(null)} open={Boolean(projectToDelete)}>
        <DialogTitle>Delete project?</DialogTitle>
        <DialogContent>
          <DialogContentText>
            This will remove {projectToDelete?.projectName} from the local project list. This action cannot be undone.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setProjectToDelete(null)}>Cancel</Button>
          <Button color="error" onClick={handleDeleteConfirm} variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>

    </Stack>
  );
}
