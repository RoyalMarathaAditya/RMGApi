import EditOutlinedIcon from '@mui/icons-material/EditOutlined';
import { Accordion, AccordionDetails, AccordionSummary, Box, Button, Card, CardContent, Stack, Tab, Tabs, Typography } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { SyntheticEvent, useMemo, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import EmployeeSkillGrid from '../components/EmployeeSkillGrid';
import SkillCategoryChip from '../components/SkillCategoryChip';
import SkillStatusChip from '../components/SkillStatusChip';
import { mockEmployeeSkills } from '../mock/mockEmployeeSkills';
// Redux: reads skills from store to display skill details
import { useAppSelector } from '../../../redux/hooks';

export default function SkillDetails() {
  const navigate = useNavigate();
  const { id } = useParams();
  const [tab, setTab] = useState(0);
  const skill = useAppSelector((state) => state.skills.skills.find((item) => item.id === Number(id)));
  const employeeSkills = useMemo(() => mockEmployeeSkills.filter((mapping) => mapping.skillId === Number(id)), [id]);

  if (!skill) {
    return (
      <Box>
        <Typography fontWeight={800} variant="h5">
          Skill not found
        </Typography>
        <Button onClick={() => navigate('/skills')} sx={{ mt: 2 }} variant="contained">
          Back to Skills
        </Button>
      </Box>
    );
  }

  return (
    <Stack spacing={3}>
      <Stack direction={{ xs: 'column', md: 'row' }} justifyContent="space-between" spacing={2}>
        <Box>
          <Typography component="h1" fontWeight={900} variant="h4">
            {skill.skillName}
          </Typography>
          <Typography color="text.secondary" mt={0.75}>
            {skill.skillCode}
          </Typography>
        </Box>
        <Button onClick={() => navigate(`/skills/edit/${skill.id}`)} startIcon={<EditOutlinedIcon />} variant="contained">
          Edit
        </Button>
      </Stack>

      <Tabs onChange={(_event: SyntheticEvent, value: number) => setTab(value)} value={tab}>
        <Tab label="Information" />
        <Tab label="Employees" />
        <Tab label="Statistics" />
      </Tabs>

      {tab === 0 ? (
        <Accordion defaultExpanded>
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Typography fontWeight={800}>Skill Information</Typography>
          </AccordionSummary>
          <AccordionDetails>
            <Stack spacing={2}>
              <Stack direction="row" spacing={1}>
                <SkillCategoryChip category={skill.category} />
                <SkillStatusChip status={skill.status} />
              </Stack>
              <Typography>{skill.description}</Typography>
            </Stack>
          </AccordionDetails>
        </Accordion>
      ) : null}

      {tab === 1 ? (
        <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
          <CardContent>
            <Typography fontWeight={800} mb={2} variant="h6">
              Employees Using Skill
            </Typography>
            <EmployeeSkillGrid employeeSkills={employeeSkills} />
          </CardContent>
        </Card>
      ) : null}

      {tab === 2 ? (
        <Box sx={{ display: 'grid', gap: 2, gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' } }}>
          {[
            ['Employee Count', skill.employeeCount],
            ['Average Experience', `${(employeeSkills.reduce((sum, item) => sum + item.yearsOfExperience, 0) / Math.max(employeeSkills.length, 1)).toFixed(1)} yrs`],
            ['Certified Employees', employeeSkills.filter((item) => item.certification).length],
          ].map(([label, value]) => (
            <Card elevation={0} key={label} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
              <CardContent>
                <Typography color="text.secondary" fontWeight={700}>
                  {label}
                </Typography>
                <Typography fontWeight={900} mt={1} variant="h4">
                  {value}
                </Typography>
              </CardContent>
            </Card>
          ))}
        </Box>
      ) : null}
    </Stack>
  );
}
