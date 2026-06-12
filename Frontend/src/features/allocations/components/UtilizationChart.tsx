import { Paper, Typography } from '@mui/material';
import { Area, AreaChart, CartesianGrid, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import type { ResourceUtilization } from '../types/resourceUtilization';

interface UtilizationChartProps {
  utilization: ResourceUtilization[];
}

export default function UtilizationChart({ utilization }: UtilizationChartProps) {
  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3, height: 360 }}>
      <Typography gutterBottom variant="h6">
        Resource utilization trend
      </Typography>
      <ResponsiveContainer width="100%" height="100%">
        <AreaChart data={utilization} margin={{ top: 16, right: 16, left: 0, bottom: 0 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="period" />
          <YAxis tickFormatter={(value) => `${value}%`} />
          <Tooltip formatter={(value: number) => `${value}%`} />
          <Area type="monotone" dataKey="utilizationRate" name="Utilization" stroke="#1976d2" fill="rgba(25,118,210,0.16)" />
          <Area type="monotone" dataKey="billableRate" name="Billable" stroke="#2e7d32" fill="rgba(46,125,50,0.14)" />
          <Area type="monotone" dataKey="benchRate" name="Bench" stroke="#ed6c02" fill="rgba(237,108,2,0.14)" />
        </AreaChart>
      </ResponsiveContainer>
    </Paper>
  );
}
