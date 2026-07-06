import { yupResolver } from '@hookform/resolvers/yup';
import SaveOutlinedIcon from '@mui/icons-material/SaveOutlined';
import { Box, Button, MenuItem, Paper, Stack, TextField } from '@mui/material';
import { Controller, useForm } from 'react-hook-form';
import TechnologySelector from './TechnologySelector';
import { projectPriorities, projectStatuses } from '../mock/projects';
import type { ProjectFormValues } from '../types/project.types';
import { projectValidationSchema } from '../validation/projectValidation';

const defaultValues: ProjectFormValues = {
  projectCode: '',
  projectName: '',
  description: '',
  clientName: '',
  clientContact: '',
  projectManager: '',
  startDate: '',
  endDate: '',
  status: 'Planned',
  priority: 'Medium',
  technologies: [],
  allocatedResources: 0,
  csm: '',
};

interface ProjectFormProps {
  initialValues?: ProjectFormValues;
  mode: 'create' | 'edit';
  onCancel: () => void;
  onSubmit: (values: ProjectFormValues) => void;
}

export default function ProjectForm({ initialValues, mode, onCancel, onSubmit }: ProjectFormProps) {
  const {
    control,
    formState: { errors, isSubmitting },
    handleSubmit,
  } = useForm<ProjectFormValues>({
    defaultValues: initialValues ?? defaultValues,
    mode: 'onBlur',
    resolver: yupResolver(projectValidationSchema),
  });

  return (
    <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: { xs: 2, md: 3 } }}>
      <Box component="form" noValidate onSubmit={handleSubmit(onSubmit)}>
        <Box
          sx={{
            display: 'grid',
            gap: 2,
            gridTemplateColumns: { xs: '1fr', md: 'repeat(2, minmax(0, 1fr))' },
          }}
        >
          <Controller
            control={control}
            name="projectCode"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.projectCode)} helperText={errors.projectCode?.message} label="Project Code" />
            )}
          />
          <Controller
            control={control}
            name="projectName"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.projectName)} helperText={errors.projectName?.message} label="Project Name" />
            )}
          />
          <Controller
            control={control}
            name="clientName"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.clientName)} helperText={errors.clientName?.message} label="Client Name" />
            )}
          />
          <Controller
            control={control}
            name="clientContact"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.clientContact)} helperText={errors.clientContact?.message} label="Client Contact" />
            )}
          />
          <Controller
            control={control}
            name="projectManager"
            render={({ field }) => (
              <TextField
                {...field}
                error={Boolean(errors.projectManager)}
                helperText={errors.projectManager?.message}
                label="Project Manager"
              />
            )}
          />
          <Controller
            control={control}
            name="csm"
            render={({ field }) => (
              <TextField
                {...field}
                error={Boolean(errors.csm)}
                helperText={errors.csm?.message}
                label="CSM"
              />
            )}
          />
          <Controller
            control={control}
            name="allocatedResources"
            render={({ field }) => (
              <TextField
                {...field}
                error={Boolean(errors.allocatedResources)}
                helperText={errors.allocatedResources?.message}
                label="Allocated Resources"
                onChange={(event) => field.onChange(Number(event.target.value))}
                type="number"
              />
            )}
          />
          <Controller
            control={control}
            name="startDate"
            render={({ field }) => (
              <TextField
                {...field}
                error={Boolean(errors.startDate)}
                helperText={errors.startDate?.message}
                InputLabelProps={{ shrink: true }}
                label="Start Date"
                type="date"
              />
            )}
          />
          <Controller
            control={control}
            name="endDate"
            render={({ field }) => (
              <TextField
                {...field}
                error={Boolean(errors.endDate)}
                helperText={errors.endDate?.message}
                InputLabelProps={{ shrink: true }}
                label="End Date"
                type="date"
              />
            )}
          />
          <Controller
            control={control}
            name="status"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.status)} helperText={errors.status?.message} label="Status" select>
                {projectStatuses.map((status) => (
                  <MenuItem key={status} value={status}>
                    {status}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />
          <Controller
            control={control}
            name="priority"
            render={({ field }) => (
              <TextField {...field} error={Boolean(errors.priority)} helperText={errors.priority?.message} label="Priority" select>
                {projectPriorities.map((priority) => (
                  <MenuItem key={priority} value={priority}>
                    {priority}
                  </MenuItem>
                ))}
              </TextField>
            )}
          />
          <Box sx={{ gridColumn: { xs: 'auto', md: '1 / -1' } }}>
            <Controller
              control={control}
              name="technologies"
              render={({ field }) => (
                <TechnologySelector
                  error={errors.technologies?.message}
                  onBlur={field.onBlur}
                  onChange={field.onChange}
                  value={field.value}
                />
              )}
            />
          </Box>
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
            {mode === 'create' ? 'Create Project' : 'Save Changes'}
          </Button>
        </Stack>
      </Box>
    </Paper>
  );
}
