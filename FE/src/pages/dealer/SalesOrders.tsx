import React, { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  TextField,
  InputAdornment,
  Chip,
  Card,
  CardContent,
  Grid,
} from '@mui/material';
import {
  Add,
  Search,
  Receipt,
  LocalShipping,
  CheckCircle,
  Cancel,
} from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import Layout from '../../components/layout/Layout';
import DataTable from '../../components/common/DataTable';
import { salesApi } from '../../services/api';

const SalesOrders: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');

  const { data: ordersResponse, isLoading } = useQuery({
    queryKey: ['sales-orders'],
    queryFn: () => salesApi.getSalesOrders(),
  });

  const orders = ordersResponse?.data || [];

  const filteredOrders = orders.filter(order =>
    order.id.toLowerCase().includes(searchTerm.toLowerCase()) ||
    order.customerId.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'DELIVERED':
        return 'success';
      case 'SHIPPED':
        return 'info';
      case 'IN_PRODUCTION':
        return 'warning';
      case 'CONFIRMED':
        return 'primary';
      case 'PENDING':
        return 'default';
      case 'CANCELLED':
        return 'error';
      default:
        return 'default';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'DELIVERED':
        return <CheckCircle />;
      case 'SHIPPED':
        return <LocalShipping />;
      case 'CANCELLED':
        return <Cancel />;
      default:
        return <Receipt />;
    }
  };

  const columns = [
    {
      field: 'id',
      headerName: 'Order ID',
      width: 150,
    },
    {
      field: 'customerId',
      headerName: 'Customer',
      width: 200,
    },
    {
      field: 'vehicleId',
      headerName: 'Vehicle',
      width: 200,
    },
    {
      field: 'orderDate',
      headerName: 'Order Date',
      width: 150,
      renderCell: (params: any) => (
        <Typography variant="body2">
          {new Date(params.value).toLocaleDateString()}
        </Typography>
      ),
    },
    {
      field: 'deliveryDate',
      headerName: 'Delivery Date',
      width: 150,
      renderCell: (params: any) => (
        <Typography variant="body2">
          {new Date(params.value).toLocaleDateString()}
        </Typography>
      ),
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 150,
      renderCell: (params: any) => (
        <Chip
          icon={getStatusIcon(params.value)}
          label={params.value.replace('_', ' ')}
          color={getStatusColor(params.value) as any}
          size="small"
        />
      ),
    },
    {
      field: 'paymentStatus',
      headerName: 'Payment',
      width: 120,
      renderCell: (params: any) => (
        <Chip
          label={params.value.replace('_', ' ')}
          color={params.value === 'FULL' ? 'success' : params.value === 'PARTIAL' ? 'warning' : 'error'}
          size="small"
        />
      ),
    },
    {
      field: 'totalAmount',
      headerName: 'Total Amount',
      width: 150,
      renderCell: (params: any) => (
        <Typography variant="body2" fontWeight="medium">
          ${params.value.toLocaleString()}
        </Typography>
      ),
    },
  ];

  const handleViewOrder = (id: string) => {
    console.log('View order:', id);
  };

  const handleEditOrder = (id: string) => {
    console.log('Edit order:', id);
  };

  const handleDeleteOrder = (id: string) => {
    console.log('Delete order:', id);
  };

  return (
    <Layout title="Sales Orders">
      <Box sx={{ mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Box>
            <Typography variant="h4" component="h1" gutterBottom>
              Sales Orders
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage and track all sales orders
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={() => console.log('Add new order')}
          >
            New Order
          </Button>
        </Box>

        <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
          <TextField
            placeholder="Search orders..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <Search />
                </InputAdornment>
              ),
            }}
            sx={{ flexGrow: 1 }}
          />
        </Box>
      </Box>

      {/* Order Statistics */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Receipt sx={{ mr: 2, color: 'primary.main' }} />
                <Box>
                  <Typography variant="h6">{orders.length}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    Total Orders
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <CheckCircle sx={{ mr: 2, color: 'success.main' }} />
                <Box>
                  <Typography variant="h6">
                    {orders.filter(o => o.status === 'DELIVERED').length}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Delivered
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <LocalShipping sx={{ mr: 2, color: 'info.main' }} />
                <Box>
                  <Typography variant="h6">
                    {orders.filter(o => o.status === 'SHIPPED').length}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    In Transit
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                <Cancel sx={{ mr: 2, color: 'error.main' }} />
                <Box>
                  <Typography variant="h6">
                    {orders.filter(o => o.status === 'CANCELLED').length}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    Cancelled
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      <DataTable
        rows={filteredOrders}
        columns={columns}
        loading={isLoading}
        onView={handleViewOrder}
        onEdit={handleEditOrder}
        onDelete={handleDeleteOrder}
        height={600}
      />
    </Layout>
  );
};

export default SalesOrders;
