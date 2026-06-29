import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Button,
  Chip,
  MenuItem,
  Paper,
  Stack,
  TextField,
  Typography,
  Alert,
} from '@mui/material';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import PageContainer from '../../components/common/PageContainer';
import { allocationService } from '../../features/rmg/services/allocationService';
import { ALLOCATION_STATUSES } from '../../features/rmg/types/allocation';
import type { AllocationDto } from '../../features/rmg/types/allocation';
import { toastService } from '../../services/toastService';

const schema = yup.object({
  projectId: yup.number().positive(),
  startDate: yup.string(),
  endDate: yup.string().nullable(),
  allocationPercentage: yup.number().min(1).max(100),
  allocationStatus: yup.string().oneOf(ALLOCATION_STATUSES),
  notes: yup.string().nullable(),
});

type FormValues = yup.InferType<typeof schema>;

export default function ResourceAllocationDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [allocation, setAllocation] = useState<AllocationDto | null>(null);
  const [loading, setLoading] = useState(true);

  const { control, handleSubmit, formState: { errors }, reset } = useForm<FormValues>({
    defaultValues: { projectId: 0, startDate: '', endDate: null, allocationPercentage: 0, allocationStatus: 'Active', notes: '' },
    resolver: yupResolver(schema),
    mode: 'onBlur',
  });

  useEffect(() => {
    if (id) loadAllocation();
  }, [id]);

  const loadAllocation = async () => {
    try {
      const data = await allocationService.getById(Number(id));
      setAllocation(data);
      reset({
        projectId: data.projectId,
        startDate: data.startDate.split('T')[0],
        endDate: data.endDate ? data.endDate.split('T')[0] : null,
        allocationPercentage: data.allocationPercentage,
        allocationStatus: data.allocationStatus,
        notes: data.notes,
      });
    } catch {
      console.error('Failed to load allocation');
    } finally {
      setLoading(false);
    }
  };

  const onSubmit = async (values: FormValues) => {
    try {
      await allocationService.update(Number(id), values);
      navigate('/rmg');
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to update allocation');
    }
  };

  if (loading) return <PageContainer title="Loading..."><Typography>Loading allocation details...</Typography></PageContainer>;
  if (!allocation) return <PageContainer title="Not Found"><Typography>Allocation not found.</Typography></PageContainer>;

  const remainingCapacity = allocation.availableCapacity;
  const wouldExceed = (allocation.allocationPercentage > remainingCapacity + allocation.allocationPercentage);

  return (
    <PageContainer title="Edit Allocation">
      <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')} sx={{ mb: 2 }}>
        Back to Dashboard
      </Button>

      <Stack spacing={3}>
        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
          <Stack spacing={1}>
            <Typography variant="h6" fontWeight={700}>{allocation.employeeName}</Typography>
            <Typography variant="body2" color="text.secondary">{allocation.employeeCode} | {allocation.designation ?? '-'} | {allocation.practice}</Typography>
            <Stack direction="row" spacing={1}>
              <Typography variant="body2"><strong>Total Allocated:</strong> {allocation.totalAllocated}%</Typography>
              <Typography variant="body2"><strong>Available:</strong> {remainingCapacity}%</Typography>
              <Chip label={allocation.resourceStatus} size="small" color={allocation.resourceStatus === 'Available' ? 'success' : 'warning'} variant="outlined" />
            </Stack>
          </Stack>
        </Paper>

        {wouldExceed && (
          <Alert severity="warning">Warning: This allocation may cause overallocation.</Alert>
        )}

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
          <Box component="form" noValidate onSubmit={handleSubmit(onSubmit)}>
            <Stack spacing={2.5} maxWidth={600}>
              <Typography fontWeight={700} variant="h6">Update Allocation</Typography>
              <Controller name="projectId" control={control} render={({ field }) => (
                <TextField {...field} label="Project ID" type="number" fullWidth
                  onChange={(e) => field.onChange(Number(e.target.value))}
                />
              )} />
              <Controller name="startDate" control={control} render={({ field }) => (
                <TextField {...field} label="Start Date" type="date" fullWidth
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
                <Button type="submit" variant="contained" startIcon={<SaveOutlinedIcon />}>Save Changes</Button>
              </Stack>
            </Stack>
          </Box>
        </Paper>
      </Stack>
    </PageContainer>
  );
}
