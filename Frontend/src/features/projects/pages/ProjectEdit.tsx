import { Alert, Box, Button, Snackbar, Stack, Typography } from '@mui/material';
import { useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import ProjectForm from '../components/ProjectForm';
import { updateProject } from '../store/projectSlice';
import type { ProjectFormValues } from '../types/project.types';

export default function ProjectEdit() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { id } = useParams();
  const projects = useAppSelector((state) => state.projects.projects);
  const [isSnackbarOpen, setIsSnackbarOpen] = useState(false);
  const project = useMemo(() => projects.find((item) => item.id === Number(id)), [id, projects]);

  if (!project) {
    return (
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Project not found
        </Typography>
        <Typography color="text.secondary" mt={1}>
          The selected project does not exist in the mock project list.
        </Typography>
        <Button onClick={() => navigate('/projects')} sx={{ mt: 3 }} variant="contained">
          Back to Projects
        </Button>
      </Box>
    );
  }

  const handleSubmit = (values: ProjectFormValues) => {
    dispatch(updateProject({ id: project.id, ...values }));
    setIsSnackbarOpen(true);
    window.setTimeout(() => navigate('/projects'), 500);
  };

  return (
    <Stack spacing={3}>
      <Stack spacing={0.75}>
        <Typography component="h1" fontWeight={800} variant="h4">
          Edit Project
        </Typography>
        <Typography color="text.secondary">Update project scope, client details, status, and resource summary.</Typography>
      </Stack>
      <ProjectForm initialValues={project} mode="edit" onCancel={() => navigate('/projects')} onSubmit={handleSubmit} />
      <Snackbar autoHideDuration={3000} onClose={() => setIsSnackbarOpen(false)} open={isSnackbarOpen}>
        <Alert severity="success" variant="filled">
          Project updated successfully.
        </Alert>
      </Snackbar>
    </Stack>
  );
}
