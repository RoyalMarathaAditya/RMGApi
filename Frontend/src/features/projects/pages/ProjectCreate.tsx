import { Alert, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
// Redux: dispatches addProject to store new project in state
import { useAppDispatch } from '../../../redux/hooks';
import ProjectForm from '../components/ProjectForm';
import { addProject } from '../../../redux/slices/projectSlice';
import type { ProjectFormValues } from '../types/project.types';
import { toastService } from '../../../services/toastService';

export default function ProjectCreate() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const handleSubmit = (values: ProjectFormValues) => {
    dispatch(addProject(values));
    toastService.success('Project created successfully.');
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
    </Stack>
  );
}
