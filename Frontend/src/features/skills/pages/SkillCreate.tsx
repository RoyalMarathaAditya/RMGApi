import { Alert, Snackbar, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppDispatch } from '../../../app/hooks';
import SkillForm from '../components/SkillForm';
import { addSkill } from '../skillSlice';
import type { SkillFormValues } from '../types';

export default function SkillCreate() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);

  const handleSubmit = (values: SkillFormValues) => {
    dispatch(addSkill(values));
    setOpen(true);
    window.setTimeout(() => navigate('/skills'), 450);
  };

  return (
    <Stack spacing={3}>
      <Typography component="h1" fontWeight={900} variant="h4">
        Add Skill
      </Typography>
      <SkillForm mode="create" onCancel={() => navigate('/skills')} onSubmit={handleSubmit} />
      <Snackbar autoHideDuration={2000} onClose={() => setOpen(false)} open={open}>
        <Alert severity="success" variant="filled">
          Skill saved successfully.
        </Alert>
      </Snackbar>
    </Stack>
  );
}
