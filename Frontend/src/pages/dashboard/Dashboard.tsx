import AssignmentTurnedInOutlinedIcon from '@mui/icons-material/AssignmentTurnedInOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import PersonOffOutlinedIcon from '@mui/icons-material/PersonOffOutlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import {
  Box,
  Card,
  CardContent,
  Chip,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import type { SvgIconComponent } from '@mui/icons-material';

const dashboardMetrics = [
  { icon: GroupsOutlinedIcon, label: 'Total Employees', value: 250 },
  { icon: WorkOutlineOutlinedIcon, label: 'Total Projects', value: 45 },
  { icon: AssignmentTurnedInOutlinedIcon, label: 'Billable Resources', value: 190 },
  { icon: PersonOffOutlinedIcon, label: 'Bench Resources', value: 60 },
];

const recentActivities = [
  {
    employee: 'John Doe',
    project: 'HRMS',
    allocation: 100,
    status: 'Billable',
  },
  {
    employee: 'Jane Smith',
    project: 'CRM',
    allocation: 50,
    status: 'Partial',
  },
];

function KpiCard({ icon: Icon, label, value }: { icon: SvgIconComponent; label: string; value: number }) {
  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', height: '100%' }}>
      <CardContent>
        <Stack alignItems="center" direction="row" spacing={2}>
          <Box
            alignItems="center"
            bgcolor="primary.50"
            borderRadius={2}
            color="primary.main"
            display="flex"
            height={52}
            justifyContent="center"
            sx={{ backgroundColor: 'rgba(25, 118, 210, 0.1)' }}
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

export default function Dashboard() {
  return (
    <Stack spacing={3}>
      <Paper
        elevation={0}
        sx={{
          border: '1px solid',
          borderColor: 'divider',
          borderRadius: 2,
          p: { xs: 2.5, md: 3 },
        }}
      >
        <Typography color="text.secondary" fontWeight={700} variant="body2">
          Dashboard
        </Typography>
        <Typography component="h1" fontWeight={800} mt={0.5} variant="h4">
          Welcome Back Admin
        </Typography>
      </Paper>

      <Box
        sx={{
          display: 'grid',
          gap: 2,
          gridTemplateColumns: {
            xs: '1fr',
            sm: 'repeat(2, minmax(0, 1fr))',
            lg: 'repeat(4, minmax(0, 1fr))',
          },
        }}
      >
        {dashboardMetrics.map((metric) => (
          <KpiCard key={metric.label} {...metric} />
        ))}
      </Box>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
        <Box px={3} py={2.5}>
          <Typography fontWeight={800} variant="h5">
            Recent Activities
          </Typography>
        </Box>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Employee Name</TableCell>
                <TableCell>Project Name</TableCell>
                <TableCell>Allocation %</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {recentActivities.map((activity) => (
                <TableRow hover key={`${activity.employee}-${activity.project}`}>
                  <TableCell>{activity.employee}</TableCell>
                  <TableCell>{activity.project}</TableCell>
                  <TableCell>{activity.allocation}%</TableCell>
                  <TableCell>
                    <Chip
                      color={activity.status === 'Billable' ? 'success' : 'warning'}
                      label={activity.status}
                      size="small"
                      variant="outlined"
                    />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Stack>
  );
}
