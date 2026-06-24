import { useEffect, useState } from 'react';
import { Box, Chip, Paper, Stack, Typography } from '@mui/material';
import PageContainer from '../../components/common/PageContainer';
import { allocationService } from '../../features/rmg/services/allocationService';
import type { TimelineViewDto } from '../../features/rmg/types/allocation';

export default function TimelineViewPage() {
  const [timeline, setTimeline] = useState<TimelineViewDto[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const data = await allocationService.getTimelineData();
      setTimeline(data);
    } catch {
      console.error('Failed to load timeline data');
    }
  };

  return (
    <PageContainer title="Timeline / Gantt View">
      <Stack spacing={3}>
        {timeline.map((project) => {
          const allDates = project.employees.flatMap((e) => {
            const d = [new Date(e.startDate)];
            if (e.endDate) d.push(new Date(e.endDate));
            return d;
          });
          const minDate = allDates.length > 0 ? new Date(Math.min(...allDates.map((d) => d.getTime()))) : new Date();
          const maxDate = allDates.length > 0 ? new Date(Math.max(...allDates.map((d) => d.getTime()))) : new Date();
          const totalDays = Math.max(1, (maxDate.getTime() - minDate.getTime()) / (1000 * 60 * 60 * 24) + 30);

          return (
            <Paper key={project.projectId} elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
              <Typography fontWeight={700} variant="h6" sx={{ mb: 2 }}>
                {project.projectName}
              </Typography>
              <Box sx={{ overflowX: 'auto' }}>
                <table style={{ borderCollapse: 'collapse', width: '100%', minWidth: 600 }}>
                  <thead>
                    <tr>
                      <th style={{ textAlign: 'left', padding: '8px 16px', borderBottom: '2px solid #e0e0e0', minWidth: 180, width: 180 }}>
                        Employee
                      </th>
                      <th style={{ textAlign: 'left', padding: '8px 16px', borderBottom: '2px solid #e0e0e0', minWidth: 80, width: 80 }}>
                        %
                      </th>
                      <th style={{ textAlign: 'left', padding: '8px 16px', borderBottom: '2px solid #e0e0e0' }}>
                        Timeline
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    {project.employees.map((emp) => {
                      const empStart = new Date(emp.startDate);
                      const empEnd = emp.endDate ? new Date(emp.endDate) : new Date(empStart);
                      const offsetDays = (empStart.getTime() - minDate.getTime()) / (1000 * 60 * 60 * 24);
                      const durationDays = Math.max(1, (empEnd.getTime() - empStart.getTime()) / (1000 * 60 * 60 * 24));
                      const offsetPercent = (offsetDays / totalDays) * 100;
                      const widthPercent = (durationDays / totalDays) * 100;

                      const barColor = emp.allocationStatus === 'Active' ? '#4caf50'
                        : emp.allocationStatus === 'Planned' ? '#2196f3'
                        : emp.allocationStatus === 'Completed' ? '#9e9e9e'
                        : '#ff9800';

                      return (
                        <tr key={`${emp.employeeId}-${emp.startDate}`}>
                          <td style={{ padding: '6px 16px', borderBottom: '1px solid #f0f0f0' }}>
                            <Typography variant="body2" fontWeight={600}>{emp.employeeName}</Typography>
                          </td>
                          <td style={{ padding: '6px 16px', borderBottom: '1px solid #f0f0f0' }}>
                            <Chip label={`${emp.allocationPercentage}%`} size="small" variant="outlined" />
                          </td>
                          <td style={{ padding: '12px 16px', borderBottom: '1px solid #f0f0f0', position: 'relative', height: 40 }}>
                            <Box
                              sx={{
                                position: 'absolute',
                                left: `calc(16px + ${offsetPercent}% )`,
                                width: `max(24px, ${widthPercent}%)`,
                                height: 24,
                                bgcolor: barColor,
                                borderRadius: 1,
                                opacity: 0.8,
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                minWidth: 24,
                              }}
                            >
                              <Typography variant="caption" sx={{ color: '#fff', fontWeight: 600, fontSize: 10 }}>
                                {emp.allocationPercentage}%
                              </Typography>
                            </Box>
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </Box>
            </Paper>
          );
        })}
      </Stack>
    </PageContainer>
  );
}
