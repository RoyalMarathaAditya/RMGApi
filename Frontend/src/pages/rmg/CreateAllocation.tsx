import { useState } from 'react';
import {
  Box,
  Button,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../components/common/PageContainer';
import { allocationService } from '../../features/rmg/services/allocationService';
import { ALLOCATION_STATUSES } from '../../features/rmg/types/allocation';
import { toastService } from '../../services/toastService';

const schema = yup.object({
  employeeId: yup.number().required('Employee ID is required').positive(),
  projectId: yup.number().required('Project ID is required').positive(),
  startDate: yup.string().required('Start date is required'),
  endDate: yup.string().nullable(),
  allocationPercentage: yup.number().required('Allocation % is required').min(1).max(100),
  allocationStatus: yup.string().oneOf(ALLOCATION_STATUSES),
  notes: yup.string().nullable(),
});

type FormValues = yup.InferType<typeof schema>;

export default function CreateAllocation() {
  const navigate = useNavigate();
  const [submitting, setSubmitting] = useState(false);

  const { control, handleSubmit, formState: { errors } } = useForm<FormValues>({
    defaultValues: {
      employeeId: undefined,
      projectId: undefined,
      startDate: '',
      endDate: null,
      allocationPercentage: 100,
      allocationStatus: 'Planned',
      notes: '',
    },
    resolver: yupResolver(schema),
    mode: 'onBlur',
  });

  const onSubmit = async (values: FormValues) => {
    setSubmitting(true);
    try {
      await allocationService.create(values);
      navigate('/rmg');
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to create allocation');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <PageContainer title="Create Allocation">
      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
        <Box component="form" noValidate onSubmit={handleSubmit(onSubmit)}>
          <Stack spacing={3} maxWidth={600}>
            <Typography fontWeight={700} variant="h6">New Resource Allocation</Typography>
            <Controller name="employeeId" control={control} render={({ field }) => (
              <TextField {...field} label="Employee ID" type="number" error={!!errors.employeeId} helperText={errors.employeeId?.message} fullWidth
                onChange={(e) => field.onChange(Number(e.target.value))}
              />
            )} />
            <Controller name="projectId" control={control} render={({ field }) => (
              <TextField {...field} label="Project ID" type="number" error={!!errors.projectId} helperText={errors.projectId?.message} fullWidth
                onChange={(e) => field.onChange(Number(e.target.value))}
              />
            )} />
            <Controller name="startDate" control={control} render={({ field }) => (
              <TextField {...field} label="Start Date" type="date" error={!!errors.startDate} helperText={errors.startDate?.message} fullWidth
                slotProps={{ inputLabel: { shrink: true } }}
              />
            )} />
            <Controller name="endDate" control={control} render={({ field }) => (
              <TextField {...field} label="End Date" type="date" fullWidth
                slotProps={{ inputLabel: { shrink: true } }}
                value={field.value ?? ''}
                onChange={(e) => field.onChange(e.target.value || null)}
              />
            )} />
            <Controller name="allocationPercentage" control={control} render={({ field }) => (
              <TextField {...field} label="Allocation %" type="number" error={!!errors.allocationPercentage} helperText={errors.allocationPercentage?.message} fullWidth
                onChange={(e) => field.onChange(Number(e.target.value))}
              />
            )} />
            <Controller name="allocationStatus" control={control} render={({ field }) => (
              <TextField {...field} label="Status" select fullWidth>
                {ALLOCATION_STATUSES.map((s) => (
                  <MenuItem key={s} value={s}>{s}</MenuItem>
                ))}
              </TextField>
            )} />
            <Controller name="notes" control={control} render={({ field }) => (
              <TextField {...field} label="Notes" multiline rows={3} fullWidth
                value={field.value ?? ''}
              />
            )} />
            <Stack direction="row" spacing={2} justifyContent="flex-end">
              <Button variant="outlined" onClick={() => navigate('/rmg')}>Cancel</Button>
              <Button type="submit" variant="contained" disabled={submitting} startIcon={<SaveOutlinedIcon />}>
                {submitting ? 'Saving...' : 'Create Allocation'}
              </Button>
            </Stack>
          </Stack>
        </Box>
      </Paper>
    </PageContainer>
  );
}
