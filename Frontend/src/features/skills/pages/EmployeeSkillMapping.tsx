import { yupResolver } from '@hookform/resolvers/yup';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import { Alert, Autocomplete, Button, Card, CardContent, MenuItem, Stack, TextField, Typography } from '@mui/material';
import { useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
// Redux: reads skills from store for skill mapping dropdown
import { useAppSelector } from '../../../redux/hooks';
import EmployeeSkillGrid from '../components/EmployeeSkillGrid';
import { mockEmployeeSkills } from '../mock/mockEmployeeSkills';
import { proficiencyLevels } from '../types';
import type { EmployeeSkillFormValues } from '../types';
import { employeeSkillValidationSchema } from '../validations/employeeSkillValidation';
import { toastService } from '../../../services/toastService';

const employees = Array.from(new Set(mockEmployeeSkills.map((item) => item.employeeName)));

const defaultValues: EmployeeSkillFormValues = {
  certification: '',
  employeeName: '',
  proficiency: 'Intermediate',
  skillId: null,
  yearsOfExperience: 1,
};

export default function EmployeeSkillMapping() {
  const skills = useAppSelector((state) => state.skills.skills);
  const {
    control,
    formState: { errors, isSubmitting },
    handleSubmit,
    reset,
  } = useForm<EmployeeSkillFormValues>({
    defaultValues,
    mode: 'onBlur',
    resolver: yupResolver(employeeSkillValidationSchema),
  });

  function onSubmit() {
    toastService.success('Skill assigned successfully.');
    reset(defaultValues);
  }

  return (
    <Stack spacing={3}>
      <Typography component="h1" fontWeight={900} variant="h4">
        Employee Skill Mapping
      </Typography>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Stack component="form" noValidate onSubmit={handleSubmit(onSubmit)} spacing={2}>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <Controller
                control={control}
                name="employeeName"
                render={({ field }) => (
                  <Autocomplete
                    freeSolo
                    onChange={(_event, value) => field.onChange(value ?? '')}
                    options={employees}
                    renderInput={(params) => (
                      <TextField {...params} error={Boolean(errors.employeeName)} helperText={errors.employeeName?.message} label="Employee" />
                    )}
                    sx={{ flex: 1 }}
                    value={field.value}
                  />
                )}
              />
              <Controller
                control={control}
                name="skillId"
                render={({ field }) => (
                  <Autocomplete
                    getOptionLabel={(option) => option.skillName}
                    onChange={(_event, value) => field.onChange(value?.id ?? null)}
                    options={skills}
                    renderInput={(params) => (
                      <TextField {...params} error={Boolean(errors.skillId)} helperText={errors.skillId?.message} label="Skill" />
                    )}
                    sx={{ flex: 1 }}
                    value={skills.find((item) => item.id === field.value) ?? null}
                  />
                )}
              />
            </Stack>
            <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
              <Controller
                control={control}
                name="proficiency"
                render={({ field }) => (
                  <TextField {...field} error={Boolean(errors.proficiency)} helperText={errors.proficiency?.message} label="Proficiency" select>
                    {proficiencyLevels.map((level) => (
                      <MenuItem key={level} value={level}>
                        {level}
                      </MenuItem>
                    ))}
                  </TextField>
                )}
              />
              <Controller
                control={control}
                name="yearsOfExperience"
                render={({ field }) => (
                  <TextField
                    {...field}
                    error={Boolean(errors.yearsOfExperience)}
                    helperText={errors.yearsOfExperience?.message}
                    label="Years Of Experience"
                    onChange={(event) => field.onChange(Number(event.target.value))}
                    type="number"
                  />
                )}
              />
              <Controller
                control={control}
                name="certification"
                render={({ field }) => (
                  <TextField {...field} error={Boolean(errors.certification)} helperText={errors.certification?.message} label="Certification" />
                )}
              />
            </Stack>
            <Button disabled={isSubmitting} startIcon={<SaveOutlinedIcon />} sx={{ alignSelf: 'flex-end' }} type="submit" variant="contained">
              Assign Skill
            </Button>
          </Stack>
        </CardContent>
      </Card>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Typography fontWeight={800} mb={2} variant="h6">
            Existing Mappings
          </Typography>
          <EmployeeSkillGrid employeeSkills={mockEmployeeSkills} />
        </CardContent>
      </Card>
    </Stack>
  );
}
