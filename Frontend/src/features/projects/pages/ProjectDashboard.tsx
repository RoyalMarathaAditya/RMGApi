import AssignmentTurnedInOutlinedIcon from '@mui/icons-material/AssignmentTurnedInOutlined';
import PauseCircleOutlineOutlinedIcon from '@mui/icons-material/PauseCircleOutlineOutlined';
import RocketLaunchOutlinedIcon from '@mui/icons-material/RocketLaunchOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import { Box, Card, CardContent, Paper, Stack, Typography } from '@mui/material';
import {
  Area,
  AreaChart,
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts';
// Redux: reads projects from store for dashboard statistics
import { useAppSelector } from '../../../redux/hooks';
import type { ProjectStatus } from '../types/project.types';

const statusColors: Record<ProjectStatus, string> = {
  Planned: '#42a5f5',
  Active: '#2e7d32',
  Completed: '#1976d2',
  'On Hold': '#ed6c02',
  Cancelled: '#d32f2f',
};

function KpiCard({
  color,
  icon: Icon,
  label,
  value,
}: {
  color: string;
  icon: typeof WorkOutlineOutlinedIcon;
  label: string;
  value: number;
}) {
  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', height: '100%' }}>
      <CardContent>
        <Stack alignItems="center" direction="row" spacing={2}>
          <Box
            alignItems="center"
            borderRadius={2}
            color={color}
            display="flex"
            height={52}
            justifyContent="center"
            sx={{ bgcolor: `${color}1a` }}
            width={52}
          >
            <Icon />
          </Box>
          <Box minWidth={0}>
            <Typography color="text.secondary" noWrap variant="body2">
              {label}
            </Typography>
            <Typography fontWeight={800} variant="h4">
              {value}
            </Typography>
          </Box>
        </Stack>
      </CardContent>
    </Card>
  );
}

export default function ProjectDashboard() {
  const projects = useAppSelector((state) => state.projects.projects);
  const totalProjects = projects.length;
  const activeProjects = projects.filter((project) => project.status === 'Active').length;
  const completedProjects = projects.filter((project) => project.status === 'Completed').length;
  const onHoldProjects = projects.filter((project) => project.status === 'On Hold').length;

  const statusDistribution = Object.entries(
    projects.reduce<Record<ProjectStatus, number>>(
      (summary, project) => {
        summary[project.status] += 1;
        return summary;
      },
      { Planned: 0, Active: 0, Completed: 0, 'On Hold': 0, Cancelled: 0 },
    ),
  ).map(([name, value]) => ({ name, value }));

  const technologyUsage = Object.entries(
    projects.reduce<Record<string, number>>((summary, project) => {
      project.technologies.forEach((technology) => {
        summary[technology] = (summary[technology] ?? 0) + 1;
      });

      return summary;
    }, {}),
  )
    .map(([technology, count]) => ({ count, technology }))
    .sort((a, b) => b.count - a.count);

  const timelineSummary = projects.reduce<Array<{ month: string; projects: number; resources: number }>>((summary, project) => {
    const month = project.startDate.slice(0, 7);
    const existing = summary.find((item) => item.month === month);

    if (existing) {
      existing.projects += 1;
      existing.resources += project.allocatedResources;
    } else {
      summary.push({ month, projects: 1, resources: project.allocatedResources });
    }

    return summary;
  }, []);

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
        <Box>
          <Typography component="h1" fontWeight={800} variant="h4">
            Project Dashboard
          </Typography>
          <Typography color="text.secondary" mt={0.75}>
            Portfolio health, delivery status, and technology utilization.
          </Typography>
        </Box>
      </Stack>

      <Box
        sx={{
          display: 'grid',
          gap: 2,
          gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', lg: 'repeat(4, 1fr)' },
        }}
      >
        <KpiCard color="#1976d2" icon={WorkOutlineOutlinedIcon} label="Total Projects" value={totalProjects} />
        <KpiCard color="#2e7d32" icon={RocketLaunchOutlinedIcon} label="Active Projects" value={activeProjects} />
        <KpiCard color="#9c27b0" icon={AssignmentTurnedInOutlinedIcon} label="Completed Projects" value={completedProjects} />
        <KpiCard color="#ed6c02" icon={PauseCircleOutlineOutlinedIcon} label="On Hold Projects" value={onHoldProjects} />
      </Box>

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', lg: '0.9fr 1.1fr' } }}>
        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
          <Typography fontWeight={800} gutterBottom variant="h6">
            Project Status Distribution
          </Typography>
          <Box sx={{ height: 320 }}>
            <ResponsiveContainer>
              <PieChart>
                <Pie data={statusDistribution} dataKey="value" innerRadius={72} nameKey="name" outerRadius={112}>
                  {statusDistribution.map((entry) => (
                    <Cell fill={statusColors[entry.name as ProjectStatus]} key={entry.name} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </Box>
        </Paper>

        <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
          <Typography fontWeight={800} gutterBottom variant="h6">
            Technology Usage
          </Typography>
          <Box sx={{ height: 320 }}>
            <ResponsiveContainer>
              <BarChart data={technologyUsage}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="technology" interval={0} tick={{ fontSize: 12 }} />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Bar dataKey="count" fill="#1976d2" radius={[6, 6, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </Box>
        </Paper>
      </Box>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
        <Typography fontWeight={800} gutterBottom variant="h6">
          Project Timeline Summary
        </Typography>
        <Box sx={{ height: 320 }}>
          <ResponsiveContainer>
            <AreaChart data={timelineSummary.sort((a, b) => a.month.localeCompare(b.month))}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="month" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Area dataKey="resources" fill="#9c27b0" fillOpacity={0.16} stroke="#9c27b0" type="monotone" />
              <Area dataKey="projects" fill="#1976d2" fillOpacity={0.18} stroke="#1976d2" type="monotone" />
            </AreaChart>
          </ResponsiveContainer>
        </Box>
      </Paper>
    </Stack>
  );
}
