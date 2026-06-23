
import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material';
import type { Employee } from '../types/employee';

interface DeleteEmployeeDialogProps {
  employee: Employee | null;
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
}

export default function DeleteEmployeeDialog({
  employee,
  open,
  onClose,
  onConfirm,
}: DeleteEmployeeDialogProps) {
  return (
    <Dialog fullWidth maxWidth="xs" onClose={onClose} open={open}>
      <DialogTitle>Delete employee</DialogTitle>
      <DialogContent>
        <DialogContentText>
            {employee
              ? `Delete ${employee.fullName}? This action cannot be undone.`
              : 'Delete this employee?'}
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Cancel</Button>
        <Button color="error" onClick={onConfirm} variant="contained">
          Delete
        </Button>
      </DialogActions>
    </Dialog>
  );
}
