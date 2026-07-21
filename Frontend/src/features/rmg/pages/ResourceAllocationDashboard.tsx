import { memo, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  Alert,
  Autocomplete,
  Box,
  Button,
  Card,
  CardContent,
  Checkbox,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  FormControlLabel,
  FormLabel,
  IconButton,
  InputAdornment,
  MenuItem,
  Paper,
  Radio,
  RadioGroup,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import RefreshOutlinedIcon from '@mui/icons-material/RefreshOutlined';
import ReplayIcon from '@mui/icons-material/Replay';
import VisibilityOutlinedIcon from '@mui/icons-material/VisibilityOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import PersonOffOutlinedIcon from '@mui/icons-material/PersonOffOutlined';
import AssignmentTurnedInOutlinedIcon from '@mui/icons-material/AssignmentTurnedInOutlined';
import HourglassEmptyOutlinedIcon from '@mui/icons-material/HourglassEmptyOutlined';
import PeopleAltOutlinedIcon from '@mui/icons-material/PeopleAltOutlined';
import CallSplitOutlinedIcon from '@mui/icons-material/CallSplitOutlined';
import DownloadIcon from '@mui/icons-material/Download';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
import PageLoader from '../../../components/common/PageLoader';
import type { DashboardSummaryDto, DashboardGridDto, PaginatedResponse } from '../types/dashboard';
import { dashboardService } from '../services/dashboardService';
import { allocationService } from '../services/allocationService';
import { columnMappingService } from '../../columnMappings/services/columnMappingService';
import type { ColumnMapping } from '../../columnMappings/types/columnMapping';
import type { ApiProject, BulkProjectAllocationDto } from '../types/allocation';
import { BILLABLE_STATUSES } from '../types/allocation';
import api from '../../../services/api';
import { toastService } from '../../../services/toastService';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Available: 'success',
  'Partially Allocated': 'info',
  'Fully Allocated': 'warning',
  Overallocated: 'error',
  Bench: 'default',
  'On Leave': 'error',
  Inactive: 'default',
};

interface ColumnDef {
  field: keyof DashboardGridDto;
  headerName: string;
  dataType: string;
}

type CellRenderer = (row: DashboardGridDto) => React.ReactNode;

const cellRenderers: Partial<Record<keyof DashboardGridDto, CellRenderer>> = {
  employeeName: (row) => (
    <Typography fontWeight={600} variant="body2">{row.employeeName}</Typography>
  ),
  employeeCode: (row) => <>{row.employeeCode}</>,
  designation: (row) => <>{row.designation ?? '-'}</>,
  totalExperience: (row) => <>{row.totalExperience > 0 ? `${row.totalExperience} Years` : '-'}</>,
  practice: (row) => <>{row.practice}</>,
  subPractice: (row) => (
    row.subPractice ? (
      <Tooltip title={row.subPractice} placement="bottom-start">
        <Typography variant="body2" noWrap>{row.subPractice}</Typography>
      </Tooltip>
    ) : <>{'-'}</>
  ),
  allocationPercentage: (row) => (
    <Typography fontWeight={600} color={row.allocationPercentage > 100 ? 'error.main' : row.allocationPercentage >= 100 ? 'warning.main' : 'text.primary'}>
      {row.allocationPercentage}%
    </Typography>
  ),
  availableCapacity: (row) => <>{row.availableCapacity}%</>,
  resourceStatus: (row) => (
    <Chip label={row.resourceStatus} size="small" color={statusColors[row.resourceStatus] ?? 'default'} variant="outlined" />
  ),
};

const defaultCellRenderer = (row: DashboardGridDto, field: keyof DashboardGridDto) => {
  const val = row[field];
  return <Typography variant="body2" noWrap>{val === null || val === undefined ? '-' : String(val)}</Typography>;
};

const dashboardFieldSet = new Set<keyof DashboardGridDto>([
  'employeeName', 'employeeCode', 'designation', 'totalExperience',
  'practice', 'subPractice', 'allocationPercentage', 'availableCapacity', 'resourceStatus',
]);

