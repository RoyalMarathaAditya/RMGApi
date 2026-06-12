import axios, { AxiosError } from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('hrms_auth_token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('hrms_auth_token');
    }
    return Promise.reject(error);
  },
);

export function setAuthToken(token: string | null) {
  if (token) {
    localStorage.setItem('hrms_auth_token', token);
  } else {
    localStorage.removeItem('hrms_auth_token');
  }
}

export default api;
