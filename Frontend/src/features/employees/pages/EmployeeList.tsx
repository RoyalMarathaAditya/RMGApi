import SearchIcon from '@mui/icons-material/Search';
import ClearIcon from '@mui/icons-material/Clear';
import DownloadIcon from '@mui/icons-material/Download';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import { Chip, Autocomplete, FormControl, InputLabel, Select, MenuItem, Tooltip } from '@mui/material';
import { Alert, Box, Button, InputAdornment, Stack, TextField, Typography } from '@mui/material';
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import api from '../../../services/api';
import BulkUploadSection from '../components/BulkUploadSection';
import EmployeeGrid from '../components/EmployeeGrid';
import EmployeeDialog from '../components/EmployeeDialog';
import DeleteEmployeeDialog from '../components/DeleteEmployeeDialog';
import { employeeService } from '../services/employeeService';
import type { UploadColumnInfo } from '../services/employeeService';
import { getEmployeeColumns } from '../config/employeeColumns';
import {
  createEmployee,
  deleteEmployee,
  fetchEmployees,
  setEmployeeFilter,
  clearEmployeeFilters,
  updateEmployee,
} from '../../../redux/slices/employeeSlice';
import type { Employee, EmployeeFormValues } from '../types/employee';

interface MasterItem {
  id: string;
  name: string;
}

function buildColumns(columnInfo: UploadColumnInfo[]): GridColDef<Employee>[] {
  return columnInfo.map((col) => {
    const base: GridColDef<Employee> = {
      field: col.field,
      headerName: col.header,
      width: col.field === 'email' ? 220 : col.field === 'fullName' ? 180 : col.field === 'employeeCode' ? 100 : 150,
    };
    if (col.field === 'employeeStatus') {
      base.renderCell = ({ row }) => (
        <Chip
          color={row.employeeStatus === 'Active' ? 'success' : 'default'}
          label={row.employeeStatus}
          size="small"
          variant="outlined"
        />
      );
    }
    if (col.field === 'doj' || col.field === 'lwd') {
      base.valueFormatter = (value: unknown) =>
        value ? new Date(value as string).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' }) : '';
    }
    return base;
  });
}

