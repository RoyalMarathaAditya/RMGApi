
import { Dialog, DialogContent, DialogTitle } from '@mui/material';
import EmployeeForm from './EmployeeForm';
import type { Employee, EmployeeFormValues } from '../types/employee';

interface EmployeeDialogProps {
  employee: Employee | null;
  open: boolean;
  onClose: () => void;
  onSubmit: (values: EmployeeFormValues) => Promise<void> | void;
}

export default function EmployeeDialog({ employee, open, onClose, onSubmit }: EmployeeDialogProps) {
  return (
    <Dialog fullWidth maxWidth="md" onClose={onClose} open={open}>
      <DialogTitle>{employee ? 'Edit employee' : 'Add employee'}</DialogTitle>
      <DialogContent>
        <EmployeeForm employee={employee} onCancel={onClose} onSubmit={onSubmit} />
      </DialogContent>
    </Dialog>
  );
}
