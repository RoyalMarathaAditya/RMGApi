import { Alert, Box, Button, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
// Redux: dispatches updateSkill, reads skills list to find the one being edited
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import SkillForm from '../components/SkillForm';
import { updateSkill } from '../../../redux/slices/skillSlice';
import type { SkillFormValues } from '../types';
import { toastService } from '../../../services/toastService';

export default function SkillEdit() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { id } = useParams();
  const skill = useAppSelector((state) => state.skills.skills.find((item) => item.id === Number(id)));
  if (!skill) {
    return (
      <Box>
        <Typography fontWeight={800} variant="h5">
          Skill not found
        </Typography>
        <Button onClick={() => navigate('/skills')} sx={{ mt: 2 }} variant="contained">
          Back to Skills
        </Button>
      </Box>
    );
  }

  const handleSubmit = (values: SkillFormValues) => {
    dispatch(updateSkill({ ...skill, ...values }));
    toastService.success('Skill updated successfully.');
    window.setTimeout(() => navigate('/skills'), 450);
  };

  return (
    <Stack spacing={3}>
      <Typography component="h1" fontWeight={900} variant="h4">
        Edit Skill
      </Typography>
      <SkillForm initialValues={skill} mode="edit" onCancel={() => navigate('/skills')} onSubmit={handleSubmit} />
    </Stack>
  );
}
