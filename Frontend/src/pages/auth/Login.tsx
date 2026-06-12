import { yupResolver } from '@hookform/resolvers/yup';
import BusinessCenterOutlinedIcon from '@mui/icons-material/BusinessCenterOutlined';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Checkbox,
  FormControlLabel,
  IconButton,
  InputAdornment,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { AxiosError } from 'axios';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import * as yup from 'yup';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { login, setLoading } from '../../features/auth/authSlice';
import { authenticate } from '../../features/auth/authService';
import type { LoginCredentials } from '../../features/auth/authTypes';

const schema: yup.ObjectSchema<LoginCredentials> = yup.object({
  email: yup.string().required('Email is required').email('Enter a valid email address'),
  password: yup.string().required('Password is required'),
  rememberMe: yup.boolean().required(),
});

export default function Login() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, loading } = useAppSelector((state) => state.auth);
  const [showPassword, setShowPassword] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const from = (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? '/dashboard';

  const {
    control,
    formState: { errors },
    handleSubmit,
  } = useForm<LoginCredentials>({
    defaultValues: {
      email: '',
      password: '',
      rememberMe: false,
    },
    mode: 'onBlur',
    resolver: yupResolver(schema),
  });

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  const onSubmit = async (values: LoginCredentials) => {
    setSubmitError(null);
    dispatch(setLoading(true));

    try {
      const response = await authenticate(values);
      dispatch(login(response));
      navigate(from, { replace: true });
    } catch (error) {
      const message =
        error instanceof AxiosError
          ? error.response?.data?.message ?? error.message
          : 'Unable to sign in. Please check your credentials and try again.';
      setSubmitError(message);
      dispatch(setLoading(false));
    }
  };

  return (
    <Box
      alignItems="center"
      display="flex"
      justifyContent="center"
      minHeight="100vh"
      px={2}
      py={4}
      sx={{ backgroundColor: 'background.default' }}
    >
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', maxWidth: 440, width: '100%' }}>
        <CardContent sx={{ p: { xs: 3, sm: 4 } }}>
          <Stack alignItems="center" spacing={2.5}>
            <Box
              alignItems="center"
              bgcolor="primary.main"
              borderRadius={2}
              color="primary.contrastText"
              display="flex"
              height={64}
              justifyContent="center"
              width={64}
            >
              <BusinessCenterOutlinedIcon fontSize="large" />
            </Box>
            <Box textAlign="center">
              <Typography component="h1" fontWeight={800} variant="h4">
                HRMS Login
              </Typography>
              <Typography color="text.secondary" mt={0.75} variant="body2">
                Sign in to Resource Management
              </Typography>
            </Box>
          </Stack>

          <Box component="form" mt={4} noValidate onSubmit={handleSubmit(onSubmit)}>
            <Stack spacing={2.25}>
              {submitError ? <Alert severity="error">{submitError}</Alert> : null}
              <Controller
                control={control}
                name="email"
                render={({ field }) => (
                  <TextField
                    {...field}
                    autoComplete="email"
                    error={Boolean(errors.email)}
                    fullWidth
                    helperText={errors.email?.message}
                    label="Email"
                    type="email"
                  />
                )}
              />
              <Controller
                control={control}
                name="password"
                render={({ field }) => (
                  <TextField
                    {...field}
                    autoComplete="current-password"
                    error={Boolean(errors.password)}
                    fullWidth
                    helperText={errors.password?.message}
                    label="Password"
                    type={showPassword ? 'text' : 'password'}
                    InputProps={{
                      endAdornment: (
                        <InputAdornment position="end">
                          <IconButton
                            aria-label={showPassword ? 'Hide password' : 'Show password'}
                            edge="end"
                            onClick={() => setShowPassword((current) => !current)}
                          >
                            {showPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
                          </IconButton>
                        </InputAdornment>
                      ),
                    }}
                  />
                )}
              />
              <Controller
                control={control}
                name="rememberMe"
                render={({ field }) => (
                  <FormControlLabel
                    control={<Checkbox checked={field.value} onBlur={field.onBlur} onChange={field.onChange} />}
                    label="Remember me"
                  />
                )}
              />
              <Button disabled={loading} fullWidth size="large" type="submit" variant="contained">
                {loading ? 'Signing in...' : 'Login'}
              </Button>
            </Stack>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
