import { createTheme } from '@mui/material/styles';

export const theme = createTheme({
  palette: {
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#9c27b0',
    },
    background: {
      default: '#f5f7fb',
      paper: '#ffffff',
    },
  },
  typography: {
    fontFamily: ['Inter', 'Roboto', 'Arial', 'sans-serif'].join(','),
    h4: {
      fontSize: '1.75rem',
      fontWeight: 700,
      lineHeight: 1.25,
    },
    h5: {
      fontSize: '1.25rem',
      fontWeight: 700,
    },
    button: {
      fontWeight: 700,
      textTransform: 'none',
    },
  },
  shape: {
    borderRadius: 8,
  },
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          boxShadow: 'none',
        },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 8,
        },
      },
    },
  },
});
