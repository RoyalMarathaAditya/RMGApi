import { Box, Stack, Typography } from '@mui/material';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import { fetchBillableResources } from '../store/allocationSlice';
import BillableResourceGrid from '../components/BillableResourceGrid';

export default function BillableResources() {
  const dispatch = useAppDispatch();
  const { billableResources, loading } = useAppSelector((state) => state.allocations);

  useEffect(() => {
    dispatch(fetchBillableResources());
  }, [dispatch]);

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Billable resources
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Track active billable staff assignments tied to project demand and utilization targets.
        </Typography>
      </Box>
      <BillableResourceGrid billableResources={billableResources} loading={loading} />
    </Stack>
  );
}
