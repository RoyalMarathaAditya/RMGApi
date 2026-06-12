import { Navigate, Route, Routes } from 'react-router-dom';
import ProtectedRoute from '../components/common/ProtectedRoute';
import PageContainer from '../components/common/PageContainer';
import MainLayout from '../layouts/MainLayout';
import ProjectCreate from '../features/projects/pages/ProjectCreate';
import ProjectDashboard from '../features/projects/pages/ProjectDashboard';
import ProjectDetails from '../features/projects/pages/ProjectDetails';
import ProjectEdit from '../features/projects/pages/ProjectEdit';
import ProjectList from '../features/projects/pages/ProjectList';
import EmployeeList from '../features/employees/pages/EmployeeList';
import EmployeeSkillMapping from '../features/skills/pages/EmployeeSkillMapping';
import ResourceSearchBySkill from '../features/skills/pages/ResourceSearchBySkill';
import SkillCreate from '../features/skills/pages/SkillCreate';
import SkillDashboard from '../features/skills/pages/SkillDashboard';
import SkillDetails from '../features/skills/pages/SkillDetails';
import SkillEdit from '../features/skills/pages/SkillEdit';
import SkillList from '../features/skills/pages/SkillList';
import SkillMatrix from '../features/skills/pages/SkillMatrix';
import AllocationDashboard from '../features/allocations/pages/AllocationDashboard';
import AllocationList from '../features/allocations/pages/AllocationList';
import BenchResources from '../features/allocations/pages/BenchResources';
import BillableResources from '../features/allocations/pages/BillableResources';
import ResourceUtilization from '../features/allocations/pages/ResourceUtilization';
import EmployeeAllocationStatus from '../features/allocations/pages/EmployeeAllocationStatus';
import Login from '../pages/auth/Login';
import Dashboard from '../pages/dashboard/Dashboard';
import Home from '../pages/home/Home';

const modulePages = {
  employees: 'Employee Management',
  skills: 'Skills Management',
  allocations: 'Resource Allocation',
  reports: 'Reports',
  settings: 'Settings',
} as const;

function ModulePlaceholder({ title }: { title: string }) {
  return (
    <PageContainer title={title}>
      Module foundation is ready. Feature implementation will be added in the next delivery phase.
    </PageContainer>
  );
}

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/login" element={<Login />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<MainLayout />}>
          {/*<Route index element={<Navigate to="/home" replace />} />
          <Route path="/home" element={<Home />} />*/}
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/employees" element={<EmployeeList />} />
          <Route path="/projects" element={<ProjectList />} />
          <Route path="/projects/create" element={<ProjectCreate />} />
          <Route path="/projects/dashboard" element={<ProjectDashboard />} />
          <Route path="/projects/edit/:id" element={<ProjectEdit />} />
          <Route path="/projects/:id" element={<ProjectDetails />} />
          <Route path="/allocations" element={<AllocationDashboard />} />
          <Route path="/allocations/list" element={<AllocationList />} />
          <Route path="/allocations/status" element={<EmployeeAllocationStatus />} />
          <Route path="/allocations/bench" element={<BenchResources />} />
          <Route path="/allocations/billable" element={<BillableResources />} />
          <Route path="/allocations/utilization" element={<ResourceUtilization />} />
          <Route path="/skills" element={<SkillList />} />
          <Route path="/skills/create" element={<SkillCreate />} />
          <Route path="/skills/dashboard" element={<SkillDashboard />} />
          <Route path="/skills/edit/:id" element={<SkillEdit />} />
          <Route path="/skills/mapping" element={<EmployeeSkillMapping />} />
          <Route path="/skills/matrix" element={<SkillMatrix />} />
          <Route path="/skills/resources" element={<ResourceSearchBySkill />} />
          <Route path="/skills/:id" element={<SkillDetails />} />
          <Route path="/reports" element={<ModulePlaceholder title={modulePages.reports} />} />
          <Route path="/settings" element={<ModulePlaceholder title={modulePages.settings} />} />
        </Route>
      </Route>
      {/*<Route path="*" element={<Navigate to="/home" replace />} />*/}
    </Routes>
  );
}
