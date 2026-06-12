import { Dialog, DialogActions, DialogContent, DialogTitle, Stack, Button, Typography } from '@mui/material';
import type { Employee } from '../../employees/types/employee';
import type { Project } from '../../projects/types/project.types';
import type { ResourceAllocation, ResourceAllocationFormValues } from '../types/resourceAllocation';
import AllocationForm from './AllocationForm';

interface AllocationDialogProps {
  open: boolean;
  allocation?: ResourceAllocation | null;
  employees: Employee[];
  projects: Project[];
  onClose: () => void;
  onSubmit: (values: ResourceAllocationFormValues) => void;
}

const defaultFormValues: ResourceAllocationFormValues = {
  employeeId: 0,
  projectId: 0,
  allocationPercentage: 50,
  allocationType: 1,
  startDate: new Date().toISOString().slice(0, 10),
  endDate: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString().slice(0, 10),
  isActive: true,
};

export default function AllocationDialog({ open, allocation, employees, projects, onClose, onSubmit }: AllocationDialogProps) {
  const initialValues: ResourceAllocationFormValues = allocation
    ? {
        employeeId: allocation.employeeId,
        projectId: allocation.projectId,
        allocationPercentage: allocation.allocationPercentage,
        allocationType: allocation.allocationType,
        startDate: allocation.startDate,
        endDate: allocation.endDate,
        isActive: allocation.isActive,
      }
    : defaultFormValues;

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{allocation ? 'Update Resource Allocation' : 'Create Resource Allocation'}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ pt: 1 }}>
          <Typography color="text.secondary" variant="body2">
            Use this form to allocate resources across active projects and maintain utilization compliance.
          </Typography>
          <AllocationForm employees={employees} projects={projects} defaultValues={initialValues} onSubmit={onSubmit} />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="outlined">
          Cancel
        </Button>
      </DialogActions>
    </Dialog>
  );
}
