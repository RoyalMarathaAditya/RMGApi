
import AddIcon from '@mui/icons-material/Add';
import SearchIcon from '@mui/icons-material/Search';
import { Alert, Box, Button, InputAdornment, MenuItem, Stack, TextField, Typography } from '@mui/material';
import type { GridPaginationModel } from '@mui/x-data-grid';
import { useEffect, useMemo, useState } from 'react';
import PageContainer from '../../../components/common/PageContainer';
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import EmployeeGrid from '../components/EmployeeGrid';
import EmployeeDialog from '../components/EmployeeDialog';
import DeleteEmployeeDialog from '../components/DeleteEmployeeDialog';
import { getEmployeeColumns } from '../config/employeeColumns';
import {
  createEmployee,
  deleteEmployee,
  fetchEmployees,
  setEmployeeSearch,
  setEmployeeStatusFilter,
  updateEmployee,
} from '../store/employeeSlice';
import type { Employee, EmployeeFilters, EmployeeFormValues } from '../types/employee';

const statusFilters: EmployeeFilters['status'][] = ['All', 'Active', 'Inactive', 'On Leave'];

export default function EmployeeList() {
  const dispatch = useAppDispatch();
  const { employees, error, filters, loading } = useAppSelector((state) => state.employees);
  const skills = useAppSelector((state) => state.skills.skills);
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
    const skillNameById = new Map(skills.map((skill) => [skill.id, skill.skillName]));

    return employees.filter((employee) => {
      const matchesStatus = filters.status === 'All' || employee.status === filters.status;
      const searchable = [
        employee.employeeCode,
        employee.firstName,
        employee.lastName,
        employee.email,
        employee.department,
        employee.designation,
        employee.role,
        ...employee.skillIds.map((skillId) => skillNameById.get(skillId) ?? ''),
      ]
        .join(' ')
        .toLowerCase();

      return matchesStatus && (!search || searchable.includes(search));
    });
  }, [employees, filters.search, filters.status, skills]);

  const columns = useMemo(
    () =>
      getEmployeeColumns({
        skills,
        onDelete: (employee) => setDeleteTarget(employee),
        onEdit: (employee) => {
          setSelectedEmployee(employee);
          setDialogOpen(true);
        },
      }),
    [skills],
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
    if (!deleteTarget) {
      return;
    }

    await dispatch(deleteEmployee(deleteTarget.id)).unwrap();
    setDeleteTarget(null);
  };

  return (
    <PageContainer title="Employee Management">
      <Stack spacing={2.5}>
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
          <Button onClick={handleAdd} startIcon={<AddIcon />} variant="contained">
            Add Employee
          </Button>
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
          <TextField
            label="Status"
            onChange={(event) =>
              dispatch(setEmployeeStatusFilter(event.target.value as EmployeeFilters['status']))
            }
            select
            sx={{ minWidth: { md: 180 } }}
            value={filters.status}
          >
            {statusFilters.map((status) => (
              <MenuItem key={status} value={status}>
                {status}
              </MenuItem>
            ))}
          </TextField>
        </Stack>

        {error ? <Alert severity="error">{error}</Alert> : null}

        <EmployeeGrid
          columns={columns}
          loading={loading}
          onPaginationModelChange={setPaginationModel}
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
