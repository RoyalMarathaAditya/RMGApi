import { Box, Stack, Typography } from '@mui/material';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../../app/hooks';
import { fetchBenchResources } from '../store/allocationSlice';
import BenchResourceGrid from '../components/BenchResourceGrid';

export default function BenchResources() {
  const dispatch = useAppDispatch();
  const { benchResources, loading } = useAppSelector((state) => state.allocations);

  useEffect(() => {
    dispatch(fetchBenchResources());
  }, [dispatch]);

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Bench resources
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Review available bench staff and upcoming deployment readiness by department.
        </Typography>
      </Box>
      <BenchResourceGrid benchResources={benchResources} loading={loading} />
    </Stack>
  );
}