const defaultColumns: ColumnDef[] = [
  { field: 'employeeName', headerName: 'Employee', dataType: 'string' },
  { field: 'employeeCode', headerName: 'Code', dataType: 'string' },
  { field: 'designation', headerName: 'Designation', dataType: 'string' },
  { field: 'totalExperience', headerName: 'Total Experience', dataType: 'decimal' },
  { field: 'practice', headerName: 'Practice', dataType: 'string' },
  { field: 'subPractice', headerName: 'Sub Practice', dataType: 'string' },
  { field: 'allocationPercentage', headerName: 'Allocation %', dataType: 'decimal' },
  { field: 'availableCapacity', headerName: 'Availability %', dataType: 'decimal' },
  { field: 'resourceStatus', headerName: 'Status', dataType: 'string' },
];

const SummaryCard = memo(function SummaryCard({ icon: Icon, label, value, color }: {
  icon: React.ElementType;
  label: string;
  value: number;
  color: string;
}) {
  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
      <CardContent sx={{ p: 1.5, '&:last-child': { pb: 1.5 } }}>
        <Stack alignItems="center" spacing={0.5}>
          <Box
            alignItems="center"
            bgcolor={`${color}15`}
            borderRadius={1.5}
            color={color}
            display="flex"
            height={36}
            justifyContent="center"
            width={36}
          >
            <Icon fontSize="small" />
          </Box>
          <Typography color="text.secondary" align="center" noWrap variant="caption">
            {label}
          </Typography>
          <Typography fontWeight={700} variant="h5">
            {value}
          </Typography>
        </Stack>
      </CardContent>
    </Card>
  );
});

const DashboardTableRow = memo(function DashboardTableRow({
  row,
  columns,
  selectedEmployeeIds,
  isDisabled,
  onSelect,
}: {
  row: DashboardGridDto;
  columns: ColumnDef[];
  selectedEmployeeIds: number[];
  isDisabled: boolean;
  onSelect: (id: number, checked: boolean) => void;
}) {
  const navigate = useNavigate();
  const isSelected = selectedEmployeeIds.includes(row.employeeId);

  return (
    <TableRow
      hover
      sx={{ cursor: 'pointer', opacity: !row.isActive ? 0.6 : undefined }}
      onClick={() => navigate(`/rmg/view/${row.employeeId}`)}
    >
      <TableCell sx={{ width: 48, px: 1 }} onClick={(e) => e.stopPropagation()}>
        <Checkbox
          size="small"
          disabled={isDisabled}
          checked={isSelected}
          onChange={(e) => onSelect(row.employeeId, e.target.checked)}
          sx={isDisabled ? { opacity: 0.4 } : undefined}
        />
      </TableCell>
      {columns.map((col) => (
        <TableCell key={(col.field as string)}>
          {cellRenderers[col.field]?.(row) ?? defaultCellRenderer(row, col.field)}
        </TableCell>
      ))}
      <TableCell align="right">
        <Stack direction="row" spacing={0.5} justifyContent="flex-end" onClick={(e) => e.stopPropagation()}>
          <Tooltip title="View Details">
            <IconButton size="small" onClick={() => navigate(`/rmg/view/${row.employeeId}`)}>
              <VisibilityOutlinedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      </TableCell>
    </TableRow>
  );
});

