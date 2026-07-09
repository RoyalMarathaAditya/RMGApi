import { CssBaseline, GlobalStyles, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import App from './App';
import { store } from './redux/store';
import { theme } from './theme/theme';

const queryClient = new QueryClient();

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
  <React.StrictMode>
    <Provider store={store}>
      <QueryClientProvider client={queryClient}>
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <GlobalStyles
            styles={{
              body: {
                minHeight: '100vh',
                backgroundColor: '#f5f7fb',
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
            }}
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
                boxShadow: '0 8px 32px rgba(0,0,0,0.12), 0 2px 8px rgba(0,0,0,0.06)',
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
      </QueryClientProvider>
    </Provider>
  </React.StrictMode>,
);
