import { Skeleton, TableBody, TableCell, TableRow } from '@mui/material';

interface TableLoaderProps {
  columns: number;
  rows?: number;
}

export default function TableLoader({ columns, rows = 8 }: TableLoaderProps) {
  return (
    <TableBody>
      {Array.from({ length: rows }).map((_, rowIdx) => (
        <TableRow key={rowIdx}>
          {Array.from({ length: columns }).map((_, colIdx) => (
            <TableCell key={colIdx}>
              <Skeleton
                animation="wave"
                height={20}
                variant="text"
                width={colIdx === 0 ? '65%' : colIdx === columns - 1 ? '50%' : '90%'}
              />
            </TableCell>
          ))}
        </TableRow>
      ))}
    </TableBody>
  );
}
