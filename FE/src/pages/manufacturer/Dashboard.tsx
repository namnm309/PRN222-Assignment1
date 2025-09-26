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
  Business,
  AttachMoney,
} from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import Layout from '../../components/layout/Layout';
import StatCard from '../../components/common/StatCard';
import { reportApi } from '../../services/api';
import SalesChart from '../../components/charts/SalesChart';
import RegionalSalesChart from '../../components/charts/RegionalSalesChart';

const Dashboard: React.FC = () => {
  const { data: stats, isLoading: statsLoading } = useQuery({
    queryKey: ['dashboard-stats'],
    queryFn: () => reportApi.getDashboardStats(),
  });

  const { data: salesChartData, isLoading: chartLoading } = useQuery({
    queryKey: ['sales-chart-data'],
    queryFn: () => reportApi.getSalesChartData(),
  });

  const { data: regionalSalesData, isLoading: regionalChartLoading } = useQuery({
    queryKey: ['regional-sales-data'],
    queryFn: () => reportApi.getRegionalSalesData(),
  });

  const dashboardStats = stats?.data;

  return (
    <Layout title="Manufacturer Dashboard">
      <Box sx={{ mb: 3 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          Manufacturer Dashboard
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Overview of your EV manufacturing and distribution network performance.
        </Typography>
      </Box>

      {/* Stats Cards */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Total Revenue"
            value={dashboardStats?.totalRevenue || 0}
            change={dashboardStats?.revenueGrowth}
            changeLabel="vs last month"
            icon={<AttachMoney />}
            color="success"
            loading={statsLoading}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Vehicles Produced"
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
            title="Active Dealers"
            value={12}
            change={8.5}
            changeLabel="vs last month"
            icon={<Business />}
            color="info"
            loading={statsLoading}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard
            title="Market Growth"
            value="24.5%"
            change={5.2}
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
                Sales Performance Trend
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
                Regional Sales Distribution
              </Typography>
              <RegionalSalesChart
                data={regionalSalesData?.data || []}
                loading={regionalChartLoading}
              />
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Additional Analytics */}
      <Grid container spacing={3} sx={{ mt: 2 }}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Top Performing Dealers
              </Typography>
              <Box sx={{ p: 2, textAlign: 'center', color: 'text.secondary' }}>
                <Typography variant="body2">
                  Top performing dealers will be displayed here
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Inventory Status
              </Typography>
              <Box sx={{ p: 2, textAlign: 'center', color: 'text.secondary' }}>
                <Typography variant="body2">
                  Current inventory status will be displayed here
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
