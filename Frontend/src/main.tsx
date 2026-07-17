import { CssBaseline, GlobalStyles, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider, useSelector } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import App from './App';
import { store } from './redux/store';
import { selectThemeMode } from './redux/slices/themeSlice';
import { lightTheme, darkTheme } from './theme/theme';

const queryClient = new QueryClient();

function ThemedApp() {
  const mode = useSelector(selectThemeMode);
  const theme = mode === 'dark' ? darkTheme : lightTheme;

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <GlobalStyles
        styles={(theme) => ({
          body: {
            minHeight: '100vh',
            backgroundColor: theme.palette.background.default,
          },
          '#root': {
            minHeight: '100vh',
          },
          '@media (max-width: 600px)': {
            '.toast-container > div': {
              minWidth: '90vw !important',
              maxWidth: '90vw !important',
            },
          },
        })}
      />
      <BrowserRouter>
        <App />
      </BrowserRouter>
      <Toaster
        position="top-center"
        gutter={12}
        containerClassName="toast-container"
        toastOptions={{
          duration: 4000,
          style: {
            borderRadius: '10px',
            padding: '16px 24px',
            fontSize: '16px',
            fontWeight: 600,
            minWidth: '420px',
            maxWidth: '600px',
            width: 'auto',
            background: theme.palette.mode === 'dark' ? '#323232' : '#fff',
            color: theme.palette.mode === 'dark' ? '#fff' : '#333',
            boxShadow: theme.palette.mode === 'dark'
              ? '0 8px 32px rgba(0,0,0,0.4), 0 2px 8px rgba(0,0,0,0.2)'
              : '0 8px 32px rgba(0,0,0,0.12), 0 2px 8px rgba(0,0,0,0.06)',
          },
          success: {
            icon: '✓',
            style: { background: '#059669', color: '#fff' },
          },
          error: {
            icon: '✕',
            style: { background: '#DC2626', color: '#fff' },
            duration: 5000,
          },
        }}
      />
    </ThemeProvider>
  );
}

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <Provider store={store}>
      <QueryClientProvider client={queryClient}>
        <ThemedApp />
      </QueryClientProvider>
    </Provider>
  </React.StrictMode>,
);
