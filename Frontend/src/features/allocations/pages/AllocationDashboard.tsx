import { Box, Card, CardContent, Stack, Tab, Tabs, Typography } from '@mui/material';
import { useState } from 'react';
import { useBillableResourcesQuery, useAllocationsQuery, useBenchResourcesQuery, useResourceUtilizationQuery } from '../services/allocationQueries';
import AllocationTypePieChart from '../components/AllocationTypePieChart';
import BenchTrendChart from '../components/BenchTrendChart';
import BillableResourceGrid from '../components/BillableResourceGrid';
import UtilizationChart from '../components/UtilizationChart';

const tabs = [
  { label: 'Summary', value: 'summary' },
  { label: 'Bench', value: 'bench' },
  { label: 'Billable', value: 'billable' },
  { label: 'Utilization', value: 'utilization' },
];

export default function AllocationDashboard() {
  const [activeTab, setActiveTab] = useState('summary');
  const { data: allocations = [] } = useAllocationsQuery();
  const { data: benchResources = [] } = useBenchResourcesQuery();
  const { data: billableResources = [] } = useBillableResourcesQuery();
  const { data: utilization = [] } = useResourceUtilizationQuery();

  const totalAllocations = allocations.length;
  const activeAllocations = allocations.filter((allocation) => allocation.isActive).length;
  const billableAllocations = allocations.filter((allocation) => allocation.allocationType === 1).length;
  const benchCount = benchResources.length;

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Resource Allocation Dashboard
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Monitor total allocation health, bench coverage, and utilization across the workforce.
        </Typography>
      </Box>

      <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', flex: 1 }}>
          <CardContent>
            <Typography color="text.secondary" variant="subtitle2">
              Total allocations
            </Typography>
            <Typography fontWeight={800} variant="h3">
              {totalAllocations}
            </Typography>
          </CardContent>
        </Card>
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', flex: 1 }}>
          <CardContent>
            <Typography color="text.secondary" variant="subtitle2">
              Active allocations
            </Typography>
            <Typography fontWeight={800} variant="h3">
              {activeAllocations}
            </Typography>
          </CardContent>
        </Card>
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', flex: 1 }}>
          <CardContent>
            <Typography color="text.secondary" variant="subtitle2">
              Billable allocations
            </Typography>
            <Typography fontWeight={800} variant="h3">
              {billableAllocations}
            </Typography>
          </CardContent>
        </Card>
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', flex: 1 }}>
          <CardContent>
            <Typography color="text.secondary" variant="subtitle2">
              Bench resources
            </Typography>
            <Typography fontWeight={800} variant="h3">
              {benchCount}
            </Typography>
          </CardContent>
        </Card>
      </Stack>

      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <Tabs value={activeTab} onChange={(_, value) => setActiveTab(value)} sx={{ px: 2 }}>
          {tabs.map((tab) => (
            <Tab key={tab.value} label={tab.label} value={tab.value} />
          ))}
        </Tabs>
        <Box sx={{ p: 3 }}>
          {activeTab === 'summary' && (
            <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', lg: 'repeat(2, minmax(0, 1fr))' } }}>
              <AllocationTypePieChart allocations={allocations} />
              <UtilizationChart utilization={utilization} />
            </Box>
          )}
          {activeTab === 'bench' && (
            <BenchTrendChart benchResources={benchResources} />
          )}
          {activeTab === 'billable' && (
            <BillableResourceGrid billableResources={billableResources} loading={false} />
          )}
          {activeTab === 'utilization' && (
            <UtilizationChart utilization={utilization} />
          )}
        </Box>
      </Card>
    </Stack>
  );
}
