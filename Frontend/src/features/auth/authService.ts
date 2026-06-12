import api, { setAuthToken } from '../../services/api';
import type { LoginCredentials, LoginResponse } from './authTypes';

export async function authenticate(credentials: LoginCredentials): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>('/auth/login', {
    email: credentials.email,
    password: credentials.password,
  });

  setAuthToken(response.data.token);
  return response.data;
}
