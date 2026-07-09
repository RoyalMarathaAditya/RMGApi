import { Box, CircularProgress, Typography } from '@mui/material';

interface PageLoaderProps {
  message?: string;
}

export default function PageLoader({ message = 'Please wait...' }: PageLoaderProps) {
  return (
    <Box
      alignItems="center"
      display="flex"
      flexDirection="column"
      justifyContent="center"
      minHeight="60vh"
      sx={{
        width: '100%',
        animation: 'fadeInPageLoader 0.4s ease-in',
        '@keyframes fadeInPageLoader': {
          '0%': { opacity: 0 },
          '100%': { opacity: 1 },
        },
      }}
    >
      <CircularProgress size={48} sx={{ mb: 2 }} />
      <Typography color="text.secondary" variant="body1">
        {message}
      </Typography>
    </Box>
  );
}
