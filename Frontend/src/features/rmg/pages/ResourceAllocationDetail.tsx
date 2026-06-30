import { yupResolver } from '@hookform/resolvers/yup';
import { Controller, useForm } from 'react-hook-form';
import * as yup from 'yup';
import { useEffect, useMemo, useRef, useState } from 'react';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { format, parse } from 'date-fns';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  Chip,
  FormControlLabel,
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
} from '@mui/material';
import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import CancelOutlinedIcon from '@mui/icons-material/CancelOutlined';
import TimelineOutlinedIcon from '@mui/icons-material/TimelineOutlined';
import PageContainer from '../../../components/common/PageContainer';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import { fetchEmployees } from '../../../redux/slices/employeeSlice';
import api from '../../../services/api';
import { toastService } from '../../../services/toastService';
import { allocationService } from '../services/allocationService';
import type { EmployeeAllocationDto, ProjectAllocationDto, AddProjectAllocationDto, UpdateProjectAllocationDto } from '../types/allocation';
import { ALLOCATION_TYPES, BILLABLE_STATUSES } from '../types/allocation';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Current: 'success',
  History: 'default',
  Active: 'success',
  Planned: 'info',
  Completed: 'default',
  Released: 'warning',
  Cancelled: 'error',
};

import CheckBoxOutlineBlankOutlinedIcon from '@mui/icons-material/CheckBoxOutlineBlankOutlined';

function computeAllocationStatus(endDate: string): string {
  if (!endDate) return 'History';
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const end = new Date(endDate + 'T00:00:00');
  return end >= today ? 'Current' : 'History';
}

function formatYearsMonths(yearsDecimal: number): string {
  const totalMonths = Math.round(yearsDecimal * 12);
  const years = Math.floor(totalMonths / 12);
  const months = totalMonths % 12;
  const parts: string[] = [];
  if (years > 0) parts.push(`${years} Year${years > 1 ? 's' : ''}`);
  if (months > 0) parts.push(`${months} Month${months > 1 ? 's' : ''}`);
  return parts.join(' ') || '0 Years';
}

function calculateNVExperienceYears(doj: string, lwd?: string | null): number {
  const from = new Date(doj);
  const to = lwd ? new Date(lwd) : new Date();
  const diffMs = to.getTime() - from.getTime();
  return Math.max(0, diffMs / (365.25 * 24 * 60 * 60 * 1000));
}
import CheckBoxOutlinedIcon from '@mui/icons-material/CheckBoxOutlined';

const allocationFormSchema = yup.object({
  primarySkillId: yup.number().nullable(),
  skillIds: yup.array(yup.number()).default([]),
  projectManagerId: yup.number().nullable(),
  isActive: yup.boolean().default(true),
  remarks: yup.string().nullable(),
});

type AllocationFormValues = yup.InferType<typeof allocationFormSchema>;

const sectionHeaderSx = {
  display: 'flex',
  alignItems: 'center',
  gap: 1.25,
  px: '24px',
  py: 1.5,
  bgcolor: '#F8FAFC',
  borderBottom: '1px solid #E5E7EB',
  borderTop: '1px solid #E5E7EB',
};

const sectionIconSx = { fontSize: '1.1rem' };

const defaultFormValues: AllocationFormValues = {
  primarySkillId: null,
  skillIds: [],
  projectManagerId: null,
  isActive: true,
  remarks: '',
};

