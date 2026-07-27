import { Box, Paper, Typography, useMediaQuery, useTheme } from '@mui/material';
import { useMemo } from 'react';
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Line,
  LineChart,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
import type { ReportChartDataDto } from '../types/report';

const CHART_COLORS = {
  billable: '#2563EB',
  nonBillable: '#9CA3AF',
  active: '#22C55E',
  completed: '#3B82F6',
  onHold: '#F59E0B',
  cancelled: '#EF4444',
};

const DONUT_COLORS = [CHART_COLORS.billable, CHART_COLORS.nonBillable];

const STATUS_COLORS = [CHART_COLORS.active, CHART_COLORS.completed, CHART_COLORS.onHold, CHART_COLORS.cancelled];

interface ReportChartsProps {
  data: ReportChartDataDto;
  chartType: 'client' | 'project';
}

const cardSx = {
  border: '1px solid',
  borderColor: 'divider',
  borderRadius: 2,
  p: 2.5,
  minHeight: 420,
  display: 'flex',
  flexDirection: 'column',
} as const;

const chartWrapperSx = {
  flex: 1,
  width: '100%',
  minHeight: 300,
} as const;

interface DonutTooltipProps {
  active?: boolean;
  payload?: Array<{ name: string; value: number }>;
}

function DonutTooltip({ active, payload }: DonutTooltipProps) {
  if (!active || !payload?.length) return null;
  return (
    <Paper elevation={3} sx={{ px: 1.5, py: 1, borderRadius: 1.5, fontSize: 13, fontWeight: 600 }}>
      <Typography variant="body2" fontWeight={700}>{payload[0].name}</Typography>
      <Typography variant="body2" color="text.secondary">Count: {payload[0].value}</Typography>
    </Paper>
  );
}

interface BarTooltipProps {
  active?: boolean;
  payload?: Array<{ name: string; value: number }>;
  label?: string;
}

function BarTooltip({ active, payload, label }: BarTooltipProps) {
  if (!active || !payload?.length) return null;
  return (
    <Paper elevation={3} sx={{ px: 1.5, py: 1, borderRadius: 1.5, fontSize: 13 }}>
      <Typography variant="body2" fontWeight={700}>{label}</Typography>
      <Typography variant="body2" color="text.secondary">
        Utilization: {payload[0].value.toFixed(1)}%
      </Typography>
    </Paper>
  );
}

interface LineTooltipProps {
  active?: boolean;
  payload?: Array<{ name: string; value: number; color: string }>;
  label?: string;
}

function LineTooltip({ active, payload, label }: LineTooltipProps) {
  if (!active || !payload?.length) return null;
  return (
    <Paper elevation={3} sx={{ px: 1.5, py: 1, borderRadius: 1.5, fontSize: 13 }}>
      <Typography variant="body2" fontWeight={700}>{label}</Typography>
      {payload.map((entry) => (
        <Box key={entry.name} sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
          <Box sx={{ width: 10, height: 10, borderRadius: '50%', bgcolor: entry.color, flexShrink: 0 }} />
          <Typography variant="body2" color="text.secondary">
            {entry.name}: {entry.value}
          </Typography>
        </Box>
      ))}
    </Paper>
  );
}

interface DonutLegendProps {
  payload?: Array<{ value: string; color: string }>;
}

function DonutLegend({ payload }: DonutLegendProps) {
  if (!payload) return null;
  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', gap: 3, pt: 1 }}>
      {payload.map((entry) => (
        <Box key={entry.value} sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
          <Box sx={{ width: 12, height: 12, borderRadius: '50%', bgcolor: entry.color, flexShrink: 0 }} />
          <Typography variant="body2" fontSize={13}>{entry.value}</Typography>
        </Box>
      ))}
    </Box>
  );
}

function truncateLabel(name: string, maxLen = 16): string {
  return name.length > maxLen ? `${name.slice(0, maxLen - 3)}...` : name;
}

function NoData() {
  return (
    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', height: '100%', minHeight: 200 }}>
      <Typography variant="body1" color="text.secondary">No Data Available</Typography>
    </Box>
  );
}

