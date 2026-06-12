import SearchOutlinedIcon from '@mui/icons-material/SearchOutlined';
import { Autocomplete, Box, Card, CardContent, Chip, MenuItem, Stack, TextField, Typography } from '@mui/material';
import { DataGrid } from '@mui/x-data-grid';
import type { GridColDef } from '@mui/x-data-grid';
import { useMemo, useState } from 'react';
import { useAppSelector } from '../../../app/hooks';
import { mockEmployeeSkills } from '../mock/mockEmployeeSkills';
import { proficiencyLevels } from '../types';
import type { EmployeeSkill, ProficiencyLevel, Skill } from '../types';

export default function ResourceSearchBySkill() {
  const skills = useAppSelector((state) => state.skills.skills);
  const [skill, setSkill] = useState<Skill | null>(null);
  const [experience, setExperience] = useState(0);
  const [proficiency, setProficiency] = useState<ProficiencyLevel | 'All'>('All');

  const rows = useMemo(
    () =>
      mockEmployeeSkills.filter((item) => {
        const matchesSkill = !skill || item.skillId === skill.id;
        const matchesExperience = item.yearsOfExperience >= experience;
        const matchesProficiency = proficiency === 'All' || item.proficiency === proficiency;
        return matchesSkill && matchesExperience && matchesProficiency;
      }),
    [experience, proficiency, skill],
  );

  const columns: GridColDef<EmployeeSkill>[] = [
    { field: 'employeeName', headerName: 'Employee', minWidth: 180, flex: 1 },
    { field: 'yearsOfExperience', headerName: 'Experience', minWidth: 130, flex: 0.7, type: 'number' },
    {
      field: 'skillName',
      headerName: 'Skills',
      minWidth: 170,
      flex: 1,
      renderCell: (params) => <Chip label={params.row.skillName} size="small" />,
    },
    { field: 'allocationPercentage', headerName: 'Allocation %', minWidth: 130, flex: 0.7, type: 'number' },
    { field: 'availability', headerName: 'Availability', minWidth: 170, flex: 1 },
  ];

  return (
    <Stack spacing={3}>
      <Box>
        <Typography component="h1" fontWeight={900} variant="h4">
          Resource Search By Skill
        </Typography>
        <Typography color="text.secondary" mt={0.75}>
          Find employees by skill, experience, proficiency, allocation, and availability.
        </Typography>
      </Box>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
            <Autocomplete
              getOptionLabel={(option) => option.skillName}
              onChange={(_event, value) => setSkill(value)}
              options={skills}
              renderInput={(params) => (
                <TextField
                  {...params}
                  InputProps={{ ...params.InputProps, startAdornment: <SearchOutlinedIcon color="action" sx={{ mr: 1 }} /> }}
                  label="Skill"
                />
              )}
              sx={{ flex: 1 }}
              value={skill}
            />
            <TextField
              label="Experience"
              onChange={(event) => setExperience(Number(event.target.value))}
              sx={{ minWidth: { md: 160 } }}
              type="number"
              value={experience}
            />
            <TextField
              label="Proficiency"
              onChange={(event) => setProficiency(event.target.value as ProficiencyLevel | 'All')}
              select
              sx={{ minWidth: { md: 190 } }}
              value={proficiency}
            >
              <MenuItem value="All">All Levels</MenuItem>
              {proficiencyLevels.map((level) => (
                <MenuItem key={level} value={level}>
                  {level}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
        </CardContent>
      </Card>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
        {rows.length > 0 ? (
          <Box sx={{ minHeight: 520 }}>
            <DataGrid columns={columns} disableRowSelectionOnClick pageSizeOptions={[5, 10, 20]} rows={rows} sx={{ border: 0 }} />
          </Box>
        ) : (
          <Box alignItems="center" display="flex" flexDirection="column" justifyContent="center" minHeight={320} p={3}>
            <Typography fontWeight={800} variant="h6">
              No resources found
            </Typography>
            <Typography color="text.secondary" mt={1} textAlign="center">
              Broaden the skill, experience, or proficiency filters to see available employees.
            </Typography>
          </Box>
        )}
      </Card>
    </Stack>
  );
}
