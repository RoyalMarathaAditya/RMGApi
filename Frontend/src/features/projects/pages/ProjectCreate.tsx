import { Alert, Snackbar, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../../../app/hooks';
import ProjectForm from '../components/ProjectForm';
import { addProject } from '../store/projectSlice';
import type { ProjectFormValues } from '../types/project.types';

export default function ProjectCreate() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [isSnackbarOpen, setIsSnackbarOpen] = useState(false);

  const handleSubmit = (values: ProjectFormValues) => {
    dispatch(addProject(values));
    setIsSnackbarOpen(true);
    window.setTimeout(() => navigate('/projects'), 500);
  };

  return (
    <Stack spacing={3}>
      <Stack spacing={0.75}>
        <Typography component="h1" fontWeight={800} variant="h4">
          Create Project
        </Typography>
        <Typography color="text.secondary">Add a new IT services project to the delivery portfolio.</Typography>
      </Stack>
      <ProjectForm mode="create" onCancel={() => navigate('/projects')} onSubmit={handleSubmit} />
      <Snackbar autoHideDuration={3000} onClose={() => setIsSnackbarOpen(false)} open={isSnackbarOpen}>
        <Alert severity="success" variant="filled">
          Project created successfully.
        </Alert>
      </Snackbar>
    </Stack>
  );
}