export default function ReportCharts({ data, chartType }: ReportChartsProps) {
  const theme = useTheme();
  const isDesktop = useMediaQuery(theme.breakpoints.up('lg'));
  const isTablet = useMediaQuery(theme.breakpoints.between('md', 'lg'));

  const gridColumns = useMemo(() => {
    return isDesktop ? 4 : isTablet ? 2 : 1;
  }, [isDesktop, isTablet]);

  const barData = useMemo(() => data.utilizationByEntity.slice(0, 10), [data.utilizationByEntity]);
  const billableData = useMemo(() => data.billableDistribution, [data.billableDistribution]);
  const statusData = useMemo(() => data.projectsByStatus, [data.projectsByStatus]);
  const trendData = useMemo(() => data.monthlyTrend, [data.monthlyTrend]);

  const hasBillableData = billableData.length > 0;
  const hasBarData = barData.length > 0;
  const hasStatusData = statusData.length > 0;
  const hasTrendData = trendData.length > 0;

  const barTickInterval = useMemo(() => {
    if (barData.length <= 6) return 0;
    if (barData.length <= 8) return 1;
    return 1;
  }, [barData.length]);

  return (
    <Box>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: `repeat(${gridColumns}, 1fr)`,
          gap: 3,
        }}
      >
        <Paper elevation={0} sx={cardSx}>
          <Typography variant="subtitle1" fontWeight={700} mb={1}>
            Billable vs Non-Billable
          </Typography>
          <Box sx={chartWrapperSx}>
            {hasBillableData ? (
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={billableData}
                    dataKey="value"
                    innerRadius="65%"
                    outerRadius="90%"
                    nameKey="name"
                    cx="50%"
                    cy="50%"
                    paddingAngle={2}
                    animationBegin={0}
                    animationDuration={600}
                  >
                    {billableData.map((entry) => (
                      <Cell
                        key={entry.name}
                        fill={DONUT_COLORS[billableData.indexOf(entry)]}
                      />
                    ))}
                  </Pie>
                  <Tooltip content={<DonutTooltip />} />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <NoData />
            )}
          </Box>
          {hasBillableData && (
            <DonutLegend
              payload={billableData.map((d) => ({
                value: `${d.name} (${d.value})`,
                color: DONUT_COLORS[billableData.indexOf(d)],
              }))}
            />
          )}
        </Paper>

        <Paper elevation={0} sx={cardSx}>
          <Typography variant="subtitle1" fontWeight={700} mb={1}>
            {chartType === 'client' ? 'Utilization by Client' : 'Utilization by Project'}
          </Typography>
          <Box sx={chartWrapperSx}>
            {hasBarData ? (
              <ResponsiveContainer width="100%" height="100%">
                <BarChart
                  data={barData}
                  margin={{ top: 10, right: 16, left: -10, bottom: 24 }}
                >
                  <CartesianGrid strokeDasharray="3 3" vertical={false} />
                  <XAxis
                    dataKey="name"
                    tick={{ fontSize: 11 }}
                    interval={barTickInterval}
                    tickFormatter={truncateLabel}
                    height={80}
                    angle={barData.length > 6 ? -25 : 0}
                    textAnchor={barData.length > 6 ? 'end' : 'middle'}
                  />
                  <YAxis domain={[0, 100]} unit="%" tick={{ fontSize: 12 }} width={36} />
                  <Tooltip content={<BarTooltip />} />
                  <Bar dataKey="utilization" fill={CHART_COLORS.billable} radius={[4, 4, 0, 0]} maxBarSize={36} />
                </BarChart>
              </ResponsiveContainer>
            ) : (
              <NoData />
            )}
          </Box>
        </Paper>

        <Paper elevation={0} sx={cardSx}>
          <Typography variant="subtitle1" fontWeight={700} mb={1}>
            Projects by Status
          </Typography>
          <Box sx={chartWrapperSx}>
            {hasStatusData ? (
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={statusData}
                    dataKey="count"
                    innerRadius="65%"
                    outerRadius="90%"
                    nameKey="name"
                    cx="50%"
                    cy="50%"
                    paddingAngle={2}
                    animationBegin={0}
                    animationDuration={600}
                  >
                    {statusData.map((entry) => (
                      <Cell
                        key={entry.name}
                        fill={STATUS_COLORS[statusData.indexOf(entry)]}
                      />
                    ))}
                  </Pie>
                  <Tooltip content={<DonutTooltip />} />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <NoData />
            )}
          </Box>
          {hasStatusData && (
            <DonutLegend
              payload={statusData.map((d) => ({
                value: `${d.name} (${d.count})`,
                color: STATUS_COLORS[statusData.indexOf(d)],
              }))}
            />
          )}
        </Paper>

        <Paper elevation={0} sx={cardSx}>
          <Typography variant="subtitle1" fontWeight={700} mb={1}>
            Monthly Trend
          </Typography>
          <Box sx={chartWrapperSx}>
            {hasTrendData ? (
              <ResponsiveContainer width="100%" height="100%">
                <LineChart
                  data={trendData}
                  margin={{ top: 10, right: 16, left: -10, bottom: 24 }}
                >
                  <CartesianGrid strokeDasharray="3 3" vertical={false} />
                  <XAxis dataKey="month" tick={{ fontSize: 11 }} height={70} />
                  <YAxis allowDecimals={false} tick={{ fontSize: 12 }} width={36} />
                  <Tooltip content={<LineTooltip />} />
                  <Legend
                    verticalAlign="bottom"
                    height={30}
                    iconType="circle"
                    iconSize={10}
                    formatter={(value: string) => (
                      <span style={{ fontSize: 13 }}>{value}</span>
                    )}
                  />
                  <Line
                    dataKey="started"
                    stroke={CHART_COLORS.active}
                    strokeWidth={2.5}
                    dot={{ r: 3, fill: CHART_COLORS.active }}
                    type="monotone"
                    name="Started"
                    animationDuration={600}
                  />
                  <Line
                    dataKey="ended"
                    stroke={CHART_COLORS.cancelled}
                    strokeWidth={2.5}
                    dot={{ r: 3, fill: CHART_COLORS.cancelled }}
                    type="monotone"
                    name="Ended"
                    animationDuration={600}
                  />
                </LineChart>
              </ResponsiveContainer>
            ) : (
              <NoData />
            )}
          </Box>
        </Paper>
      </Box>
    </Box>
  );
}
