import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  IconButton,
  InputAdornment,
  MenuItem,
  Paper,
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
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
import PageLoader from '../../../components/common/PageLoader';
import type { DashboardSummaryDto, DashboardGridDto } from '../types/dashboard';
import { dashboardService } from '../services/dashboardService';
import { columnMappingService } from '../../columnMappings/services/columnMappingService';
import type { ColumnMapping } from '../../columnMappings/types/columnMapping';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Available: 'success',
  'Partially Allocated': 'info',
  'Fully Allocated': 'warning',
  Overallocated: 'error',
  Bench: 'default',
  'On Leave': 'error',
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

function defaultCellRenderer(row: DashboardGridDto, field: keyof DashboardGridDto) {
  const val = row[field];
  return <Typography variant="body2" noWrap>{val === null || val === undefined ? '-' : String(val)}</Typography>;
}

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

export default function ResourceAllocationDashboard() {
  const navigate = useNavigate();
  const [summary, setSummary] = useState<DashboardSummaryDto | null>(null);
  const [gridData, setGridData] = useState<DashboardGridDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [practiceFilter, setPracticeFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);
  const [columns, setColumns] = useState<ColumnDef[]>(defaultColumns);
  const abortRef = useRef<AbortController | null>(null);

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
      const [summaryData, grid] = await Promise.all([
        dashboardService.getSummary(),
        dashboardService.getGridData(),
      ]);
      if (!controller.signal.aborted) {
        setSummary(summaryData);
        setGridData(grid);
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
  }, []);

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

  const filteredData = useMemo(() => {
    let data = gridData;
    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      data = data.filter(
        (r) =>
          r.employeeName.toLowerCase().includes(term) ||
          r.employeeCode.toLowerCase().includes(term)
      );
    }
    if (practiceFilter) {
      data = data.filter((r) => r.practice === practiceFilter);
    }
    if (statusFilter) {
      data = data.filter((r) => r.resourceStatus === statusFilter);
    }
    return data;
  }, [gridData, searchTerm, practiceFilter, statusFilter]);

  const practices = useMemo(() => {
    const set = new Set(gridData.map((r) => r.practice).filter(Boolean));
    return Array.from(set).sort();
  }, [gridData]);

  const paginatedData = filteredData.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage);

  if (loading) {
    return <PageLoader message="Loading Resource Allocation Dashboard..." />;
  }

  if (loadError) {
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
            <Card key={card.label} elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ p: 1.5, '&:last-child': { pb: 1.5 } }}>
                <Stack alignItems="center" spacing={0.5}>
                  <Box
                    alignItems="center"
                    bgcolor={`${card.color}15`}
                    borderRadius={1.5}
                    color={card.color}
                    display="flex"
                    height={36}
                    justifyContent="center"
                    width={36}
                  >
                    <card.icon fontSize="small" />
                  </Box>
                  <Typography color="text.secondary" align="center" noWrap variant="caption">
                    {card.label}
                  </Typography>
                  <Typography fontWeight={700} variant="h5">
                    {card.value}
                  </Typography>
                </Stack>
              </CardContent>
            </Card>
          ))}
        </Box>

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <Stack direction="row" alignItems="center" justifyContent="space-between" flexWrap="wrap" gap={2} sx={{ px: 3, py: 2 }}>
            <Typography fontWeight={800} variant="h5">
              Resources
            </Typography>
            <Stack direction="row" spacing={1.5} alignItems="center" flexWrap="wrap">
              <TextField
                disabled={loading}
                size="small"
                placeholder="Search name or code..."
                value={searchTerm}
                onChange={(e) => { setSearchTerm(e.target.value); setPage(0); }}
                slotProps={{
                  input: {
                    startAdornment: (
                      <InputAdornment position="start">
                        <SearchOutlinedIcon fontSize="small" />
                      </InputAdornment>
                    ),
                  },
                }}
                sx={{ minWidth: 250 }}
              />
              <Select disabled={loading} size="small" value={practiceFilter} onChange={(e) => { setPracticeFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
                <MenuItem value="">All Practices</MenuItem>
                {practices.map((p) => (
                  <MenuItem key={p} value={p}>{p}</MenuItem>
                ))}
              </Select>
              <Select disabled={loading} size="small" value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
                <MenuItem value="">All Status</MenuItem>
                <MenuItem value="Available">Available</MenuItem>
                <MenuItem value="Partially Allocated">Partially Allocated</MenuItem>
                <MenuItem value="Fully Allocated">Fully Allocated</MenuItem>
                <MenuItem value="Overallocated">Overallocated</MenuItem>
                <MenuItem value="On Leave">On Leave</MenuItem>
              </Select>
              <IconButton disabled={loading} onClick={loadData}><RefreshOutlinedIcon /></IconButton>
            </Stack>
          </Stack>

          <TableContainer>
            <Table size="small" sx={{ '& .MuiTableCell-root': { whiteSpace: 'nowrap', pl: 1.5, pr: '10px' } }}>
              <TableHead>
                <TableRow>
                  {columns.map((col) => (
                    <TableCell key={col.field} sx={{ fontWeight: 700 }}>{col.headerName}</TableCell>
                  ))}
                  <TableCell sx={{ fontWeight: 700 }} align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {paginatedData.map((row) => (
                  <TableRow
                    key={row.employeeId}
                    hover
                    sx={{ cursor: 'pointer' }}
                    onClick={() => navigate(`/rmg/view/${row.employeeId}`)}
                  >
                    {columns.map((col) => (
                      <TableCell key={col.field}>
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
                ))}
                {paginatedData.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={columns.length + 1} align="center" sx={{ py: 4 }}>
                      <Typography color="text.secondary">No resources found.</Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
          <TablePagination
            component="div"
            count={filteredData.length}
            page={page}
            onPageChange={(_, p) => setPage(p)}
            rowsPerPage={rowsPerPage}
            onRowsPerPageChange={(e) => { setRowsPerPage(Number(e.target.value)); setPage(0); }}
            rowsPerPageOptions={[10, 25, 50, 100]}
            disabled={loading}
          />
        </Paper>
      </Stack>
    </PageContainer>
  );
}
