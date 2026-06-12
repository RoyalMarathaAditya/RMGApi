import CalendarTodayOutlinedIcon from '@mui/icons-material/CalendarTodayOutlined';
import LocationOnOutlinedIcon from '@mui/icons-material/LocationOnOutlined';
import QrCode2OutlinedIcon from '@mui/icons-material/QrCode2Outlined';
import WorkOutlineOutlinedIcon from '@mui/icons-material/WorkOutlineOutlined';
import { Avatar, Box, Button, Chip, Divider, Grid, List, ListItem, ListItemAvatar, ListItemText, Paper, Stack, Typography } from '@mui/material';

const profileCompletion = 92;

const teamMembers = [
  { name: 'Seema Waman Karandikar', status: 'On Leave' },
  { name: 'Ankit Vishwakarma', status: 'Active' },
  { name: 'Vanguri Sesha Venkata Ramakrishna Santosh', status: 'Active' },
];

const quickStats = [
  { label: 'Total Employees', value: '250', caption: 'Active this month' },
  { label: 'Projects Live', value: '45', caption: 'Ongoing deliveries' },
  { label: 'Bench Resources', value: '60', caption: 'Available for staffing' },
];

const suggestions = ['My Leave', 'My Attendance', 'New Non-CTC', 'Salary Letters', 'My Letters'];

const attendanceDetails = [
  { icon: CalendarTodayOutlinedIcon, label: 'Punch in', value: '10:00 AM' },
  { icon: CalendarTodayOutlinedIcon, label: 'Punch out', value: 'Not yet' },
  { icon: LocationOnOutlinedIcon, label: 'Location', value: 'Office' },
];

const wishes = [
  { name: 'Jitender Sharma', role: 'Senior Software Engineer', badge: 'Birthday' },
  { name: 'Nisha Patel', role: 'Product Designer', badge: 'Work Anniversary' },
];

const announcements = [
  { title: 'HR policy update', detail: 'New leave policy takes effect from 10 June.' },
  { title: 'Office closed', detail: 'In observance of public holiday on 8 June.' },
];

