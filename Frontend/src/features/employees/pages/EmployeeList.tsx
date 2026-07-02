import { Chip } from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';
import { Alert, Box, Button, InputAdornment, Stack, TextField, Typography } from '@mui/material';
import type { GridColDef, GridPaginationModel, GridRowParams } from '@mui/x-data-grid';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
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
  setEmployeeSearch,
  updateEmployee,
} from '../../../redux/slices/employeeSlice';
import type { Employee, EmployeeFormValues } from '../types/employee';

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

  useEffect(() => {
    void dispatch(fetchEmployees());
    employeeService.getLastUploadColumns().then(setUploadColumns).catch(() => {});
  }, [dispatch]);

  const filteredEmployees = useMemo(() => {
    const search = filters.search.trim().toLowerCase();
    return employees.filter((employee) => {
      const searchable = [
        employee.employeeCode,
        employee.fullName,
        employee.email,
        employee.employmentType,
        employee.designation,
        employee.practice,
        employee.subPractice,
        employee.location,
        employee.reportingManagerName,
        employee.practiceHeadName,
        employee.departmentType,
        employee.employeeStatus,
        employee.doj,
        employee.lwd,
      ]
        .join(' ')
        .toLowerCase();
      return !search || searchable.includes(search);
    });
  }, [employees, filters.search]);

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

  const handleImportComplete = useCallback(
    (result?: { columns?: UploadColumnInfo[] | null }) => {
      dispatch(fetchEmployees());
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
              {filteredEmployees.length} of {employees.length} employee records
              {uploadColumns.length > 0 && ` | ${uploadColumns.length} columns`}
            </Typography>
          </Box>
        </Stack>

        <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
          <TextField
            fullWidth
            onChange={(event) => dispatch(setEmployeeSearch(event.target.value))}
            placeholder="Search employees"
            value={filters.search}
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
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}

        <EmployeeGrid
          columns={columns}
          loading={loading}
          onPaginationModelChange={setPaginationModel}
          onRowClick={handleRowClick}
          paginationModel={paginationModel}
          rows={filteredEmployees}
        />
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
