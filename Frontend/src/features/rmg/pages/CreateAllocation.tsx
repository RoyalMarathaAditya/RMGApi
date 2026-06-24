import 'bootstrap/dist/css/bootstrap.min.css';
import { useEffect, useState } from 'react';
import {
  Autocomplete,
  Box,
  Button,
  Card,
  CardContent,
  MenuItem,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import InputAdornment from '@mui/material/InputAdornment';
import PercentOutlinedIcon from '@mui/icons-material/PercentOutlined';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { useNavigate } from 'react-router-dom';
import * as yup from 'yup';
import { useAppDispatch, useAppSelector } from '../../../redux/hooks';
import { fetchEmployees } from '../../../redux/slices/employeeSlice';
import { allocationService } from '../services/allocationService';
import type { CreateAllocationDto } from '../types/allocation';

const ALLOCATION_TYPES = ['Full Time', 'Part Time', 'Contract', 'Sow'] as const;
const BILLABLE_STATUSES = ['Billable', 'Non-Billable', 'Shadow'] as const;

const schema = yup.object({
  employeeId: yup.number().required('Employee is required').positive('Select a valid employee'),
  employeeName: yup.string(),
  projectId: yup.number().required('Project is required').positive('Select a valid project'),
  projectName: yup.string(),
  allocationPercentage: yup.number().required('Allocation % is required').min(1, 'Minimum 1%').max(100, 'Maximum 100%'),
  allocationType: yup.string().required('Allocation type is required').oneOf(ALLOCATION_TYPES),
  billableStatus: yup.string().required('Billable status is required').oneOf(BILLABLE_STATUSES),
  startDate: yup.string().required('Start date is required'),
  endDate: yup.string().nullable(),
  reportingManagerId: yup.number().nullable(),
  reportingManagerName: yup.string().nullable(),
  notes: yup.string().nullable(),
});

type FormValues = yup.InferType<typeof schema>;

export default function CreateAllocation() {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const employees = useAppSelector((state) => state.employees.employees);
  const projects = useAppSelector((state) => state.projects.projects);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (employees.length === 0) {
      dispatch(fetchEmployees());
    }
  }, [dispatch, employees.length]);

  const reportingManagers = employees.filter(
    (e) => e.reportingManagerId || e.designation?.toLowerCase().includes('manager') || e.designation?.toLowerCase().includes('lead'),
  );

  const { control, handleSubmit, formState: { errors }, watch, setValue } = useForm<FormValues>({
    defaultValues: {
      employeeId: undefined,
      employeeName: '',
      projectId: undefined,
      projectName: '',
      startDate: '',
      endDate: null,
      allocationPercentage: 100,
      allocationType: 'Full Time',
      billableStatus: 'Billable',
      reportingManagerId: null,
      reportingManagerName: null,
      notes: '',
    },
    resolver: yupResolver(schema),
    mode: 'onBlur',
  });

  const selectedEmployeeId = watch('employeeId');

  useEffect(() => {
    if (selectedEmployeeId && employees.length > 0) {
      const emp = employees.find((e) => e.id === selectedEmployeeId);
      if (emp?.reportingManagerId) {
        setValue('reportingManagerId', emp.reportingManagerId);
        setValue('reportingManagerName', emp.reportingManagerName ?? null);
      }
    }
  }, [selectedEmployeeId, employees, setValue]);

  const onSubmit = async (values: FormValues) => {
    setSubmitting(true);
    try {
      const payload: CreateAllocationDto = {
        employeeId: values.employeeId,
        projectId: values.projectId,
        startDate: values.startDate,
        endDate: values.endDate || null,
        allocationPercentage: values.allocationPercentage,
        allocationStatus: 'Active',
        allocationType: values.allocationType,
        billableStatus: values.billableStatus,
        reportingManagerId: values.reportingManagerId || null,
        notes: values.notes || '',
      };
      await allocationService.create(payload);
      navigate('/rmg');
    } catch {
      navigate('/rmg');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Box sx={{ px: { xs: 2, md: 4 }, py: { xs: 2, md: 3 } }}>
      <Typography component="h1" fontWeight={800} variant="h4" sx={{ mb: 3 }}>
        Allocate Resource
      </Typography>

      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
        <Box sx={{ px: { xs: 3, sm: 4 }, py: 2, borderBottom: '1px solid', borderColor: 'divider', bgcolor: 'grey.50' }}>
          <Typography fontWeight={700} variant="subtitle1">Resource Allocation Details</Typography>
        </Box>
        <CardContent sx={{ p: { xs: 3, sm: 4 } }}>
          <div className="row g-4">
            <div className="col-lg-6 col-md-6 col-sm-12">
              <Stack spacing={3}>
                <Controller
                  name="employeeId"
                  control={control}
                  render={({ field }) => {
                    const selected = employees.find((e) => e.id === field.value) || null;
                    return (
                      <Autocomplete
                        options={employees}
                        getOptionLabel={(option) => `${option.fullName} (${option.employeeCode})`}
                        isOptionEqualToValue={(option, value) => option.id === value.id}
                        value={selected}
                        disableClearable={false}
                        onChange={(_, value) => {
                          field.onChange(value?.id ?? undefined);
                          setValue('employeeName', value?.fullName ?? '');
                          if (value?.reportingManagerId) {
                            setValue('reportingManagerId', value.reportingManagerId);
                            setValue('reportingManagerName', value.reportingManagerName ?? null);
                          } else {
                            setValue('reportingManagerId', null);
                            setValue('reportingManagerName', null);
                          }
                        }}
                        fullWidth
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Employee *"
                            placeholder="Search employee by name or code"
                            error={!!errors.employeeId}
                            helperText={errors.employeeId?.message}
                          />
                        )}
                      />
                    );
                  }}
                />

                <Controller
                  name="projectId"
                  control={control}
                  render={({ field }) => {
                    const selected = projects.find((p) => p.id === field.value) || null;
                    return (
                      <Autocomplete
                        options={projects}
                        getOptionLabel={(option) => `${option.projectName} (${option.projectCode})`}
                        isOptionEqualToValue={(option, value) => option.id === value.id}
                        value={selected}
                        disableClearable={false}
                        onChange={(_, value) => {
                          field.onChange(value?.id ?? undefined);
                          setValue('projectName', value?.projectName ?? '');
                        }}
                        fullWidth
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Project *"
                            placeholder="Search project by name or code"
                            error={!!errors.projectId}
                            helperText={errors.projectId?.message}
                          />
                        )}
                      />
                    );
                  }}
                />

                <Controller
                  name="allocationPercentage"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Allocation % *"
                      type="number"
                      error={!!errors.allocationPercentage}
                      helperText={errors.allocationPercentage?.message}
                      fullWidth
                      inputProps={{ min: 1, max: 100 }}
                      slotProps={{
                        input: {
                          endAdornment: (
                            <InputAdornment position="end">
                              <PercentOutlinedIcon fontSize="small" sx={{ color: 'text.secondary' }} />
                            </InputAdornment>
                          ),
                        },
                      }}
                    />
                  )}
                />

                <Controller
                  name="allocationType"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Allocation Type *"
                      select
                      fullWidth
                      error={!!errors.allocationType}
                      helperText={errors.allocationType?.message}
                    >
                      {ALLOCATION_TYPES.map((t) => (
                        <MenuItem key={t} value={t}>{t}</MenuItem>
                      ))}
                    </TextField>
                  )}
                />

                <Controller
                  name="billableStatus"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Billable Status *"
                      select
                      fullWidth
                      error={!!errors.billableStatus}
                      helperText={errors.billableStatus?.message}
                    >
                      {BILLABLE_STATUSES.map((s) => (
                        <MenuItem key={s} value={s}>{s}</MenuItem>
                      ))}
                    </TextField>
                  )}
                />
              </Stack>
            </div>

            <div className="col-lg-6 col-md-6 col-sm-12">
              <Stack spacing={3}>
                <Controller
                  name="startDate"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Start Date *"
                      type="date"
                      fullWidth
                      error={!!errors.startDate}
                      helperText={errors.startDate?.message}
                      slotProps={{ inputLabel: { shrink: true } }}
                    />
                  )}
                />

                <Controller
                  name="endDate"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="End Date"
                      type="date"
                      fullWidth
                      value={field.value ?? ''}
                      onChange={(e) => field.onChange(e.target.value || null)}
                      slotProps={{ inputLabel: { shrink: true } }}
                    />
                  )}
                />

                <Controller
                  name="reportingManagerId"
                  control={control}
                  render={({ field }) => {
                    const selected = reportingManagers.find((e) => e.id === field.value) || null;
                    return (
                      <Autocomplete
                        options={reportingManagers}
                        getOptionLabel={(option) => `${option.fullName} (${option.employeeCode})`}
                        isOptionEqualToValue={(option, value) => option.id === value.id}
                        value={selected}
                        disableClearable={false}
                        onChange={(_, value) => {
                          field.onChange(value?.id ?? null);
                          setValue('reportingManagerName', value?.fullName ?? null);
                        }}
                        fullWidth
                        renderInput={(params) => (
                          <TextField
                            {...params}
                            label="Reporting Manager"
                            placeholder="Select reporting manager"
                          />
                        )}
                      />
                    );
                  }}
                />

                <Controller
                  name="notes"
                  control={control}
                  render={({ field }) => (
                    <TextField
                      {...field}
                      label="Remarks"
                      multiline
                      rows={6}
                      fullWidth
                      placeholder="Enter any additional remarks (optional)"
                      value={field.value ?? ''}
                    />
                  )}
                />
              </Stack>
            </div>
          </div>

          <Stack direction="row" spacing={2} justifyContent="flex-end" sx={{ mt: 4.5, pt: 3 }}>
            <Button
              variant="outlined"
              onClick={() => navigate('/rmg')}
              disabled={submitting}
              size="large"
              sx={{ px: 4 }}
            >
              Cancel
            </Button>
            <Button
              type="submit"
              variant="contained"
              disabled={submitting}
              startIcon={<SaveOutlinedIcon />}
              size="large"
              sx={{ px: 4 }}
            >
              {submitting ? 'Saving...' : 'Create Allocation'}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
}