export default function Home() {
  return (
    <Stack spacing={3}>
      <Box>
        <Typography color="text.secondary" fontWeight={700} variant="body2">
          Home
        </Typography>
        <Typography component="h1" fontWeight={800} mt={0.5} variant="h4">
          Good Afternoon, Neha
        </Typography>
      </Box>

      <Grid container spacing={3}>
        <Grid item xs={12} lg={7}>
          <Grid container spacing={3}>
            <Grid item xs={12} md={6}>
              <Paper
                elevation={0}
                sx={{
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  p: 3,
                  height: '100%',
                }}
              >
                <Stack spacing={2}>
                  <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" alignItems="flex-start" spacing={2}>
                    <Box>
                      <Typography color="text.secondary" variant="body2">
                        Your Profile & Team
                      </Typography>
                      <Typography fontWeight={800} variant="h5">
                        {profileCompletion}% PROFILE COMPLETED
                      </Typography>
                    </Box>
                    <Box
                      sx={{
                        bgcolor: 'primary.main',
                        borderRadius: 2,
                        color: 'primary.contrastText',
                        px: 2,
                        py: 1,
                        fontWeight: 800,
                        textAlign: 'center',
                        minWidth: 88,
                      }}
                    >
                      {profileCompletion}%
                    </Box>
                  </Stack>

                  <Stack spacing={1}>
                    {teamMembers.map((member) => (
                      <Paper
                        key={member.name}
                        elevation={0}
                        sx={{
                          bgcolor: 'background.default',
                          borderRadius: 2,
                          p: 2,
                        }}
                      >
                        <Stack direction="row" justifyContent="space-between" alignItems="center" spacing={2}>
                          <Typography noWrap variant="body2" sx={{ minWidth: 0 }}>
                            {member.name}
                          </Typography>
                          <Chip
                            label={member.status}
                            size="small"
                            color={member.status === 'On Leave' ? 'warning' : 'success'}
                            variant="outlined"
                          />
                        </Stack>
                      </Paper>
                    ))}
                  </Stack>

                  <Box mt={1}>
                    <Button disableElevation size="small" variant="contained">
                      View All
                    </Button>
                  </Box>
                </Stack>
              </Paper>
            </Grid>

            <Grid item xs={12} md={6}>
              <Paper
                elevation={0}
                sx={{
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  p: 3,
                  height: '100%',
                }}
              >
                <Stack spacing={2}>
                  <Stack direction={{ xs: 'column', sm: 'row' }} justifyContent="space-between" alignItems="flex-start" spacing={2}>
                    <Box>
                      <Typography color="text.secondary" variant="body2">
                        Attendance
                      </Typography>
                      <Typography fontWeight={800} variant="h5">
                        Today's Shift
                      </Typography>
                    </Box>
                    <Button disableElevation variant="contained" size="small">
                      Punch out
                    </Button>
                  </Stack>

                  <Stack spacing={1}>
                    {attendanceDetails.map((item) => (
                      <Stack key={item.label} direction="row" spacing={1} alignItems="center">
                        <item.icon color={item.value === 'Not yet' ? 'disabled' : 'primary'} />
                        <Typography variant="body2">{item.label}: </Typography>
                        <Typography fontWeight={700} variant="body2">
                          {item.value}
                        </Typography>
                      </Stack>
                    ))}
                    <Divider sx={{ my: 1 }} />
                    <Grid container spacing={1}>
                      <Grid item xs={12} sm={6}>
                        <Paper elevation={0} sx={{ bgcolor: 'background.default', borderRadius: 2, p: 2 }}>
                          <Typography color="text.secondary" variant="caption">
                            Shift name
                          </Typography>
                          <Typography fontWeight={700} variant="subtitle2">
                            Flexi Shift NV
                          </Typography>
                        </Paper>
                      </Grid>
                      <Grid item xs={12} sm={6}>
                        <Paper elevation={0} sx={{ bgcolor: 'background.default', borderRadius: 2, p: 2 }}>
                          <Typography color="text.secondary" variant="caption">
                            Shift premises
                          </Typography>
                          <Typography fontWeight={700} variant="subtitle2">
                            Office
                          </Typography>
                        </Paper>
                      </Grid>
                      <Grid item xs={12} sm={6}>
                        <Paper elevation={0} sx={{ bgcolor: 'background.default', borderRadius: 2, p: 2 }}>
                          <Typography color="text.secondary" variant="caption">
                            Last punch
                          </Typography>
                          <Typography fontWeight={700} variant="subtitle2">
                            10:00 AM
                          </Typography>
                        </Paper>
                      </Grid>
                      <Grid item xs={12} sm={6}>
                        <Paper elevation={0} sx={{ bgcolor: 'background.default', borderRadius: 2, p: 2 }}>
                          <Typography color="text.secondary" variant="caption">
                            Working from
                          </Typography>
                          <Typography fontWeight={700} variant="subtitle2">
                            Work From Home
                          </Typography>
                        </Paper>
                      </Grid>
                    </Grid>
                  </Stack>
                </Stack>
              </Paper>
            </Grid>

            <Grid item xs={12}>
              <Paper
                elevation={0}
                sx={{
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  p: 3,
                }}
              >
                <Stack spacing={2}>
                  <Typography fontWeight={700} variant="h6">
                    Suggested for you
                  </Typography>
                  <Stack direction="row" flexWrap="wrap" gap={1}>
                    {suggestions.map((suggestion) => (
                      <Chip key={suggestion} label={suggestion} />
                    ))}
                  </Stack>
                </Stack>
              </Paper>
            </Grid>

            <Grid item xs={12} md={6}>
              <Paper
                elevation={0}
                sx={{
                  border: '1px solid',
                  borderColor: 'divider',
                  borderRadius: 2,
                  p: 3,
                }}
              >
                <Stack spacing={2}>
                  <Stack direction="row" spacing={2} alignItems="center">
                    <QrCode2OutlinedIcon color="primary" />
                    <Typography fontWeight={700} variant="h6">
                      Download our app
                    </Typography>
                  </Stack>
                  <Paper
                    elevation={0}
                    sx={{
                      border: '1px dashed',
                      borderColor: 'divider',
                      borderRadius: 2,
                      height: 180,
                      display: 'grid',
                      placeItems: 'center',
                      bgcolor: 'background.default',
                    }}
                  >
                    <Box sx={{ textAlign: 'center' }}>
                      <Typography fontWeight={700}>QR Code</Typography>
                      <Typography color="text.secondary" variant="body2">
                        Scan to download
                      </Typography>
                    </Box>
                  </Paper>
                  <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1}>
                    <Button fullWidth disableElevation size="small" variant="outlined">
                      Play Store
                    </Button>
                    <Button fullWidth disableElevation size="small" variant="outlined">
                      App Store
                    </Button>
                  </Stack>
                </Stack>
              </Paper>
            </Grid>

            <Grid item xs={12} md={6}>
              <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
                <Stack spacing={2}>
                  <Stack direction="row" spacing={2} alignItems="center">
                    <Box sx={{ bgcolor: 'primary.main', borderRadius: 2, color: 'primary.contrastText', p: 1.25 }}>
                      <WorkOutlineOutlinedIcon />
                    </Box>
                    <Typography fontWeight={700} variant="h6">
                      Wishes
                    </Typography>
                  </Stack>
                  <List disablePadding>
                    {wishes.map((wish) => (
                      <ListItem disableGutters key={wish.name} sx={{ mb: 1, bgcolor: 'background.default', borderRadius: 2, p: 1 }}>
                        <ListItemAvatar>
                          <Avatar sx={{ bgcolor: 'secondary.main' }}>{wish.name.charAt(0)}</Avatar>
                        </ListItemAvatar>
                        <ListItemText
                          primary={wish.name}
                          primaryTypographyProps={{ fontWeight: 700 }}
                          secondary={wish.role}
                        />
                        <Chip label={wish.badge} size="small" color="secondary" />
                      </ListItem>
                    ))}
                  </List>
                </Stack>
              </Paper>
            </Grid>
          </Grid>
        </Grid>

        <Grid item xs={12} lg={5}>
          <Grid container spacing={3}>
            {quickStats.map((stat) => (
              <Grid item xs={12} sm={6} key={stat.label}>
                <Paper
                  elevation={0}
                  sx={{
                    border: '1px solid',
                    borderColor: 'divider',
                    borderRadius: 2,
                    p: 3,
                    height: '100%',
                  }}
                >
                  <Typography color="text.secondary" variant="caption">
                    {stat.label}
                  </Typography>
                  <Typography fontWeight={800} mt={0.5} variant="h4">
                    {stat.value}
                  </Typography>
                  <Typography color="text.secondary" variant="body2">
                    {stat.caption}
                  </Typography>
                </Paper>
              </Grid>
            ))}

            <Grid item xs={12}>
              <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
                <Stack spacing={2}>
                  <Stack direction="row" justifyContent="space-between" alignItems="center" flexWrap="wrap" spacing={2}>
                    <Box>
                      <Typography color="text.secondary" variant="body2">
                        Announcements
                      </Typography>
                      <Typography fontWeight={700} variant="h5">
                        Latest updates
                      </Typography>
                    </Box>
                    <Button disableElevation size="small" variant="contained">
                      View all
                    </Button>
                  </Stack>
                  <Stack spacing={1}>
                    {announcements.map((announcement) => (
                      <Paper key={announcement.title} elevation={0} sx={{ bgcolor: 'background.default', borderRadius: 2, p: 2 }}>
                        <Typography fontWeight={700} variant="subtitle2">
                          {announcement.title}
                        </Typography>
                        <Typography color="text.secondary" variant="body2">
                          {announcement.detail}
                        </Typography>
                      </Paper>
                    ))}
                  </Stack>
                </Stack>
              </Paper>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </Stack>
  );
}
