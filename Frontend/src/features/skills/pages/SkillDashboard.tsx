import CategoryOutlinedIcon from '@mui/icons-material/CategoryOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import PsychologyOutlinedIcon from '@mui/icons-material/PsychologyOutlined';
import WorkspacePremiumOutlinedIcon from '@mui/icons-material/WorkspacePremiumOutlined';
import { Box, Card, CardContent, Skeleton, Stack, Typography } from '@mui/material';
import { Bar, BarChart, CartesianGrid, Cell, Pie, PieChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from 'recharts';
import SkillCard from '../components/SkillCard';
import { useSkills } from '../hooks/useSkills';

const palette = ['#1976d2', '#2e7d32', '#ed6c02', '#9c27b0', '#00897b', '#c62828', '#5d4037', '#546e7a', '#6a1b9a', '#ef6c00'];

export default function SkillDashboard() {
  const { data: skills = [], isLoading } = useSkills();
  const activeSkills = skills.filter((skill) => skill.status === 'Active');
  const employeesSkilled = new Set(skills.flatMap((skill) => Array.from({ length: Math.min(skill.employeeCount, 4) }, (_, index) => `${skill.id}-${index}`))).size;
  const topSkills = [...skills].sort((a, b) => b.employeeCount - a.employeeCount).slice(0, 8);
  const categoryData = Object.entries(
    skills.reduce<Record<string, number>>((acc, skill) => {
      acc[skill.category] = (acc[skill.category] ?? 0) + 1;
      return acc;
    }, {}),
  ).map(([name, value]) => ({ name, value }));
  const statusData = Object.entries(
    skills.reduce<Record<string, number>>((acc, skill) => {
      acc[skill.status] = (acc[skill.status] ?? 0) + 1;
      return acc;
    }, {}),
  ).map(([name, value]) => ({ name, value }));

  if (isLoading) {
    return (
      <Stack spacing={3}>
        <Skeleton height={56} width={320} />
        <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(4, 1fr)' } }}>
          {[1, 2, 3, 4].map((item) => (
            <Skeleton height={150} key={item} variant="rounded" />
          ))}
        </Box>
      </Stack>
    );
  }

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={900} variant="h4">
          Skill Dashboard
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Skill coverage, category mix, and resource readiness across the organization.
        </Typography>
      </Box>

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', lg: 'repeat(4, 1fr)' } }}>
        <SkillCard helper="Registered skills" icon={<PsychologyOutlinedIcon color="primary" />} label="Total Skills" value={skills.length} />
        <SkillCard helper="Ready for assignment" icon={<WorkspacePremiumOutlinedIcon color="success" />} label="Active Skills" value={activeSkills.length} />
        <SkillCard helper="Local mapping estimate" icon={<GroupsOutlinedIcon color="info" />} label="Employees Skilled" value={employeesSkilled} />
        <SkillCard helper={topSkills[0]?.skillName ?? 'No skills'} icon={<CategoryOutlinedIcon color="warning" />} label="Top Skills" value={topSkills.length} />
      </Box>

      <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', lg: '1.2fr 0.8fr' } }}>
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <CardContent>
            <Typography fontWeight={800} mb={2} variant="h6">
              Skill Distribution
            </Typography>
            <ResponsiveContainer height={320} width="100%">
              <BarChart data={topSkills}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="skillName" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="employeeCount" fill="#1976d2" radius={[6, 6, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <CardContent>
            <Typography fontWeight={800} mb={2} variant="h6">
              Skill Categories
            </Typography>
            <ResponsiveContainer height={320} width="100%">
              <PieChart>
                <Pie data={categoryData} dataKey="value" innerRadius={65} nameKey="name" outerRadius={110}>
                  {categoryData.map((entry, index) => (
                    <Cell fill={palette[index % palette.length]} key={entry.name} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </Box>

      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Typography fontWeight={800} mb={2} variant="h6">
            Top Skills
          </Typography>
          <ResponsiveContainer height={300} width="100%">
            <BarChart data={statusData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Bar dataKey="value" fill="#2e7d32" radius={[6, 6, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </CardContent>
      </Card>
    </Stack>
  );
}
