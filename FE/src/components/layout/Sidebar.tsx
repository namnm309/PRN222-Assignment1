import React from 'react';
import {
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Box,
  Typography,
  Divider,
  Collapse,
} from '@mui/material';
import {
  Dashboard,
  DirectionsCar,
  People,
  ShoppingCart,
  Description,
  Assessment,
  Business,
  Inventory,
  TrendingUp,
  ExpandLess,
  ExpandMore,
  Receipt,
  Schedule,
  Campaign,
} from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { useAppSelector } from '../../hooks/useAppSelector';

interface SidebarProps {
  open: boolean;
  onClose: () => void;
}

interface MenuItem {
  id: string;
  label: string;
  icon: React.ReactNode;
  path?: string;
  children?: MenuItem[];
  roles?: string[];
}

const Sidebar: React.FC<SidebarProps> = ({ onClose }) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, isDealerUser } = useAuth();
  const { sidebarOpen } = useAppSelector(state => state.ui);
  const [expandedItems, setExpandedItems] = React.useState<string[]>([]);

  const handleItemClick = (item: MenuItem) => {
    if (item.children) {
      setExpandedItems(prev =>
        prev.includes(item.id)
          ? prev.filter(id => id !== item.id)
          : [...prev, item.id]
      );
    } else if (item.path) {
      navigate(item.path);
      onClose();
    }
  };

  const isItemActive = (item: MenuItem): boolean => {
    if (item.path) {
      return location.pathname === item.path;
    }
    if (item.children) {
      return item.children.some(child => child.path === location.pathname);
    }
    return false;
  };

  const canAccessItem = (item: MenuItem): boolean => {
    if (!item.roles) return true;
    return user ? item.roles.includes(user.role) : false;
  };

  const dealerMenuItems: MenuItem[] = [
    {
      id: 'dashboard',
      label: 'Dashboard',
      icon: <Dashboard />,
      path: '/dealer/dashboard',
      roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
    },
    {
      id: 'vehicles',
      label: 'Vehicles',
      icon: <DirectionsCar />,
      children: [
        {
          id: 'vehicle-catalog',
          label: 'Vehicle Catalog',
          icon: <DirectionsCar />,
          path: '/dealer/vehicles',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
        {
          id: 'vehicle-comparison',
          label: 'Compare Models',
          icon: <Assessment />,
          path: '/dealer/vehicles/compare',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
      ],
      roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
    },
    {
      id: 'sales',
      label: 'Sales Management',
      icon: <ShoppingCart />,
      children: [
        {
          id: 'quotations',
          label: 'Quotations',
          icon: <Description />,
          path: '/dealer/sales/quotations',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
        {
          id: 'orders',
          label: 'Sales Orders',
          icon: <Receipt />,
          path: '/dealer/sales/orders',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
        {
          id: 'contracts',
          label: 'Sales Contracts',
          icon: <Description />,
          path: '/dealer/sales/contracts',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
        {
          id: 'promotions',
          label: 'Promotions',
          icon: <Campaign />,
          path: '/dealer/sales/promotions',
          roles: ['DEALER_MANAGER'],
        },
      ],
      roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
    },
    {
      id: 'customers',
      label: 'Customer Management',
      icon: <People />,
      children: [
        {
          id: 'customer-list',
          label: 'Customer List',
          icon: <People />,
          path: '/dealer/customers',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
        {
          id: 'test-drives',
          label: 'Test Drives',
          icon: <Schedule />,
          path: '/dealer/customers/test-drives',
          roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
        },
      ],
      roles: ['DEALER_STAFF', 'DEALER_MANAGER'],
    },
    {
      id: 'reports',
      label: 'Reports',
      icon: <Assessment />,
      path: '/dealer/reports',
      roles: ['DEALER_MANAGER'],
    },
  ];

  const manufacturerMenuItems: MenuItem[] = [
    {
      id: 'dashboard',
      label: 'Dashboard',
      icon: <Dashboard />,
      path: '/manufacturer/dashboard',
      roles: ['EVM_STAFF', 'ADMIN'],
    },
    {
      id: 'products',
      label: 'Product Management',
      icon: <DirectionsCar />,
      children: [
        {
          id: 'vehicle-catalog',
          label: 'Vehicle Catalog',
          icon: <DirectionsCar />,
          path: '/manufacturer/products/vehicles',
          roles: ['EVM_STAFF', 'ADMIN'],
        },
        {
          id: 'inventory',
          label: 'Inventory Management',
          icon: <Inventory />,
          path: '/manufacturer/products/inventory',
          roles: ['EVM_STAFF', 'ADMIN'],
        },
        {
          id: 'pricing',
          label: 'Pricing & Promotions',
          icon: <TrendingUp />,
          path: '/manufacturer/products/pricing',
          roles: ['EVM_STAFF', 'ADMIN'],
        },
      ],
      roles: ['EVM_STAFF', 'ADMIN'],
    },
    {
      id: 'dealers',
      label: 'Dealer Management',
      icon: <Business />,
      children: [
        {
          id: 'dealer-list',
          label: 'Dealer List',
          icon: <Business />,
          path: '/manufacturer/dealers',
          roles: ['EVM_STAFF', 'ADMIN'],
        },
        {
          id: 'dealer-accounts',
          label: 'Dealer Accounts',
          icon: <People />,
          path: '/manufacturer/dealers/accounts',
          roles: ['ADMIN'],
        },
      ],
      roles: ['EVM_STAFF', 'ADMIN'],
    },
    {
      id: 'analytics',
      label: 'Analytics & Reports',
      icon: <Assessment />,
      children: [
        {
          id: 'sales-analytics',
          label: 'Sales Analytics',
          icon: <TrendingUp />,
          path: '/manufacturer/analytics/sales',
          roles: ['EVM_STAFF', 'ADMIN'],
        },
        {
          id: 'demand-forecasting',
          label: 'Demand Forecasting',
          icon: <Assessment />,
          path: '/manufacturer/analytics/forecasting',
          roles: ['EVM_STAFF', 'ADMIN'],
        },
      ],
      roles: ['EVM_STAFF', 'ADMIN'],
    },
  ];

  const menuItems = isDealerUser() ? dealerMenuItems : manufacturerMenuItems;

  const renderMenuItem = (item: MenuItem, level = 0) => {
    if (!canAccessItem(item)) return null;

    const isActive = isItemActive(item);
    const isExpanded = expandedItems.includes(item.id);
    const hasChildren = item.children && item.children.length > 0;

    return (
      <React.Fragment key={item.id}>
        <ListItem disablePadding>
          <ListItemButton
            onClick={() => handleItemClick(item)}
            sx={{
              pl: 2 + level * 2,
              backgroundColor: isActive ? 'primary.light' : 'transparent',
              color: isActive ? 'primary.contrastText' : 'inherit',
              '&:hover': {
                backgroundColor: isActive ? 'primary.light' : 'action.hover',
              },
            }}
          >
            <ListItemIcon
              sx={{
                color: isActive ? 'primary.contrastText' : 'inherit',
                minWidth: 40,
              }}
            >
              {item.icon}
            </ListItemIcon>
            <ListItemText
              primary={item.label}
              primaryTypographyProps={{
                fontSize: '0.875rem',
                fontWeight: isActive ? 600 : 400,
              }}
            />
            {hasChildren && (
              <Box sx={{ ml: 1 }}>
                {isExpanded ? <ExpandLess /> : <ExpandMore />}
              </Box>
            )}
          </ListItemButton>
        </ListItem>
        {hasChildren && (
          <Collapse in={isExpanded} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
              {item.children?.map(child => renderMenuItem(child, level + 1))}
            </List>
          </Collapse>
        )}
      </React.Fragment>
    );
  };

  return (
    <Drawer
      variant="persistent"
      anchor="left"
      open={sidebarOpen}
      sx={{
        width: 280,
        flexShrink: 0,
        '& .MuiDrawer-paper': {
          width: 280,
          boxSizing: 'border-box',
          borderRight: '1px solid',
          borderColor: 'divider',
          backgroundColor: 'background.paper',
        },
      }}
    >
      <Box sx={{ p: 2 }}>
        <Typography variant="h6" noWrap component="div" color="primary">
          EV Dealer System
        </Typography>
        <Typography variant="body2" color="text.secondary">
          {isDealerUser() ? 'Dealer Portal' : 'Manufacturer Portal'}
        </Typography>
      </Box>
      <Divider />
      <List sx={{ pt: 1 }}>
        {menuItems.map(item => renderMenuItem(item))}
      </List>
    </Drawer>
  );
};

export default Sidebar;
