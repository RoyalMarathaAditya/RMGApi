import { CssBaseline, GlobalStyles, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import React from 'react';
import ReactDOM from 'react-dom/client';
// Redux: Provider makes the Redux store available to all components
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
            }}
          />
          <BrowserRouter>
            <App />
          </BrowserRouter>
          <Toaster
            position="top-right"
            toastOptions={{
              duration: 4000,
              style: {
                borderRadius: '10px',
                background: '#333',
                color: '#fff',
                fontSize: '14px',
                fontWeight: 500,
              },
              success: {
                iconTheme: { primary: '#059669', secondary: '#fff' },
                style: { background: '#ECFDF5', color: '#065F46', border: '1px solid #A7F3D0' },
              },
              error: {
                iconTheme: { primary: '#DC2626', secondary: '#fff' },
                style: { background: '#FEF2F2', color: '#991B1B', border: '1px solid #FECACA' },
                duration: 5000,
              },
            }}
          />
        </ThemeProvider>
      </QueryClientProvider>
    </Provider>
  </React.StrictMode>,
);
