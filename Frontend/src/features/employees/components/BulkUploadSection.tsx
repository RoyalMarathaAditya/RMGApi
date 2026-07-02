import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import {
  Alert,
  Box,
  Button,
  LinearProgress,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import { useRef, useState } from 'react';
import { employeeService } from '../services/employeeService';
import type { BulkUploadResponse } from '../services/employeeService';

interface UploadResult {
  success: boolean;
  totalRows: number;
  successRows: number;
  failedRows: number;
  errors: Array<{ rowNumber: number; employeeName: string | null; email: string | null; errorMessage: string }>;
  errorFileUrl: string | null;
}

export default function BulkUploadSection({
  onImportComplete,
}: {
  onImportComplete?: (result?: BulkUploadResponse) => void;
}) {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [progress, setProgress] = useState(0);
  const [result, setResult] = useState<UploadResult | null>(null);
  const [dragOver, setDragOver] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const isValidFile = (f: File) => {
    const ext = f.name.split('.').pop()?.toLowerCase();
    return (ext === 'xlsx' || ext === 'xls') && f.size <= 10 * 1024 * 1024;
  };

  const handleFile = (f: File) => {
    if (!isValidFile(f)) {
      setResult({
        success: false,
        totalRows: 0,
        successRows: 0,
        failedRows: 0,
        errors: [{ rowNumber: 0, employeeName: null, email: null, errorMessage: 'Invalid file. Only .xlsx/.xls files up to 10 MB are accepted.' }],
        errorFileUrl: null,
      });
      return;
    }
    setFile(f);
    setResult(null);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setDragOver(false);
    const f = e.dataTransfer.files[0];
    if (f) handleFile(f);
  };

  const handleBrowse = () => inputRef.current?.click();

  const handleUpload = async () => {
    if (!file) return;
    setUploading(true);
    setProgress(0);
    setResult(null);

    try {
      const res = await employeeService.uploadEmployees(file, setProgress);
      setResult(res);
      if (res.success) {
        if (onImportComplete) onImportComplete(res);
      }
    } catch (err: any) {
      const serverMsg = err?.response?.data?.message || err?.response?.data?.title || err?.message || 'Upload failed. Please try again.';
      setResult({
        success: false,
        totalRows: 0,
        successRows: 0,
        failedRows: 0,
        errors: [{ rowNumber: 0, employeeName: null, email: null, errorMessage: serverMsg }],
        errorFileUrl: null,
      });
    } finally {
      setUploading(false);
    }
  };

  const handleClear = () => {
    setFile(null);
    setResult(null);
    setProgress(0);
    if (inputRef.current) inputRef.current.value = '';
  };

  return (
    <Paper sx={{ p: 3, mb: 3 }}>
      <Typography variant="h6" fontWeight={600} gutterBottom>
        Bulk Employee Upload
      </Typography>

      <Box
        onDragOver={(e) => { e.preventDefault(); setDragOver(true); }}
        onDragLeave={() => setDragOver(false)}
        onDrop={handleDrop}
        sx={{
          border: '2px dashed',
          borderColor: dragOver ? 'primary.main' : 'grey.300',
          borderRadius: 2,
          bgcolor: dragOver ? 'action.hover' : 'background.paper',
          p: 4,
          textAlign: 'center',
          cursor: 'pointer',
          transition: 'all 0.2s',
          mb: 2,
        }}
        onClick={handleBrowse}
      >
        <CloudUploadIcon sx={{ fontSize: 48, color: 'text.secondary', mb: 1 }} />
        <Typography color="text.secondary">
          Drag & Drop Excel File Here
        </Typography>
        <Typography variant="body2" color="text.disabled" sx={{ mt: 0.5 }}>
          OR
        </Typography>
        <Button variant="outlined" sx={{ mt: 1 }} onClick={(e) => { e.stopPropagation(); handleBrowse(); }}>
          Browse File
        </Button>
        <input
          ref={inputRef}
          type="file"
          accept=".xlsx,.xls"
          hidden
          onChange={(e) => { const f = e.target.files?.[0]; if (f) handleFile(f); }}
        />
      </Box>

      {file && (
        <Stack spacing={2}>
          <Typography variant="body2" color="text.secondary">
            Selected File: <strong>{file.name}</strong> ({(file.size / 1024).toFixed(1)} KB)
          </Typography>

          <Stack direction="row" spacing={1}>
            <Button variant="contained" size="small" onClick={handleUpload} disabled={uploading}>
              {uploading ? 'Uploading...' : 'Upload'}
            </Button>
            <Button variant="text" size="small" onClick={handleClear} disabled={uploading}>
              Clear
            </Button>
          </Stack>

          {uploading && (
            <Box>
              <LinearProgress variant="determinate" value={progress} />
              <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5 }}>
                {progress}%
              </Typography>
            </Box>
          )}

          {result && (
            <Stack spacing={1}>
              {result.success ? (
                <Alert severity="success">
                  Import completed. {result.successRows} of {result.totalRows} employees imported successfully.
                  {result.failedRows > 0 && ` ${result.failedRows} rows failed.`}
                </Alert>
              ) : (
                <Alert severity="error">
                  Import failed. {result.failedRows} of {result.totalRows} rows have errors.
                </Alert>
              )}

              {result.errors.length > 0 && (
                <TableContainer component={Paper} variant="outlined">
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Row</TableCell>
                        <TableCell>Employee</TableCell>
                        <TableCell>Email</TableCell>
                        <TableCell>Error</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {result.errors.map((err, i) => (
                        <TableRow key={i}>
                          <TableCell>{err.rowNumber}</TableCell>
                          <TableCell>{err.employeeName ?? '-'}</TableCell>
                          <TableCell>{err.email ?? '-'}</TableCell>
                          <TableCell>{err.errorMessage}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              )}

              {result.errorFileUrl && (
                <Button
                  variant="outlined"
                  size="small"
                  href={result.errorFileUrl}
                  target="_blank"
                >
                  Download Error Report
                </Button>
              )}
            </Stack>
          )}
        </Stack>
      )}
    </Paper>
  );
}
