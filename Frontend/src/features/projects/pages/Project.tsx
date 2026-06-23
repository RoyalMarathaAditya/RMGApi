import ArrowBackOutlinedIcon from '@mui/icons-material/ArrowBackOutlined';
import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Box, Button, Chip, Divider, LinearProgress, Paper, Stack, Typography } from '@mui/material';
import { useMemo } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
// Redux: reads projects from store to find the currently selected project
import { useAppSelector } from '../../../redux/hooks';
import ProjectStatusChip from '../components/ProjectStatusChip';

export default function Project() {
  const navigate = useNavigate();
  const { id } = useParams();
  const projects = useAppSelector((state) => state.projects.projects);
  const project = useMemo(() => projects.find((item) => item.id === Number(id)), [id, projects]);

  if (!project) {
    return (
      <Box>
        <Typography component="h1" fontWeight={800} variant="h4">
          Project not found
        </Typography>
        <Typography color="text.secondary" mt={1}>
          The selected project does not exist in the mock project list.
        </Typography>
        <Button onClick={() => navigate('/projects')} sx={{ mt: 3 }} variant="contained">
          Back to Projects
        </Button>
      </Box>
    );
  }

  const utilization = Math.min(100, Math.round((project.allocatedResources / 30) * 100));

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
        <Box>
          <Button onClick={() => navigate('/projects')} startIcon={<ArrowBackOutlinedIcon />} sx={{ mb: 1 }} variant="text">
            Back to Projects
          </Button>
          <Typography component="h1" fontWeight={800} variant="h4">
            {project.projectName}
          </Typography>
          <Typography color="text.secondary" mt={0.75}>
            {project.projectCode} · {project.clientName}
          </Typography>
        </Box>
        <Button onClick={() => navigate(`/projects/edit/${project.id}`)} startIcon={<EditOutlinedIcon />} variant="contained">
          Edit Project
        </Button>
      </Stack>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: { xs: 2, md: 3 } }}>
        <Stack divider={<Divider />} spacing={3}>
          <Box>
            <Typography fontWeight={800} gutterBottom variant="h6">
              Project Information
            </Typography>
            <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' } }}>
              <Detail label="Project Code" value={project.projectCode} />
              <Detail label="Project Name" value={project.projectName} />
              <Detail label="Client" value={`${project.clientName} (${project.clientContact})`} />
              <Detail label="Manager" value={project.projectManager} />
              <Detail label="Start Date" value={project.startDate} />
              <Detail label="End Date" value={project.endDate} />
              <Box>
                <Typography color="text.secondary" variant="body2">
                  Status
                </Typography>
                <ProjectStatusChip status={project.status} />
              </Box>
              <Detail label="Priority" value={project.priority} />
            </Box>
            <Typography color="text.secondary" mt={3} variant="body2">
              Description
            </Typography>
            <Typography mt={0.5}>{project.description}</Typography>
          </Box>

          <Box>
            <Typography fontWeight={800} gutterBottom variant="h6">
              Technology Stack
            </Typography>
            <Stack direction="row" flexWrap="wrap" gap={1}>
              {project.technologies.map((technology) => (
                <Chip key={technology} label={technology} />
              ))}
            </Stack>
          </Box>

          <Box>
            <Typography fontWeight={800} gutterBottom variant="h6">
              Resource Summary
            </Typography>
            <Box sx={{ maxWidth: 520 }}>
              <Stack direction="row" justifyContent="space-between" mb={1}>
                <Typography color="text.secondary">Allocated Resources</Typography>
                <Typography fontWeight={800}>{project.allocatedResources}</Typography>
              </Stack>
              <Stack direction="row" justifyContent="space-between" mb={1}>
                <Typography color="text.secondary">Project Utilization</Typography>
                <Typography fontWeight={800}>{utilization}%</Typography>
              </Stack>
              <LinearProgress value={utilization} variant="determinate" />
            </Box>
          </Box>
        </Stack>
      </Paper>
    </Stack>
  );
}

function Detail({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography color="text.secondary" variant="body2">
        {label}
      </Typography>
      <Typography fontWeight={700} mt={0.5}>
        {value}
      </Typography>
    </Box>
  );
}
