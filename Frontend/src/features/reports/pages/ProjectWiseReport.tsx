import BusinessOutlinedIcon from '@mui/icons-material/BusinessOutlined';
import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined';
import GroupOutlinedIcon from '@mui/icons-material/GroupOutlined';
import PeopleOutlinedIcon from '@mui/icons-material/PeopleOutlined';
import RefreshOutlinedIcon from '@mui/icons-material/RefreshOutlined';
import SearchIcon from '@mui/icons-material/Search';
import ShowChartOutlinedIcon from '@mui/icons-material/ShowChartOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import {
  Box,
  Button,
  Collapse,
  FormControl,
  IconButton,
  InputAdornment,
  InputLabel,
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
  Typography,
} from '@mui/material';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import api from '../../../services/api';
import KpiCard from '../components/KpiCard';
import { reportService } from '../services/reportService';
import type { ProjectWiseReportDto, ReportChartDataDto } from '../types/report';
import ReportCharts from '../components/ReportCharts';

interface MasterItem {
  id: string;
  name: string;
}

export default function ProjectWiseReport() {
  const [data, setData] = useState<ProjectWiseReportDto[]>([]);
  const [chartData, setChartData] = useState<ReportChartDataDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [clients, setClients] = useState<MasterItem[]>([]);
  const [practices, setPractices] = useState<MasterItem[]>([]);
  const [expandedId, setExpandedId] = useState<number | null>(null);

  const [projectSearch, setProjectSearch] = useState('');
  const [clientFilter, setClientFilter] = useState('');
  const [practiceFilter, setPracticeFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);

  const debounceRef = useRef<ReturnType<typeof setTimeout>>();

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const params: Record<string, string> = {};
      if (projectSearch) params.search = projectSearch;
      if (clientFilter) params.client = clientFilter;
      if (practiceFilter) params.practice = practiceFilter;
      if (statusFilter) params.status = statusFilter;
      const result = await reportService.getProjectWiseReport(
        Object.keys(params).length > 0 ? params : undefined,
      );
      setData(result);
    } catch {
      console.error('Failed to load project wise report');
    } finally {
      setLoading(false);
    }
  }, [projectSearch, clientFilter, practiceFilter, statusFilter]);

  const loadChartData = useCallback(async () => {
    try {
      const result = await reportService.getProjectWiseChartData();
      setChartData(result);
    } catch {
      console.error('Failed to load chart data');
    }
  }, []);

  const paginatedData = useMemo(
    () => data.slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage),
    [data, page, rowsPerPage],
  );

  const handleChangePage = (_: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  useEffect(() => {
    api.get('/master/practices').then((r) => setPractices(r.data as MasterItem[])).catch(() => {});
    api.get('/clients').then((r) => {
      const items: MasterItem[] = (r.data?.data ?? r.data ?? []).map((c: any) => ({ id: String(c.id), name: c.name }));
      setClients(items);
    }).catch(() => {});
    loadChartData();
  }, [loadChartData]);

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => {
      loadData();
      loadChartData();
    }, 300);
    return () => { if (debounceRef.current) clearTimeout(debounceRef.current); };
  }, [loadData, loadChartData]);

  const totals = {
    projects: data.length,
    employees: data.reduce((s, p) => s + p.totalEmployees, 0),
    billable: data.reduce((s, p) => s + p.billableCount, 0),
  };
  const avgAlloc =
    data.length > 0
      ? Math.round(data.reduce((s, p) => s + p.avgAllocation, 0) / data.length)
      : 0;
  const totalAvgAlloc =
    data.reduce((s, p) => s + p.avgAllocation * p.totalEmployees, 0) /
    (totals.employees || 1);

  const handleReset = () => {
    setProjectSearch('');
    setClientFilter('');
    setPracticeFilter('');
    setStatusFilter('');
    setPage(0);
  };

  return (
    <PageContainer title="Project Wise Report">
      <Stack spacing={3}>
        <Stack direction="row" spacing={2} flexWrap="wrap" useFlexGap>
          <TextField
            size="small"
            placeholder="Search project..."
            value={projectSearch}
            onChange={(e) => setProjectSearch(e.target.value)}
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
            sx={{ minWidth: 200 }}
          />
          <FormControl size="small" sx={{ minWidth: 160 }}>
            <InputLabel id="client-label">Client</InputLabel>
            <Select
              labelId="client-label"
              value={clientFilter}
              label="Client"
              onChange={(e) => setClientFilter(e.target.value)}
            >
              <MenuItem value="">All Clients</MenuItem>
              {clients.map((c) => (
                <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl size="small" sx={{ minWidth: 160 }}>
            <InputLabel id="practice-label">Practice</InputLabel>
            <Select
              labelId="practice-label"
              value={practiceFilter}
              label="Practice"
              onChange={(e) => setPracticeFilter(e.target.value)}
            >
              <MenuItem value="">All Practices</MenuItem>
              {practices.map((p) => (
                <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl size="small" sx={{ minWidth: 140 }}>
            <InputLabel id="status-label">Status</InputLabel>
            <Select
              labelId="status-label"
              value={statusFilter}
              label="Status"
              onChange={(e) => setStatusFilter(e.target.value)}
            >
              <MenuItem value="">All</MenuItem>
              <MenuItem value="Active">Active</MenuItem>
              <MenuItem value="Completed">Completed</MenuItem>
            </Select>
          </FormControl>
          <Button variant="outlined" onClick={handleReset} size="small">
            Reset
          </Button>
          <Button variant="outlined" startIcon={<FileDownloadOutlinedIcon />} size="small">
            Export
          </Button>
          <IconButton onClick={loadData} size="small">
            <RefreshOutlinedIcon />
          </IconButton>
        </Stack>

        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
            gap: 3,
          }}
        >
          <KpiCard icon={WorkOutlineOutlinedIcon} label="Projects" value={totals.projects} color="primary.main" />
          <KpiCard icon={GroupOutlinedIcon} label="Associates" value={totals.employees} color="success.main" />
          <KpiCard icon={PeopleOutlinedIcon} label="Billable" value={totals.billable} color="info.main" />
          <KpiCard
            icon={ShowChartOutlinedIcon}
            label="Avg Alloc %"
            value={`${avgAlloc}%`}
            color={avgAlloc >= 70 ? 'success.main' : avgAlloc >= 40 ? 'warning.main' : 'error.main'}
          />
          <KpiCard icon={BusinessOutlinedIcon} label="Weighted Alloc %" value={`${Math.round(totalAvgAlloc)}%`} color="warning.main" />
        </Box>

        {chartData && <ReportCharts data={chartData} chartType="project" />}

        <TableContainer
          component={Paper}
          elevation={0}
          sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflowX: 'auto' }}
        >
          <Table sx={{ minWidth: 1200 }}>
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 700 }}>Project</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Client</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Manager</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Start</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">End</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Employees</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Billable</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Avg Alloc %</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Status</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center" />
              </TableRow>
            </TableHead>
            <TableBody>
              {paginatedData.map((project) => (
                <>
                  <TableRow key={project.projectId} hover>
                    <TableCell>
                      <Typography fontWeight={600}>{project.projectName}</Typography>
                    </TableCell>
                    <TableCell>{project.clientName}</TableCell>
                    <TableCell>{project.projectManager ?? '—'}</TableCell>
                    <TableCell align="center">{new Date(project.projectStartDate).toLocaleDateString()}</TableCell>
                    <TableCell align="center">{project.projectEndDate ? new Date(project.projectEndDate).toLocaleDateString() : '—'}</TableCell>
                    <TableCell align="center">{project.totalEmployees}</TableCell>
                    <TableCell align="center">{project.billableCount}</TableCell>
                    <TableCell align="center">
                      <Typography fontWeight={700} color={project.avgAllocation >= 70 ? 'success.main' : project.avgAllocation >= 40 ? 'warning.main' : 'error.main'}>
                        {project.avgAllocation.toFixed(1)}%
                      </Typography>
                    </TableCell>
                    <TableCell align="center">
                      <Typography variant="body2" fontWeight={600} color={project.status === 'Active' ? 'success.main' : 'text.secondary'}>
                        {project.status}
                      </Typography>
                    </TableCell>
                    <TableCell align="center">
                      <IconButton size="small" onClick={() => setExpandedId(expandedId === project.projectId ? null : project.projectId)}>
                        {expandedId === project.projectId ? <ExpandLess /> : <ExpandMore />}
                      </IconButton>
                    </TableCell>
                  </TableRow>
                  <TableRow key={`${project.projectId}-expanded`}>
                    <TableCell colSpan={10} sx={{ py: 0, border: 'none' }}>
                      <Collapse in={expandedId === project.projectId} timeout="auto" unmountOnExit>
                        <Box sx={{ py: 2, px: 4 }}>
                          <Typography variant="subtitle2" fontWeight={700} mb={1}>
                            Employees on {project.projectName}
                          </Typography>
                          {project.employees.length > 0 ? (
                            <Table size="small">
                              <TableHead>
                                <TableRow>
                                  <TableCell sx={{ fontWeight: 600 }}>Employee Code</TableCell>
                                  <TableCell sx={{ fontWeight: 600 }}>Name</TableCell>
                                  <TableCell sx={{ fontWeight: 600 }} align="center">Allocation %</TableCell>
                                  <TableCell sx={{ fontWeight: 600 }} align="center">Billable Status</TableCell>
                                  <TableCell sx={{ fontWeight: 600 }} align="center">Utilization</TableCell>
                                </TableRow>
                              </TableHead>
                              <TableBody>
                                {project.employees.map((emp) => (
                                  <TableRow key={emp.employeeCode}>
                                    <TableCell>{emp.employeeCode}</TableCell>
                                    <TableCell>{emp.fullName}</TableCell>
                                    <TableCell align="center">{emp.allocationPercentage}%</TableCell>
                                    <TableCell align="center">
                                      <Typography variant="body2" fontWeight={600} color={emp.billableStatus === 'Billable' ? 'success.main' : 'error.main'}>
                                        {emp.billableStatus ?? '—'}
                                      </Typography>
                                    </TableCell>
                                    <TableCell align="center">{emp.allocationPercentage}%</TableCell>
                                  </TableRow>
                                ))}
                              </TableBody>
                            </Table>
                          ) : (
                            <Typography variant="body2" color="text.secondary">No employees allocated</Typography>
                          )}
                        </Box>
                      </Collapse>
                    </TableCell>
                  </TableRow>
                </>
              ))}
              {!loading && data.length === 0 && (
                <TableRow>
                  <TableCell colSpan={10} align="center" sx={{ py: 4 }}>
                    No project data available
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
          <TablePagination
            component="div"
            count={data.length}
            onPageChange={handleChangePage}
            onRowsPerPageChange={handleChangeRowsPerPage}
            page={page}
            rowsPerPage={rowsPerPage}
            rowsPerPageOptions={[5, 10, 25, 50]}
          />
      </Stack>
    </PageContainer>
  );
}
