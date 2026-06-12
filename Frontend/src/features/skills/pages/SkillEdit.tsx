import { Alert, Box, Button, Snackbar, Stack, Typography } from '@mui/material';
import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import SkillForm from '../components/SkillForm';
import { updateSkill } from '../skillSlice';
import type { SkillFormValues } from '../types';

export default function SkillEdit() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { id } = useParams();
  const skill = useAppSelector((state) => state.skills.skills.find((item) => item.id === Number(id)));
  const [open, setOpen] = useState(false);

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
    setOpen(true);
    window.setTimeout(() => navigate('/skills'), 450);
  };

  return (
    <Stack spacing={3}>
      <Typography component="h1" fontWeight={900} variant="h4">
        Edit Skill
      </Typography>
      <SkillForm initialValues={skill} mode="edit" onCancel={() => navigate('/skills')} onSubmit={handleSubmit} />
      <Snackbar autoHideDuration={2000} onClose={() => setOpen(false)} open={open}>
        <Alert severity="success" variant="filled">
          Skill updated successfully.
        </Alert>
      </Snackbar>
    </Stack>
  );
}
