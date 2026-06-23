import { Box, Stack, Typography } from '@mui/material';
// Redux: reads employees from store to show their allocation status
import { useAppSelector } from '../../../redux/hooks';
import { useAllocationsQuery } from '../services/allocationQueries';
import EmployeeAllocationSummary from '../components/EmployeeAllocationSummary';

export default function EmployeeAllocationStatus() {
  const { data: allocations = [] } = useAllocationsQuery();
  const employees = useAppSelector((state) => state.employees.employees);

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Employee allocation status
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Track billable and non-billable employee allocations with detailed breakdown by department.
        </Typography>
      </Box>
      <EmployeeAllocationSummary allocations={allocations} employees={employees} />
    </Stack>
  );
}
