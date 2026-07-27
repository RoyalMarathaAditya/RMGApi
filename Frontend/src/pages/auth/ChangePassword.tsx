import { yupResolver } from '@hookform/resolvers/yup';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  IconButton,
  InputAdornment,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { AxiosError } from 'axios';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import * as yup from 'yup';
import api from '../../services/api';
import { useAppDispatch, useAppSelector } from '../../redux/hooks';
import { logout } from '../../redux/slices/authSlice';

interface ChangePasswordForm {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}

const schema: yup.ObjectSchema<ChangePasswordForm> = yup.object({
  currentPassword: yup.string().required('Current password is required'),
  newPassword: yup
    .string()
    .required('New password is required')
    .min(8, 'Password must be at least 8 characters')
    .matches(/[A-Z]/, 'Password must contain an uppercase letter')
    .matches(/[a-z]/, 'Password must contain a lowercase letter')
    .matches(/[0-9]/, 'Password must contain a number')
    .matches(/[^A-Za-z0-9]/, 'Password must contain a special character')
    .test('not-default', 'New password cannot be the default password', (val) => val !== 'NV@12345#'),
  confirmPassword: yup
    .string()
    .required('Confirm password is required')
    .oneOf([yup.ref('newPassword')], 'Passwords must match'),
});

export default function ChangePassword() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const forcePasswordChange = useAppSelector((state) => state.auth.forcePasswordChange);
  const [showCurrent, setShowCurrent] = useState(false);
  const [showNew, setShowNew] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);

  const {
    control,
    formState: { errors },
    handleSubmit,
  } = useForm<ChangePasswordForm>({
    defaultValues: {
      currentPassword: '',
      newPassword: '',
      confirmPassword: '',
    },
    mode: 'onBlur',
    resolver: yupResolver(schema),
  });

  const onSubmit = async (values: ChangePasswordForm) => {
    setSubmitError(null);
    setIsSubmitting(true);

    try {
      await api.post('/auth/change-password', {
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
        confirmPassword: values.confirmPassword,
      });

      localStorage.removeItem('hrms_auth_token');
      localStorage.removeItem('hrms_auth_refresh_token');
      sessionStorage.clear();

      dispatch(logout());

      navigate('/login', {
        state: { passwordChanged: true },
        replace: true,
      });
    } catch (error) {
      const message =
        error instanceof AxiosError
          ? error.response?.data?.message ?? error.message
          : 'Failed to change password. Please try again.';
      setSubmitError(message);
      setIsSubmitting(false);
    }
  };

  const handleLogout = () => {
    api.post('/auth/logout').catch(() => {});
    dispatch(logout());
    navigate('/login', { replace: true });
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
            <Box textAlign="center">
              <Typography component="h1" fontWeight={800} variant="h5">
                Change Password
              </Typography>
              <Typography color="text.secondary" mt={0.75} variant="body2">
                {forcePasswordChange
                  ? 'You must change your password before accessing the dashboard.'
                  : 'Update your account password.'}
              </Typography>
            </Box>
          </Stack>

          <Box component="form" mt={4} noValidate onSubmit={handleSubmit(onSubmit)}>
            <Stack spacing={2.25}>
              {submitError ? <Alert severity="error">{submitError}</Alert> : null}

              <Controller
                control={control}
                name="currentPassword"
                render={({ field }) => (
                  <TextField
                    {...field}
                    autoComplete="current-password"
                    error={Boolean(errors.currentPassword)}
                    fullWidth
                    helperText={errors.currentPassword?.message}
                    label="Current Password"
                    type={showCurrent ? 'text' : 'password'}
                    slotProps={{
                      input: {
                        endAdornment: (
                          <InputAdornment position="end">
                            <IconButton
                              aria-label={showCurrent ? 'Hide password' : 'Show password'}
                              edge="end"
                              onClick={() => setShowCurrent((c) => !c)}
                            >
                              {showCurrent ? <VisibilityOffIcon /> : <VisibilityIcon />}
                            </IconButton>
                          </InputAdornment>
                        ),
                      },
                    }}
                  />
                )}
              />

              <Controller
                control={control}
                name="newPassword"
                render={({ field }) => (
                  <TextField
                    {...field}
                    autoComplete="new-password"
                    error={Boolean(errors.newPassword)}
                    fullWidth
                    helperText={errors.newPassword?.message || 'Min 8 chars, uppercase, lowercase, number, special char'}
                    label="New Password"
                    type={showNew ? 'text' : 'password'}
                    slotProps={{
                      input: {
                        endAdornment: (
                          <InputAdornment position="end">
                            <IconButton
                              aria-label={showNew ? 'Hide password' : 'Show password'}
                              edge="end"
                              onClick={() => setShowNew((c) => !c)}
                            >
                              {showNew ? <VisibilityOffIcon /> : <VisibilityIcon />}
                            </IconButton>
                          </InputAdornment>
                        ),
                      },
                    }}
                  />
                )}
              />

              <Controller
                control={control}
                name="confirmPassword"
                render={({ field }) => (
                  <TextField
                    {...field}
                    autoComplete="new-password"
                    error={Boolean(errors.confirmPassword)}
                    fullWidth
                    helperText={errors.confirmPassword?.message}
                    label="Confirm Password"
                    type={showConfirm ? 'text' : 'password'}
                    slotProps={{
                      input: {
                        endAdornment: (
                          <InputAdornment position="end">
                            <IconButton
                              aria-label={showConfirm ? 'Hide password' : 'Show password'}
                              edge="end"
                              onClick={() => setShowConfirm((c) => !c)}
                            >
                              {showConfirm ? <VisibilityOffIcon /> : <VisibilityIcon />}
                            </IconButton>
                          </InputAdornment>
                        ),
                      },
                    }}
                  />
                )}
              />

              <Button disabled={isSubmitting} fullWidth size="large" type="submit" variant="contained">
                {isSubmitting ? 'Changing...' : 'Change Password'}
              </Button>

              {forcePasswordChange && (
                <Button color="error" fullWidth onClick={handleLogout} variant="text">
                  Logout
                </Button>
              )}
            </Stack>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}
