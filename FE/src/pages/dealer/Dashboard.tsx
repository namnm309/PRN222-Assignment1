import React from 'react';
import {
  Grid,
  Typography,
  Box,
  Card,
  CardContent,
} from '@mui/material';
import {
  TrendingUp,
  DirectionsCar,
  People,
  AttachMoney,
} from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import Layout from '../../components/layout/Layout';
import StatCard from '../../components/common/StatCard';
import { reportApi } from '../../services/api';
import SalesChart from '../../components/charts/SalesChart';
import VehicleSalesChart from '../../components/charts/VehicleSalesChart';

const Dashboard: React.FC = () => {
  const { data: stats, isLoading: statsLoading } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: () => reportApi.getDashboardStats(),
  });

  const { data: salesChartData, isLoading: chartLoading } = useQuery({
    queryKey: ['sales-chart-data'],
    queryFn: () => reportApi.getSalesChartData(),
  });

  const { data: vehicleSalesData, isLoading: vehicleChartLoading } = useQuery({
    queryKey: ['vehicle-sales-data'],
    queryFn: () => reportApi.getVehicleSalesData(),
  });

  const dashboardStats = stats?.data;

  return (
    <Layout title="Dealer Dashboard">
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Dashboard
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Welcome to your dealer dashboard. Here's an overview of your business performance.
        </Typography>
      </Box>

      {/* Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Sales"
            value={dashboardStats?.totalSales || 0}
            change={dashboardStats?.salesGrowth}
            changeLabel="vs last month"
            icon={<AttachMoney />}
            color="success"
            loading={statsLoading}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Vehicles Sold"
            value={dashboardStats?.totalVehicles || 0}
            change={dashboardStats?.vehicleGrowth}
            changeLabel="vs last month"
            icon={<DirectionsCar />}
            color="primary"
            loading={statsLoading}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Customers"
            value={dashboardStats?.totalCustomers || 0}
            change={dashboardStats?.customerGrowth}
            changeLabel="vs last month"
            icon={<People />}
            color="info"
            loading={statsLoading}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Revenue"
            value={dashboardStats?.totalRevenue || 0}
            change={dashboardStats?.revenueGrowth}
            changeLabel="vs last month"
            icon={<TrendingUp />}
            color="warning"
            loading={statsLoading}
          />
        </Grid>
      </Grid>

      {/* Charts */}
      <Grid container spacing={3}>
        <Grid item xs={12} lg={8}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Sales Trend
              </Typography>
              <SalesChart
                data={salesChartData?.data || []}
                loading={chartLoading}
              />
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} lg={4}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Vehicle Sales Distribution
              </Typography>
              <VehicleSalesChart
                data={vehicleSalesData?.data || []}
                loading={vehicleChartLoading}
              />
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Recent Activity */}
      <Grid container spacing={3} sx={{ mt: 2 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Recent Sales Orders
              </Typography>
              <Box sx={{ p: 2, textAlign: 'center', color: 'text.secondary' }}>
                <Typography variant="body2">
                  Recent sales orders will be displayed here
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Upcoming Test Drives
              </Typography>
              <Box sx={{ p: 2, textAlign: 'center', color: 'text.secondary' }}>
                <Typography variant="body2">
                  Upcoming test drives will be displayed here
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Layout>
  );
};

export default Dashboard;
