
import AddIcon from '@mui/icons-material/Add';
import SearchIcon from '@mui/icons-material/Search';
import { Alert, Box, Button, InputAdornment, Stack, TextField, Typography } from '@mui/material';
import type { GridPaginationModel, GridRowParams } from '@mui/x-data-grid';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import PageContainer from '../../../components/common/PageContainer';
// Redux: dispatches thunks for employee CRUD, reads employee list and filter state
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import BulkUploadSection from '../components/BulkUploadSection';
import EmployeeGrid from '../components/EmployeeGrid';
import EmployeeDialog from '../components/EmployeeDialog';
import DeleteEmployeeDialog from '../components/DeleteEmployeeDialog';
import { getEmployeeColumns } from '../config/employeeColumns';
// Redux async thunks and actions for employee operations
import {
  createEmployee,
  deleteEmployee,
  fetchEmployees,
  setEmployeeSearch,
  updateEmployee,
} from '../../../redux/slices/employeeSlice';
import type { Employee, EmployeeFormValues } from '../types/employee';

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

  useEffect(() => {
    void dispatch(fetchEmployees());
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
        employee.location,
        employee.departmentType,
        employee.employeeStatus,
      ]
        .join(' ')
        .toLowerCase();
      return !search || searchable.includes(search);
    });
  }, [employees, filters.search]);

  const columns = useMemo(
    () =>
      getEmployeeColumns({
        onDelete: (employee) => setDeleteTarget(employee),
        onEdit: (employee) => {
          setSelectedEmployee(employee);
          setDialogOpen(true);
        },
      }),
    [],
  );

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

  const handleImportComplete = useCallback(() => {
    dispatch(fetchEmployees());
  }, [dispatch]);

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
            </Typography>
          </Box>
          {/* <Button onClick={handleAdd} startIcon={<AddIcon />} variant="contained">
            Add Employee
          </Button> */}
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
