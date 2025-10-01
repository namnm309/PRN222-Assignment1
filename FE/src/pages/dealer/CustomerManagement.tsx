import React, { useState } from 'react';
import {
  Box,
  Typography,
  Button,
  TextField,
  InputAdornment,
  Grid,
} from '@mui/material';
import {
  Add,
  Search,
} from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import Layout from '../../components/layout/Layout';
import DataTable from '../../components/common/DataTable';
import Modal from '../../components/common/Modal';
import CustomerForm from '../../components/forms/CustomerForm';
import { customerApi } from '../../services/api';
import { Customer } from '../../types';

const CustomerManagement: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCustomer, setEditingCustomer] = useState<Customer | null>(null);
  const [viewingCustomer, setViewingCustomer] = useState<Customer | null>(null);

  const { data: customersResponse, isLoading, refetch } = useQuery({
    queryKey: ['customers'],
    queryFn: () => customerApi.getCustomers(),
  });

  const customers = customersResponse?.data || [];

  const filteredCustomers = customers.filter(customer =>
    customer.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    customer.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    customer.phone.includes(searchTerm)
  );

  const columns = [
    {
      field: 'name',
      headerName: 'Name',
      width: 200,
      renderCell: (params: any) => (
        <Box>
          <Typography variant="body2" fontWeight="medium">
            {params.value}
          </Typography>
          <Typography variant="caption" color="text.secondary">
            {params.row.email}
          </Typography>
        </Box>
      ),
    },
    {
      field: 'phone',
      headerName: 'Phone',
      width: 150,
    },
    {
      field: 'address',
      headerName: 'Address',
      width: 250,
      renderCell: (params: any) => (
        <Typography variant="body2" noWrap>
          {params.value}
        </Typography>
      ),
    },
    {
      field: 'dateOfBirth',
      headerName: 'Date of Birth',
      width: 120,
      renderCell: (params: any) => (
        <Typography variant="body2">
          {new Date(params.value).toLocaleDateString()}
        </Typography>
      ),
    },
    {
      field: 'licenseNumber',
      headerName: 'License',
      width: 150,
    },
    {
      field: 'createdAt',
      headerName: 'Customer Since',
      width: 150,
      renderCell: (params: any) => (
        <Typography variant="body2">
          {new Date(params.value).toLocaleDateString()}
        </Typography>
      ),
    },
  ];

  const handleAddCustomer = () => {
    setEditingCustomer(null);
    setIsModalOpen(true);
  };

  const handleEditCustomer = (id: string) => {
    const customer = customers.find(c => c.id === id);
    if (customer) {
      setEditingCustomer(customer);
      setIsModalOpen(true);
    }
  };

  const handleViewCustomer = (id: string) => {
    const customer = customers.find(c => c.id === id);
    if (customer) {
      setViewingCustomer(customer);
    }
  };

  const handleDeleteCustomer = (id: string) => {
    // Implement delete functionality
    console.log('Delete customer:', id);
  };

  const handleModalClose = () => {
    setIsModalOpen(false);
    setEditingCustomer(null);
  };

  const handleFormSubmit = async (formData: any) => {
    try {
      if (editingCustomer) {
        await customerApi.updateCustomer(editingCustomer.id, formData);
      } else {
        await customerApi.createCustomer(formData);
      }
      refetch();
      handleModalClose();
    } catch (error) {
      console.error('Error saving customer:', error);
    }
  };

  return (
    <Layout title="Customer Management">
      <Box sx={{ mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Box>
            <Typography variant="h4" component="h1" gutterBottom>
              Customer Management
            </Typography>
            <Typography variant="body1" color="text.secondary">
              Manage your customer database and relationships
            </Typography>
          </Box>
          <Button
            variant="contained"
            startIcon={<Add />}
            onClick={handleAddCustomer}
          >
            Add Customer
          </Button>
        </Box>

        <Box sx={{ display: 'flex', gap: 2, mb: 3 }}>
          <TextField
            placeholder="Search customers..."
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

      <DataTable
        rows={filteredCustomers}
        columns={columns}
        loading={isLoading}
        onEdit={handleEditCustomer}
        onDelete={handleDeleteCustomer}
        onView={handleViewCustomer}
        height={600}
      />

      {/* Add/Edit Customer Modal */}
      <Modal
        open={isModalOpen}
        onClose={handleModalClose}
        title={editingCustomer ? 'Edit Customer' : 'Add New Customer'}
        maxWidth="md"
      >
        <CustomerForm
          customer={editingCustomer}
          onSubmit={handleFormSubmit}
          onCancel={handleModalClose}
        />
      </Modal>

      {/* View Customer Modal */}
      <Modal
        open={!!viewingCustomer}
        onClose={() => setViewingCustomer(null)}
        title="Customer Details"
        maxWidth="md"
      >
        {viewingCustomer && (
          <Box sx={{ p: 2 }}>
            <Grid container spacing={2}>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Name
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {viewingCustomer.name}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Email
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {viewingCustomer.email}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Phone
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {viewingCustomer.phone}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Date of Birth
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {new Date(viewingCustomer.dateOfBirth).toLocaleDateString()}
                </Typography>
              </Grid>
              <Grid item xs={12}>
                <Typography variant="subtitle2" color="text.secondary">
                  Address
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {viewingCustomer.address}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  License Number
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {viewingCustomer.licenseNumber}
                </Typography>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Typography variant="subtitle2" color="text.secondary">
                  Customer Since
                </Typography>
                <Typography variant="body1" gutterBottom>
                  {new Date(viewingCustomer.createdAt).toLocaleDateString()}
                </Typography>
              </Grid>
            </Grid>
          </Box>
        )}
      </Modal>
    </Layout>
  );
};

export default CustomerManagement;
