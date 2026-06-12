import { yupResolver } from '@hookform/resolvers/yup';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import { Box, Button, MenuItem, Paper, Stack, TextField } from '@mui/material';
import { Controller, useForm } from 'react-hook-form';
import { skillValidationSchema } from '../validations/skillValidation';
import { skillCategories, skillStatuses } from '../types';
import type { SkillFormValues } from '../types';

const defaultValues: SkillFormValues = {
  category: 'Frontend',
  description: '',
  skillCode: '',
  skillName: '',
  status: 'Active',
};

interface SkillFormProps {
  initialValues?: SkillFormValues;
  mode: 'create' | 'edit';
  onCancel: () => void;
  onSubmit: (values: SkillFormValues) => void;
}

export default function SkillForm({ initialValues, mode, onCancel, onSubmit }: SkillFormProps) {
  const {
    control,
    formState: { errors, isSubmitting },
    handleSubmit,
  } = useForm<SkillFormValues>({
    defaultValues: initialValues ?? defaultValues,
    mode: 'onBlur',
    resolver: yupResolver(skillValidationSchema),
  });

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: { xs: 2, md: 3 } }}>
      <Box component="form" noValidate onSubmit={handleSubmit(onSubmit)}>
        <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' } }}>
          <Controller
            control={control}
            name="skillCode"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.skillCode)} helperText={errors.skillCode?.message} label="Skill Code" />
            )}
          />
          <Controller
            control={control}
            name="skillName"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.skillName)} helperText={errors.skillName?.message} label="Skill Name" />
            )}
          />
          <Controller
            control={control}
            name="category"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.category)} helperText={errors.category?.message} label="Category" select>
                {skillCategories.map((category) => (
                  <MenuItem key={category} value={category}>
                    {category}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />
          <Controller
            control={control}
            name="status"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.status)} helperText={errors.status?.message} label="Status" select>
                {skillStatuses.map((status) => (
                  <MenuItem key={status} value={status}>
                    {status}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />
          <Box sx={{ gridColumn: { xs: 'auto', md: '1 / -1' } }}>
            <Controller
              control={control}
              name="description"
              render={({ field }) => (
                <TextField
                  {...field}
                  error={Boolean(errors.description)}
                  fullWidth
                  helperText={errors.description?.message}
                  label="Description"
                  minRows={4}
                  multiline
                />
              )}
            />
          </Box>
        </Box>
        <Stack direction="row" justifyContent="flex-end" spacing={1.5} sx={{ mt: 3 }}>
          <Button onClick={onCancel} variant="outlined">
            Cancel
          </Button>
          <Button disabled={isSubmitting} startIcon={<SaveOutlinedIcon />} type="submit" variant="contained">
            {mode === 'create' ? 'Save' : 'Save Changes'}
          </Button>
        </Stack>
      </Box>
    </Paper>
  );
}
