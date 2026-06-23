import { Box, Stack, Typography } from '@mui/material';
import { useEffect } from 'react';
// Redux: dispatches fetchUtilization, reads utilization/allocations data from store
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import { fetchUtilization } from '../../../redux/slices/allocationSlice';
import AllocationTypePieChart from '../components/AllocationTypePieChart';
import UtilizationChart from '../components/UtilizationChart';

export default function ResourceUtilization() {
  const dispatch = useAppDispatch();
  const { allocations, utilization, loading } = useAppSelector((state) => state.allocations);

  useEffect(() => {
    dispatch(fetchUtilization());
  }, [dispatch]);

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Resource utilization
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Slice resource utilization into billable, bench, and allocation type trends for staffing decisions.
        </Typography>
      </Box>
      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', lg: 'repeat(2, minmax(0, 1fr))' } }}>
        <AllocationTypePieChart allocations={allocations} />
        <UtilizationChart utilization={utilization} />
      </Box>
    </Stack>
  );
}
