import { yupResolver } from '@hookform/resolvers/yup';
import { Autocomplete, Button, Checkbox, FormControl, FormControlLabel, FormHelperText, InputLabel, MenuItem, Select, Stack, TextField } from '@mui/material';
import { DesktopDatePicker, LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { Controller, useForm, type SubmitHandler } from 'react-hook-form';
import type { Employee } from '../../employees/types/employee';
import type { Project } from '../../projects/types/project.types';
import type { ResourceAllocationFormValues } from '../types/resourceAllocation';
import { allocationTypeOptions } from '../types/allocationType';
import { allocationValidationSchema } from '../validation/allocationValidation';

interface AllocationFormProps {
  employees: Employee[];
  projects: Project[];
  defaultValues: ResourceAllocationFormValues;
  onSubmit: (values: ResourceAllocationFormValues) => void;
}

export default function AllocationForm({ employees, projects, defaultValues, onSubmit }: AllocationFormProps) {
  const form = useForm<ResourceAllocationFormValues>({
    mode: 'onBlur',
    defaultValues,
    resolver: yupResolver(allocationValidationSchema) as any,
  });

  const onSubmitHandler: SubmitHandler<ResourceAllocationFormValues> = onSubmit;

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <form onSubmit={form.handleSubmit(onSubmitHandler)} noValidate>
        <Stack spacing={2}>
          <Controller
            name="employeeId"
            control={form.control}
            render={({ field, fieldState }) => (
              <FormControl fullWidth error={Boolean(fieldState.error)}>
                <Autocomplete
                  disablePortal
                  options={employees}
                  getOptionLabel={(option) => `${option.firstName} ${option.lastName}`}
                  value={employees.find((item) => item.id === field.value) ?? null}
                  onChange={(_, value) => field.onChange(value?.id ?? 0)}
                  renderInput={(params) => (
                    <TextField {...params} label="Employee" error={Boolean(fieldState.error)} />
                  )}
                />
                {fieldState.error ? <FormHelperText>{fieldState.error.message}</FormHelperText> : null}
              </FormControl>
            )}
          />

          <Controller
            name="projectId"
            control={form.control}
            render={({ field, fieldState }) => (
              <FormControl fullWidth error={Boolean(fieldState.error)}>
                <Autocomplete
                  disablePortal
                  options={projects}
                  getOptionLabel={(option) => option.projectName}
                  value={projects.find((item) => item.id === field.value) ?? null}
                  onChange={(_, value) => field.onChange(value?.id ?? 0)}
                  renderInput={(params) => (
                    <TextField {...params} label="Project" error={Boolean(fieldState.error)} />
                  )}
                />
                {fieldState.error ? <FormHelperText>{fieldState.error.message}</FormHelperText> : null}
              </FormControl>
            )}
          />

          <Controller
            name="allocationPercentage"
            control={form.control}
            render={({ field, fieldState }) => (
              <TextField
                {...field}
                fullWidth
                label="Allocation %"
                type="number"
                error={Boolean(fieldState.error)}
                helperText={fieldState.error?.message}
                inputProps={{ min: 1, max: 100 }}
              />
            )}
          />

          <Controller
            name="allocationType"
            control={form.control}
            render={({ field, fieldState }) => (
              <FormControl fullWidth error={Boolean(fieldState.error)}>
                <InputLabel id="allocation-type-label">Allocation Type</InputLabel>
                <Select
                  {...field}
                  labelId="allocation-type-label"
                  label="Allocation Type"
                  value={field.value}
                >
                  {allocationTypeOptions.map((option) => (
                    <MenuItem key={option.value} value={option.value}>
                      {option.label}
                    </MenuItem>
                  ))}
                </Select>
                {fieldState.error ? <FormHelperText>{fieldState.error.message}</FormHelperText> : null}
              </FormControl>
            )}
          />

          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
            <Controller
              name="startDate"
              control={form.control}
              render={({ field, fieldState }) => (
                <DesktopDatePicker
                  label="Start Date"
                  value={field.value ? new Date(field.value) : null}
                  onChange={(value) => field.onChange(value?.toISOString().slice(0, 10) ?? '')}
                  renderInput={(params) => (
                    <TextField {...params} fullWidth error={Boolean(fieldState.error)} helperText={fieldState.error?.message} />
                  )}
                />
              )}
            />
            <Controller
              name="endDate"
              control={form.control}
              render={({ field, fieldState }) => (
                <DesktopDatePicker
                  label="End Date"
                  value={field.value ? new Date(field.value) : null}
                  onChange={(value) => field.onChange(value?.toISOString().slice(0, 10) ?? '')}
                  renderInput={(params) => (
                    <TextField {...params} fullWidth error={Boolean(fieldState.error)} helperText={fieldState.error?.message} />
                  )}
                />
              )}
            />
          </Stack>

          <Controller
            name="isActive"
            control={form.control}
            render={({ field }) => (
              <FormControlLabel
                control={<Checkbox checked={field.value} onChange={(event) => field.onChange(event.target.checked)} />}
                label="Active allocation"
              />
            )}
          />

          <Button type="submit" variant="contained" fullWidth>
            Save allocation
          </Button>
        </Stack>
      </form>
    </LocalizationProvider>
  );
}
