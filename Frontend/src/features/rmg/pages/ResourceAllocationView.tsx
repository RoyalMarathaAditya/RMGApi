import { useEffect, useMemo, useState, SyntheticEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { format, parse } from 'date-fns';
import {
  Autocomplete, Box, Button, Chip, CircularProgress, Dialog, DialogActions,
  DialogContent, DialogTitle, FormControl, FormControlLabel,
  FormLabel, IconButton, MenuItem, Radio, RadioGroup, Stack, Tab, Tabs,
  TextField, Tooltip, Typography, Alert, Table, TableBody, TableCell,
  TableContainer, TableHead, TableRow, Paper, TablePagination, TableSortLabel,
  useTheme,
} from '@mui/material';
import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import PersonOutlineOutlinedIcon from '@mui/icons-material/PersonOutlineOutlined';
import WorkspacePremiumOutlinedIcon from '@mui/icons-material/WorkspacePremiumOutlined';
import AssignmentOutlinedIcon from '@mui/icons-material/AssignmentOutlined';
import TimelineOutlinedIcon from '@mui/icons-material/TimelineOutlined';
import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import BadgeOutlinedIcon from '@mui/icons-material/BadgeOutlined';
import EmailOutlinedIcon from '@mui/icons-material/EmailOutlined';
import PhoneOutlinedIcon from '@mui/icons-material/PhoneOutlined';
import WcOutlinedIcon from '@mui/icons-material/WcOutlined';
import BloodtypeOutlinedIcon from '@mui/icons-material/BloodtypeOutlined';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import PsychologyOutlinedIcon from '@mui/icons-material/PsychologyOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import AccountBalanceOutlinedIcon from '@mui/icons-material/AccountBalanceOutlined';
import HistoryEduOutlinedIcon from '@mui/icons-material/HistoryEduOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import ProfileHeader from '../../employees/components/ProfileHeader';
import InfoSection from '../../employees/components/InfoSection';
import InfoField from '../../employees/components/InfoField';
import InfoGrid from '../../employees/components/InfoGrid';
import ProjectDrawer from '../components/ProjectDrawer';
import EditExperienceModal from '../components/EditExperienceModal';
import { allocationService } from '../services/allocationService';
import api from '../../../services/api';
import { toastService } from '../../../services/toastService';
import { BILLABLE_STATUSES } from '../types/allocation';
import type { EmployeeResourceDetailsDto, ProjectAllocationDetailDto, ProjectAllocationDto, AddProjectAllocationDto, UpdateProjectAllocationDto, ApiProject } from '../types/allocation';

function safeArray<T>(arr: T[] | null | undefined | unknown): T[] {
  return Array.isArray(arr) ? arr : [];
}

function formatDate(dateStr: string | null | undefined): string {
  if (!dateStr) return '—';
  const d = new Date(dateStr);
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
}

function computeAllocationStatus(endDate: string): string {
  if (!endDate) return 'History';
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const end = new Date(endDate + 'T00:00:00');
  return end >= today ? 'Current' : 'History';
}

function computeDurationDays(startDate: string, endDate: string): number {
  if (!startDate || !endDate) return 0;
  const s = new Date(startDate + 'T00:00:00');
  const e = new Date(endDate + 'T00:00:00');
  return Math.max(0, Math.round((e.getTime() - s.getTime()) / (1000 * 60 * 60 * 24)) + 1);
}

function computeAgeingDays(startDate: string, allocationPct: number): number {
  if (!startDate) return 0;
  const s = new Date(startDate + 'T00:00:00');
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const rawDays = Math.round((today.getTime() - s.getTime()) / (1000 * 60 * 60 * 24)) + 1;
  return Math.max(0, Math.round(rawDays * allocationPct / 100));
}

const projectStatusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Billable: 'success', 'Non-Billable': 'warning', Shadow: 'info',
  Active: 'success', Planned: 'info', Completed: 'default',
  Released: 'warning', Cancelled: 'error',
};

type SortKey = 'project' | 'allocationPercentage' | 'startDate' | 'endDate' | 'client';
type SortDir = 'asc' | 'desc';

function getSectionConfig(theme: ReturnType<typeof useTheme>) {
  const isDark = theme.palette.mode === 'dark';
  return {
    personal: {
      icon: <PersonOutlineOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
      title: 'Personal Information',
      subtitle: 'Basic employee details and contact information',
      headerBg: isDark ? '#1E3A5F' : '#EFF6FF', accentColor: '#2563EB',
    },
    organization: {
      icon: <BadgeOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
      title: 'Organization Information',
      subtitle: 'Role, department and reporting structure',
      headerBg: isDark ? '#3B2E5F' : '#F5F3FF', accentColor: '#7C3AED',
    },
    employment: {
      icon: <AssignmentOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
      title: 'Employment Information',
      subtitle: 'Employment type, status and timeline',
      headerBg: isDark ? '#1B4D3B' : '#ECFDF5', accentColor: '#059669',
    },
    experience: {
      icon: <WorkspacePremiumOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
      title: 'Experience Information',
      subtitle: 'Skills, experience breakdown and certifications',
      headerBg: isDark ? '#5C3D1B' : '#FFF7ED', accentColor: '#D97706',
    },
    allocation: {
      icon: <TimelineOutlinedIcon sx={{ fontSize: '1.25rem' }} />,
      title: 'Allocation Information',
      subtitle: 'Project allocations and capacity summary',
      headerBg: isDark ? '#1B4D5C' : '#ECFEFF', accentColor: '#0891B2',
    },
  };
}

const tabs = ['Employee Information', 'Experience Details', 'Employment Details', 'Project Allocation'];

