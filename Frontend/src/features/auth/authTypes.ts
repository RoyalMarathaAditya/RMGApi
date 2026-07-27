export interface AuthUser {
  id: string;
  name: string;
  email: string;
  roleId: string;
  roleName: string;
}

export interface AuthState {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  forcePasswordChange: boolean;
}

export interface LoginCredentials {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: AuthUser;
  forcePasswordChange: boolean;
}
