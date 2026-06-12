import { Box, CircularProgress } from '@mui/material';

export default function Loader() {
  return (
    <Box alignItems="center" display="flex" justifyContent="center" minHeight={240}>
      <CircularProgress />
    </Box>
  );
}
