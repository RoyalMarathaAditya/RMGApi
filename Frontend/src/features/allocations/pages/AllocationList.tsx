import { Box, Button, Card, CardContent, Stack, Typography, Alert } from '@mui/material';
import { useEffect, useMemo, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import { createAllocation, deleteAllocation, fetchAllocations, updateAllocation } from '../store/allocationSlice';
import AllocationDialog from '../components/AllocationDialog';
import DeleteAllocationDialog from '../components/DeleteAllocationDialog';
import ResourceAllocationGrid from '../components/ResourceAllocationGrid';
import type { ResourceAllocation } from '../types/resourceAllocation';
import type { ResourceAllocationFormValues } from '../types/resourceAllocation';

export default function AllocationList() {
  const dispatch = useAppDispatch();
  const { allocations, loading, error } = useAppSelector((state) => state.allocations);
  const employees = useAppSelector((state) => state.employees.employees);
  const projects = useAppSelector((state) => state.projects.projects);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [selectedAllocation, setSelectedAllocation] = useState<ResourceAllocation | null>(null);
  const [deleteOpen, setDeleteOpen] = useState(false);

  useEffect(() => {
    dispatch(fetchAllocations());
  }, [dispatch]);

  const handleOpenCreate = () => {
    setSelectedAllocation(null);
    setDialogOpen(true);
  };

  const handleEdit = (allocation: ResourceAllocation) => {
    setSelectedAllocation(allocation);
    setDialogOpen(true);
  };

  const handleDelete = (allocation: ResourceAllocation) => {
    setSelectedAllocation(allocation);
    setDeleteOpen(true);
  };

  const handleSubmit = (values: ResourceAllocationFormValues) => {
    if (selectedAllocation) {
      dispatch(updateAllocation({ id: selectedAllocation.id, values }));
    } else {
      dispatch(createAllocation(values));
    }
    setDialogOpen(false);
  };

  const handleConfirmDelete = () => {
    if (selectedAllocation) {
      dispatch(deleteAllocation(selectedAllocation.id));
    }
    setDeleteOpen(false);
  };

  const allocationGridData = useMemo(() => allocations, [allocations]);

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Resource Allocation Management
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Track allocation assignments, maintain bench and billable visibility, and adjust staffing allocations.
        </Typography>
      </Box>

      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" spacing={2}>
            <Typography variant="h6">Allocations</Typography>
            <Button variant="contained" onClick={handleOpenCreate}>
              New allocation
            </Button>
          </Stack>
        </CardContent>
      </Card>

      {error ? <Alert severity="error">{error}</Alert> : null}

      <ResourceAllocationGrid
        allocations={allocationGridData}
        employees={employees}
        projects={projects}
        loading={loading}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />

      <AllocationDialog
        open={dialogOpen}
        allocation={selectedAllocation}
        employees={employees}
        projects={projects}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
      />

      <DeleteAllocationDialog
        open={deleteOpen}
        allocationName={selectedAllocation ? `${selectedAllocation.employeeId} - ${selectedAllocation.projectId}` : ''}
        onConfirm={handleConfirmDelete}
        onClose={() => setDeleteOpen(false)}
      />
    </Stack>
  );
}