export default function ResourceAllocationView() {
  const theme = useTheme();
  const { employeeId } = useParams<{ employeeId: string }>();
  const navigate = useNavigate();
  const [data, setData] = useState<EmployeeResourceDetailsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [tabValue, setTabValue] = useState(0);
  const [allocationPage, setAllocationPage] = useState(0);
  const [allocationRowsPerPage, setAllocationRowsPerPage] = useState(5);
  const [sortKey, setSortKey] = useState<SortKey>('project');
  const [sortDir, setSortDir] = useState<SortDir>('asc');
  const [drawerProject, setDrawerProject] = useState<ProjectAllocationDetailDto | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [experienceModalOpen, setExperienceModalOpen] = useState(false);

  const [dialogOpen, setDialogOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [allocationToDelete, setAllocationToDelete] = useState<ProjectAllocationDto | null>(null);
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
    allocationPercentage: '' as string | number,
    billableStatus: '',
    allocationStatus: 'Active',
    engineering: null as boolean | null,
  });
  const [formError, setFormError] = useState('');
  const [dateError, setDateError] = useState('');
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [clients, setClients] = useState<{ id: number; name: string }[]>([]);
  const [projectStatuses, setProjectStatuses] = useState<{ id: string; name: string }[]>([]);
  const [statuses, setStatuses] = useState<{ id: string; name: string }[]>([]);
  const [probableNextAssignments, setProbableNextAssignments] = useState<{ id: string; name: string }[]>([]);
  const [billableDateProbabilities, setBillableDateProbabilities] = useState<{ id: string; name: string }[]>([]);
  const [currentBillingStatuses, setCurrentBillingStatuses] = useState<{ id: string; name: string }[]>([]);
  const [billingBuckets, setBillingBuckets] = useState<{ id: string; name: string }[]>([]);
  const [ageingBuckets, setAgeingBuckets] = useState<{ id: string; name: string }[]>([]);
  const [projects, setProjects] = useState<ApiProject[]>([]);
  const [projectsLoading, setProjectsLoading] = useState(false);

  const selectedProject = useMemo(() => {
    return safeArray(projects).find((p) => p.id === formData.projectId) ?? null;
  }, [projects, formData.projectId]);

  const selectedClient = useMemo(() => {
    return safeArray(clients).find((c) => c.id === formData.clientId) ?? null;
  }, [clients, formData.clientId]);

  const id = Number(employeeId);

  useEffect(() => {
    if (employeeId) {
      loadData();
      fetchClients();
      fetchProjects();
      fetchProjectStatuses();
      fetchStatuses();
      fetchProbableNextAssignments();
      fetchBillableDateProbabilities();
      fetchCurrentBillingStatuses();
      fetchBillingBuckets();
      fetchAgeingBuckets();
    }
  }, [employeeId]);

  const loadData = async () => {
    setLoading(true);
    setError('');
    try {
      const result = await allocationService.getEmployeeDetails(id);
      setData(result);
    } catch {
      setError('Failed to load employee details');
    } finally {
      setLoading(false);
    }
  };

  const refreshGrid = async () => {
    const result = await allocationService.getEmployeeDetails(id);
    setData(result);
  };

  const fetchClients = async () => {
    try {
      const res = await api.get<{ success: boolean; data: { id: number; name: string }[] }>('/clients');
      setClients(res.data.data ?? []);
    } catch {
      console.error('Failed to load clients');
    }
  };

  const fetchProjects = async () => {
    setProjectsLoading(true);
    try {
      const res = await api.get<ApiProject[]>('/projects/active');
      setProjects(res.data);
    } catch {
      console.error('Failed to load projects');
      toastService.error('Failed to load projects');
    } finally {
      setProjectsLoading(false);
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

  const handleTabChange = (_: SyntheticEvent, newValue: number) => setTabValue(newValue);

  const totalAllocated = useMemo(() => {
    if (!data) return 0;
    return safeArray(data.projectAllocations).reduce((sum, pa) => sum + (pa.allocationPercentage ?? 0), 0);
  }, [data]);

  const availableCapacity = Math.max(0, 100 - totalAllocated);

  const handleSort = (key: SortKey) => {
    if (sortKey === key) setSortDir((prev) => (prev === 'asc' ? 'desc' : 'asc'));
    else { setSortKey(key); setSortDir('asc'); }
  };

  const getOtherAllocationsTotal = (excludeId?: number) => {
    if (!data) return 0;
    const total = safeArray(data.projectAllocations).reduce((sum, a) => sum + (a.allocationPercentage ?? 0), 0);
    if (excludeId != null && editingAllocation) {
      return total - (editingAllocation.allocationPercentage ?? 0);
    }
    return total;
  };

  const openAddDialog = () => {
    setEditingAllocation(null);
    setFormData({
      projectId: 0, projectName: '', clientId: null, clientName: '',
      projectStatusId: null, statusId: null,
      probableNextAssignmentId: null, probableNextAssignmentDate: null,
      billableDateProbabilityId: null, currentBillingStatusId: null,
      billingBucketId: null, ageingBucketId: null,
      actionItem: null, remarks: null,
      startDate: '', endDate: '', allocationPercentage: '',
      billableStatus: '', allocationStatus: 'History',
      engineering: null,
    });
    setFormError('');
    setDateError('');
    setDialogOpen(true);
  };

  const openEditDialog = async (pa: ProjectAllocationDetailDto) => {
    setFormError('');
    setDateError('');
    try {
      const employeeAlloc = await allocationService.getEmployeeAllocations(id);
      const matched = safeArray(employeeAlloc.allocations).find(
        (a) => a.projectName === pa.project && a.allocationPercentage === (pa.allocationPercentage ?? 0)
      );
      if (!matched) {
        toastService.error('Could not load allocation details for editing');
        return;
      }
      setEditingAllocation(matched);
      const project = safeArray(projects).find((p) => p.id === matched.projectId);
      const clientName = matched.clientName || project?.clientName || '';
      const matchedClient = clientName ? safeArray(clients).find((c) => c.name.toLowerCase() === clientName.toLowerCase()) : null;
      setFormData({
        projectId: matched.projectId,
        projectName: matched.projectName,
        clientId: matched.clientId ?? matchedClient?.id ?? null,
        clientName: matchedClient?.name ?? clientName,
        projectStatusId: matched.projectStatusId ?? null,
        statusId: matched.statusId ?? null,
        probableNextAssignmentId: matched.probableNextAssignmentId ?? null,
        probableNextAssignmentDate: matched.probableNextAssignmentDate ?? null,
        billableDateProbabilityId: matched.billableDateProbabilityId ?? null,
        currentBillingStatusId: matched.currentBillingStatusId ?? null,
        billingBucketId: matched.billingBucketId ?? null,
        ageingBucketId: matched.ageingBucketId ?? null,
        actionItem: matched.actionItem ?? null,
        remarks: matched.remarks ?? null,
        startDate: matched.startDate.split('T')[0],
        endDate: matched.endDate ? matched.endDate.split('T')[0] : '',
        allocationPercentage: matched.allocationPercentage,
        billableStatus: matched.billableStatus ?? '',
        allocationStatus: matched.allocationStatus,
        engineering: pa.engineering === 'Yes' ? true : pa.engineering === 'No' ? false : null,
      });
      setFormError('');
      setDialogOpen(true);
    } catch {
      toastService.error('Failed to load allocation details');
    }
  };

  const handleSave = async () => {
    setFormError('');

    if (formData.engineering === null) {
      toastService.warning('Engineering selection is required.');
      return;
    }
    if (!formData.billableStatus) {
      toastService.warning('Please select Billable Status.');
      return;
    }
    if (!formData.projectId) {
      toastService.warning('Project Name is required');
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
      setDateError('End Date cannot be earlier than Start Date.');
      return;
    }
    const allocPct = Number(formData.allocationPercentage);
    if (!formData.allocationPercentage || allocPct < 1 || allocPct > 100) {
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
    if (!formData.currentBillingStatusId) {
      toastService.warning('Current Billing Status is required');
      return;
    }
    if (!formData.billingBucketId) {
      toastService.warning('Billing Bucket is required');
      return;
    }

    const otherTotal = getOtherAllocationsTotal(editingAllocation?.id);
    if (otherTotal + allocPct > 100) {
      toastService.warning(`Total allocation cannot exceed 100%. Current allocation: ${otherTotal}%. Adding ${allocPct}% would make it ${otherTotal + allocPct}%.`);
      return;
    }

    setSaving(true);
    try {
      if (editingAllocation) {
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
          actionItem: formData.actionItem,
          remarks: formData.remarks,
          startDate: formData.startDate,
          endDate: formData.endDate || null,
          allocationPercentage: allocPct,
          billableStatus: formData.billableStatus,
          allocationStatus: computeAllocationStatus(formData.endDate),
          engineering: formData.engineering ? 'Yes' : 'No',
        };
        await allocationService.updateProjectAllocation(editingAllocation.id, dto);
      } else {
        const dto: AddProjectAllocationDto = {
          employeeId: id,
          projectId: formData.projectId,
          clientId: formData.clientId,
          projectStatusId: formData.projectStatusId,
          statusId: formData.statusId,
          probableNextAssignmentId: formData.probableNextAssignmentId,
          probableNextAssignmentDate: formData.probableNextAssignmentDate,
          billableDateProbabilityId: formData.billableDateProbabilityId,
          currentBillingStatusId: formData.currentBillingStatusId,
          billingBucketId: formData.billingBucketId,
          actionItem: formData.actionItem,
          remarks: formData.remarks,
          startDate: formData.startDate,
          endDate: formData.endDate || null,
          allocationPercentage: allocPct,
          billableStatus: formData.billableStatus,
          allocationStatus: computeAllocationStatus(formData.endDate),
          engineering: formData.engineering ? 'Yes' : 'No',
        };
        await allocationService.addProjectAllocation(dto);
      }
      setDialogOpen(false);
      await refreshGrid();
      toastService.success(editingAllocation ? 'Project allocation updated successfully.' : 'Project allocation saved successfully.');
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to save allocation');
    } finally {
      setSaving(false);
    }
  };

  const openDeleteDialog = (pa: ProjectAllocationDetailDto) => {
    const alloc: ProjectAllocationDto = {
      id: 0,
      projectId: 0,
      projectName: pa.project ?? '',
      clientId: null,
      clientName: pa.client,
      projectStatusId: null,
      statusId: null,
      probableNextAssignmentId: null,
      probableNextAssignmentDate: null,
      billableDateProbabilityId: null,
      currentBillingStatusId: null,
      billingBucketId: null,
      ageingBucketId: null,
      actionItem: null,
      remarks: pa.remarks,
      startDate: pa.startDate ?? '',
      endDate: pa.endDate,
      allocationPercentage: pa.allocationPercentage ?? 0,
      billableStatus: null,
      allocationStatus: '',
    };
    setAllocationToDelete(alloc);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!allocationToDelete) return;
    setDeleting(true);
    try {
      const employeeAlloc = await allocationService.getEmployeeAllocations(id);
      const matched = safeArray(employeeAlloc.allocations).find(
        (a) => a.projectName === allocationToDelete.projectName && a.allocationPercentage === allocationToDelete.allocationPercentage
      );
      if (!matched) {
        toastService.error('Could not find the allocation to delete');
        setDeleting(false);
        setDeleteDialogOpen(false);
        return;
      }
      await allocationService.deleteProjectAllocation(matched.id);
      setDeleteDialogOpen(false);
      setAllocationToDelete(null);
      await refreshGrid();
      toastService.success('Project allocation deleted successfully.');
    } catch {
      toastService.error('Failed to delete allocation');
    } finally {
      setDeleting(false);
    }
  };

  const sortedAllocations = useMemo(() => {
    if (!data) return [];
    const list = [...safeArray(data.projectAllocations)];
    list.sort((a, b) => {
      let cmp = 0;
      switch (sortKey) {
        case 'project': cmp = (a.project ?? '').localeCompare(b.project ?? ''); break;
        case 'allocationPercentage': cmp = (a.allocationPercentage ?? 0) - (b.allocationPercentage ?? 0); break;
        case 'startDate': cmp = (a.startDate ?? '').localeCompare(b.startDate ?? ''); break;
        case 'endDate': cmp = (a.endDate ?? '').localeCompare(b.endDate ?? ''); break;
        case 'client': cmp = (a.client ?? '').localeCompare(b.client ?? ''); break;
      }
      return sortDir === 'asc' ? cmp : -cmp;
    });
    return list;
  }, [data, sortKey, sortDir]);

  const paginatedAllocations = useMemo(() => {
    return sortedAllocations.slice(allocationPage * allocationRowsPerPage, allocationPage * allocationRowsPerPage + allocationRowsPerPage);
  }, [sortedAllocations, allocationPage, allocationRowsPerPage]);

  const sectionConfig = useMemo(() => getSectionConfig(theme), [theme]);

  if (loading) {
    return (
      <Box sx={{ bgcolor: theme.palette.background.default, minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 2, flexDirection: 'column' }}>
        <CircularProgress size={40} sx={{ color: theme.palette.primary.main }} />
        <Typography sx={{ fontSize: 14, color: theme.palette.text.secondary }}>Loading employee details...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ bgcolor: theme.palette.background.default, minHeight: '100vh', p: 3 }}>
        <Alert severity="error" sx={{ borderRadius: 2, mb: 2 }}>{error}</Alert>
        <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate(-1)} sx={{ textTransform: 'none', fontWeight: 600 }}>Go Back</Button>
      </Box>
    );
  }

  if (!data) {
    return (
      <Box sx={{ bgcolor: theme.palette.background.default, minHeight: '100vh', p: 3 }}>
        <Box sx={{ maxWidth: 480, mx: 'auto', mt: 8, textAlign: 'center' }}>
          <Typography sx={{ fontSize: 20, fontWeight: 700, color: theme.palette.text.primary, mb: 1 }}>Employee Not Found</Typography>
          <Typography sx={{ fontSize: 14, color: theme.palette.text.secondary, mb: 3 }}>The requested employee does not exist or has been removed.</Typography>
          <Button startIcon={<ArrowBackOutlinedIcon />} onClick={() => navigate('/rmg')} sx={{ textTransform: 'none', fontWeight: 600 }}>Back to Dashboard</Button>
        </Box>
      </Box>
    );
  }

  return (
    <Box sx={{ bgcolor: theme.palette.background.default, minHeight: '100vh' }}>
      <Box sx={{ maxWidth: 1600, mx: 'auto', px: '24px', pt: '16px', pb: '24px' }}>
        <Button
          startIcon={<ArrowBackOutlinedIcon />}
          onClick={() => navigate('/rmg')}
          size="small"
          sx={{ textTransform: 'none', fontWeight: 600, fontSize: 12, color: theme.palette.text.secondary, p: 0, minWidth: 0, mb: 1.5, '&:hover': { color: theme.palette.primary.main, bgcolor: 'transparent' } }}
        >
          Back to Dashboard
        </Button>

        <Typography sx={{ fontSize: 32, fontWeight: 700, color: theme.palette.text.primary, lineHeight: 1.15, mb: 2 }}>
          Resource Allocation Preview
        </Typography>

        <ProfileHeader data={data} totalAllocated={totalAllocated} />

        <Box sx={{ mt: 2, borderBottom: `1px solid ${theme.palette.divider}`, bgcolor: theme.palette.background.paper, borderRadius: '12px 12px 0 0', border: `1px solid ${theme.palette.divider}`, borderBottom: 'none' }}>
          <Tabs
            value={tabValue}
            onChange={handleTabChange}
            variant="scrollable"
            scrollButtons="auto"
            sx={{
              minHeight: 48, px: 1.5,
              '& .MuiTab-root': {
                textTransform: 'none', fontWeight: 600, fontSize: '0.8125rem',
                minHeight: 48, px: 2.5, py: 0, color: theme.palette.text.secondary,
                borderRadius: '8px 8px 0 0', transition: 'all 200ms ease',
                '&.Mui-selected': { color: theme.palette.primary.main, bgcolor: theme.palette.action.selected, fontWeight: 700 },
                '&:hover:not(.Mui-selected)': { bgcolor: theme.palette.action.hover, color: theme.palette.text.primary },
              },
              '& .MuiTabs-indicator': { height: 3, borderRadius: '3px 3px 0 0', bgcolor: theme.palette.primary.main },
            }}
          >
            {tabs.map((tab) => (<Tab key={tab} label={tab} />))}
          </Tabs>
        </Box>

        {/* ── TAB 0: EMPLOYEE INFORMATION ── */}
        {tabValue === 0 && (
          <Box sx={{ mt: '16px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
            <InfoSection {...sectionConfig.personal}>
              <InfoGrid>
                <InfoField icon={<BadgeOutlinedIcon sx={{ fontSize: 16 }} />} label="Employee Code" value={data.employeeCode} />
                <InfoField icon={<PersonOutlineOutlinedIcon sx={{ fontSize: 16 }} />} label="Full Name" value={data.employeeName} />
                <InfoField icon={<EmailOutlinedIcon sx={{ fontSize: 16 }} />} label="Email" value={data.email} />
                <InfoField icon={<PhoneOutlinedIcon sx={{ fontSize: 16 }} />} label="Contact" value="—" />
                <InfoField icon={<WcOutlinedIcon sx={{ fontSize: 16 }} />} label="Gender" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Date of Birth" value="—" />
                <InfoField icon={<BloodtypeOutlinedIcon sx={{ fontSize: 16 }} />} label="Blood Group" value="—" />
                <InfoField icon={<WcOutlinedIcon sx={{ fontSize: 16 }} />} label="Marital Status" value="—" />
              </InfoGrid>
            </InfoSection>

            <InfoSection {...sectionConfig.organization}>
              <InfoGrid>
                <InfoField icon={<BadgeOutlinedIcon sx={{ fontSize: 16 }} />} label="Designation" value={data.role ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Department" value={data.practice ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Business Unit" value={data.practice ?? '—'} />
                <InfoField icon={<LocationOnOutlinedIcon sx={{ fontSize: 16 }} />} label="Office" value={data.location ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Primary Skill" value={data.primarySkill ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Secondary Skills" value={data.skill ?? '—'} colSpan={2} />
                <InfoField icon={<GroupsOutlinedIcon sx={{ fontSize: 16 }} />} label="Reporting Manager" value={data.l1Manager ?? '—'} />
                <InfoField icon={<GroupsOutlinedIcon sx={{ fontSize: 16 }} />} label="Resource Manager" value={data.practiceHead ?? '—'} />
                <InfoField icon={<LocationOnOutlinedIcon sx={{ fontSize: 16 }} />} label="Work Location" value={data.location ?? '—'} />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Joining Date" value={formatDate(data.doj)} />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Practice" value={data.practice ?? '—'} />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Sub Practice" value={data.subPractice ?? '—'} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ── TAB 1: EXPERIENCE DETAILS ── */}
        {tabValue === 1 && (
          <Box sx={{ mt: '16px', position: 'relative' }}>
            <InfoSection {...sectionConfig.experience}>
              <InfoGrid>
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Total Experience" value={`${data.totalExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="NV Experience" value={`${data.nvExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Prior Experience" value={`${data.priorExperience} yrs`} />
                <InfoField icon={<WorkspacePremiumOutlinedIcon sx={{ fontSize: 16 }} />} label="Experience Range" value={data.experienceRange} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Primary Skill" value={data.primarySkill ?? '—'} />
                <InfoField icon={<PsychologyOutlinedIcon sx={{ fontSize: 16 }} />} label="Key Skills" value={data.skill ?? '—'} colSpan={3} />
              </InfoGrid>
            </InfoSection>
            <IconButton
              onClick={() => setExperienceModalOpen(true)}
              sx={{ position: 'absolute', top: 0, right: 4, color: theme.palette.text.secondary, '&:hover': { color: theme.palette.primary.main, bgcolor: theme.palette.action.hover } }}
            >
              <EditOutlinedIcon fontSize="small" />
            </IconButton>
          </Box>
        )}

        {/* ── TAB 2: EMPLOYMENT DETAILS ── */}
        {tabValue === 2 && (
          <Box sx={{ mt: '16px' }}>
            <InfoSection {...sectionConfig.employment}>
              <InfoGrid>
                <InfoField icon={<AssignmentOutlinedIcon sx={{ fontSize: 16 }} />} label="Employment Status" value={data.status ?? '—'} />
                <InfoField icon={<AssignmentOutlinedIcon sx={{ fontSize: 16 }} />} label="Employee Type" value={data.fteConsultant ?? '—'} />
                <InfoField icon={<LocationOnOutlinedIcon sx={{ fontSize: 16 }} />} label="Work Model" value="—" />
                <InfoField icon={<HistoryEduOutlinedIcon sx={{ fontSize: 16 }} />} label="Notice Period" value="—" />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Joining Date" value={formatDate(data.doj)} />
                <InfoField icon={<CalendarTodayOutlinedIcon sx={{ fontSize: 16 }} />} label="Last Working Day" value="—" />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Billing Status" value={data.billable ?? '—'} />
                <InfoField icon={<AccountBalanceOutlinedIcon sx={{ fontSize: 16 }} />} label="Department" value={data.practice ?? '—'} />
              </InfoGrid>
            </InfoSection>
          </Box>
        )}

        {/* ── TAB 3: PROJECT ALLOCATION ── */}
        {tabValue === 3 && (
          <Box sx={{ mt: '16px', display: 'flex', flexDirection: 'column', gap: '16px' }}>
            <InfoSection {...sectionConfig.allocation}>
              <InfoGrid>
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Total Allocation" value={`${totalAllocated}%`} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Available Capacity" value={`${availableCapacity}%`} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Active Projects" value={safeArray(data.projectAllocations).length} />
                <InfoField icon={<TimelineOutlinedIcon sx={{ fontSize: 16 }} />} label="Utilisation" value={totalAllocated > 100 ? 'Overallocated' : totalAllocated >= 80 ? 'High' : 'Normal'} />
              </InfoGrid>
            </InfoSection>

            {/* Allocations Table */}
            <Box sx={{ borderRadius: '14px', border: `1px solid ${theme.palette.divider}`, overflow: 'hidden', bgcolor: theme.palette.background.paper, boxShadow: '0 1px 3px rgba(0,0,0,.04)' }}>
              <Box sx={{ px: '20px', py: 1.5, bgcolor: theme.palette.action.hover, borderBottom: `1px solid ${theme.palette.divider}`, display: 'flex', alignItems: 'center', gap: 1.5 }}>
                <TimelineOutlinedIcon sx={{ fontSize: '1rem', color: '#0891B2' }} />
                <Typography sx={{ fontSize: 14, fontWeight: 600, color: theme.palette.text.primary }}>Project Allocations</Typography>
                <Typography sx={{ fontSize: 11, fontWeight: 400, color: theme.palette.text.secondary, ml: 'auto' }}>{safeArray(data.projectAllocations).length} record(s)</Typography>
                <Button
                  variant="contained"
                  size="small"
                  startIcon={<AddOutlinedIcon />}
                  onClick={openAddDialog}
                  sx={{ textTransform: 'none', fontWeight: 600, fontSize: 12, borderRadius: '8px' }}
                >
                  Add Project Allocation
                </Button>
              </Box>

              {safeArray(data.projectAllocations).length === 0 ? (
                <Box sx={{ textAlign: 'center', py: 6 }}>
                  <Typography sx={{ fontSize: 14, color: theme.palette.text.secondary }}>No Active Allocations</Typography>
                </Box>
              ) : (
                <>
                  <TableContainer sx={{ maxHeight: 480, overflowX: 'auto' }}>
                    <Table size="small" sx={{ minWidth: 1600 }}>
                      <TableHead>
                        <TableRow>
                          {[
                            { key: 'project', label: 'Project Name' },
                            { key: null as any, label: 'Project Code' },
                            { key: 'client', label: 'Client Name' },
                            { key: null as any, label: 'Project Status' },
                            { key: 'allocationPercentage', label: 'Allocation %' },
                            { key: 'startDate', label: 'Start Date' },
                            { key: 'endDate', label: 'End Date' },
                            { key: null as any, label: 'Billable Status' },
                            { key: null as any, label: 'Allocation Status' },
                            { key: null as any, label: 'Status' },
                            { key: null as any, label: 'Engineering' },
                            { key: null as any, label: 'Billing Bucket' },
                            { key: null as any, label: 'Aging Bucket' },
                            { key: null as any, label: 'Duration' },
                            { key: null as any, label: 'Actions' },
                          ].map((col) => (
                            <TableCell
                              key={col.label}
                              sx={{
                                fontWeight: 600, fontSize: '0.7rem', color: theme.palette.text.secondary,
                                textTransform: 'uppercase', letterSpacing: '0.4px',
                                bgcolor: theme.palette.action.hover, borderBottom: `1px solid ${theme.palette.divider}`, py: 1.25,
                                position: 'sticky', top: 0, zIndex: 2, whiteSpace: 'nowrap',
                              }}
                            >
                              {col.key ? (
                                <TableSortLabel
                                  active={sortKey === col.key}
                                  direction={sortKey === col.key ? sortDir : 'asc'}
                                  onClick={() => handleSort(col.key)}
                                >
                                  {col.label}
                                </TableSortLabel>
                              ) : col.label}
                            </TableCell>
                          ))}
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {paginatedAllocations.map((pa, i) => {
                          const isOver = (pa.allocationPercentage ?? 0) > 100;
                          return (
                            <TableRow
                              key={i}
                              hover
                              sx={{ '&:hover': { bgcolor: theme.palette.action.hover }, '&:last-child td': { borderBottom: 'none' } }}
                            >
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 14, fontWeight: 600, color: theme.palette.text.primary }}>{pa.project ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, fontWeight: 600, color: theme.palette.text.secondary, fontFamily: 'monospace' }}>{pa.projectCode ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.client ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Chip
                                  label={pa.projectStatus ?? '—'}
                                  size="small"
                                  color={projectStatusColors[pa.projectStatus ?? ''] ?? 'default'}
                                  variant="outlined"
                                  sx={{ fontWeight: 600, fontSize: '0.65rem', height: 20, borderRadius: '999px' }}
                                />
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 14, fontWeight: 700, color: isOver ? '#DC2626' : (pa.allocationPercentage ?? 0) >= 100 ? '#F59E0B' : '#16A34A' }}>
                                  {pa.allocationPercentage != null ? `${pa.allocationPercentage}%` : '—'}
                                </Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{formatDate(pa.startDate)}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{formatDate(pa.endDate)}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                              <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.billableStatus ?? '—'}</Typography>
                            </TableCell>
                            <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.allocationStatus ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.status ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.engineering ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.billingBucket ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.ageingBucket ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Typography sx={{ fontSize: 13, color: theme.palette.text.secondary }}>{pa.durationInProject ?? '—'}</Typography>
                              </TableCell>
                              <TableCell sx={{ borderBottom: `1px solid ${theme.palette.divider}`, py: 1, whiteSpace: 'nowrap' }}>
                                <Stack direction="row" spacing={0.5}>
                                  <Tooltip title="View Details">
                                    <IconButton size="small" onClick={() => { setDrawerProject(pa); setDrawerOpen(true); }}
                                      sx={{ color: theme.palette.primary.main, '&:hover': { bgcolor: theme.palette.action.hover } }}>
                                      <VisibilityOutlinedIcon sx={{ fontSize: 16 }} />
                                    </IconButton>
                                  </Tooltip>
                                  <Tooltip title="Edit">
                                    <IconButton size="small" onClick={() => openEditDialog(pa)}
                                      sx={{ color: '#D97706', '&:hover': { bgcolor: theme.palette.action.hover } }}>
                                      <EditOutlinedIcon sx={{ fontSize: 16 }} />
                                    </IconButton>
                                  </Tooltip>
                                  <Tooltip title="Delete">
                                    <IconButton size="small" onClick={() => openDeleteDialog(pa)}
                                      sx={{ color: '#DC2626', '&:hover': { bgcolor: theme.palette.action.hover } }}>
                                      <DeleteOutlinedIcon sx={{ fontSize: 16 }} />
                                    </IconButton>
                                  </Tooltip>
                                </Stack>
                              </TableCell>
                            </TableRow>
                          );
                        })}
                      </TableBody>
                    </Table>
                  </TableContainer>
                  <TablePagination
                    component="div"
                    count={sortedAllocations.length}
                    page={allocationPage}
                    onPageChange={(_, p) => setAllocationPage(p)}
                    rowsPerPage={allocationRowsPerPage}
                    onRowsPerPageChange={(e) => { setAllocationRowsPerPage(Number(e.target.value)); setAllocationPage(0); }}
                    rowsPerPageOptions={[5, 10, 25]}
                    sx={{ borderTop: `1px solid ${theme.palette.divider}`, fontSize: '0.8125rem' }}
                  />
                </>
              )}
            </Box>
          </Box>
        )}
      </Box>

      <ProjectDrawer
        open={drawerOpen}
        onClose={() => setDrawerOpen(false)}
        project={drawerProject}
      />

      {/* ── Add/Edit Project Allocation Dialog ── */}
      <Dialog
        open={dialogOpen}
        onClose={() => !saving && setDialogOpen(false)}
        maxWidth={false}
        fullWidth
        PaperProps={{
          sx: { borderRadius: '14px', maxWidth: 1140, boxShadow: '0 20px 60px rgba(0,0,0,0.12)' },
        }}
      >
        <DialogTitle sx={{ px: 3, py: 2.5, borderBottom: `1px solid ${theme.palette.divider}`, fontSize: 18, fontWeight: 700, color: theme.palette.text.primary }}>
          {editingAllocation ? 'Edit Project Allocation' : 'Add Project Allocation'}
        </DialogTitle>
        <DialogContent sx={{ px: 3, py: 2.5, overflowY: 'auto' }}>
          {formError && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{formError}</Alert>}
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
              loading={projectsLoading}
              onChange={(_, value) => {
                const matchedClient = value?.clientId
                  ? safeArray(clients).find((c) => c.id === value.clientId)
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
                  placeholder="Select Project Name"
                  slotProps={{
                    input: {
                      ...params.InputProps,
                      endAdornment: (
                        <>
                          {projectsLoading ? <CircularProgress color="inherit" size={20} /> : null}
                          {params.InputProps.endAdornment}
                        </>
                      ),
                    },
                  }}
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
              value={formData.clientName || selectedClient?.name || selectedProject?.clientName || ''}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="Project Manager"
              fullWidth
              size="small"
              value={selectedProject?.projectManager ?? ''}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="Delivery Head"
              fullWidth
              size="small"
              value={selectedProject?.deliveryHead ?? ''}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="CSM"
              fullWidth
              size="small"
              value={selectedProject?.csm ?? selectedProject?.csmRevenueTypeName ?? ''}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <Autocomplete
              options={projectStatuses}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={safeArray(projectStatuses).find((s) => s.id === formData.projectStatusId) ?? null}
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
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <TextField
              label="Start Date *"
              type="date"
              fullWidth
              size="small"
              value={formData.startDate}
              onChange={(e) => {
                const newStart = e.target.value;
                setFormData((prev) => ({
                  ...prev,
                  startDate: newStart,
                  endDate: prev.endDate && newStart && prev.endDate < newStart ? '' : prev.endDate,
                }));
                if (newStart && formData.endDate && formData.endDate < newStart) {
                  setDateError('');
                } else if (newStart && formData.endDate && formData.endDate >= newStart) {
                  setDateError('');
                }
              }}
              slotProps={{ inputLabel: { shrink: true } }}
            />
            <TextField
              label="End Date *"
              type="date"
              fullWidth
              size="small"
              value={formData.endDate}
              onChange={(e) => {
                const newEnd = e.target.value;
                setFormData((prev) => ({ ...prev, endDate: newEnd }));
                if (formData.startDate && newEnd && newEnd < formData.startDate) {
                  setDateError('End Date cannot be earlier than Start Date.');
                } else {
                  setDateError('');
                }
              }}
              error={Boolean(dateError)}
              helperText={dateError}
              slotProps={{
                inputLabel: { shrink: true },
                htmlInput: { min: formData.startDate || undefined },
              }}
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
              label="Allocation % *"
              type="number"
              fullWidth
              size="small"
              value={formData.allocationPercentage}
              onChange={(e) => setFormData((prev) => ({ ...prev, allocationPercentage: e.target.value }))}
              slotProps={{ inputLabel: { shrink: true }, htmlInput: { min: 1, max: 100 } }}
            />
            <TextField
              select
              label="Billable Status *"
              fullWidth
              size="small"
              value={formData.billableStatus}
              onChange={(e) => setFormData((prev) => ({ ...prev, billableStatus: e.target.value }))}
              slotProps={{ inputLabel: { shrink: true } }}
            >
              <MenuItem value="" disabled sx={{ color: 'text.disabled' }}>
                Select Billable Status
              </MenuItem>
              {BILLABLE_STATUSES.map((s) => (
                <MenuItem key={s} value={s}>{s}</MenuItem>
              ))}
            </TextField>
            <Autocomplete
              options={statuses}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={safeArray(statuses).find((s) => s.id === formData.statusId) ?? null}
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
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <Autocomplete
              options={probableNextAssignments}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={safeArray(probableNextAssignments).find((s) => s.id === formData.probableNextAssignmentId) ?? null}
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
                  label="Probable Next Assignment"
                  placeholder="Select Probable Next Assignment"
                  slotProps={{ inputLabel: { shrink: true } }}
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
                    slotProps: { inputLabel: { shrink: true } },
                  },
                }}
              />
            </LocalizationProvider>
            <Autocomplete
              options={billableDateProbabilities}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={safeArray(billableDateProbabilities).find((s) => s.id === formData.billableDateProbabilityId) ?? null}
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
                  label="Billable Date Probability"
                  placeholder="Select Billable Date Probability"
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <Autocomplete
              options={currentBillingStatuses}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={safeArray(currentBillingStatuses).find((s) => s.id === formData.currentBillingStatusId) ?? null}
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
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <Autocomplete
              options={billingBuckets}
              getOptionLabel={(option) => option.name}
              isOptionEqualToValue={(option, value) => option.id === value.id}
              value={safeArray(billingBuckets).find((s) => s.id === formData.billingBucketId) ?? null}
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
                  slotProps={{ inputLabel: { shrink: true } }}
                />
              )}
            />
            <TextField
              label="Duration"
              fullWidth
              size="small"
              value={formData.startDate && formData.endDate ? `${computeDurationDays(formData.startDate, formData.endDate)} Days` : '—'}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="Ageing"
              fullWidth
              size="small"
              value={formData.startDate && formData.allocationPercentage ? `${computeAgeingDays(formData.startDate, Number(formData.allocationPercentage))} Days` : '—'}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <TextField
              label="Ageing Bucket"
              fullWidth
              size="small"
              value={(() => {
                if (!formData.startDate || !formData.allocationPercentage) return '—';
                const ageingDays = computeAgeingDays(formData.startDate, Number(formData.allocationPercentage));
                let index;
                if (ageingDays <= 30) index = 0;
                else if (ageingDays <= 90) index = 1;
                else if (ageingDays <= 181) index = 2;
                else index = 3;
                return safeArray(ageingBuckets)[index]?.name ?? '—';
              })()}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }}
            />
            <FormControl sx={{ justifyContent: 'center' }}>
              <FormLabel id="engineering-radio-label" sx={{ fontSize: 14, fontWeight: 500, color: theme.palette.text.secondary, mb: 0.5, '&.Mui-focused': { color: theme.palette.text.secondary } }}>
                Engineering *
              </FormLabel>
              <RadioGroup
                row
                aria-labelledby="engineering-radio-label"
                value={formData.engineering === null ? '' : formData.engineering ? 'Yes' : 'No'}
                onChange={(e) => setFormData((prev) => ({ ...prev, engineering: e.target.value === 'Yes' }))}
              >
                <FormControlLabel value="Yes" control={<Radio size="small" />} label="Yes" />
                <FormControlLabel value="No" control={<Radio size="small" />} label="No" />
              </RadioGroup>
            </FormControl>
            <TextField
              label="Action Item"
              fullWidth
              size="small"
              multiline
              minRows={3}
              value={formData.actionItem ?? ''}
              onChange={(e) => setFormData((prev) => ({ ...prev, actionItem: e.target.value || null }))}
              slotProps={{ inputLabel: { shrink: true } }}
            />
            <TextField
              label="Remark"
              fullWidth
              size="small"
              multiline
              minRows={3}
              value={formData.remarks ?? ''}
              onChange={(e) => setFormData((prev) => ({ ...prev, remarks: e.target.value || null }))}
              slotProps={{ inputLabel: { shrink: true } }}
            />
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2, borderTop: `1px solid ${theme.palette.divider}`, gap: 1 }}>
          <Button
            onClick={() => setDialogOpen(false)}
            disabled={saving}
            sx={{ textTransform: 'none', fontWeight: 600, color: theme.palette.text.secondary }}
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={handleSave}
            disabled={saving}
            startIcon={saving ? <CircularProgress size={16} color="inherit" /> : undefined}
            sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px' }}
          >
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* ── Delete Confirmation Dialog ── */}
      <Dialog
        open={deleteDialogOpen}
        onClose={() => !deleting && setDeleteDialogOpen(false)}
        maxWidth="xs"
        fullWidth
        PaperProps={{ sx: { borderRadius: '14px' } }}
      >
        <DialogTitle sx={{ px: 3, py: 2.5, borderBottom: `1px solid ${theme.palette.divider}`, fontSize: 16, fontWeight: 700, color: theme.palette.text.primary }}>
          Delete Project Allocation
        </DialogTitle>
        <DialogContent sx={{ px: 3, py: 2.5 }}>
          <Typography sx={{ fontSize: 14, color: theme.palette.text.secondary }}>
            Are you sure you want to delete this project allocation?
          </Typography>
          {allocationToDelete && (
            <Typography sx={{ fontSize: 14, color: theme.palette.text.secondary, mt: 1 }}>
              Project: <strong>{allocationToDelete.projectName}</strong>
            </Typography>
          )}
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2, borderTop: `1px solid ${theme.palette.divider}`, gap: 1 }}>
          <Button
            onClick={() => setDeleteDialogOpen(false)}
            disabled={deleting}
            sx={{ textTransform: 'none', fontWeight: 600, color: theme.palette.text.secondary }}
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            color="error"
            onClick={handleDeleteConfirm}
            disabled={deleting}
            startIcon={deleting ? <CircularProgress size={16} color="inherit" /> : undefined}
            sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px' }}
          >
            {deleting ? 'Deleting...' : 'Delete'}
          </Button>
        </DialogActions>
      </Dialog>

      {data && (
        <EditExperienceModal
          open={experienceModalOpen}
          onClose={() => setExperienceModalOpen(false)}
          onSaved={refreshGrid}
          employeeId={id}
          data={data}
        />
      )}
    </Box>
  );
}
