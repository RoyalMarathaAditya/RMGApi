import { Card, CardContent, Typography } from '@mui/material';
import { Cell, Pie, PieChart, ResponsiveContainer, Tooltip } from 'recharts';
import type { ResourceAllocation } from '../types/resourceAllocation';
import { allocationTypeLabelMap } from '../types/allocationType';

interface AllocationTypePieChartProps {
  allocations: ResourceAllocation[];
}

const colors = ['#1976d2', '#2e7d32', '#9c27b0', '#ed6c02', '#0288d1', '#d32f2f'];

export default function AllocationTypePieChart({ allocations }: AllocationTypePieChartProps) {
  const chartData = Object.entries(
    allocations.reduce<Record<number, number>>((summary, allocation) => {
      summary[allocation.allocationType] = (summary[allocation.allocationType] ?? 0) + 1;
      return summary;
    }, {}),
  ).map(([key, value]) => ({
    name: allocationTypeLabelMap[Number(key) as number] ?? 'Unknown',
    value,
  }));

  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, height: 360 }}>
      <CardContent>
        <Typography gutterBottom variant="h6">
          Allocation type split
        </Typography>
        <ResponsiveContainer width="100%" height={280}>
          <PieChart>
            <Pie data={chartData} dataKey="value" innerRadius={72} outerRadius={108} paddingAngle={3}>
              {chartData.map((entry, index) => (
                <Cell key={entry.name} fill={colors[index % colors.length]} />
              ))}
            </Pie>
            <Tooltip formatter={(value: number) => `${value} allocations`} />
          </PieChart>
        </ResponsiveContainer>
      </CardContent>
    </Card>
  );
}
