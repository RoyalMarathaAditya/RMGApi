import { useEffect, useMemo, useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  Grid,
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
import AddOutlinedIcon from '@mui/icons-material/AddOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import DeleteOutlinedIcon from '@mui/icons-material/DeleteOutlined';
import HistoryOutlinedIcon from '@mui/icons-material/HistoryOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import PersonOffOutlinedIcon from '@mui/icons-material/PersonOffOutlined';
import AssignmentTurnedInOutlinedIcon from '@mui/icons-material/AssignmentTurnedInOutlined';
import HourglassEmptyOutlinedIcon from '@mui/icons-material/HourglassEmptyOutlined';
import PeopleAltOutlinedIcon from '@mui/icons-material/PeopleAltOutlined';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
import type { DashboardSummaryDto, DashboardGridDto, DashboardFilterDto } from '../types/dashboard';
import { dashboardService } from '../services/dashboardService';

const statusColors: Record<string, 'success' | 'info' | 'warning' | 'error' | 'default'> = {
  Available: 'success',
  'Partially Allocated': 'info',
  'Fully Allocated': 'warning',
  Overallocated: 'error',
  Bench: 'default',
  'On Leave': 'error',
};

export default function ResourceAllocationDashboard() {
  const navigate = useNavigate();
  const [summary, setSummary] = useState<DashboardSummaryDto | null>(null);
  const [gridData, setGridData] = useState<DashboardGridDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [practiceFilter, setPracticeFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(25);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    try {
      const [summaryData, grid] = await Promise.all([
        dashboardService.getSummary(),
        dashboardService.getGridData(),
      ]);
      setSummary(summaryData);
      setGridData(grid);
    } catch {
      console.error('Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  const summaryCards = useMemo(() => {
    if (!summary) return [];
    return [
      { icon: PeopleAltOutlinedIcon, label: 'Total Employees', value: summary.totalEmployees, color: 'primary.main' },
      { icon: GroupsOutlinedIcon, label: 'Total Practices', value: summary.totalPractices, color: 'secondary.main' },
      { icon: HourglassEmptyOutlinedIcon, label: 'Available', value: summary.availableResources, color: 'success.main' },
      { icon: AssignmentTurnedInOutlinedIcon, label: 'Allocated', value: summary.allocatedResources, color: 'info.main' },
      { icon: WorkOutlineOutlinedIcon, label: 'Fully Allocated', value: summary.fullyAllocatedResources, color: 'warning.main' },
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
          r.employeeCode.toLowerCase().includes(term) ||
          r.skills?.toLowerCase().includes(term)
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

  return (
    <PageContainer title="Resource Allocation Dashboard">
      <Stack spacing={3}>
        <Box
          sx={{
            display: 'grid',
            gap: 2,
            gridTemplateColumns: { xs: 'repeat(2, minmax(0, 1fr))', sm: 'repeat(3, minmax(0, 1fr))', lg: 'repeat(6, minmax(0, 1fr))' },
          }}
        >
          {summaryCards.map((card) => (
            <Card key={card.label} elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
              <CardContent sx={{ p: 2, '&:last-child': { pb: 2 } }}>
                <Stack alignItems="center" direction="row" spacing={1.5}>
                  <Box
                    alignItems="center"
                    bgcolor={`${card.color}15`}
                    borderRadius={1.5}
                    color={card.color}
                    display="flex"
                    height={40}
                    justifyContent="center"
                    width={40}
                  >
                    <card.icon fontSize="small" />
                  </Box>
                  <Box minWidth={0}>
                    <Typography color="text.secondary" noWrap variant="caption">
                      {card.label}
                    </Typography>
                    <Typography fontWeight={700} variant="h5">
                      {card.value}
                    </Typography>
                  </Box>
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
                size="small"
                placeholder="Search name, code, skill..."
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
              <Select size="small" value={practiceFilter} onChange={(e) => { setPracticeFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
                <MenuItem value="">All Practices</MenuItem>
                {practices.map((p) => (
                  <MenuItem key={p} value={p}>{p}</MenuItem>
                ))}
              </Select>
              <Select size="small" value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }} displayEmpty sx={{ minWidth: 140 }}>
                <MenuItem value="">All Status</MenuItem>
                <MenuItem value="Available">Available</MenuItem>
                <MenuItem value="Partially Allocated">Partially Allocated</MenuItem>
                <MenuItem value="Fully Allocated">Fully Allocated</MenuItem>
                <MenuItem value="Overallocated">Overallocated</MenuItem>
                <MenuItem value="On Leave">On Leave</MenuItem>
              </Select>
              <Button variant="contained" startIcon={<AddOutlinedIcon />} onClick={() => navigate('/rmg/create')}>
                Allocate
              </Button>
              <IconButton onClick={loadData}><RefreshOutlinedIcon /></IconButton>
            </Stack>
          </Stack>

          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell sx={{ fontWeight: 700 }}>Employee</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Code</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Designation</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Practice</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Skills</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Current Project</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Allocation %</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Availability %</TableCell>
                  <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                  <TableCell sx={{ fontWeight: 700 }} align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {paginatedData.map((row) => (
                  <TableRow
                    key={row.employeeId}
                    hover
                    sx={{ cursor: 'pointer' }}
                    onClick={() => navigate(`/rmg/allocations/${row.employeeId}`)}
                  >
                    <TableCell>
                      <Typography fontWeight={600} variant="body2">{row.employeeName}</Typography>
                    </TableCell>
                    <TableCell>{row.employeeCode}</TableCell>
                    <TableCell>{row.designation ?? '-'}</TableCell>
                    <TableCell>{row.practice}</TableCell>
                    <TableCell>
                      <Typography variant="caption" color="text.secondary" sx={{ maxWidth: 150, display: 'block', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                        {row.skills ?? '-'}
                      </Typography>
                    </TableCell>
                    <TableCell>{row.currentProject ?? '-'}</TableCell>
                    <TableCell>
                      <Typography fontWeight={600} color={row.allocationPercentage > 100 ? 'error.main' : row.allocationPercentage >= 100 ? 'warning.main' : 'text.primary'}>
                        {row.allocationPercentage}%
                      </Typography>
                    </TableCell>
                    <TableCell>{row.availableCapacity}%</TableCell>
                    <TableCell>
                      <Chip label={row.resourceStatus} size="small" color={statusColors[row.resourceStatus] ?? 'default'} variant="outlined" />
                    </TableCell>
                    <TableCell align="right">
                      <Stack direction="row" spacing={0.5} justifyContent="flex-end" onClick={(e) => e.stopPropagation()}>
                        <Tooltip title="Edit Allocation">
                          <IconButton size="small" onClick={() => navigate(`/rmg/edit/${row.employeeId}`)}>
                            <EditOutlinedIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="View History">
                          <IconButton size="small" onClick={() => navigate(`/rmg/history/${row.employeeId}`)}>
                            <HistoryOutlinedIcon fontSize="small" />
                          </IconButton>
                        </Tooltip>
                      </Stack>
                    </TableCell>
                  </TableRow>
                ))}
                {paginatedData.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={10} align="center" sx={{ py: 4 }}>
                      <Typography color="text.secondary">No resources found</Typography>
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
          />
        </Paper>
      </Stack>
    </PageContainer>
  );
}
