import React from 'react';
import {
  DataGrid,
  GridColDef,
  GridActionsCellItem,
  GridRowParams,
  GridToolbarContainer,
  GridToolbarExport,
  GridToolbarFilterButton,
  GridToolbarDensitySelector,
  GridRowSelectionModel,
} from '@mui/x-data-grid';
import { Box, Tooltip } from '@mui/material';
import { Edit, Delete, Visibility } from '@mui/icons-material';

interface DataTableProps {
  rows: any[];
  columns: GridColDef[];
  loading?: boolean;
  onEdit?: (id: string) => void;
  onDelete?: (id: string) => void;
  onView?: (id: string) => void;
  height?: number;
  pageSize?: number;
  checkboxSelection?: boolean;
  onSelectionChange?: (selection: string[]) => void;
}

const DataTable: React.FC<DataTableProps> = ({
  rows,
  columns,
  loading = false,
  onEdit,
  onDelete,
  onView,
  height = 400,
  pageSize = 10,
  checkboxSelection = false,
  onSelectionChange,
}) => {
  const [selectionModel, setSelectionModel] = React.useState<GridRowSelectionModel>([]);

  const handleSelectionChange = (newSelection: GridRowSelectionModel) => {
    setSelectionModel(newSelection);
    onSelectionChange?.(newSelection as string[]);
  };

  const actionColumns: GridColDef[] = [];

  if (onView || onEdit || onDelete) {
    actionColumns.push({
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      getActions: (params: GridRowParams) => {
        const actions = [];

        if (onView) {
          actions.push(
            <GridActionsCellItem
              icon={
                <Tooltip title="View">
                  <Visibility />
                </Tooltip>
              }
              label="View"
              onClick={() => onView(params.id as string)}
            />
          );
        }

        if (onEdit) {
          actions.push(
            <GridActionsCellItem
              icon={
                <Tooltip title="Edit">
                  <Edit />
                </Tooltip>
              }
              label="Edit"
              onClick={() => onEdit(params.id as string)}
            />
          );
        }

        if (onDelete) {
          actions.push(
            <GridActionsCellItem
              icon={
                <Tooltip title="Delete">
                  <Delete />
                </Tooltip>
              }
              label="Delete"
              onClick={() => onDelete(params.id as string)}
            />
          );
        }

        return actions;
      },
    });
  }

  const allColumns = [...columns, ...actionColumns];

  const CustomToolbar = () => (
    <GridToolbarContainer>
      <GridToolbarFilterButton />
      <GridToolbarDensitySelector />
      <GridToolbarExport />
    </GridToolbarContainer>
  );

  return (
    <Box sx={{ height, width: '100%' }}>
      <DataGrid
        rows={rows}
        columns={allColumns}
        loading={loading}
        initialState={{
          pagination: {
            paginationModel: { pageSize: pageSize },
          },
        }}
        pageSizeOptions={[5, 10, 25, 50]}
        checkboxSelection={checkboxSelection}
        onRowSelectionModelChange={handleSelectionChange}
        rowSelectionModel={selectionModel}
        disableRowSelectionOnClick
        slots={{
          toolbar: CustomToolbar,
        }}
        sx={{
          border: 'none',
          '& .MuiDataGrid-cell': {
            borderBottom: '1px solid',
            borderColor: 'divider',
          },
          '& .MuiDataGrid-columnHeaders': {
            backgroundColor: 'grey.50',
            borderBottom: '2px solid',
            borderColor: 'divider',
          },
        }}
      />
    </Box>
  );
};

export default DataTable;
