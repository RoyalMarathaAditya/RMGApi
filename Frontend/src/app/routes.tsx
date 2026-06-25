import { Navigate, Route, Routes } from 'react-router-dom';
import ProtectedRoute from '../components/common/ProtectedRoute';
import PageContainer from '../components/common/PageContainer';
import MainLayout from '../layouts/MainLayout';
import ProjectCreate from '../features/projects/pages/ProjectCreate';
import ProjectDashboard from '../features/projects/pages/ProjectDashboard';
import Project from '../features/projects/pages/Project';
import ProjectEdit from '../features/projects/pages/ProjectEdit';
import ProjectList from '../features/projects/pages/ProjectList';
import EmployeeList from '../features/employees/pages/EmployeeList';
import EmployeeSkillMapping from '../features/skills/pages/EmployeeSkillMapping';
import ResourceSearchBySkill from '../features/skills/pages/ResourceSearchBySkill';
import SkillCreate from '../features/skills/pages/SkillCreate';
import SkillDashboard from '../features/skills/pages/SkillDashboard';
import SkillDetails from '../features/skills/pages/SkillDetails';
import SkillEdit from '../features/skills/pages/SkillEdit';
import DesignationList from '../features/designations/pages/DesignationList';
import SkillList from '../features/skills/pages/SkillList';
import SkillMatrix from '../features/skills/pages/SkillMatrix';
import ResourceAllocationDashboard from '../features/rmg/pages/ResourceAllocationDashboard';
import ResourceAllocationDetail from '../features/rmg/pages/ResourceAllocationDetail';
import ResourceFinder from '../features/rmg/pages/ResourceFinder';
import PracticeUtilization from '../features/rmg/pages/PracticeUtilization';
import ResourceRequestPage from '../features/rmg/pages/ResourceRequestPage';
import AllocationHistory from '../features/rmg/pages/AllocationHistory';
import CalendarViewPage from '../features/rmg/pages/CalendarViewPage';
import TimelineViewPage from '../features/rmg/pages/TimelineViewPage';
import Login from '../pages/auth/Login';
import Dashboard from '../pages/dashboard/Dashboard';
import Home from '../pages/home/Home';

const modulePages = {
  employees: 'Employee Management',
  skills: 'Skills Management',
  rmg: 'Resource Allocation',
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
          <Route path="/projects/:id" element={<Project />} />
          <Route path="/designations" element={<DesignationList />} />
          <Route path="/skills" element={<SkillList />} />
          <Route path="/skills/create" element={<SkillCreate />} />
          <Route path="/skills/dashboard" element={<SkillDashboard />} />
          <Route path="/skills/edit/:id" element={<SkillEdit />} />
          <Route path="/skills/mapping" element={<EmployeeSkillMapping />} />
          <Route path="/skills/matrix" element={<SkillMatrix />} />
          <Route path="/skills/resources" element={<ResourceSearchBySkill />} />
          <Route path="/skills/:id" element={<SkillDetails />} />
          <Route path="/rmg" element={<ResourceAllocationDashboard />} />
          <Route path="/rmg/edit/:id" element={<ResourceAllocationDetail />} />
          <Route path="/rmg/finder" element={<ResourceFinder />} />
          <Route path="/rmg/practice-utilization" element={<PracticeUtilization />} />
          <Route path="/rmg/requests" element={<ResourceRequestPage />} />
          <Route path="/rmg/history/:allocationId" element={<AllocationHistory />} />
          <Route path="/rmg/calendar" element={<CalendarViewPage />} />
          <Route path="/rmg/timeline" element={<TimelineViewPage />} />
          <Route path="/reports" element={<ModulePlaceholder title={modulePages.reports} />} />
          <Route path="/settings" element={<ModulePlaceholder title={modulePages.settings} />} />
        </Route>
      </Route>
      {/*<Route path="*" element={<Navigate to="/home" replace />} />*/}
    </Routes>
  );
}
