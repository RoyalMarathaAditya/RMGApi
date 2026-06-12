import { Card, CardContent, Typography } from '@mui/material';
import { Area, AreaChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import type { BenchResource } from '../types/benchResource';

interface BenchTrendChartProps {
  benchResources: BenchResource[];
}

export default function BenchTrendChart({ benchResources }: BenchTrendChartProps) {
  const timeline = benchResources.reduce<Record<string, number>>((summary, resource) => {
    summary[resource.availabilityDate] = (summary[resource.availabilityDate] ?? 0) + 1;
    return summary;
  }, {});

  const chartData = Object.entries(timeline)
    .map(([date, count]) => ({ period: date, count }))
    .sort((a, b) => a.period.localeCompare(b.period));

  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: 360 }}>
      <CardContent>
        <Typography gutterBottom variant="h6">
          Bench availability trend
        </Typography>
        <ResponsiveContainer width="100%" height={280}>
          <AreaChart data={chartData} margin={{ top: 16, right: 16, left: 0, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="period" />
            <YAxis allowDecimals={false} />
            <Tooltip />
            <Area dataKey="count" name="Bench count" stroke="#9c27b0" fill="rgba(156,39,176,0.18)" />
          </AreaChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}
