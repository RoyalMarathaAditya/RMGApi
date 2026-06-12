import { Card, CardContent, Paper, Stack, Tab, Tabs, Typography, Box, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from '@mui/material';
import { useState } from 'react';
import { BarChart, Bar, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis, Cell } from 'recharts';
import type { ResourceAllocation } from '../types/resourceAllocation';
import type { Employee } from '../../employees/types/employee';
import { allocationTypeLabelMap } from '../types/allocationType';

interface EmployeeAllocationSummaryProps {
  allocations: ResourceAllocation[];
  employees: Employee[];
}

export default function EmployeeAllocationSummary({ allocations, employees }: EmployeeAllocationSummaryProps) {
  const [activeTab, setActiveTab] = useState<'summary' | 'billable' | 'nonbillable'>('summary');

  const billableAllocations = allocations.filter((a) => a.allocationType === 1);
  const nonbillableAllocations = allocations.filter((a) => a.allocationType === 2);

  const billableEmployees = Array.from(new Set(billableAllocations.map((a) => a.employeeId)));
  const nonbillableEmployees = Array.from(new Set(nonbillableAllocations.map((a) => a.employeeId)));

  const billableEmployeeDetails = billableEmployees.map((empId) => {
    const employee = employees.find((e) => e.id === empId);
    const allocCount = billableAllocations.filter((a) => a.employeeId === empId).length;
    return {
      employeeId: empId,
      employeeName: employee ? `${employee.firstName} ${employee.lastName}` : 'Unknown',
      department: employee?.department ?? 'Unknown',
      allocationCount: allocCount,
    };
  });

  const nonbillableEmployeeDetails = nonbillableEmployees.map((empId) => {
    const employee = employees.find((e) => e.id === empId);
    const allocCount = nonbillableAllocations.filter((a) => a.employeeId === empId).length;
    return {
      employeeId: empId,
      employeeName: employee ? `${employee.firstName} ${employee.lastName}` : 'Unknown',
      department: employee?.department ?? 'Unknown',
      allocationCount: allocCount,
    };
  });

  const summaryData = [
    { name: 'Billable', count: billableEmployees.length, fill: '#2e7d32' },
    { name: 'Non-billable', count: nonbillableEmployees.length, fill: '#ed6c02' },
  ];

  return (
    <Stack spacing={3}>
      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <CardContent>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3}>
            <Box>
              <Typography color="text.secondary" variant="subtitle2">
                Billable employees
              </Typography>
              <Typography fontWeight={800} variant="h3">
                {billableEmployees.length}
              </Typography>
            </Box>
            <Box>
              <Typography color="text.secondary" variant="subtitle2">
                Non-billable employees
              </Typography>
              <Typography fontWeight={800} variant="h3">
                {nonbillableEmployees.length}
              </Typography>
            </Box>
            <Box>
              <Typography color="text.secondary" variant="subtitle2">
                Total allocations
              </Typography>
              <Typography fontWeight={800} variant="h3">
                {allocations.length}
              </Typography>
            </Box>
          </Stack>
        </CardContent>
      </Card>

      <Paper elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, p: 3 }}>
        <Typography gutterBottom variant="h6">
          Allocation type distribution
        </Typography>
        <Box sx={{ height: 300 }}>
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={summaryData}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="name" />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Bar dataKey="count" radius={[8, 8, 0, 0]}>
                {summaryData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.fill} />
                ))}
              </Bar>
            </BarChart>
          </ResponsiveContainer>
        </Box>
      </Paper>

      <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2 }}>
        <Tabs value={activeTab} onChange={(_, value) => setActiveTab(value)} sx={{ px: 2 }}>
          <Tab label="Billable employees" value="billable" />
          <Tab label="Non-billable employees" value="nonbillable" />
        </Tabs>

        <Box sx={{ p: 3 }}>
          {activeTab === 'billable' && (
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow sx={{ backgroundColor: '#f5f7fb' }}>
                    <TableCell fontWeight={700}>Employee name</TableCell>
                    <TableCell fontWeight={700}>Department</TableCell>
                    <TableCell align="center" fontWeight={700}>
                      Allocation count
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {billableEmployeeDetails.length > 0 ? (
                    billableEmployeeDetails.map((emp) => (
                      <TableRow key={emp.employeeId}>
                        <TableCell>{emp.employeeName}</TableCell>
                        <TableCell>{emp.department}</TableCell>
                        <TableCell align="center">{emp.allocationCount}</TableCell>
                      </TableRow>
                    ))
                  ) : (
                    <TableRow>
                      <TableCell colSpan={3} align="center">
                        No billable employees
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </TableContainer>
          )}

          {activeTab === 'nonbillable' && (
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow sx={{ backgroundColor: '#f5f7fb' }}>
                    <TableCell fontWeight={700}>Employee name</TableCell>
                    <TableCell fontWeight={700}>Department</TableCell>
                    <TableCell align="center" fontWeight={700}>
                      Allocation count
                    </TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {nonbillableEmployeeDetails.length > 0 ? (
                    nonbillableEmployeeDetails.map((emp) => (
                      <TableRow key={emp.employeeId}>
                        <TableCell>{emp.employeeName}</TableCell>
                        <TableCell>{emp.department}</TableCell>
                        <TableCell align="center">{emp.allocationCount}</TableCell>
                      </TableRow>
                    ))
                  ) : (
                    <TableRow>
                      <TableCell colSpan={3} align="center">
                        No non-billable employees
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>
            </TableContainer>
          )}
        </Box>
      </Card>
    </Stack>
  );
}
