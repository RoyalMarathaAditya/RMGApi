import CalendarMonthOutlinedIcon from '@mui/icons-material/CalendarMonthOutlined';
import GroupsOutlinedIcon from '@mui/icons-material/GroupsOutlined';
import { Box, Card, CardContent, Chip, Stack, Typography } from '@mui/material';
import ProjectStatusChip from './ProjectStatusChip';
import type { Project } from '../types/project.types';

export default function ProjectCard({ project }: { project: Project }) {
  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', height: '100%' }}>
      <CardContent>
        <Stack spacing={2}>
          <Stack alignItems="flex-start" direction="row" justifyContent="space-between" spacing={2}>
            <Box minWidth={0}>
              <Typography color="text.secondary" fontWeight={700} variant="caption">
                {project.projectCode}
              </Typography>
              <Typography fontWeight={800} noWrap variant="h6">
                {project.projectName}
              </Typography>
              <Typography color="text.secondary" noWrap variant="body2">
                {project.clientName}
              </Typography>
            </Box>
            <ProjectStatusChip status={project.status} />
          </Stack>
          <Typography color="text.secondary" sx={{ minHeight: 44 }} variant="body2">
            {project.description}
          </Typography>
          <Stack direction="row" flexWrap="wrap" gap={1}>
            {project.technologies.slice(0, 4).map((technology) => (
              <Chip key={technology} label={technology} size="small" />
            ))}
          </Stack>
          <Stack direction="row" justifyContent="space-between">
            <Stack alignItems="center" direction="row" spacing={0.75}>
              <GroupsOutlinedIcon color="action" fontSize="small" />
              <Typography variant="body2">{project.allocatedResources} resources</Typography>
            </Stack>
            <Stack alignItems="center" direction="row" spacing={0.75}>
              <CalendarMonthOutlinedIcon color="action" fontSize="small" />
              <Typography variant="body2">{project.endDate}</Typography>
            </Stack>
          </Stack>
        </Stack>
      </CardContent>
    </Card>
  );
}