export default function ResourceAllocationDetail() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const [employeeData, setEmployeeData] = useState<EmployeeAllocationDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingAllocation, setEditingAllocation] = useState<ProjectAllocationDto | null>(null);
  const [formData, setFormData] = useState({
    projectId: 0,
    projectName: '',
    clientId: null as number | null,
    clientName: '',
    projectStatusId: null as string | null,
    statusId: null as string | null,
    probableNextAssignmentId: null as string | null,
    probableNextAssignmentDate: null as string | null,
    billableDateProbabilityId: null as string | null,
    currentBillingStatusId: null as string | null,
    billingBucketId: null as string | null,
    ageingBucketId: null as string | null,
    actionItem: null as string | null,
    remarks: null as string | null,
    startDate: '',
    endDate: '',
    allocationPercentage: 0,
    allocationType: 'Full Time',
    billableStatus: 'Billable',
    allocationStatus: 'Active',
  });
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);
  const [isEditMode, setIsEditMode] = useState(true);
  const [clients, setClients] = useState<{ id: number; name: string }[]>([]);
  const [projectStatuses, setProjectStatuses] = useState<{ id: string; name: string }[]>([]);
  const [statuses, setStatuses] = useState<{ id: string; name: string }[]>([]);
  const [probableNextAssignments, setProbableNextAssignments] = useState<{ id: string; name: string }[]>([]);
  const [billableDateProbabilities, setBillableDateProbabilities] = useState<{ id: string; name: string }[]>([]);
  const [currentBillingStatuses, setCurrentBillingStatuses] = useState<{ id: string; name: string }[]>([]);
  const [billingBuckets, setBillingBuckets] = useState<{ id: string; name: string }[]>([]);
  const [ageingBuckets, setAgeingBuckets] = useState<{ id: string; name: string }[]>([]);

  const employeeId = Number(id);
  const projects = useAppSelector((state) => state.projects.projects);
  const skills = useAppSelector((state) => state.skills.skills);
  const employees = useAppSelector((state) => state.employees.employees);

  const {
    control,
    handleSubmit: handleFormSubmit,
    formState: { errors: formErrors },
    watch,
    reset,
    getValues,
  } = useForm<AllocationFormValues>({
    resolver: yupResolver(allocationFormSchema),
    defaultValues: defaultFormValues,
    mode: 'onBlur',
  });

  const emp = useMemo(() => employees.find((e) => e.id === employeeId) ?? null, [employees, employeeId]);

  const priorExperienceYears = useMemo(() => emp?.priorExperience ?? 0, [emp]);

  const nvExperienceYears = useMemo(() => {
    if (!emp?.doj) return 0;
    return calculateNVExperienceYears(emp.doj, emp.lwd);
  }, [emp]);

  const totalExperienceYears = useMemo(() => priorExperienceYears + nvExperienceYears, [priorExperienceYears, nvExperienceYears]);

  const priorDisplay = useMemo(() => formatYearsMonths(priorExperienceYears), [priorExperienceYears]);
  const nvDisplay = useMemo(() => formatYearsMonths(nvExperienceYears), [nvExperienceYears]);
  const totalDisplay = useMemo(() => formatYearsMonths(totalExperienceYears), [totalExperienceYears]);

  const selectedProject = useMemo(() => {
    return projects.find((p) => p.id === formData.projectId) ?? null;
  }, [projects, formData.projectId]);

  const selectedClient = useMemo(() => {
    return clients.find((c) => c.id === formData.clientId) ?? null;
  }, [clients, formData.clientId]);

  useEffect(() => {
    if (selectedProject && !formData.clientId) {
      setFormData((prev) => ({ ...prev, clientId: null, clientName: '' }));
    }
  }, [selectedProject]);

  const initialFormPopulated = useRef(false);

  useEffect(() => {
    if (id) loadEmployeeData();
    fetchClients();
    fetchProjectStatuses();
    fetchStatuses();
    fetchProbableNextAssignments();
    fetchBillableDateProbabilities();
    fetchCurrentBillingStatuses();
    fetchBillingBuckets();
    fetchAgeingBuckets();
  }, [id]);

  useEffect(() => {
    if (employeeData && !initialFormPopulated.current) {
      initialFormPopulated.current = true;
      const current = getValues();
      reset({
        ...current,
        projectManagerId: employeeData.reportingManagerId ?? null,
        isActive: employeeData.isActive,
      });
    }
  }, [employeeData, reset, getValues]);

  useEffect(() => {
    if (employees.length === 0) {
      dispatch(fetchEmployees());
    }
  }, [dispatch, employees.length]);

  const fetchClients = async () => {
    try {
      const res = await api.get<{ id: number; name: string }[]>('/clients');
      setClients(res.data);
    } catch {
      console.error('Failed to load clients');
    }
  };

  const fetchProjectStatuses = async () => {
    try {
      const res = await api.get('/project-status');
      setProjectStatuses(res.data.data);
    } catch {
      console.error('Failed to load project statuses');
    }
  };

  const fetchStatuses = async () => {
    try {
      const res = await api.get('/master/statuses');
      setStatuses(res.data);
    } catch {
      console.error('Failed to load statuses');
    }
  };

  const fetchProbableNextAssignments = async () => {
    try {
      const res = await api.get('/probable-next-assignments');
      setProbableNextAssignments(res.data.data);
    } catch {
      console.error('Failed to load probable next assignments');
    }
  };

  const fetchBillableDateProbabilities = async () => {
    try {
      const res = await api.get('/billable-date-probabilities');
      setBillableDateProbabilities(res.data.data);
    } catch {
      console.error('Failed to load billable date probabilities');
    }
  };

  const fetchCurrentBillingStatuses = async () => {
    try {
      const res = await api.get('/current-billing-statuses');
      setCurrentBillingStatuses(res.data.data);
    } catch {
      console.error('Failed to load current billing statuses');
    }
  };

  const fetchBillingBuckets = async () => {
    try {
      const res = await api.get('/billing-buckets');
      setBillingBuckets(res.data.data);
    } catch {
      console.error('Failed to load billing buckets');
    }
  };

  const fetchAgeingBuckets = async () => {
    try {
      const res = await api.get('/ageing-buckets');
      setAgeingBuckets(res.data.data);
    } catch {
      console.error('Failed to load ageing buckets');
    }
  };

  const loadEmployeeData = async () => {
    setLoading(true);
    setError('');
    try {
      const data = await allocationService.getEmployeeAllocations(employeeId);
      setEmployeeData(data);
    } catch {
      toastService.error('Failed to load employee allocation data');
    } finally {
      setLoading(false);
    }
  };

  const totalAllocated = useMemo(() => {
    return employeeData?.allocations?.reduce((sum, a) => sum + a.allocationPercentage, 0) ?? 0;
  }, [employeeData]);

  const openAddDialog = () => {
    setEditingAllocation(null);
    setFormData({ projectId: 0, projectName: '', clientId: null, clientName: '', projectStatusId: null, statusId: null, probableNextAssignmentId: null, probableNextAssignmentDate: null, billableDateProbabilityId: null, currentBillingStatusId: null, billingBucketId: null, ageingBucketId: null, actionItem: null, remarks: null, startDate: '', endDate: '', allocationPercentage: 0, allocationType: 'Full Time', billableStatus: 'Billable', allocationStatus: 'History' });
    setFormError('');
    setDialogOpen(true);
  };

  const openEditDialog = (allocation: ProjectAllocationDto) => {
    setEditingAllocation(allocation);
    const project = projects.find((p) => p.id === allocation.projectId);
    const clientName = allocation.clientName || project?.clientName || '';
    const matchedClient = clientName ? clients.find((c) => c.name.toLowerCase() === clientName.toLowerCase()) : null;
    setFormData({
      projectId: allocation.projectId,
      projectName: allocation.projectName,
      clientId: allocation.clientId ?? matchedClient?.id ?? null,
      clientName: matchedClient?.name ?? clientName,
      projectStatusId: allocation.projectStatusId ?? null,
      statusId: allocation.statusId ?? null,
      probableNextAssignmentId: allocation.probableNextAssignmentId ?? null,
      probableNextAssignmentDate: allocation.probableNextAssignmentDate ?? null,
      billableDateProbabilityId: allocation.billableDateProbabilityId ?? null,
      currentBillingStatusId: allocation.currentBillingStatusId ?? null,
      billingBucketId: allocation.billingBucketId ?? null,
      ageingBucketId: allocation.ageingBucketId ?? null,
      actionItem: allocation.actionItem ?? null,
      remarks: allocation.remarks ?? null,
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
      toastService.success('Project allocation deleted successfully.');
    } catch {
      toastService.error('Failed to delete allocation');
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
      toastService.warning('Project Code is required');
      return;
    }
    if (!formData.clientId) {
      toastService.warning('Client is required');
      return;
    }
    if (!formData.startDate) {
      toastService.warning('Start date is required');
      return;
    }
    if (!formData.endDate) {
      toastService.warning('End date is required');
      return;
    }
    if (formData.endDate < formData.startDate) {
      toastService.warning('End date cannot be earlier than start date');
      return;
    }
    if (formData.allocationPercentage <= 0 || formData.allocationPercentage > 100) {
      toastService.warning('Allocation percentage must be between 1 and 100');
      return;
    }
    if (!formData.projectStatusId) {
      toastService.warning('Project Status is required');
      return;
    }
    if (!formData.statusId) {
      toastService.warning('Status is required');
      return;
    }
    if (!formData.probableNextAssignmentId) {
      toastService.warning('Probable Next Assignment is required');
      return;
    }
    if (!formData.billableDateProbabilityId) {
      toastService.warning('Billable Date Probability is required');
      return;
    }
    if (!formData.currentBillingStatusId) {
      toastService.warning('Current Billing Status is required');
      return;
    }
    if (!formData.billingBucketId) {
      toastService.warning('Billing Bucket is required');
      return;
    }
    if (!formData.ageingBucketId) {
      toastService.warning('Ageing Bucket is required');
      return;
    }

    const otherTotal = getOtherAllocationsTotal(editingAllocation?.id);
    if (otherTotal + formData.allocationPercentage > 100) {
      toastService.warning(`Total allocation cannot exceed 100%. Current allocation: ${otherTotal}%. Adding ${formData.allocationPercentage}% would make it ${otherTotal + formData.allocationPercentage}%.`);
      return;
    }

    setSaving(true);
    try {
      if (editingAllocation) {
        const computedStatus = computeAllocationStatus(formData.endDate);
        const dto: UpdateProjectAllocationDto = {
          projectId: formData.projectId,
          clientId: formData.clientId,
          projectStatusId: formData.projectStatusId,
          statusId: formData.statusId,
          probableNextAssignmentId: formData.probableNextAssignmentId,
          probableNextAssignmentDate: formData.probableNextAssignmentDate,
          billableDateProbabilityId: formData.billableDateProbabilityId,
          currentBillingStatusId: formData.currentBillingStatusId,
          billingBucketId: formData.billingBucketId,
          ageingBucketId: formData.ageingBucketId,
          actionItem: formData.actionItem,
          remarks: formData.remarks,
          startDate: formData.startDate,
          endDate: formData.endDate || null,
          allocationPercentage: formData.allocationPercentage,
          allocationType: formData.allocationType,
          billableStatus: formData.billableStatus,
          allocationStatus: computedStatus,
        };
        await allocationService.updateProjectAllocation(editingAllocation.id, dto);
      } else {
        const computedStatus = computeAllocationStatus(formData.endDate);
        const dto: AddProjectAllocationDto = {
          employeeId,
          projectId: formData.projectId,
          clientId: formData.clientId,
          projectStatusId: formData.projectStatusId,
          statusId: formData.statusId,
          probableNextAssignmentId: formData.probableNextAssignmentId,
          probableNextAssignmentDate: formData.probableNextAssignmentDate,
          billableDateProbabilityId: formData.billableDateProbabilityId,
          currentBillingStatusId: formData.currentBillingStatusId,
          billingBucketId: formData.billingBucketId,
          ageingBucketId: formData.ageingBucketId,
          actionItem: formData.actionItem,
          remarks: formData.remarks,
          startDate: formData.startDate,
          endDate: formData.endDate || null,
          allocationPercentage: formData.allocationPercentage,
          allocationType: formData.allocationType,
          billableStatus: formData.billableStatus,
          allocationStatus: computedStatus,
        };
        await allocationService.addProjectAllocation(dto);
      }
      setDialogOpen(false);
      await loadEmployeeData();
      toastService.success(editingAllocation ? 'Project allocation updated successfully.' : 'Project allocation saved successfully.');
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to save allocation');
    } finally {
      setSaving(false);
    }
  };

  const onFormSubmit = async (values: AllocationFormValues) => {
    setSaving(true);
    setError('');
    try {
      if (employeeData) {
        const payload = {
          employeeId,
          experienceInNV: nvExperienceYears,
          primarySkillId: values.primarySkillId,
          skillIds: values.skillIds,
          projectManagerId: values.projectManagerId,
          isActive: values.isActive,
          remarks: values.remarks ?? '',
        };
        await api.put(`/resource-allocations/employee/${employeeId}/details`, payload);
        const freshData = await allocationService.getEmployeeAllocations(employeeId);
        setEmployeeData(freshData);
        reset({
          ...values,
          projectManagerId: freshData.reportingManagerId ?? null,
          isActive: freshData.isActive,
        });
        setIsEditMode(false);
        toastService.success('Employee details saved successfully.');
      }
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to save allocation details');
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
    <Box sx={{ bgcolor: '#F8FAFC', minHeight: '100vh' }}>
      <Box sx={{ maxWidth: 1400, mx: 'auto', px: '24px', pt: '16px', pb: '24px' }}>
        <Button
          startIcon={<ArrowBackOutlinedIcon />}
          onClick={() => navigate('/rmg')}
          size="small"
          sx={{ textTransform: 'none', fontWeight: 600, fontSize: 12, color: '#6B7280', p: 0, minWidth: 0, mb: 1.5, '&:hover': { color: '#2563EB', bgcolor: 'transparent' } }}
        >
          Back to Dashboard
        </Button>



        <Box component="form" noValidate onSubmit={handleFormSubmit(onFormSubmit)}>
          {/* ── MAIN EDIT CARD ── */}
          <Paper
            elevation={0}
            sx={{
              border: '1px solid #E5E7EB',
              borderRadius: '16px',
              overflow: 'hidden',
              bgcolor: '#FFF',
              boxShadow: '0 1px 3px rgba(0,0,0,.04)',
              mb: 3,
              display: 'flex',
              flexDirection: 'column',
              maxHeight: 'calc(100vh - 260px)',
            }}
          >
            {/* Header */}
            <Box sx={{ px: '28px', py: 2.5, borderBottom: '1px solid #E5E7EB', flexShrink: 0 }}>
              <Typography sx={{ fontSize: 22, fontWeight: 700, color: '#111827', lineHeight: 1.2 }}>
                Edit Resource Allocation
              </Typography>
              <Typography sx={{ fontSize: 13, color: '#6B7280', mt: 0.25 }}>
                Update project allocation details for {employeeData.employeeName}
              </Typography>
            </Box>

            {/* Section: Allocation Details */}
            <Box
              sx={{
                overflowY: 'auto',
                flex: 1,
                '&::-webkit-scrollbar': { width: 6 },
                '&::-webkit-scrollbar-track': { bgcolor: '#F1F5F9' },
                '&::-webkit-scrollbar-thumb': { bgcolor: '#CBD5E1', borderRadius: 3 },
              }}
            >
              <Box sx={sectionHeaderSx}>
                <TimelineOutlinedIcon sx={{ ...sectionIconSx, color: '#059669' }} />
                <Box>
                  <Typography sx={{ fontSize: 15, fontWeight: 600, color: '#111827', lineHeight: 1.3 }}>
                    Allocation Details
                  </Typography>
                  <Typography sx={{ fontSize: 11, fontWeight: 400, color: '#6B7280', lineHeight: 1.3 }}>
                    Resource allocation and experience information
                  </Typography>
                </Box>
              </Box>
              <Box
                sx={{
                  display: 'grid',
                  gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' },
                  gap: '16px',
                  px: '24px',
                  py: 2.5,
                }}
              >
                <TextField
                  label="Experience Prior to NV"
                  value={priorDisplay}
                  size="small"
                  disabled
                  fullWidth
                  helperText="This value is maintained in Employee Management and cannot be edited here."
                  slotProps={{ input: { readOnly: true } }}
                  sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
                />
                <TextField
                  label="Experience in NV"
                  value={nvDisplay}
                  size="small"
                  disabled
                  fullWidth
                  helperText={emp?.doj ? 'Auto-calculated from Date of Joining' : 'Date of Joining not available'}
                  slotProps={{ input: { readOnly: true } }}
                  sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
                />
                <TextField
                  label="Total Experience (Prior + NV)"
                  value={totalDisplay}
                  size="small"
                  disabled
                  fullWidth
                  slotProps={{ input: { readOnly: true } }}
                  sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
                />
                <Controller
                  name="primarySkillId"
                  control={control}
                  render={({ field }) => {
                    const selected = skills.find((s) => s.id === field.value) || null;
                    return (
                      <Autocomplete
                        options={skills}
                        getOptionLabel={(option) => option.skillName}
                        isOptionEqualToValue={(option, value) => option.id === value.id}
                        value={selected}
                        onChange={(_, value) => field.onChange(value?.id ?? null)}
                        size="small"
                        fullWidth
                        disabled={!isEditMode}
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Primary Skill"
                            placeholder="Search skill"
                          />
                        )}
                      />
                    );
                  }}
                />
                <Controller
                  name="skillIds"
                  control={control}
                  render={({ field }) => {
                    const selected = skills.filter((s) => field.value.includes(s.id));
                    return (
                      <Autocomplete
                        multiple
                        options={skills}
                        getOptionLabel={(option) => option.skillName}
                        isOptionEqualToValue={(option, value) => option.id === value.id}
                        value={selected}
                        onChange={(_, value) => field.onChange(value.map((v) => v.id))}
                        disableCloseOnSelect
                        limitTags={2}
                        size="small"
                        fullWidth
                        disabled={!isEditMode}
                        renderOption={(props, option, { selected: isSelected }) => (
                          <li {...props}>
                            <Checkbox
                              icon={<CheckBoxOutlineBlankOutlinedIcon fontSize="small" />}
                              checkedIcon={<CheckBoxOutlinedIcon fontSize="small" />}
                              checked={isSelected}
                              sx={{ mr: 1 }}
                            />
                            {option.skillName}
                          </li>
                        )}
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Skill"
                            placeholder="Search skills"
                          />
                        )}
                      />
                    );
                  }}
                />
                <Controller
                  name="projectManagerId"
                  control={control}
                  render={({ field }) => {
                    const selected = employees.find((e) => e.id === field.value) || null;
                    return (
                      <Autocomplete
                        options={employees}
                        getOptionLabel={(option) => `${option.fullName} (${option.employeeCode})`}
                        isOptionEqualToValue={(option, value) => option.id === value.id}
                        value={selected}
                        onChange={(_, value) => field.onChange(value?.id ?? null)}
                        size="small"
                        fullWidth
                        disabled={!isEditMode}
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Project Manager"
                            placeholder="Search employee"
                          />
                        )}
                      />
                    );
                  }}
                />
                <Controller
                  name="isActive"
                  control={control}
                  render={({ field }) => (
                    <FormControlLabel
                      control={
                        <Checkbox
                          checked={field.value ?? false}
                          onChange={(e) => field.onChange(e.target.checked)}
                          disabled={!isEditMode}
                        />
                      }
                      label={<Typography sx={{ fontSize: 14, fontWeight: 500, color: '#374151' }}>Is Active</Typography>}
                      sx={{ mt: 1 }}
                    />
                  )}
                />
                <Box sx={{ gridColumn: { xs: '1 / -1', md: '1 / -1' } }}>
                  <Controller
                    name="remarks"
                    control={control}
                    render={({ field }) => (
                      <TextField
                        {...field}
                        label="Remarks"
                        multiline
                        minRows={3}
                        size="small"
                        fullWidth
                        value={field.value ?? ''}
                        slotProps={{ input: { readOnly: !isEditMode } }}
                        sx={{
                          '& .MuiInputBase-root': { resize: 'vertical', overflow: 'auto' },
                          ...(!isEditMode ? { '& .MuiInputBase-root': { bgcolor: 'action.hover' } } : {}),
                        }}
                      />
                    )}
                  />
                </Box>
              </Box>
            </Box>

            {/* Action Buttons */}
            <Box
              sx={{
                px: '24px',
                py: 2,
                borderTop: '1px solid #E5E7EB',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'flex-end',
                gap: 1.5,
                flexShrink: 0,
                position: 'sticky',
                bottom: 0,
                bgcolor: '#FFF',
                boxShadow: '0 -2px 8px rgba(0,0,0,.04)',
                zIndex: 10,
              }}
            >
              <Button
                variant="outlined"
                onClick={() => navigate('/rmg')}
                disabled={saving}
                startIcon={<ArrowBackOutlinedIcon />}
                sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px', px: 3 }}
              >
                Back
              </Button>
              {isEditMode ? (
                <Button
                  type="submit"
                  variant="contained"
                  disabled={saving}
                  startIcon={<SaveOutlinedIcon />}
                  sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px', px: 3, bgcolor: '#2563EB', '&:hover': { bgcolor: '#1D4ED8' } }}
                >
                  {saving ? 'Saving...' : 'Save Changes'}
                </Button>
              ) : (
                <Button
                  variant="contained"
                  onClick={() => setIsEditMode(true)}
                  startIcon={<EditOutlinedIcon />}
                  sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px', px: 3, bgcolor: '#2563EB', '&:hover': { bgcolor: '#1D4ED8' } }}
                >
                  Edit
                </Button>
              )}
            </Box>
          </Paper>
        </Box>

        {/* ── PROJECT ALLOCATIONS TABLE ── */}
        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
          <Stack direction="row" alignItems="center" justifyContent="space-between" sx={{ px: 3, py: 2, borderBottom: '1px solid', borderColor: 'divider', flexShrink: 0 }}>
            <Typography fontWeight={700} variant="h6">Project Allocations</Typography>
            <Button variant="contained" startIcon={<AddOutlinedIcon />} onClick={openAddDialog} size="small">
              Add Project Allocation
            </Button>
          </Stack>
          <TableContainer
            sx={{
              maxHeight: 400,
              overflowX: 'auto',
              overflowY: 'auto',
              '&::-webkit-scrollbar': { width: 6, height: 6 },
              '&::-webkit-scrollbar-track': { bgcolor: '#F1F5F9' },
              '&::-webkit-scrollbar-thumb': { bgcolor: '#CBD5E1', borderRadius: 3 },
            }}
          >
            <Table size="small" sx={{ minWidth: 900 }}>
              <TableHead sx={{ position: 'sticky', top: 0, zIndex: 2, bgcolor: '#FFF' }}>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Project</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Start Date</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>End Date</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Allocation %</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Billable Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Allocation Type</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Allocation Status</TableCell>
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
          <Alert severity="error" sx={{ mt: 2 }}>
            Total allocation percentage cannot exceed 100%. Current total: {totalAllocated}%.
          </Alert>
        )}
      </Box>

      <Dialog
        open={dialogOpen}
        onClose={() => !saving && setDialogOpen(false)}
        maxWidth={false}
        fullWidth
        PaperProps={{
          sx: {
            maxWidth: 1140,
            borderRadius: '16px',
            maxHeight: '85vh',
            display: 'flex',
            flexDirection: 'column',
          },
        }}
      >
        <DialogTitle sx={{ px: 3, py: 2.5, borderBottom: '1px solid #E5E7EB', fontSize: 18, fontWeight: 700, color: '#111827' }}>
          {editingAllocation ? 'Edit Project Allocation' : 'Add Project Allocation'}
        </DialogTitle>
        <DialogContent sx={{ px: 3, py: 2.5, overflowY: 'auto' }}>
          <Box
            sx={{
              display: 'grid',
              gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' },
              gap: '16px',
              pt: 1,
            }}
          >
            <Autocomplete
              options={projects}
              getOptionLabel={(option) => option.projectName}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={selectedProject}
              onChange={(_, value) => {
                const matchedClient = value?.clientName
                  ? clients.find((c) => c.name.toLowerCase() === value.clientName.toLowerCase())
                  : null;
                setFormData((prev) => ({
                  ...prev,
                  projectId: value?.id ?? 0,
                  projectName: value?.projectName ?? '',
                  clientId: matchedClient?.id ?? null,
                  clientName: matchedClient?.name ?? value?.clientName ?? '',
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Project Name *"
                  placeholder="Search project..."
                />
              )}
            />
            <TextField
              label="Project Code"
              fullWidth
              size="small"
              value={selectedProject?.projectCode ?? ''}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="Client"
              fullWidth
              size="small"
              value={formData.clientName}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <Autocomplete
              options={projectStatuses}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={projectStatuses.find((s) => s.id === formData.projectStatusId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  projectStatusId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Project Status *"
                  placeholder="Select Project Status"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
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
              label="Allocation Status"
              fullWidth
              size="small"
              value={computeAllocationStatus(formData.endDate)}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
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
            <Autocomplete
              options={statuses}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={statuses.find((s) => s.id === formData.statusId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  statusId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Status *"
                  placeholder="Select Status"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
            />
            <Autocomplete
              options={probableNextAssignments}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={probableNextAssignments.find((s) => s.id === formData.probableNextAssignmentId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  probableNextAssignmentId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Probable Next Assignment *"
                  placeholder="Select Probable Next Assignment"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
            />
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                label="Probable Next Assignment Date"
                value={formData.probableNextAssignmentDate ? parse(formData.probableNextAssignmentDate.split('T')[0], 'yyyy-MM-dd', new Date()) : null}
                onChange={(date) => {
                  setFormData((prev) => ({
                    ...prev,
                    probableNextAssignmentDate: date ? format(date, 'yyyy-MM-dd') : null,
                  }));
                }}
                format="dd-MM-yyyy"
                slotProps={{
                  textField: {
                    fullWidth: true,
                    size: 'small',
                    placeholder: 'Select Date',
                    slotProps: {
                      inputLabel: { shrink: true },
                    },
                  },
                }}
              />
            </LocalizationProvider>
            <Autocomplete
              options={billableDateProbabilities}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={billableDateProbabilities.find((s) => s.id === formData.billableDateProbabilityId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  billableDateProbabilityId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Billable Date Probability *"
                  placeholder="Select Billable Date Probability"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
            />
            <Autocomplete
              options={currentBillingStatuses}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={currentBillingStatuses.find((s) => s.id === formData.currentBillingStatusId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  currentBillingStatusId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Current Billing Status *"
                  placeholder="Select Current Billing Status"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
            />
            <Autocomplete
              options={billingBuckets}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={billingBuckets.find((s) => s.id === formData.billingBucketId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  billingBucketId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Billing Bucket *"
                  placeholder="Select Billing Bucket"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
            />
            <Autocomplete
              options={ageingBuckets}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={ageingBuckets.find((s) => s.id === formData.ageingBucketId) ?? null}
              onChange={(_, value) => {
                setFormData((prev) => ({
                  ...prev,
                  ageingBucketId: value?.id ?? null,
                }));
              }}
              fullWidth
              size="small"
              renderInput={(params) => (
                <TextField
                  {...params}
                  label="Ageing Bucket *"
                  placeholder="Select Ageing Bucket"
                  slotProps={{
                    inputLabel: { shrink: true },
                  }}
                />
              )}
            />
            <Box
              sx={{
                gridColumn: { xs: '1', sm: '1 / -1', md: '1 / -1' },
                display: 'grid',
                gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' },
                gap: '16px',
              }}
            >
              <TextField
                label="Action Item"
                fullWidth
                size="small"
                multiline
                minRows={3}
                maxRows={4}
                value={formData.actionItem ?? ''}
                onChange={(e) => setFormData({ ...formData, actionItem: e.target.value })}
                placeholder="Enter Action Item"
                slotProps={{ inputLabel: { shrink: true } }}
                inputProps={{ maxLength: 500 }}
              />
              <TextField
                label="Remark"
                fullWidth
                size="small"
                multiline
                minRows={3}
                maxRows={4}
                value={formData.remarks ?? ''}
                onChange={(e) => setFormData({ ...formData, remarks: e.target.value })}
                placeholder="Enter Remark"
                slotProps={{ inputLabel: { shrink: true } }}
                inputProps={{ maxLength: 1000 }}
              />
            </Box>
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2, borderTop: '1px solid #E5E7EB', flexShrink: 0 }}>
          <Button onClick={() => setDialogOpen(false)} disabled={saving} startIcon={<CancelOutlinedIcon />} sx={{ borderRadius: '8px', textTransform: 'none', fontWeight: 600 }}>
            Cancel
          </Button>
          <Button onClick={handleSave} variant="contained" disabled={saving} startIcon={<SaveOutlinedIcon />} sx={{ borderRadius: '8px', textTransform: 'none', fontWeight: 600, bgcolor: '#2563EB', '&:hover': { bgcolor: '#1D4ED8' } }}>
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
