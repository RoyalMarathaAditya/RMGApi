import { Card, CardContent, MenuItem, Stack, TextField, Typography } from '@mui/material';
import { useMemo, useState } from 'react';
// Redux: reads skills from store for skill matrix grid
import { useAppSelector } from '../../../redux/hooks';
import SkillMatrixGrid from '../components/SkillMatrixGrid';
import { mockEmployeeSkills } from '../mock/mockEmployeeSkills';
import { skillCategories } from '../types';
import type { SkillMatrixEmployee } from '../types';

export default function SkillMatrix() {
  const skills = useAppSelector((state) => state.skills.skills);
  const [department, setDepartment] = useState('All');
  const [project, setProject] = useState('All');
  const [category, setCategory] = useState('All');

  const departments = Array.from(new Set(mockEmployeeSkills.map((item) => item.department)));
  const projects = Array.from(new Set(mockEmployeeSkills.map((item) => item.project)));
  const visibleSkills = category === 'All' ? skills : skills.filter((skill) => skill.category === category);

  const rows = useMemo(() => {
    const filtered = mockEmployeeSkills.filter((item) => {
      const matchesDepartment = department === 'All' || item.department === department;
      const matchesProject = project === 'All' || item.project === project;
      const matchesCategory = category === 'All' || item.skillCategory === category;
      return matchesDepartment && matchesProject && matchesCategory;
    });

    return Object.values(
      filtered.reduce<Record<number, SkillMatrixEmployee>>((acc, item) => {
        acc[item.employeeId] ??= {
          department: item.department,
          employeeId: item.employeeId,
          employeeName: item.employeeName,
          project: item.project,
          skills: {},
        };
        acc[item.employeeId].skills[item.skillId] = item.proficiency;
        return acc;
      }, {}),
    );
  }, [category, department, project]);

  return (
    <Stack spacing={3}>
      <Typography component="h1" fontWeight={900} variant="h4">
        Skill Matrix
      </Typography>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Stack direction={{ xs: 'column', md: 'row' }} spacing={2}>
            <TextField label="Department" onChange={(event) => setDepartment(event.target.value)} select value={department}>
              <MenuItem value="All">All Departments</MenuItem>
              {departments.map((item) => (
                <MenuItem key={item} value={item}>
                  {item}
                </MenuItem>
              ))}
            </TextField>
            <TextField label="Project" onChange={(event) => setProject(event.target.value)} select value={project}>
              <MenuItem value="All">All Projects</MenuItem>
              {projects.map((item) => (
                <MenuItem key={item} value={item}>
                  {item}
                </MenuItem>
              ))}
            </TextField>
            <TextField label="Skill Category" onChange={(event) => setCategory(event.target.value)} select value={category}>
              <MenuItem value="All">All Categories</MenuItem>
              {skillCategories.map((item) => (
                <MenuItem key={item} value={item}>
                  {item}
                </MenuItem>
              ))}
            </TextField>
          </Stack>
        </CardContent>
      </Card>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, overflow: 'hidden' }}>
        <SkillMatrixGrid rows={rows} skills={visibleSkills} />
      </Card>
    </Stack>
  );
}
