import { useEffect, useState } from 'react';
import { Box, Chip, Paper, Stack, Tooltip, Typography } from '@mui/material';
import PageContainer from '../../../components/common/PageContainer';
import { allocationService } from '../services/allocationService';
import type { CalendarViewDto } from '../types/allocation';

export default function CalendarViewPage() {
  const [events, setEvents] = useState<CalendarViewDto[]>([]);

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const data = await allocationService.getCalendarData();
      setEvents(data);
    } catch {
      console.error('Failed to load calendar data');
    }
  };

  const getMonths = () => {
    if (events.length === 0) return [];
    const dates = events.flatMap((e) => {
      const d = [new Date(e.startDate)];
      if (e.endDate) d.push(new Date(e.endDate));
      return d;
    });
    const min = new Date(Math.min(...dates.map((d) => d.getTime())));
    const max = new Date(Math.max(...dates.map((d) => d.getTime())));
    const months: Date[] = [];
    const current = new Date(min.getFullYear(), min.getMonth(), 1);
    while (current <= max) {
      months.push(new Date(current));
      current.setMonth(current.getMonth() + 1);
    }
    return months;
  };

  const months = getMonths();
  const monthLabels = months.map((m) => m.toLocaleString('default', { month: 'short', year: '2-digit' }));

  return (
    <PageContainer title="Calendar View">
      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
        <Stack spacing={2}>
          <Stack direction="row" spacing={2} alignItems="center" flexWrap="wrap">
            <Chip label="Available" size="small" sx={{ bgcolor: '#4caf50', color: '#fff' }} />
            <Chip label="Allocated" size="small" sx={{ bgcolor: '#2196f3', color: '#fff' }} />
            <Chip label="Fully Allocated" size="small" sx={{ bgcolor: '#ff9800', color: '#fff' }} />
            <Chip label="Overallocated" size="small" sx={{ bgcolor: '#f44336', color: '#fff' }} />
          </Stack>

          <Box sx={{ overflowX: 'auto' }}>
            <table style={{ borderCollapse: 'collapse', width: '100%', minWidth: 800 }}>
              <thead>
                <tr>
                  <th style={{ textAlign: 'left', padding: '8px 16px', borderBottom: '2px solid #e0e0e0', position: 'sticky', left: 0, background: '#fff', minWidth: 200 }}>
                    Resource / Project
                  </th>
                  {monthLabels.map((label) => (
                    <th key={label} style={{ textAlign: 'center', padding: '8px 4px', borderBottom: '2px solid #e0e0e0', fontSize: 12, minWidth: 80 }}>
                      {label}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {events.map((event) => {
                  const startMonth = new Date(event.startDate);
                  const endMonth = event.endDate ? new Date(event.endDate) : new Date(startMonth);
                  return (
                    <tr key={event.allocationId}>
                      <td style={{ padding: '8px 16px', borderBottom: '1px solid #f0f0f0', position: 'sticky', left: 0, background: '#fff' }}>
                        <Tooltip title={`${event.employeeName} - ${event.projectName} (${event.allocationPercentage}%)`}>
                          <Typography variant="body2" fontWeight={600} noWrap sx={{ maxWidth: 200 }}>
                            {event.employeeName}
                          </Typography>
                        </Tooltip>
                        <Typography variant="caption" color="text.secondary" noWrap sx={{ maxWidth: 200 }}>
                          {event.projectName}
                        </Typography>
                      </td>
                      {months.map((month) => {
                        const monthStart = new Date(month.getFullYear(), month.getMonth(), 1);
                        const monthEnd = new Date(month.getFullYear(), month.getMonth() + 1, 0);
                        const isActive = startMonth <= monthEnd && endMonth >= monthStart;
                        return (
                          <td key={month.toISOString()} style={{ padding: 0, borderBottom: '1px solid #f0f0f0' }}>
                            {isActive && (
                              <Box
                                sx={{
                                  height: 40,
                                  bgcolor: event.colorCode,
                                  opacity: 0.7,
                                  m: 0.5,
                                  borderRadius: 1,
                                  display: 'flex',
                                  alignItems: 'center',
                                  justifyContent: 'center',
                                }}
                              >
                                <Typography variant="caption" sx={{ color: '#fff', fontWeight: 600, fontSize: 10 }}>
                                  {event.allocationPercentage}%
                                </Typography>
                              </Box>
                            )}
                          </td>
                        );
                      })}
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </Box>
        </Stack>
      </Paper>
    </PageContainer>
  );
}
