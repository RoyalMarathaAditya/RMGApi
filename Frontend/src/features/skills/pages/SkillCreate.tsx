import { Alert, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
// Redux: dispatches addSkill to store new skill in state
import { useAppDispatch } from '../../../redux/hooks';
import SkillForm from '../components/SkillForm';
import { addSkill } from '../../../redux/slices/skillSlice';
import type { SkillFormValues } from '../types';
import { toastService } from '../../../services/toastService';

export default function SkillCreate() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const handleSubmit = (values: SkillFormValues) => {
    dispatch(addSkill(values));
    toastService.success('Skill saved successfully.');
    window.setTimeout(() => navigate('/skills'), 450);
  };

  return (
    <Stack spacing={3}>
      <Typography component="h1" fontWeight={900} variant="h4">
        Add Skill
      </Typography>
      <SkillForm mode="create" onCancel={() => navigate('/skills')} onSubmit={handleSubmit} />
    </Stack>
  );
}