export default function EmployeeList() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const { employees, error, filters, loading } = useAppSelector((state) => state.employees);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState<Employee | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Employee | null>(null);
  const [paginationModel, setPaginationModel] = useState<GridPaginationModel>({
    page: 0,
    pageSize: 10,
  });
  const [uploadColumns, setUploadColumns] = useState<UploadColumnInfo[]>([]);
  const [practices, setPractices] = useState<MasterItem[]>([]);
  const [statuses, setStatuses] = useState<MasterItem[]>([]);
  const [fullNameInput, setFullNameInput] = useState(filters.search);

  const debounceRef = useRef<ReturnType<typeof setTimeout>>();

  useEffect(() => {
    api.get('/master/practices').then((r) => setPractices(r.data as MasterItem[])).catch(() => {});
    api.get('/master/statuses').then((r) => {
      const all = r.data as MasterItem[];
      setStatuses(all.filter((s) => /^active$/i.test(s.name) || /^inactive$/i.test(s.name)));
    }).catch(() => {});
    employeeService.getLastUploadColumns().then(setUploadColumns).catch(() => {});
  }, []);

  useEffect(() => {
    const params: { fullName?: string; practiceId?: string; doj?: string; statusId?: string } = {};
    if (filters.search) params.fullName = filters.search;
    if (filters.practiceId) params.practiceId = filters.practiceId;
    if (filters.doj) params.doj = `${filters.doj}-01`;
    if (filters.statusId) params.statusId = filters.statusId;
    void dispatch(fetchEmployees(Object.keys(params).length > 0 ? params : undefined));
  }, [dispatch, filters.search, filters.practiceId, filters.doj, filters.statusId]);

  const handleFullNameChange = (value: string) => {
    setFullNameInput(value);
    if (debounceRef.current) clearTimeout(debounceRef.current);
    debounceRef.current = setTimeout(() => {
      dispatch(setEmployeeFilter({ search: value }));
    }, 300);
  };

  const handlePracticeChange = (_: React.SyntheticEvent, value: MasterItem | null) => {
    dispatch(setEmployeeFilter({ practiceId: value?.id ?? '' }));
  };

  const handleStatusChange = (value: string) => {
    dispatch(setEmployeeFilter({ statusId: value }));
  };

  const handleDojChange = (value: string) => {
    dispatch(setEmployeeFilter({ doj: value }));
  };

  const handleExport = () => {
    const params: { fullName?: string; practiceId?: string; doj?: string; statusId?: string } = {};
    if (filters.search) params.fullName = filters.search;
    if (filters.practiceId) params.practiceId = filters.practiceId;
    if (filters.doj) params.doj = `${filters.doj}-01`;
    if (filters.statusId) params.statusId = filters.statusId;
    employeeService.exportEmployees(Object.keys(params).length > 0 ? params : undefined);
  };

  const handleClearFilters = () => {
    setFullNameInput('');
    dispatch(clearEmployeeFilters());
  };

  const hasActiveFilters = filters.search || filters.practiceId || filters.doj || filters.statusId;

  const columns = useMemo(() => uploadColumns.length > 0 ? buildColumns(uploadColumns) : getEmployeeColumns(), [uploadColumns]);

  const handleAdd = () => {
    setSelectedEmployee(null);
    setDialogOpen(true);
  };

  const handleSubmit = async (values: EmployeeFormValues) => {
    if (selectedEmployee) {
      await dispatch(updateEmployee({ id: selectedEmployee.id, values })).unwrap();
    } else {
      await dispatch(createEmployee(values)).unwrap();
    }
    setDialogOpen(false);
    setSelectedEmployee(null);
  };

  const handleDelete = async () => {
    if (!deleteTarget) return;
    await dispatch(deleteEmployee(deleteTarget.id)).unwrap();
    setDeleteTarget(null);
  };

  const filtersRef = useRef(filters);
  filtersRef.current = filters;

  const handleImportComplete = useCallback(
    (result?: { columns?: UploadColumnInfo[] | null }) => {
      const current = filtersRef.current;
      const params: { fullName?: string; practiceId?: string; doj?: string; statusId?: string } = {};
      if (current.search) params.fullName = current.search;
      if (current.practiceId) params.practiceId = current.practiceId;
      if (current.doj) params.doj = `${current.doj}-01`;
      if (current.statusId) params.statusId = current.statusId;
      void dispatch(fetchEmployees(Object.keys(params).length > 0 ? params : undefined));
      if (result?.columns) {
        setUploadColumns(result.columns);
      }
    },
    [dispatch],
  );

  const handleRowClick = useCallback(
    (params: GridRowParams<Employee>) => {
      navigate(`/employees/${params.row.id}`);
    },
    [navigate],
  );

  return (
    <PageContainer title="Employee Management">
      <Stack spacing={2.5}>
        <BulkUploadSection onImportComplete={handleImportComplete} />
        <Stack
          alignItems={{ xs: 'stretch', md: 'center' }}
          direction={{ xs: 'column', md: 'row' }}
          justifyContent="space-between"
          spacing={2}
        >
          <Box>
            <Typography color="text.primary" fontWeight={700} variant="h6">
              Employees
            </Typography>
            <Typography variant="body2">
              {employees.length} employee records
              {hasActiveFilters && ` (filtered)`}
              {uploadColumns.length > 0 && ` | ${uploadColumns.length} columns`}
            </Typography>
          </Box>
          <Button
            variant="contained"
            color="primary"
            startIcon={<DownloadIcon />}
            onClick={handleExport}
            size="medium"
            sx={{ whiteSpace: 'nowrap' }}
          >
            Export
          </Button>
        </Stack>

        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2} alignItems={{ md: 'flex-end' }}>
          <TextField
            size="small"
            placeholder="Search by Full Name"
            value={fullNameInput}
            onChange={(e) => handleFullNameChange(e.target.value)}
            sx={{ minWidth: 220 }}
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
          />
          <Autocomplete
            size="small"
            options={practices}
            getOptionLabel={(option) => option.name}
            value={practices.find((p) => p.id === filters.practiceId) ?? null}
            onChange={handlePracticeChange}
            renderInput={(params) => <TextField {...params} label="Practice" placeholder="Search Practice" />}
            sx={{ minWidth: 240 }}
            ListboxProps={{ style: { maxHeight: 200 } }}
            isOptionEqualToValue={(option, value) => option.id === value.id}
          />
          <FormControl size="small" sx={{ minWidth: 150 }}>
            <InputLabel id="emp-status-label">Emp Status</InputLabel>
            <Select
              labelId="emp-status-label"
              value={filters.statusId}
              label="Emp Status"
              onChange={(e) => handleStatusChange(e.target.value)}
            >
              <MenuItem value="">All Statuses</MenuItem>
              {statuses.map((s) => (
                <MenuItem key={s.id} value={s.id}>{s.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <TextField
            size="small"
            label="DOJ (Month)"
            type="month"
            value={filters.doj}
            onChange={(e) => handleDojChange(e.target.value)}
            slotProps={{
              inputLabel: { shrink: true },
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <CalendarTodayIcon fontSize="small" />
                  </InputAdornment>
                ),
              },
            }}
            sx={{ minWidth: 200 }}
          />
          <Tooltip title="Clear Filters">
            <span>
              <Button
                variant="outlined"
                color="secondary"
                startIcon={<ClearIcon />}
                onClick={handleClearFilters}
                disabled={!hasActiveFilters}
                size="medium"
              >
                Clear Filters
              </Button>
            </span>
          </Tooltip>
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}

        {!loading && employees.length === 0 && hasActiveFilters ? (
          <Alert severity="info" icon={<SearchIcon />}>
            No employees found. Try changing or clearing the filters.
          </Alert>
        ) : (
          <EmployeeGrid
            columns={columns}
            loading={loading}
            onPaginationModelChange={setPaginationModel}
            onRowClick={handleRowClick}
            paginationModel={paginationModel}
            rows={employees}
          />
        )}
      </Stack>

      <EmployeeDialog
        employee={selectedEmployee}
        onClose={() => {
          setDialogOpen(false);
          setSelectedEmployee(null);
        }}
        onSubmit={handleSubmit}
        open={dialogOpen}
      />
      <DeleteEmployeeDialog
        employee={deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        open={Boolean(deleteTarget)}
      />
    </PageContainer>
  );
}
