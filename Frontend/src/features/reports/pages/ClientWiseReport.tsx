import Autocomplete from '@mui/material/Autocomplete';
import BusinessOutlinedIcon from '@mui/icons-material/BusinessOutlined';
import Chip from '@mui/material/Chip';
import FileDownloadOutlinedIcon from '@mui/icons-material/FileDownloadOutlined';
import GroupOutlinedIcon from '@mui/icons-material/GroupOutlined';
import PeopleOutlinedIcon from '@mui/icons-material/PeopleOutlined';
import RefreshOutlinedIcon from '@mui/icons-material/RefreshOutlined';
import ShowChartOutlinedIcon from '@mui/icons-material/ShowChartOutlined';
import TextField from '@mui/material/TextField';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import {
  Box,
  Button,
  FormControl,
  IconButton,
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
  TableRow,
  Typography,
} from '@mui/material';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import api from '../../../services/api';
import KpiCard from '../components/KpiCard';
import { reportService } from '../services/reportService';
import type { ClientWiseReportDto, ReportChartDataDto } from '../types/report';
import ReportCharts from '../components/ReportCharts';

interface MasterItem {
  id: string;
  name: string;
}

export default function ClientWiseReport() {
  const [data, setData] = useState<ClientWiseReportDto[]>([]);
  const [chartData, setChartData] = useState<ReportChartDataDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [clients, setClients] = useState<MasterItem[]>([]);

  const [selectedClients, setSelectedClients] = useState<MasterItem[]>([]);
  const [statusFilter, setStatusFilter] = useState('');

  const debounceRef = useRef<ReturnType<typeof setTimeout>>();

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const params: Record<string, string> = {};
      if (selectedClients.length > 0) params.clientIds = selectedClients.map((c) => c.id).join(',');
      if (statusFilter) params.status = statusFilter;
      const result = await reportService.getClientWiseReport(
        Object.keys(params).length > 0 ? params : undefined,
      );
      setData(result);
    } catch {
      console.error('Failed to load client wise report');
    } finally {
      setLoading(false);
    }
  }, [selectedClients, statusFilter]);

  const loadChartData = useCallback(async () => {
    try {
      const result = await reportService.getClientWiseChartData();
      setChartData(result);
    } catch {
      console.error('Failed to load chart data');
    }
  }, []);

  useEffect(() => {
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

  const flatRows = useMemo(() => {
    return data.flatMap((c) =>
      c.projects.map((p) => ({
        clientName: c.clientName,
        projectName: p.projectName,
        status: p.status,
        employeeCount: p.employeeCount,
        avgAllocation: p.avgAllocation,
      })),
    );
  }, [data]);

  const totals = {
    clients: data.length,
    projects: data.reduce((s, c) => s + c.totalProjects, 0),
    billable: data.reduce((s, c) => s + c.billableCount, 0),
    employees: data.reduce((s, c) => s + c.totalEmployees, 0),
  };
  const avgAlloc =
    data.length > 0
      ? Math.round(data.reduce((s, c) => s + c.avgAllocation, 0) / data.length)
      : 0;

  const handleReset = () => {
    setSelectedClients([]);
    setStatusFilter('');
  };

  return (
    <PageContainer title="Client Wise Report">
      <Stack spacing={3}>
        <Stack direction="row" spacing={2} flexWrap="wrap" useFlexGap alignItems="center">
          <Autocomplete
            multiple
            size="small"
            options={clients}
            value={selectedClients}
            onChange={(_, value) => setSelectedClients(value)}
            getOptionLabel={(option) => option.name}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            renderInput={(params) => (
              <TextField
                {...params}
                placeholder="Select clients..."
                sx={{ minWidth: 260 }}
              />
            )}
            renderTags={(value, getTagProps) =>
              value.map((option, index) => (
                <Chip key={option.id} label={option.name} size="small" {...getTagProps({ index })} />
              ))
            }
            sx={{ minWidth: 260 }}
          />
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
          <KpiCard icon={BusinessOutlinedIcon} label="Clients" value={totals.clients} color="primary.main" />
          <KpiCard icon={WorkOutlineOutlinedIcon} label="Projects" value={totals.projects} color="success.main" />
          <KpiCard icon={PeopleOutlinedIcon} label="Billable" value={totals.billable} color="info.main" />
          <KpiCard
            icon={ShowChartOutlinedIcon}
            label="Avg Alloc %"
            value={`${avgAlloc}%`}
            color={avgAlloc >= 70 ? 'success.main' : avgAlloc >= 40 ? 'warning.main' : 'error.main'}
          />
          <KpiCard icon={GroupOutlinedIcon} label="Employees" value={totals.employees} color="primary.main" />
        </Box>

        {chartData && <ReportCharts data={chartData} chartType="client" />}

        <TableContainer
          component={Paper}
          elevation={0}
          sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflowX: 'auto' }}
        >
          <Table sx={{ minWidth: 700 }}>
            <TableHead>
              <TableRow>
                <TableCell sx={{ fontWeight: 700 }}>Client</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Project</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Status</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Employees</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Avg Alloc %</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {flatRows.map((row, idx) => (
                <TableRow key={idx} hover>
                  <TableCell>
                    <Typography fontWeight={600}>{row.clientName}</Typography>
                  </TableCell>
                  <TableCell>{row.projectName}</TableCell>
                  <TableCell align="center">
                    <Typography variant="body2" fontWeight={600} color={row.status === 'Active' ? 'success.main' : 'text.secondary'}>
                      {row.status}
                    </Typography>
                  </TableCell>
                  <TableCell align="center">{row.employeeCount}</TableCell>
                  <TableCell align="center">
                    <Typography fontWeight={700} color={row.avgAllocation >= 70 ? 'success.main' : row.avgAllocation >= 40 ? 'warning.main' : 'error.main'}>
                      {row.avgAllocation.toFixed(1)}%
                    </Typography>
                  </TableCell>
                </TableRow>
              ))}
              {!loading && flatRows.length === 0 && (
                <TableRow>
                  <TableCell colSpan={5} align="center" sx={{ py: 4 }}>
                    No client data available
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Stack>
    </PageContainer>
  );
}


