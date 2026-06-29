
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { Autocomplete, Box, Button, Checkbox, FormControlLabel, MenuItem, Stack, TextField } from '@mui/material';
import { useEffect, useState } from 'react';
import api from '../../../services/api';
import type { Employee, EmployeeFormValues } from '../types/employee';

interface MasterItem {
  id: string;
  name: string;
}


const schema = yup.object({
  employeeCode: yup.string().trim().required('Required').max(20, 'Max 20 characters'),
  fullName: yup.string().trim().required('Required').max(200, 'Max 200 characters'),
  email: yup.string().trim().email('Invalid email').required('Required'),
  doj: yup.string().required('Required'),
  priorExperience: yup.number().typeError('Enter a number').min(0).max(50).required('Required'),
  employmentTypeId: yup.string().required('Required'),
  locationId: yup.string().required('Required'),
  workModelId: yup.string().required('Required'),
  practiceId: yup.string().required('Required'),
  departmentTypeId: yup.string().required('Required'),
  designationId: yup.string().required('Required'),
  statusId: yup.string().required('Required'),
});

export default function EmployeeForm({
  employee,
  onCancel,
  onSubmit,
}: {
  employee?: Employee | null;
  onCancel: () => void;
  onSubmit: (values: EmployeeFormValues) => Promise<void> | void;
}) {
  const [employmentTypes, setEmploymentTypes] = useState<MasterItem[]>([]);
  const [locations, setLocations] = useState<MasterItem[]>([]);
  const [workModels, setWorkModels] = useState<MasterItem[]>([]);
  const [practices, setPractices] = useState<MasterItem[]>([]);
  const [departmentTypes, setDepartmentTypes] = useState<MasterItem[]>([]);
  const [designations, setDesignations] = useState<MasterItem[]>([]);
  const [statuses, setStatuses] = useState<MasterItem[]>([]);
  const [skills, setSkills] = useState<MasterItem[]>([]);

  const { control, handleSubmit, setValue, reset } = useForm<EmployeeFormValues>({
    defaultValues: {
      employeeCode: '',
      fullName: '',
      email: '',
      doj: '',
      lwd: null,
      priorExperience: 0,
      relevantExperience: null,
      employmentTypeId: '',
      locationId: '',
      workModelId: '',
      practiceId: '',
      departmentTypeId: '',
      designationId: '',
      statusId: '',
      reportingManagerName: null,
      practiceHeadName: null,
      deloitteFitment: null,
      engineering: null,
      mobileNumber: '',
      skillIds: [],
    },
    resolver: yupResolver(schema),
  });

  useEffect(() => {
    const apiGet = (path: string) => api.get(path).then((r) => r.data as MasterItem[]);
    apiGet('/master/employmenttypes').then(setEmploymentTypes);
    apiGet('/master/locations').then(setLocations);
    apiGet('/master/workmodels').then(setWorkModels);
    apiGet('/master/practices').then(setPractices);
    apiGet('/master/departmenttypes').then(setDepartmentTypes);
    apiGet('/master/designations').then(setDesignations);
    apiGet('/master/statuses').then(setStatuses);
    apiGet('/master/skills').then(setSkills);
  }, []);

  useEffect(() => {
    if (employee) {
      reset({
        employeeCode: employee.employeeCode,
        fullName: employee.fullName,
        email: employee.email,
        doj: employee.doj ?? '',
        lwd: employee.lwd ?? null,
        priorExperience: employee.priorExperience ?? 0,
        relevantExperience: employee.relevantExperience ?? null,
        employmentTypeId: employee.employmentTypeId,
        locationId: employee.locationId,
        workModelId: employee.workModelId,
        practiceId: employee.practiceId,
        departmentTypeId: employee.departmentTypeId,
        designationId: employee.designationId ?? '',
        statusId: employee.statusId,
        reportingManagerName: employee.reportingManagerName ?? null,
        practiceHeadName: employee.practiceHeadName ?? null,
        deloitteFitment: employee.deloitteFitment ?? null,
        engineering: employee.engineering ?? null,
        mobileNumber: employee.mobileNumber ?? '',
        skillIds: employee.skills.map((s) => s.id),
      });
    }
  }, [employee, reset]);

  return (
    <Box component="form" noValidate onSubmit={handleSubmit(onSubmit)} sx={{ pt: 1 }}>
      <Stack spacing={2}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="employeeCode" render={({ field, fieldState }) => (
            <TextField {...field} fullWidth label="Employee Code" error={!!fieldState.error} helperText={fieldState.error?.message} />
          )} />
          <Controller control={control} name="doj" render={({ field, fieldState }) => (
            <TextField {...field} fullWidth label="Date Of Joining" type="date" InputLabelProps={{ shrink: true }} error={!!fieldState.error} helperText={fieldState.error?.message} />
          )} />
        </Stack>

        <Controller control={control} name="fullName" render={({ field, fieldState }) => (
          <TextField {...field} fullWidth label="Full Name" error={!!fieldState.error} helperText={fieldState.error?.message} />
        )} />

        <Controller control={control} name="email" render={({ field, fieldState }) => (
          <TextField {...field} fullWidth label="Email" type="email" error={!!fieldState.error} helperText={fieldState.error?.message} />
        )} />

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="priorExperience" render={({ field, fieldState }) => (
            <TextField {...field} fullWidth label="Prior Experience (yrs)" type="number" error={!!fieldState.error} helperText={fieldState.error?.message} onChange={(e) => field.onChange(Number(e.target.value))} />
          )} />
          <Controller control={control} name="relevantExperience" render={({ field }) => (
            <TextField {...field} fullWidth label="Relevant Experience (yrs)" type="number" onChange={(e) => field.onChange(e.target.value ? Number(e.target.value) : null)} value={field.value ?? ''} />
          )} />
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="mobileNumber" render={({ field }) => (
            <TextField {...field} fullWidth label="Mobile Number" />
          )} />
          <Controller control={control} name="lwd" render={({ field }) => (
            <TextField {...field} fullWidth label="Last Working Day" type="date" InputLabelProps={{ shrink: true }} value={field.value ?? ''} onChange={(e) => field.onChange(e.target.value || null)} />
          )} />
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="employmentTypeId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Employment Type" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {employmentTypes.map((e) => (
                <MenuItem key={e.id} value={e.id}>{e.name}</MenuItem>
              ))}
            </TextField>
          )} />
          <Controller control={control} name="workModelId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Work Model" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {workModels.map((w) => (
                <MenuItem key={w.id} value={w.id}>{w.name}</MenuItem>
              ))}
            </TextField>
          )} />
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="practiceId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Practice" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {practices.map((p) => (
                <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
              ))}
            </TextField>
          )} />
          <Controller control={control} name="departmentTypeId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Department Type" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {departmentTypes.map((d) => (
                <MenuItem key={d.id} value={d.id}>{d.name}</MenuItem>
              ))}
            </TextField>
          )} />
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="designationId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Designation" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {designations.map((d) => (
                <MenuItem key={d.id} value={d.id}>{d.name}</MenuItem>
              ))}
            </TextField>
          )} />
          <Controller control={control} name="locationId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Location" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {locations.map((l) => (
                <MenuItem key={l.id} value={l.id}>{l.name}</MenuItem>
              ))}
            </TextField>
          )} />
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="statusId" render={({ field, fieldState }) => (
            <TextField {...field} select fullWidth label="Status" error={!!fieldState.error} helperText={fieldState.error?.message}>
              {statuses.map((s) => (
                <MenuItem key={s.id} value={s.id}>{s.name}</MenuItem>
              ))}
            </TextField>
          )} />
          <Box sx={{ width: '100%' }} />
        </Stack>

        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
          <Controller control={control} name="reportingManagerName" render={({ field }) => (
            <TextField {...field} fullWidth label="Reporting Manager" value={field.value ?? ''} onChange={(e) => field.onChange(e.target.value || null)} />
          )} />
          <Controller control={control} name="practiceHeadName" render={({ field }) => (
            <TextField {...field} fullWidth label="Practice Head" value={field.value ?? ''} onChange={(e) => field.onChange(e.target.value || null)} />
          )} />
        </Stack>

        <Stack direction="row" spacing={2}>
          <Controller control={control} name="deloitteFitment" render={({ field }) => (
            <FormControlLabel control={<Checkbox checked={field.value ?? false} onChange={(e) => field.onChange(e.target.checked)} />} label="Deloitte Fitment" />
          )} />
          <Controller control={control} name="engineering" render={({ field }) => (
            <FormControlLabel control={<Checkbox checked={field.value ?? false} onChange={(e) => field.onChange(e.target.checked)} />} label="Engineering" />
          )} />
        </Stack>

        <Controller control={control} name="skillIds" render={({ field }) => (
          <Autocomplete
            multiple
            options={skills}
            getOptionLabel={(option) => option.name}
            isOptionEqualToValue={(option, value) => option.id === value.id}
            onChange={(_, value) => field.onChange(value.map((s) => s.id))}
            value={skills.filter((s) => field.value?.includes(s.id))}
            renderInput={(params) => <TextField {...params} label="Skills" />}
          />
        )} />

        <Stack direction="row" justifyContent="flex-end" spacing={1}>
          <Button type="button" onClick={onCancel}>Cancel</Button>
          <Button type="submit" variant="contained">Save Employee</Button>
        </Stack>
      </Stack>
    </Box>
  );
}
