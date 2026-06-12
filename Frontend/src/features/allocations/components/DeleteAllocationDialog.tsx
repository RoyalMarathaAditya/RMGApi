import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material';

interface DeleteAllocationDialogProps {
  open: boolean;
  allocationName: string;
  onConfirm: () => void;
  onClose: () => void;
}

export default function DeleteAllocationDialog({ open, allocationName, onConfirm, onClose }: DeleteAllocationDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Confirm deletion</DialogTitle>
      <DialogContent>
        <DialogContentText>
          Are you sure you want to remove the allocation for <strong>{allocationName}</strong>? This action cannot be undone.
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
