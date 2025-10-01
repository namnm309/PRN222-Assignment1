import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { useAuth } from './hooks/useAuth';
import ProtectedRoute from './components/auth/ProtectedRoute';
import LoginForm from './components/auth/LoginForm';

// Dealer Pages
import DealerDashboard from './pages/dealer/Dashboard';
import VehicleCatalog from './pages/dealer/VehicleCatalog';
import CustomerManagement from './pages/dealer/CustomerManagement';
import SalesOrders from './pages/dealer/SalesOrders';

// Manufacturer Pages
import ManufacturerDashboard from './pages/manufacturer/Dashboard';

// Placeholder components for other pages
const PlaceholderPage: React.FC<{ title: string }> = ({ title }) => (
  <div style={{ padding: '2rem', textAlign: 'center' }}>
    <h2>{title}</h2>
    <p>This page is under construction.</p>
  </div>
);

const App: React.FC = () => {
  const { isAuthenticated, user } = useAuth();

  // Simulate checking for existing session on app load
  useQuery({
    queryKey: ['current-user'],
    queryFn: async () => {
      if (isAuthenticated && user) {
        return user;
      }
      return null;
    },
    enabled: false, // Don't auto-fetch, just for type safety
  });

  return (
    <Routes>
      {/* Public Routes */}
      <Route
        path="/login"
        element={
          <ProtectedRoute requireAuth={false}>
            <LoginForm />
          </ProtectedRoute>
        }
      />

      {/* Dealer Routes */}
      <Route
        path="/dealer/*"
        element={
          <ProtectedRoute allowedRoles={['DEALER_STAFF', 'DEALER_MANAGER']}>
            <Routes>
              <Route path="dashboard" element={<DealerDashboard />} />
              <Route path="vehicles" element={<VehicleCatalog />} />
              <Route path="vehicles/compare" element={<PlaceholderPage title="Vehicle Comparison" />} />
              <Route path="sales/quotations" element={<PlaceholderPage title="Quotations" />} />
              <Route path="sales/orders" element={<SalesOrders />} />
              <Route path="sales/contracts" element={<PlaceholderPage title="Sales Contracts" />} />
              <Route path="sales/promotions" element={<PlaceholderPage title="Promotions" />} />
              <Route path="customers" element={<CustomerManagement />} />
              <Route path="customers/test-drives" element={<PlaceholderPage title="Test Drives" />} />
              <Route path="reports" element={<PlaceholderPage title="Reports" />} />
              <Route path="*" element={<Navigate to="/dealer/dashboard" replace />} />
            </Routes>
          </ProtectedRoute>
        }
      />

      {/* Manufacturer Routes */}
      <Route
        path="/manufacturer/*"
        element={
          <ProtectedRoute allowedRoles={['EVM_STAFF', 'ADMIN']}>
            <Routes>
              <Route path="dashboard" element={<ManufacturerDashboard />} />
              <Route path="products/vehicles" element={<PlaceholderPage title="Vehicle Catalog Management" />} />
              <Route path="products/inventory" element={<PlaceholderPage title="Inventory Management" />} />
              <Route path="products/pricing" element={<PlaceholderPage title="Pricing & Promotions" />} />
              <Route path="dealers" element={<PlaceholderPage title="Dealer Management" />} />
              <Route path="dealers/accounts" element={<PlaceholderPage title="Dealer Accounts" />} />
              <Route path="analytics/sales" element={<PlaceholderPage title="Sales Analytics" />} />
              <Route path="analytics/forecasting" element={<PlaceholderPage title="Demand Forecasting" />} />
              <Route path="*" element={<Navigate to="/manufacturer/dashboard" replace />} />
            </Routes>
          </ProtectedRoute>
        }
      />

      {/* Default Route */}
      <Route
        path="/"
        element={
          <ProtectedRoute>
            {user?.role === 'DEALER_STAFF' || user?.role === 'DEALER_MANAGER' ? (
              <Navigate to="/dealer/dashboard" replace />
            ) : (
              <Navigate to="/manufacturer/dashboard" replace />
            )}
          </ProtectedRoute>
        }
      />

      {/* Catch all route */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

export default App;
