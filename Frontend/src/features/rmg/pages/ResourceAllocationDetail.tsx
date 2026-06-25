import { useEffect, useMemo, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Autocomplete,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  IconButton,
  MenuItem,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Grid,
} from '@mui/material';
import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import PageContainer from '../../../components/common/PageContainer';
import { useAppSelector } from '../../../redux/hooks';
import { allocationService } from '../services/allocationService';
import type { EmployeeAllocationDto, ProjectAllocationDto, AddProjectAllocationDto, UpdateProjectAllocationDto } from '../types/allocation';
import type { Project } from '../../projects/types/project.types';
import { ALLOCATION_STATUSES, ALLOCATION_TYPES, BILLABLE_STATUSES } from '../types/allocation';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Active: 'success',
  Planned: 'info',
  Completed: 'default',
  Released: 'warning',
  Cancelled: 'error',
};

export default function ResourceAllocationDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [employeeData, setEmployeeData] = useState<EmployeeAllocationDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingAllocation, setEditingAllocation] = useState<ProjectAllocationDto | null>(null);
  const [formData, setFormData] = useState({
    projectId: 0,
    projectName: '',
    startDate: '',
    endDate: '',
    allocationPercentage: 0,
    allocationType: 'Full Time',
    billableStatus: 'Billable',
    allocationStatus: 'Active',
  });
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  const employeeId = Number(id);
  const projects = useAppSelector((state) => state.projects.projects);

  const selectedProject = useMemo(() => {
    return projects.find((p) => p.id === formData.projectId) ?? null;
  }, [projects, formData.projectId]);

  useEffect(() => {
    if (id) loadEmployeeData();
  }, [id]);

  const loadEmployeeData = async () => {
    setLoading(true);
    setError('');
    try {
      const data = await allocationService.getEmployeeAllocations(employeeId);
      setEmployeeData(data);
    } catch {
      setError('Failed to load employee allocation data');
    } finally {
      setLoading(false);
    }
  };

  const totalAllocated = useMemo(() => {
    return employeeData?.allocations?.reduce((sum, a) => sum + a.allocationPercentage, 0) ?? 0;
  }, [employeeData]);

  const summary = useMemo(() => {
    const allocated = totalAllocated;
    const available = Math.max(0, 100 - allocated);
    let status = 'Available';
    if (allocated > 100) status = 'Overallocated';
    else if (allocated >= 100) status = 'Fully Allocated';
    else if (allocated > 0) status = 'Partially Allocated';
    return { totalCapacity: 100, allocatedCapacity: allocated, availableCapacity: available, resourceStatus: status };
  }, [totalAllocated]);

  const openAddDialog = () => {
    setEditingAllocation(null);
    setFormData({ projectId: 0, projectName: '', startDate: '', endDate: '', allocationPercentage: 0, allocationType: 'Full Time', billableStatus: 'Billable', allocationStatus: 'Active' });
    setFormError('');
    setDialogOpen(true);
  };

  const openEditDialog = (allocation: ProjectAllocationDto) => {
    setEditingAllocation(allocation);
    setFormData({
      projectId: allocation.projectId,
      projectName: allocation.projectName,
      startDate: allocation.startDate.split('T')[0],
      endDate: allocation.endDate ? allocation.endDate.split('T')[0] : '',
      allocationPercentage: allocation.allocationPercentage,
      allocationType: allocation.allocationType ?? 'Full Time',
      billableStatus: allocation.billableStatus ?? 'Billable',
      allocationStatus: allocation.allocationStatus,
    });
    setFormError('');
    setDialogOpen(true);
  };

  const handleDeleteAllocation = async (allocationId: number) => {
    if (!window.confirm('Are you sure you want to remove this project allocation?')) return;
    try {
      await allocationService.deleteProjectAllocation(allocationId);
      await loadEmployeeData();
    } catch {
      setError('Failed to delete allocation');
    }
  };

  const getOtherAllocationsTotal = (excludeId?: number) => {
    return (employeeData?.allocations ?? [])
      .filter(a => a.id !== excludeId)
      .reduce((sum, a) => sum + a.allocationPercentage, 0);
  };

  const handleSave = async () => {
    setFormError('');

    if (!formData.projectId) {
      setFormError('Project Code is required');
      return;
    }
    if (!formData.startDate) {
      setFormError('Start date is required');
      return;
    }
    if (formData.allocationPercentage <= 0 || formData.allocationPercentage > 100) {
      setFormError('Allocation percentage must be between 1 and 100');
      return;
    }

    const otherTotal = getOtherAllocationsTotal(editingAllocation?.id);
    if (otherTotal + formData.allocationPercentage > 100) {
      setFormError(`Total allocation cannot exceed 100%. Current allocation: ${otherTotal}%. Adding ${formData.allocationPercentage}% would make it ${otherTotal + formData.allocationPercentage}%.`);
      return;
    }

    setSaving(true);
    try {
      if (editingAllocation) {
        const dto: UpdateProjectAllocationDto = {
          projectId: formData.projectId,
          startDate: formData.startDate,
          endDate: formData.endDate || null,
          allocationPercentage: formData.allocationPercentage,
          allocationType: formData.allocationType,
          billableStatus: formData.billableStatus,
          allocationStatus: formData.allocationStatus,
        };
        await allocationService.updateProjectAllocation(editingAllocation.id, dto);
      } else {
        const dto: AddProjectAllocationDto = {
          employeeId,
          projectId: formData.projectId,
          startDate: formData.startDate,
          endDate: formData.endDate || null,
          allocationPercentage: formData.allocationPercentage,
          allocationType: formData.allocationType,
          billableStatus: formData.billableStatus,
          allocationStatus: formData.allocationStatus,
        };
        await allocationService.addProjectAllocation(dto);
      }
      setDialogOpen(false);
      await loadEmployeeData();
    } catch (err: any) {
      setFormError(err.response?.data?.message || 'Failed to save allocation');
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <PageContainer title="Loading..."><Typography>Loading employee allocation details...</Typography></PageContainer>;

  if (!employeeData) return (
    <PageContainer title="Not Found">
      <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')} sx={{ mb: 2 }}>Back to Dashboard</Button>
      <Typography color="error">{error || 'Employee not found.'}</Typography>
    </PageContainer>
  );

  return (
    <PageContainer title="Resource Allocation Management">
      <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')} sx={{ mb: 2 }}>
        Back to Dashboard
      </Button>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Stack spacing={3}>
        <Grid container spacing={2}>
          <Grid item xs={12} md={4}>
            <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: '100%' }}>
              <CardContent sx={{ p: 3 }}>
                <Typography variant="subtitle2" color="text.secondary" gutterBottom>Total Capacity</Typography>
                <Typography variant="h4" fontWeight={800}>{summary.totalCapacity}%</Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={4}>
            <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: '100%' }}>
              <CardContent sx={{ p: 3 }}>
                <Typography variant="subtitle2" color="text.secondary" gutterBottom>Allocated Capacity</Typography>
                <Typography variant="h4" fontWeight={800} color={summary.allocatedCapacity > 100 ? 'error.main' : summary.allocatedCapacity >= 100 ? 'warning.main' : 'primary.main'}>
                  {summary.allocatedCapacity}%
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid item xs={12} md={4}>
            <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: '100%' }}>
              <CardContent sx={{ p: 3 }}>
                <Typography variant="subtitle2" color="text.secondary" gutterBottom>Available Capacity</Typography>
                <Typography variant="h4" fontWeight={800} color="success.main">{summary.availableCapacity}%</Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
          <Stack spacing={2}>
            <Stack direction="row" justifyContent="space-between" alignItems="flex-start" flexWrap="wrap" gap={2}>
              <Stack spacing={0.5}>
                <Typography variant="h6" fontWeight={800}>{employeeData.employeeName}</Typography>
                <Typography variant="body2" color="text.secondary">
                  {employeeData.employeeCode} | {employeeData.designation ?? '-'} | {employeeData.practice}
                </Typography>
                {employeeData.skills && (
                  <Typography variant="body2" color="text.secondary">
                    Skills: {employeeData.skills}
                  </Typography>
                )}
              </Stack>
              <Stack direction="row" spacing={1} alignItems="center">
                <Typography variant="body2" fontWeight={600}>
                  Utilization: {summary.allocatedCapacity}%
                </Typography>
                <Typography variant="body2" fontWeight={600} color="success.main">
                  Available: {summary.availableCapacity}%
                </Typography>
                <Chip
                  label={summary.resourceStatus}
                  size="small"
                  color={summary.resourceStatus === 'Available' ? 'success' : summary.resourceStatus === 'Partially Allocated' ? 'info' : summary.resourceStatus === 'Fully Allocated' ? 'warning' : 'error'}
                  variant="outlined"
                />
              </Stack>
            </Stack>
          </Stack>
        </Paper>

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ px: 3, py: 2, borderBottom: '1px solid', borderColor: 'divider' }}>
            <Typography fontWeight={700} variant="h6">Project Allocations</Typography>
            <Button variant="contained" startIcon={<AddOutlinedIcon />} onClick={openAddDialog} size="small">
              Add Project Allocation
            </Button>
          </Stack>
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Project</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Start Date</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>End Date</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Allocation %</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Billable Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Allocation Type</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }} align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {employeeData.allocations.map((a) => (
                  <TableRow key={a.id} hover>
                    <TableCell>
                      <Typography fontWeight={600} variant="body2">{a.projectName}</Typography>
                    </TableCell>
                    <TableCell>{new Date(a.startDate).toLocaleDateString()}</TableCell>
                    <TableCell>{a.endDate ? new Date(a.endDate).toLocaleDateString() : '-'}</TableCell>
                    <TableCell>
                      <Typography fontWeight={600}>{a.allocationPercentage}%</Typography>
                    </TableCell>
                    <TableCell>
                      <Chip label={a.billableStatus ?? '-'} size="small" variant="outlined" color={a.billableStatus === 'Billable' ? 'success' : 'default'} />
                    </TableCell>
                    <TableCell>{a.allocationType ?? '-'}</TableCell>
                    <TableCell>
                      <Chip label={a.allocationStatus} size="small" color={statusColors[a.allocationStatus] ?? 'default'} variant="outlined" />
                    </TableCell>
                    <TableCell align="right">
                      <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                        <Tooltip title="Edit">
                          <IconButton size="small" onClick={() => openEditDialog(a)}>
                            <EditOutlinedIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Remove">
                          <IconButton size="small" onClick={() => handleDeleteAllocation(a.id)}>
                            <DeleteOutlinedIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </Stack>
                    </TableCell>
                  </TableRow>
                ))}
                {employeeData.allocations.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={8} align="center" sx={{ py: 4 }}>
                      <Typography color="text.secondary">No project allocations. Click "Add Project Allocation" to assign.</Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>

        {totalAllocated > 100 && (
          <Alert severity="error">
            Total allocation percentage cannot exceed 100%. Current total: {totalAllocated}%.
          </Alert>
        )}
      </Stack>

      <Dialog open={dialogOpen} onClose={() => !saving && setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editingAllocation ? 'Edit Project Allocation' : 'Add Project Allocation'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2.5} sx={{ mt: 1 }}>
            {formError && <Alert severity="error" size="small">{formError}</Alert>}
            <Autocomplete
              options={projects}
              getOptionLabel={(option) => `${option.projectCode} - ${option.projectName}`}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={selectedProject}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  projectId: value?.id ?? 0,
                  projectName: value?.projectName ?? '',
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Project Code *"
                  placeholder="Search by project code or name"
                />
              )}
            />
            <TextField
              label="Project Name"
              fullWidth
              size="small"
              value={formData.projectName}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="Start Date"
              type="date"
              fullWidth
              size="small"
              value={formData.startDate}
              onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
              slotProps={{ inputLabel: { shrink: true } }}
            />
            <TextField
              label="End Date"
              type="date"
              fullWidth
              size="small"
              value={formData.endDate}
              onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
              slotProps={{ inputLabel: { shrink: true } }}
            />
            <TextField
              label="Allocation %"
              type="number"
              fullWidth
              size="small"
              value={formData.allocationPercentage || ''}
              onChange={(e) => setFormData({ ...formData, allocationPercentage: Number(e.target.value) })}
              inputProps={{ min: 1, max: 100 }}
            />
            <TextField
              label="Allocation Type"
              select
              fullWidth
              size="small"
              value={formData.allocationType}
              onChange={(e) => setFormData({ ...formData, allocationType: e.target.value })}
            >
              {ALLOCATION_TYPES.map((t) => (
                <MenuItem key={t} value={t}>{t}</MenuItem>
              ))}
            </TextField>
            <TextField
              label="Billable Status"
              select
              fullWidth
              size="small"
              value={formData.billableStatus}
              onChange={(e) => setFormData({ ...formData, billableStatus: e.target.value })}
            >
              {BILLABLE_STATUSES.map((s) => (
                <MenuItem key={s} value={s}>{s}</MenuItem>
              ))}
            </TextField>
            <TextField
              label="Status"
              select
              fullWidth
              size="small"
              value={formData.allocationStatus}
              onChange={(e) => setFormData({ ...formData, allocationStatus: e.target.value })}
            >
              {ALLOCATION_STATUSES.map((s) => (
                <MenuItem key={s} value={s}>{s}</MenuItem>
              ))}
            </TextField>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)} disabled={saving} startIcon={<CancelOutlinedIcon />}>Cancel</Button>
          <Button onClick={handleSave} variant="contained" disabled={saving} startIcon={<SaveOutlinedIcon />}>
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </PageContainer>
  );
}