import { useEffect, useMemo, useState } from 'react';
import { Controller, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import CheckBoxOutlineBlankOutlinedIcon from '@mui/icons-material/CheckBoxOutlineBlankOutlined';
import CheckBoxOutlinedIcon from '@mui/icons-material/CheckBoxOutlined';
import {
  Autocomplete, Box, Button, Checkbox, CircularProgress, Dialog, DialogActions,
  DialogContent, DialogTitle, FormControlLabel, TextField, Typography,
} from '@mui/material';
import { useAppSelector } from '../../../redux/hooks';
import api from '../../../services/api';
import { toastService } from '../../../services/toastService';
import type { EmployeeResourceDetailsDto } from '../types/allocation';

interface EditExperienceModalProps {
  open: boolean;
  onClose: () => void;
  onSaved: () => Promise<void>;
  employeeId: number;
  data: EmployeeResourceDetailsDto;
}

function calculateNVExperienceYears(doj: string, lwd?: string | null): number {
  const from = new Date(doj);
  const to = lwd ? new Date(lwd) : new Date();
  const diffMs = to.getTime() - from.getTime();
  return Math.max(0, diffMs / (365.25 * 24 * 60 * 60 * 1000));
}

function formatYearsMonths(years: number): string {
  const y = Math.floor(years);
  const m = Math.round((years - y) * 12);
  if (y === 0) return `${m} months`;
  if (m === 0) return `${y} years`;
  return `${y} years ${m} months`;
}

const schema = yup.object({
  primarySkillId: yup.number().nullable(),
  skillIds: yup.array(yup.number()).default([]),
  projectManagerId: yup.number().nullable(),
  isActive: yup.boolean().default(true),
  remarks: yup.string().nullable(),
});

type FormValues = yup.InferType<typeof schema>;

export default function EditExperienceModal({ open, onClose, onSaved, employeeId, data }: EditExperienceModalProps) {
  const [saving, setSaving] = useState(false);
  const [employees, setEmployees] = useState<{ id: number; fullName: string; employeeCode: string }[]>([]);
  const [employeesLoading, setEmployeesLoading] = useState(false);
  const skills = useAppSelector((state) => state.skills.skills);

  const priorExperienceYears = useMemo(() => data.priorExperience ?? 0, [data]);
  const nvExperienceYears = useMemo(() => {
    if (!data.doj) return 0;
    return calculateNVExperienceYears(data.doj);
  }, [data.doj]);
  const totalExperienceYears = useMemo(() => priorExperienceYears + nvExperienceYears, [priorExperienceYears, nvExperienceYears]);

  const priorDisplay = useMemo(() => formatYearsMonths(priorExperienceYears), [priorExperienceYears]);
  const nvDisplay = useMemo(() => formatYearsMonths(nvExperienceYears), [nvExperienceYears]);
  const totalDisplay = useMemo(() => formatYearsMonths(totalExperienceYears), [totalExperienceYears]);

  const { control, handleSubmit, reset, formState: { errors } } = useForm<FormValues>({
    resolver: yupResolver(schema),
    defaultValues: {
      primarySkillId: null,
      skillIds: [],
      projectManagerId: data.projectManagerId ?? null,
      isActive: data.active ?? true,
      remarks: data.remarks ?? '',
    },
  });

  useEffect(() => {
    if (open) {
      const primarySkillId = skills.find((s) => s.skillName === data.primarySkill)?.id ?? null;
      const skillIds = skills.filter((s) => data.skill?.includes(s.skillName)).map((s) => s.id);
      reset({
        primarySkillId,
        skillIds,
        projectManagerId: data.projectManagerId ?? null,
        isActive: data.active ?? true,
        remarks: data.remarks ?? '',
      });
      fetchEmployees();
    }
  }, [open, data, reset, skills]);

  const fetchEmployees = async () => {
    setEmployeesLoading(true);
    try {
      const res = await api.get('/employees');
      const list: any[] = res.data.data ?? [];
      setEmployees(list.map((e: any) => ({ id: e.id, fullName: e.fullName, employeeCode: e.employeeCode })));
    } catch {
      console.error('Failed to load employees');
    } finally {
      setEmployeesLoading(false);
    }
  };

  const onSubmit = async (values: FormValues) => {
    setSaving(true);
    try {
      const selectedPrimarySkill = skills.find((s) => s.id === values.primarySkillId);
      const selectedSkills = skills.filter((s) => values.skillIds.includes(s.id));
      await api.put(`/resource-allocations/employee/${employeeId}/details`, {
        employeeId,
        experienceInNV: nvExperienceYears,
        primarySkillId: values.primarySkillId,
        skillIds: values.skillIds,
        primarySkillName: selectedPrimarySkill?.skillName ?? null,
        skillNames: selectedSkills.map((s) => s.skillName).join(', ') || null,
        projectManagerId: values.projectManagerId,
        isActive: values.isActive,
        remarks: values.remarks ?? '',
      });
      await onSaved();
      toastService.success('Experience Information updated successfully.');
      onClose();
    } catch (err: any) {
      toastService.error(err.response?.data?.message || 'Failed to save experience information');
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onClose={() => !saving && onClose()} maxWidth="md" fullWidth
      PaperProps={{ sx: { borderRadius: '14px', maxWidth: 720, boxShadow: '0 20px 60px rgba(0,0,0,0.12)' } }}>
      <DialogTitle sx={{ px: 3, py: 2.5, borderBottom: '1px solid #E5E7EB', fontSize: 18, fontWeight: 700, color: '#111827' }}>
        Edit Experience Information
      </DialogTitle>
      <form onSubmit={handleSubmit(onSubmit)}>
        <DialogContent sx={{ px: 3, py: 2.5 }}>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', md: '1fr 1fr 1fr' }, gap: '16px', pt: 1 }}>
            <TextField label="Experience Prior to NV" value={priorDisplay} size="small" disabled fullWidth
              helperText="This value is maintained in Employee Management and cannot be edited here."
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
            <TextField label="Experience in NV" value={nvDisplay} size="small" disabled fullWidth
              helperText={data.doj ? 'Auto-calculated from Date of Joining' : 'Date of Joining not available'}
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />
            <TextField label="Total Experience (Prior + NV)" value={totalDisplay} size="small" disabled fullWidth
              slotProps={{ input: { readOnly: true } }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'action.hover' } }} />

            <Controller name="primarySkillId" control={control} render={({ field }) => {
              const selected = skills.find((s) => s.id === field.value) || null;
              return (
                <Autocomplete
                  options={skills}
                  getOptionLabel={(option) => option.skillName}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  value={selected}
                  onChange={(_, value) => field.onChange(value?.id ?? null)}
                  size="small" fullWidth
                  renderInput={(params) => <TextField {...params} label="Primary Skill" placeholder="Search skill" />}
                />
              );
            }} />

            <Controller name="skillIds" control={control} render={({ field }) => {
              const selected = skills.filter((s) => field.value.includes(s.id));
              return (
                <Autocomplete
                  multiple
                  options={skills}
                  getOptionLabel={(option) => option.skillName}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  value={selected}
                  onChange={(_, value) => field.onChange(value.map((v) => v.id))}
                  disableCloseOnSelect
                  limitTags={2}
                  size="small" fullWidth
                  renderOption={(props, option, { selected: isSelected }) => (
                    <li {...props}>
                      <Checkbox icon={<CheckBoxOutlineBlankOutlinedIcon fontSize="small" />}
                        checkedIcon={<CheckBoxOutlinedIcon fontSize="small" />} checked={isSelected} sx={{ mr: 1 }} />
                      {option.skillName}
                    </li>
                  )}
                  renderInput={(params) => <TextField {...params} label="Key Skills" placeholder="Search skills" />}
                />
              );
            }} />

            <Controller name="projectManagerId" control={control} render={({ field }) => {
              const selected = employees.find((e) => e.id === field.value) || null;
              return (
                <Autocomplete
                  options={employees}
                  getOptionLabel={(option) => `${option.fullName} (${option.employeeCode})`}
                  isOptionEqualToValue={(option, value) => option.id === value.id}
                  value={selected}
                  onChange={(_, value) => field.onChange(value?.id ?? null)}
                  loading={employeesLoading}
                  size="small" fullWidth
                  renderInput={(params) => (
                    <TextField {...params} label="Project Manager" placeholder="Search employee"
                      slotProps={{
                        input: {
                          ...params.InputProps,
                          endAdornment: (
                            <>
                              {employeesLoading ? <CircularProgress color="inherit" size={20} /> : null}
                              {params.InputProps.endAdornment}
                            </>
                          ),
                        },
                      }} />
                  )}
                />
              );
            }} />

            <Controller name="isActive" control={control} render={({ field }) => (
              <FormControlLabel control={<Checkbox checked={field.value ?? false} onChange={(e) => field.onChange(e.target.checked)} />}
                label={<Typography sx={{ fontSize: 14, fontWeight: 500, color: '#374151' }}>Is Active</Typography>}
                sx={{ mt: 1 }} />
            )} />

            <Box sx={{ gridColumn: { xs: '1 / -1', md: '1 / -1' } }}>
              <Controller name="remarks" control={control} render={({ field }) => (
                <TextField {...field} label="Remarks" multiline minRows={3} size="small" fullWidth
                  value={field.value ?? ''}
                  sx={{ '& .MuiInputBase-root': { resize: 'vertical', overflow: 'auto' } }} />
              )} />
            </Box>
          </Box>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2, borderTop: '1px solid #E5E7EB', gap: 1 }}>
          <Button onClick={onClose} disabled={saving} variant="outlined" color="inherit"
            sx={{ borderRadius: 2, textTransform: 'none', fontWeight: 600, px: 3 }}>
            Cancel
          </Button>
          <Button type="submit" disabled={saving} variant="contained"
            sx={{ borderRadius: 2, textTransform: 'none', fontWeight: 600, px: 3 }}>
            {saving ? <CircularProgress size={20} sx={{ mr: 1 }} /> : null}
            Save Changes
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  );
}