export default function ResourceAllocationDashboard() {
  const navigate = useNavigate();
  const [summary, setSummary] = useState<DashboardSummaryDto | null>(null);
  const [gridResponse, setGridResponse] = useState<PaginatedResponse<DashboardGridDto> | null>(null);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [practiceFilter, setPracticeFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);
  const [columns, setColumns] = useState<ColumnDef[]>(defaultColumns);
  const abortRef = useRef<AbortController | null>(null);

  const [selectedEmployeeIds, setSelectedEmployeeIds] = useState<number[]>([]);
  const [bulkDialogOpen, setBulkDialogOpen] = useState(false);
  const [bulkSaving, setBulkSaving] = useState(false);
  const [bulkFormError, setBulkFormError] = useState('');
  const [bulkDateError, setBulkDateError] = useState('');
  const [projects, setProjects] = useState<ApiProject[]>([]);
  const [projectStatuses, setProjectStatuses] = useState<{ id: string; name: string }[]>([]);
  const [statuses, setStatuses] = useState<{ id: string; name: string }[]>([]);
  const [probableNextAssignments, setProbableNextAssignments] = useState<{ id: string; name: string }[]>([]);
  const [billableDateProbabilities, setBillableDateProbabilities] = useState<{ id: string; name: string }[]>([]);
  const [currentBillingStatuses, setCurrentBillingStatuses] = useState<{ id: string; name: string }[]>([]);
  const [billingBuckets, setBillingBuckets] = useState<{ id: string; name: string }[]>([]);
  const [ageingBuckets, setAgeingBuckets] = useState<{ id: string; name: string }[]>([]);
  const [clients, setClients] = useState<{ id: number; name: string }[]>([]);

  const loadColumns = useCallback(async () => {
    try {
      const data = await columnMappingService.getAll();
      const active = data
        .filter((m: ColumnMapping) => m.entityType === 'resource-allocation' && m.isActive)
        .sort((a: ColumnMapping, b: ColumnMapping) => a.displayOrder - b.displayOrder);
      if (active.length > 0) {
        const cols = active
          .map((m: ColumnMapping) => ({
            field: m.targetProperty.charAt(0).toLowerCase() + m.targetProperty.slice(1) as keyof DashboardGridDto,
            headerName: m.targetDisplayName,
            dataType: m.dataType,
          }))
          .filter((col) => dashboardFieldSet.has(col.field));
        if (cols.length > 0) {
          setColumns(cols);
        }
      }
    } catch (err) {
      console.error('[Dashboard] Failed to load column mappings:', err);
    }
  }, []);

  const loadData = useCallback(async () => {
    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;

    setLoading(true);
    setLoadError('');
    try {
      const summaryData = await dashboardService.getSummary();
      const grid = await dashboardService.getGridData(
        { searchTerm: searchTerm || undefined, practice: practiceFilter || undefined, resourceStatus: statusFilter || undefined },
        { page: page + 1, pageSize: rowsPerPage }
      );
      if (!controller.signal.aborted) {
        setSummary(summaryData);
        setGridResponse(grid);
      }
    } catch (err: any) {
      if (!controller.signal.aborted) {
        const message = err?.response?.data?.message || err?.message || 'Failed to load dashboard data';
        console.error('Dashboard load error:', err);
        setLoadError(message);
      }
    } finally {
      if (!controller.signal.aborted) {
        setLoading(false);
      }
    }
  }, [searchTerm, practiceFilter, statusFilter, page, rowsPerPage]);

  useEffect(() => {
    loadData();
    loadColumns();
    return () => {
      abortRef.current?.abort();
    };
  }, [loadData, loadColumns]);

  const summaryCards = useMemo(() => {
    if (!summary) return [];
    return [
      { icon: PeopleAltOutlinedIcon, label: 'Total Employees', value: summary.totalEmployees, color: 'primary.main' },
      { icon: GroupsOutlinedIcon, label: 'Total Practices', value: summary.totalPractices, color: 'secondary.main' },
      { icon: HourglassEmptyOutlinedIcon, label: 'Available', value: summary.availableResources, color: 'success.main' },
      { icon: CallSplitOutlinedIcon, label: 'Partially Allocated', value: summary.partiallyAllocatedResources, color: 'info.main' },
      { icon: WorkOutlineOutlinedIcon, label: 'Fully Allocated', value: summary.fullyAllocatedResources, color: 'warning.main' },
      { icon: AssignmentTurnedInOutlinedIcon, label: 'Total Allocated', value: summary.totalAllocatedResources, color: 'info.main' },
      { icon: PersonOffOutlinedIcon, label: 'Bench', value: summary.benchResources, color: 'text.secondary' },
    ];
  }, [summary]);

  const gridData = gridResponse?.items ?? [];
  const totalCount = gridResponse?.totalCount ?? 0;

  const handleSelectAll = (checked: boolean) => {
    if (checked) {
      const selectable = gridData
        .filter((r) => r.isActive && r.allocationPercentage < 100)
        .map((r) => r.employeeId);
      setSelectedEmployeeIds(selectable);
    } else {
      setSelectedEmployeeIds([]);
    }
  };

  const handleSelectOne = (employeeId: number, checked: boolean) => {
    setSelectedEmployeeIds((prev) =>
      checked ? [...prev, employeeId] : prev.filter((id) => id !== employeeId)
    );
  };

  const handleSearch = () => {
    setPage(0);
    loadData();
  };

  const handleFilterChange = (setter: React.Dispatch<React.SetStateAction<string>>) => (value: string) => {
    setter(value);
    setPage(0);
  };

  const [bulkFormData, setBulkFormData] = useState({
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

  const selectedProject = useMemo(() => {
    return projects.find((p) => p.id === (bulkFormData?.projectId ?? 0)) ?? null;
  }, [projects, bulkFormData?.projectId]);

  const fetchBulkDropdownData = useCallback(async () => {
    const promises = [];
    promises.push(api.get<ApiProject[]>('/projects/active').then(r => setProjects(r.data)).catch(() => {}));
    promises.push(api.get('/project-status').then(r => setProjectStatuses(r.data.data ?? [])).catch(() => {}));
    promises.push(api.get('/master/statuses').then(r => setStatuses(r.data ?? [])).catch(() => {}));
    promises.push(api.get('/probable-next-assignments').then(r => setProbableNextAssignments(r.data.data ?? [])).catch(() => {}));
    promises.push(api.get('/billable-date-probabilities').then(r => setBillableDateProbabilities(r.data.data ?? [])).catch(() => {}));
    promises.push(api.get('/current-billing-statuses').then(r => setCurrentBillingStatuses(r.data.data ?? [])).catch(() => {}));
    promises.push(api.get('/billing-buckets').then(r => setBillingBuckets(r.data.data ?? [])).catch(() => {}));
    promises.push(api.get('/ageing-buckets').then(r => setAgeingBuckets(r.data.data ?? [])).catch(() => {}));
    promises.push(api.get<{ success: boolean; data: { id: number; name: string }[] }>('/clients').then(r => setClients(r.data.data ?? [])).catch(() => {}));
    await Promise.all(promises);
  }, []);

  useEffect(() => {
    fetchBulkDropdownData();
  }, [fetchBulkDropdownData]);

  const computeAllocationStatus = (endDate: string) => {
    if (!endDate) return 'History';
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const end = new Date(endDate + 'T00:00:00');
    return end >= today ? 'Current' : 'History';
  };

  const computeDurationDays = (startDate: string, endDate: string) => {
    if (!startDate || !endDate) return 0;
    const s = new Date(startDate + 'T00:00:00');
    const e = new Date(endDate + 'T00:00:00');
    return Math.max(0, Math.round((e.getTime() - s.getTime()) / (1000 * 60 * 60 * 24)) + 1);
  };

  const computeAgeingDays = (startDate: string, allocationPct: number) => {
    if (!startDate) return 0;
    const s = new Date(startDate + 'T00:00:00');
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const rawDays = Math.round((today.getTime() - s.getTime()) / (1000 * 60 * 60 * 24)) + 1;
    return Math.max(0, Math.round(rawDays * allocationPct / 100));
  };

  const openBulkDialog = () => {
    setBulkFormData({
      projectId: 0, projectName: '', clientId: null, clientName: '',
      projectStatusId: null, statusId: null,
      probableNextAssignmentId: null, probableNextAssignmentDate: null,
      billableDateProbabilityId: null, currentBillingStatusId: null,
      billingBucketId: null, ageingBucketId: null,
      actionItem: null, remarks: null,
      startDate: new Date().toISOString().slice(0, 10), endDate: '', allocationPercentage: 100,
      billableStatus: 'Billable', allocationStatus: 'History',
      engineering: null,
    });
    setBulkFormError('');
    setBulkDateError('');
    setBulkDialogOpen(true);
  };

  const handleBulkSave = async () => {
    setBulkFormError('');
    if (bulkFormData.engineering === null) { toastService.warning('Engineering selection is required.'); return; }
    if (!bulkFormData.billableStatus) { toastService.warning('Please select Billable Status.'); return; }
    if (!bulkFormData.projectId) { toastService.warning('Project Name is required'); return; }
    if (!bulkFormData.startDate) { toastService.warning('Start date is required'); return; }
    if (!bulkFormData.endDate) { toastService.warning('End date is required'); return; }
    if (bulkFormData.endDate < bulkFormData.startDate) { setBulkDateError('End Date cannot be earlier than Start Date.'); return; }
    const allocPct = Number(bulkFormData.allocationPercentage);
    if (!bulkFormData.allocationPercentage || allocPct < 1 || allocPct > 100) { toastService.warning('Allocation percentage must be between 1 and 100'); return; }
    if (!bulkFormData.projectStatusId) { toastService.warning('Project Status is required'); return; }
    if (!bulkFormData.statusId) { toastService.warning('Status is required'); return; }
    if (!bulkFormData.currentBillingStatusId) { toastService.warning('Current Billing Status is required'); return; }
    if (!bulkFormData.billingBucketId) { toastService.warning('Billing Bucket is required'); return; }

    setBulkSaving(true);
    try {
      const dto: BulkProjectAllocationDto = {
        employeeIds: selectedEmployeeIds,
        projectId: bulkFormData.projectId,
        clientId: bulkFormData.clientId,
        projectStatusId: bulkFormData.projectStatusId,
        statusId: bulkFormData.statusId,
        probableNextAssignmentId: bulkFormData.probableNextAssignmentId,
        probableNextAssignmentDate: bulkFormData.probableNextAssignmentDate,
        billableDateProbabilityId: bulkFormData.billableDateProbabilityId,
        currentBillingStatusId: bulkFormData.currentBillingStatusId,
        billingBucketId: bulkFormData.billingBucketId,
        actionItem: bulkFormData.actionItem,
        remarks: bulkFormData.remarks,
        startDate: bulkFormData.startDate,
        endDate: bulkFormData.endDate || null,
        allocationPercentage: allocPct,
        billableStatus: bulkFormData.billableStatus,
        allocationStatus: computeAllocationStatus(bulkFormData.endDate),
        engineering: bulkFormData.engineering ? 'Yes' : 'No',
      };
      await allocationService.addBulkProjectAllocation(dto);
      setBulkDialogOpen(false);
      setSelectedEmployeeIds([]);
      await loadData();
      toastService.success('Bulk allocation completed successfully.');
    } catch (err: any) {
      const message = err.response?.data?.message || 'Failed to save bulk allocation';
      setBulkFormError(message);
    } finally {
      setBulkSaving(false);
    }
  };

  const handleExport = () => {
    dashboardService.exportGridData({
      searchTerm: searchTerm || undefined,
      practice: practiceFilter || undefined,
      resourceStatus: statusFilter || undefined,
    });
  };

  const practices = useMemo(() => {
    const set = new Set(gridData.map((r) => r.practice).filter(Boolean));
    return Array.from(set).sort();
  }, [gridData]);

  const handlePageChange = (_: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleRowsPerPageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(Number(event.target.value));
    setPage(0);
  };

  if (loading && !summary) {
    return <PageLoader message="Loading Resource Allocation Dashboard..." />;
  }

  if (loadError && !summary) {
    return (
      <PageContainer title="Resource Allocation Dashboard">
        <Alert
          action={<Button color="inherit" onClick={loadData} size="small" startIcon={<ReplayIcon />}>Retry</Button>}
          severity="error"
          sx={{ borderRadius: 2 }}
        >
          {loadError}
        </Alert>
      </PageContainer>
    );
  }

  return (
    <PageContainer title="Resource Allocation Dashboard">
      <Stack spacing={3}>
        <Box
          sx={{
            display: 'grid',
            gap: 1.5,
            gridTemplateColumns: {
              xs: '1fr',
              sm: 'repeat(2, 1fr)',
              md: 'repeat(4, 1fr)',
              lg: 'repeat(7, 1fr)',
            },
            width: '100%',
          }}
        >
          {summaryCards.map((card) => (
            <SummaryCard key={card.label} {...card} />
          ))}
        </Box>

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <Stack direction="row" alignItems="center" justifyContent="space-between" flexWrap="wrap" gap={2} sx={{ px: 3, py: 2 }}>
            <Typography fontWeight={800} variant="h5">
              Resources
            </Typography>
            <Stack direction="row" spacing={1.5} alignItems="center" flexWrap="wrap">
              {selectedEmployeeIds.length > 0 && (
                <Button
                  variant="contained"
                  size="small"
                  onClick={openBulkDialog}
                  sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px', bgcolor: '#2563EB', '&:hover': { bgcolor: '#1D4ED8' }, whiteSpace: 'nowrap' }}
                >
                  Bulk Allocate ({selectedEmployeeIds.length})
                </Button>
              )}
              <TextField
                disabled={loading}
                size="small"
                placeholder="Search name or code..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                onKeyDown={(e) => { if (e.key === 'Enter') handleSearch(); }}
                slotProps={{
                  input: {
                    startAdornment: (
                      <InputAdornment position="start">
                        <SearchOutlinedIcon fontSize="small" />
                      </InputAdornment>
                    ),
                    endAdornment: searchTerm ? (
                      <Button size="small" onClick={handleSearch} sx={{ minWidth: 'auto', p: '2px 8px' }}>
                        Search
                      </Button>
                    ) : undefined,
                  },
                }}
                sx={{ minWidth: 250 }}
              />
              <Select disabled={loading} size="small" value={practiceFilter} onChange={(e) => handleFilterChange(setPracticeFilter)(e.target.value)} displayEmpty sx={{ minWidth: 140 }}>
                <MenuItem value="">All Practices</MenuItem>
                {practices.map((p) => (
                  <MenuItem key={p} value={p}>{p}</MenuItem>
                ))}
              </Select>
              <Select disabled={loading} size="small" value={statusFilter} onChange={(e) => handleFilterChange(setStatusFilter)(e.target.value)} displayEmpty sx={{ minWidth: 140 }}>
                <MenuItem value="">All Status</MenuItem>
                <MenuItem value="Available">Available</MenuItem>
                <MenuItem value="Partially Allocated">Partially Allocated</MenuItem>
                <MenuItem value="Fully Allocated">Fully Allocated</MenuItem>
                <MenuItem value="Overallocated">Overallocated</MenuItem>
                <MenuItem value="On Leave">On Leave</MenuItem>
                <MenuItem value="Inactive">Inactive</MenuItem>
              </Select>
              <Button
                variant="contained"
                color="primary"
                startIcon={<DownloadIcon />}
                onClick={handleExport}
                disabled={loading}
                size="medium"
                sx={{ whiteSpace: 'nowrap' }}
              >
                Export
              </Button>
              <IconButton disabled={loading} onClick={loadData}><RefreshOutlinedIcon /></IconButton>
            </Stack>
          </Stack>

          <TableContainer>
            <Table size="small" sx={{ '& .MuiTableCell-root': { whiteSpace: 'nowrap', pl: 1.5, pr: '10px' } }}>
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700, width: 48, px: 1 }}>
                    <Checkbox
                      size="small"
                      indeterminate={selectedEmployeeIds.length > 0 && selectedEmployeeIds.length < gridData.filter((r) => r.isActive && r.allocationPercentage < 100).length}
                      checked={gridData.length > 0 && selectedEmployeeIds.length === gridData.filter((r) => r.isActive && r.allocationPercentage < 100).length}
                      onChange={(e) => handleSelectAll(e.target.checked)}
                    />
                  </TableCell>
                  {columns.map((col) => (
                    <TableCell key={(col.field as string)} sx={{ fontWeight: 700 }}>{col.headerName}</TableCell>
                  ))}
                  <TableCell sx={{ fontWeight: 700 }} align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {gridData.map((row) => {
                  const isFullyAllocated = row.allocationPercentage >= 100;
                  const isInactive = !row.isActive;
                  const isDisabled = isFullyAllocated || isInactive;
                  return (
                    <DashboardTableRow
                      key={row.employeeId}
                      row={row}
                      columns={columns}
                      selectedEmployeeIds={selectedEmployeeIds}
                      isDisabled={isDisabled}
                      onSelect={handleSelectOne}
                    />
                  );
                })}
                {gridData.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={columns.length + 2} align="center" sx={{ py: 4 }}>
                      <Typography color="text.secondary">No resources found.</Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
          <TablePagination
            component="div"
            count={totalCount}
            page={page}
            onPageChange={handlePageChange}
            rowsPerPage={rowsPerPage}
            onRowsPerPageChange={handleRowsPerPageChange}
            rowsPerPageOptions={[10, 25, 50, 100]}
            disabled={loading}
          />
        </Paper>

        <Dialog
          open={bulkDialogOpen}
          onClose={() => !bulkSaving && setBulkDialogOpen(false)}
          maxWidth={false}
          fullWidth
          PaperProps={{
            sx: { borderRadius: '14px', maxWidth: 1140, boxShadow: '0 20px 60px rgba(0,0,0,0.12)' },
          }}
        >
          <DialogTitle sx={{ px: 3, py: 2.5, borderBottom: 1, borderColor: 'divider', fontSize: 18, fontWeight: 700, color: 'text.primary' }}>
            Bulk Allocate
          </DialogTitle>
          <DialogContent sx={{ px: 3, py: 2.5, overflowY: 'auto' }}>
            <Typography sx={{ mb: 2, fontSize: 14, color: 'text.secondary' }}>
              Selected Employees: <strong>{selectedEmployeeIds.length}</strong> employee(s)
            </Typography>
            {bulkFormError && <Alert severity="error" sx={{ mb: 2, borderRadius: 2 }}>{bulkFormError}</Alert>}
            <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' }, gap: '16px', pt: 1 }}>
              <Autocomplete
                options={projects}
                getOptionLabel={(option) => option.projectName}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                value={selectedProject}
                onChange={(_, value) => {
                  const matchedClient = value?.clientId ? clients.find((c) => c.id === value.clientId) : null;
                  setBulkFormData((prev) => ({
                    ...prev,
                    projectId: value?.id ?? 0,
                    projectName: value?.projectName ?? '',
                    clientId: matchedClient?.id ?? null,
                    clientName: matchedClient?.name ?? value?.clientName ?? '',
                  }));
                }}
                fullWidth size="small"
                renderInput={(params) => (
                  <TextField {...params} label="Project Name *" placeholder="Select Project Name" />
                )}
              />
              <TextField label="Project Code" fullWidth size="small" value={selectedProject?.projectCode ?? ''} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="Client" fullWidth size="small" value={bulkFormData.clientName || selectedProject?.clientName || ''} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="Project Manager" fullWidth size="small" value={selectedProject?.projectManager ?? ''} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="Delivery Head" fullWidth size="small" value={selectedProject?.deliveryHead ?? ''} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="CSM" fullWidth size="small" value={selectedProject?.csm ?? selectedProject?.csmRevenueTypeName ?? ''} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <Autocomplete
                options={projectStatuses}
                getOptionLabel={(option) => option.name}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                value={projectStatuses.find((s) => s.id === bulkFormData.projectStatusId) ?? null}
                onChange={(_, value) => setBulkFormData((prev) => ({ ...prev, projectStatusId: value?.id ?? null }))}
                fullWidth size="small"
                renderInput={(params) => <TextField {...params} label="Project Status *" placeholder="Select Project Status" slotProps={{ inputLabel: { shrink: true } }} />}
              />
              <TextField label="Start Date *" type="date" fullWidth size="small" value={bulkFormData.startDate}
                onChange={(e) => {
                  const newStart = e.target.value;
                  setBulkFormData((prev) => ({ ...prev, startDate: newStart, endDate: prev.endDate && newStart && prev.endDate < newStart ? '' : prev.endDate }));
                  if (newStart && bulkFormData.endDate && bulkFormData.endDate >= newStart) setBulkDateError('');
                }}
                slotProps={{ inputLabel: { shrink: true } }} />
              <TextField label="End Date *" type="date" fullWidth size="small" value={bulkFormData.endDate}
                onChange={(e) => {
                  const newEnd = e.target.value;
                  setBulkFormData((prev) => ({ ...prev, endDate: newEnd }));
                  if (bulkFormData.startDate && newEnd && newEnd < bulkFormData.startDate) setBulkDateError('End Date cannot be earlier than Start Date.');
                  else setBulkDateError('');
                }}
                error={Boolean(bulkDateError)} helperText={bulkDateError}
                slotProps={{ inputLabel: { shrink: true }, htmlInput: { min: bulkFormData.startDate || undefined } }} />
              <TextField label="Allocation Status" fullWidth size="small" value={computeAllocationStatus(bulkFormData.endDate)} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="Allocation % *" type="number" fullWidth size="small" value={bulkFormData.allocationPercentage}
                onChange={(e) => setBulkFormData((prev) => ({ ...prev, allocationPercentage: e.target.value }))}
                slotProps={{ inputLabel: { shrink: true }, htmlInput: { min: 1, max: 100 } }} />
              <TextField select label="Billable Status *" fullWidth size="small" value={bulkFormData.billableStatus}
                onChange={(e) => setBulkFormData((prev) => ({ ...prev, billableStatus: e.target.value }))}
                slotProps={{ inputLabel: { shrink: true } }}>
                <MenuItem value="" disabled sx={{ color: 'text.disabled' }}>Select Billable Status</MenuItem>
                {BILLABLE_STATUSES.map((s) => (<MenuItem key={s} value={s}>{s}</MenuItem>))}
              </TextField>
              <Autocomplete
                options={statuses} getOptionLabel={(option) => option.name}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                value={statuses.find((s) => s.id === bulkFormData.statusId) ?? null}
                onChange={(_, value) => setBulkFormData((prev) => ({ ...prev, statusId: value?.id ?? null }))}
                fullWidth size="small"
                renderInput={(params) => <TextField {...params} label="Status *" placeholder="Select Status" slotProps={{ inputLabel: { shrink: true } }} />} />
              <Autocomplete
                options={currentBillingStatuses} getOptionLabel={(option) => option.name}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                value={currentBillingStatuses.find((s) => s.id === bulkFormData.currentBillingStatusId) ?? null}
                onChange={(_, value) => setBulkFormData((prev) => ({ ...prev, currentBillingStatusId: value?.id ?? null }))}
                fullWidth size="small"
                renderInput={(params) => <TextField {...params} label="Current Billing Status *" placeholder="Select Current Billing Status" slotProps={{ inputLabel: { shrink: true } }} />} />
              <Autocomplete
                options={billingBuckets} getOptionLabel={(option) => option.name}
                isOptionEqualToValue={(option, value) => option.id === value.id}
                value={billingBuckets.find((s) => s.id === bulkFormData.billingBucketId) ?? null}
                onChange={(_, value) => setBulkFormData((prev) => ({ ...prev, billingBucketId: value?.id ?? null }))}
                fullWidth size="small"
                renderInput={(params) => <TextField {...params} label="Billing Bucket *" placeholder="Select Billing Bucket" slotProps={{ inputLabel: { shrink: true } }} />} />
              <TextField label="Duration" fullWidth size="small" value={bulkFormData.startDate && bulkFormData.endDate ? `${computeDurationDays(bulkFormData.startDate, bulkFormData.endDate)} Days` : '—'} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="Ageing" fullWidth size="small" value={bulkFormData.startDate && bulkFormData.allocationPercentage ? `${computeAgeingDays(bulkFormData.startDate, Number(bulkFormData.allocationPercentage))} Days` : '—'} slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <TextField label="Ageing Bucket" fullWidth size="small"
                value={(() => {
                  if (!bulkFormData.startDate || !bulkFormData.allocationPercentage) return '—';
                  const ageingDays = computeAgeingDays(bulkFormData.startDate, Number(bulkFormData.allocationPercentage));
                  let index;
                  if (ageingDays <= 30) index = 0;
                  else if (ageingDays <= 90) index = 1;
                  else if (ageingDays <= 181) index = 2;
                  else index = 3;
                  return ageingBuckets[index]?.name ?? '—';
                })()}
                slotProps={{ input: { readOnly: true } }} sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
              <FormControl sx={{ justifyContent: 'center' }}>
                <FormLabel id="bulk-engineering-radio-label" sx={{ fontSize: 14, fontWeight: 500, color: 'text.secondary', mb: 0.5, '&.Mui-focused': { color: 'text.secondary' } }}>Engineering *</FormLabel>
                <RadioGroup row aria-labelledby="bulk-engineering-radio-label" value={bulkFormData.engineering === null ? '' : bulkFormData.engineering ? 'Yes' : 'No'}
                  onChange={(e) => setBulkFormData((prev) => ({ ...prev, engineering: e.target.value === 'Yes' }))}>
                  <FormControlLabel value="Yes" control={<Radio size="small" />} label="Yes" />
                  <FormControlLabel value="No" control={<Radio size="small" />} label="No" />
                </RadioGroup>
              </FormControl>
            </Box>
          </DialogContent>
          <DialogActions sx={{ px: 3, py: 2, borderTop: 1, borderColor: 'divider', gap: 1 }}>
            <Button onClick={() => setBulkDialogOpen(false)} disabled={bulkSaving} sx={{ textTransform: 'none', fontWeight: 600, color: 'text.secondary' }}>Cancel</Button>
            <Button variant="contained" onClick={handleBulkSave} disabled={bulkSaving}
              startIcon={bulkSaving ? <CircularProgress size={16} color="inherit" /> : undefined}
              sx={{ textTransform: 'none', fontWeight: 600, borderRadius: '8px', bgcolor: '#2563EB', '&:hover': { bgcolor: '#1D4ED8' } }}>
              {bulkSaving ? 'Saving...' : 'Save'}
            </Button>
          </DialogActions>
        </Dialog>
      </Stack>
    </PageContainer>
  );
}
