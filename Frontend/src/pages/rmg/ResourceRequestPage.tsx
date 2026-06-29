import { useEffect, useState } from 'react';
import {
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import PageContainer from '../../components/common/PageContainer';
import { requestService } from '../../features/rmg/services/requestService';
import type { ResourceRequestDto } from '../../features/rmg/types/request';
import { toastService } from '../../services/toastService';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Submitted: 'info',
  Reviewing: 'warning',
  Approved: 'success',
  Rejected: 'error',
  Fulfilled: 'success',
  Draft: 'default',
};

const schema = yup.object({
  projectId: yup.number().required('Project ID is required').positive(),
  requiredCount: yup.number().required().min(1).default(1),
  requiredByDate: yup.string().required('Required by date is required'),
  priority: yup.string().oneOf(['Low', 'Medium', 'High', 'Critical']),
  notes: yup.string().nullable(),
});

type FormValues = yup.InferType<typeof schema>;

export default function ResourceRequestPage() {
  const [requests, setRequests] = useState<ResourceRequestDto[]>([]);
  const [dialogOpen, setDialogOpen] = useState(false);

  const { control, handleSubmit, formState: { errors }, reset } = useForm<FormValues>({
    defaultValues: { projectId: undefined, requiredCount: 1, requiredByDate: '', priority: 'Medium', notes: '' },
    resolver: yupResolver(schema),
    mode: 'onBlur',
  });

  useEffect(() => {
    loadRequests();
  }, []);

  const loadRequests = async () => {
    try {
      const data = await requestService.getAll();
      setRequests(data);
    } catch {
      console.error('Failed to load resource requests');
    }
  };

  const onSubmit = async (values: FormValues) => {
    try {
      await requestService.create(values);
      setDialogOpen(false);
      reset();
      loadRequests();
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to create request');
    }
  };

  return (
    <PageContainer title="Resource Requests">
      <Stack spacing={2}>
        <Stack direction="row" justifyContent="flex-end">
          <Button variant="contained" startIcon={<AddOutlinedIcon />} onClick={() => setDialogOpen(true)}>
            New Request
          </Button>
        </Stack>

        <TableContainer component={Box} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 700 }}>Project</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Requested By</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Required Count</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Required By</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Priority</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Skills Required</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {requests.map((r) => (
                <TableRow key={r.id} hover>
                  <TableCell>
                    <Typography fontWeight={600} variant="body2">{r.projectName}</Typography>
                  </TableCell>
                  <TableCell>{r.requestedByName}</TableCell>
                  <TableCell>{r.requiredCount}</TableCell>
                  <TableCell>{new Date(r.requiredByDate).toLocaleDateString()}</TableCell>
                  <TableCell>
                    <Chip label={r.priority ?? 'Normal'} size="small" color={r.priority === 'Critical' ? 'error' : r.priority === 'High' ? 'warning' : 'default'} variant="outlined" />
                  </TableCell>
                  <TableCell>
                    <Chip label={r.status} size="small" color={statusColors[r.status] ?? 'default'} variant="outlined" />
                  </TableCell>
                  <TableCell>
                    <Typography variant="caption" color="text.secondary">{r.requiredSkillNames ?? '-'}</Typography>
                  </TableCell>
                </TableRow>
              ))}
              {requests.length === 0 && (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 4 }}>No resource requests found</TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Stack>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <form onSubmit={handleSubmit(onSubmit)}>
          <DialogTitle>New Resource Request</DialogTitle>
          <DialogContent>
            <Stack spacing={2} sx={{ mt: 1 }}>
              <Controller name="projectId" control={control} render={({ field }) => (
                <TextField {...field} label="Project ID" type="number" error={!!errors.projectId} helperText={errors.projectId?.message} fullWidth
                  onChange={(e) => field.onChange(Number(e.target.value))}
                />
              )} />
              <Controller name="requiredCount" control={control} render={({ field }) => (
                <TextField {...field} label="Required Count" type="number" error={!!errors.requiredCount} helperText={errors.requiredCount?.message} fullWidth
                  onChange={(e) => field.onChange(Number(e.target.value))}
                />
              )} />
              <Controller name="requiredByDate" control={control} render={({ field }) => (
                <TextField {...field} label="Required By" type="date" error={!!errors.requiredByDate} helperText={errors.requiredByDate?.message} fullWidth
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )} />
              <Controller name="priority" control={control} render={({ field }) => (
                <TextField {...field} label="Priority" select fullWidth>
                  <MenuItem value="Low">Low</MenuItem>
                  <MenuItem value="Medium">Medium</MenuItem>
                  <MenuItem value="High">High</MenuItem>
                  <MenuItem value="Critical">Critical</MenuItem>
                </TextField>
              )} />
              <Controller name="notes" control={control} render={({ field }) => (
                <TextField {...field} label="Notes" multiline rows={3} fullWidth value={field.value ?? ''} />
              )} />
            </Stack>
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
            <Button type="submit" variant="contained">Submit Request</Button>
          </DialogActions>
        </form>
      </Dialog>
    </PageContainer>
  );
}
