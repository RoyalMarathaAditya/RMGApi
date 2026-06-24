import { useState } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  LinearProgress,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import PageContainer from '../../../components/common/PageContainer';
import { dashboardService } from '../services/dashboardService';
import type { ResourceSuggestionDto } from '../types/allocation';

export default function ResourceFinder() {
  const [projectId, setProjectId] = useState('');
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<ResourceSuggestionDto[]>([]);
  const [searched, setSearched] = useState(false);

  const handleSearch = async () => {
    if (!projectId) return;
    setLoading(true);
    setSearched(true);
    try {
      const data = await dashboardService.getSuitableResources(Number(projectId));
      setResults(data);
    } catch {
      console.error('Failed to find suitable resources');
    } finally {
      setLoading(false);
    }
  };

  return (
    <PageContainer title="Resource Finder">
      <Stack spacing={3}>
        <Stack direction="row" spacing={2} alignItems="center">
          <TextField
            label="Project ID"
            type="number"
            value={projectId}
            onChange={(e) => setProjectId(e.target.value)}
            size="small"
            sx={{ width: 200 }}
          />
          <Button variant="contained" startIcon={<SearchOutlinedIcon />} onClick={handleSearch} disabled={loading || !projectId}>
            Find Suitable Resources
          </Button>
        </Stack>

        {loading && <LinearProgress />}

        {searched && !loading && (
          <Stack spacing={2}>
            <Typography fontWeight={700} variant="h6">
              Suggested Resources ({results.length})
            </Typography>
            <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))', lg: 'repeat(3, minmax(0, 1fr))' } }}>
              {results.map((r) => (
                <Card key={r.employeeId} elevation={0} sx={{ border: '1px solid', borderColor: 'divider' }}>
                  <CardContent>
                    <Stack spacing={1.5}>
                      <Typography fontWeight={700}>{r.employeeName}</Typography>
                      <Stack direction="row" spacing={1} alignItems="center">
                        <Box sx={{ flex: 1 }}>
                          <Typography variant="caption" color="text.secondary">Skill Match</Typography>
                          <LinearProgress variant="determinate" value={r.skillMatchPercentage} sx={{ height: 8, borderRadius: 4 }} />
                        </Box>
                        <Typography fontWeight={700} variant="body2">{r.skillMatchPercentage}%</Typography>
                      </Stack>
                      <Stack direction="row" spacing={1} alignItems="center">
                        <Box sx={{ flex: 1 }}>
                          <Typography variant="caption" color="text.secondary">Availability</Typography>
                          <LinearProgress variant="determinate" value={r.availabilityPercentage} color="success" sx={{ height: 8, borderRadius: 4 }} />
                        </Box>
                        <Typography fontWeight={700} variant="body2">{r.availabilityPercentage}%</Typography>
                      </Stack>
                      <Stack direction="row" spacing={1} justifyContent="space-between">
                        <Typography variant="caption" color="text.secondary">Allocated: {r.totalAllocated}%</Typography>
                        <Chip label={r.resourceStatus} size="small" variant="outlined" color={r.resourceStatus === 'Available' ? 'success' : 'warning'} />
                      </Stack>
                    </Stack>
                  </CardContent>
                </Card>
              ))}
              {results.length === 0 && (
                <Typography color="text.secondary">No suitable resources found for this project.</Typography>
              )}
            </Box>
          </Stack>
        )}
      </Stack>
    </PageContainer>
  );
}
