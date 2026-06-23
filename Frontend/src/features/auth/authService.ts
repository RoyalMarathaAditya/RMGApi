import api from '../../services/api';
import type { LoginCredentials, LoginResponse } from './authTypes';

const TOKEN_KEY = 'hrms_auth_token';
const REFRESH_TOKEN_KEY = 'hrms_auth_refresh_token';

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

export function getStoredRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function clearStoredTokens(): void {
  localStorage.removeItem(TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
}

export async function authenticate(credentials: LoginCredentials): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>('/auth/login', {
    email: credentials.email,
    password: credentials.password,
  });

  localStorage.setItem(TOKEN_KEY, response.data.token);
  localStorage.setItem(REFRESH_TOKEN_KEY, response.data.refreshToken);

  return response.data;
}

export async function refreshTokens(): Promise<LoginResponse | null> {
  const refreshToken = getStoredRefreshToken();
  if (!refreshToken) return null;

  try {
    const response = await api.post<LoginResponse>('/auth/refresh-token', { refreshToken });
    localStorage.setItem(TOKEN_KEY, response.data.token);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.data.refreshToken);
    return response.data;
  } catch {
    clearStoredTokens();
    return null;
  }
}
