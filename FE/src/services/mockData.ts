import { 
  User, 
  Vehicle, 
  Customer, 
  Quotation, 
  SalesOrder, 
  SalesContract, 
  Dealer, 
  TestDrive, 
  Promotion, 
  SalesReport, 
  InventoryReport,
  DashboardStats,
  ChartData,
  TimeSeriesData
} from '../types';

// Mock Users
export const mockUsers: User[] = [
  {
    id: '1',
    email: 'admin@evm.com',
    name: 'Admin User',
    role: 'ADMIN',
    avatar: '/avatars/admin.jpg'
  },
  {
    id: '2',
    email: 'evm.staff@evm.com',
    name: 'EVM Staff',
    role: 'EVM_STAFF',
    avatar: '/avatars/evm-staff.jpg'
  },
  {
    id: '3',
    email: 'manager@dealer1.com',
    name: 'Dealer Manager',
    role: 'DEALER_MANAGER',
    dealerId: 'dealer1',
    avatar: '/avatars/manager.jpg'
  },
  {
    id: '4',
    email: 'staff@dealer1.com',
    name: 'Dealer Staff',
    role: 'DEALER_STAFF',
    dealerId: 'dealer1',
    avatar: '/avatars/staff.jpg'
  }
];

// Mock Dealers
export const mockDealers: Dealer[] = [
  {
    id: 'dealer1',
    name: 'Green Auto Dealership',
    address: '123 Main Street',
    city: 'New York',
    state: 'NY',
    zipCode: '10001',
    phone: '+1-555-0123',
    email: 'info@greenauto.com',
    managerId: '3',
    salesTarget: 1000000,
    currentSales: 750000,
    debt: 50000,
    status: 'ACTIVE',
    createdAt: '2023-01-15T00:00:00Z'
  },
  {
    id: 'dealer2',
    name: 'Eco Motors',
    address: '456 Oak Avenue',
    city: 'Los Angeles',
    state: 'CA',
    zipCode: '90210',
    phone: '+1-555-0456',
    email: 'contact@ecomotors.com',
    managerId: '5',
    salesTarget: 800000,
    currentSales: 600000,
    debt: 25000,
    status: 'ACTIVE',
    createdAt: '2023-02-20T00:00:00Z'
  }
];

// Mock Vehicles
export const mockVehicles: Vehicle[] = [
  {
    id: 'vehicle1',
    model: 'Tesla Model 3',
    version: 'Standard Range Plus',
    color: 'Pearl White',
    year: 2024,
    basePrice: 45000,
    features: ['Autopilot', 'Premium Interior', 'Supercharging', 'Over-the-Air Updates'],
    specifications: {
      batteryCapacity: '54 kWh',
      range: '263 miles',
      chargingTime: '8 hours (Level 2)',
      power: '283 hp',
      acceleration: '5.3 seconds 0-60 mph',
      topSpeed: '140 mph',
      weight: '3,627 lbs',
      dimensions: '184.8" L x 72.8" W x 56.8" H'
    },
    images: ['/vehicles/tesla-model3-1.jpg', '/vehicles/tesla-model3-2.jpg'],
    stock: 15,
    dealerId: 'dealer1'
  },
  {
    id: 'vehicle2',
    model: 'Tesla Model Y',
    version: 'Long Range',
    color: 'Midnight Silver',
    year: 2024,
    basePrice: 55000,
    features: ['Autopilot', 'Premium Interior', 'Supercharging', 'Over-the-Air Updates', '7-Seat Option'],
    specifications: {
      batteryCapacity: '75 kWh',
      range: '326 miles',
      chargingTime: '10 hours (Level 2)',
      power: '384 hp',
      acceleration: '4.8 seconds 0-60 mph',
      topSpeed: '135 mph',
      weight: '4,416 lbs',
      dimensions: '187.0" L x 75.6" W x 63.9" H'
    },
    images: ['/vehicles/tesla-modely-1.jpg', '/vehicles/tesla-modely-2.jpg'],
    stock: 8,
    dealerId: 'dealer1'
  },
  {
    id: 'vehicle3',
    model: 'BMW i4',
    version: 'eDrive40',
    color: 'Mineral Gray',
    year: 2024,
    basePrice: 56000,
    features: ['BMW iDrive 8', 'Premium Sound System', 'Adaptive Cruise Control', 'Wireless Charging'],
    specifications: {
      batteryCapacity: '83.9 kWh',
      range: '300 miles',
      chargingTime: '8.5 hours (Level 2)',
      power: '335 hp',
      acceleration: '5.5 seconds 0-60 mph',
      topSpeed: '118 mph',
      weight: '4,680 lbs',
      dimensions: '188.3" L x 72.9" W x 57.0" H'
    },
    images: ['/vehicles/bmw-i4-1.jpg', '/vehicles/bmw-i4-2.jpg'],
    stock: 12,
    dealerId: 'dealer2'
  }
];

// Mock Customers
export const mockCustomers: Customer[] = [
  {
    id: 'customer1',
    name: 'John Smith',
    email: 'john.smith@email.com',
    phone: '+1-555-0101',
    address: '789 Pine Street, New York, NY 10002',
    dateOfBirth: '1985-03-15',
    licenseNumber: 'DL123456789',
    dealerId: 'dealer1',
    createdAt: '2023-06-01T00:00:00Z',
    updatedAt: '2023-06-01T00:00:00Z'
  },
  {
    id: 'customer2',
    name: 'Sarah Johnson',
    email: 'sarah.johnson@email.com',
    phone: '+1-555-0102',
    address: '321 Elm Avenue, Los Angeles, CA 90211',
    dateOfBirth: '1990-07-22',
    licenseNumber: 'DL987654321',
    dealerId: 'dealer2',
    createdAt: '2023-06-15T00:00:00Z',
    updatedAt: '2023-06-15T00:00:00Z'
  }
];

// Mock Quotations
export const mockQuotations: Quotation[] = [
  {
    id: 'quote1',
    customerId: 'customer1',
    vehicleId: 'vehicle1',
    dealerId: 'dealer1',
    salespersonId: '4',
    basePrice: 45000,
    discount: 2000,
    finalPrice: 43000,
    status: 'SENT',
    validUntil: '2024-02-15T00:00:00Z',
    createdAt: '2024-01-15T00:00:00Z',
    notes: 'Customer interested in financing options'
  }
];

// Mock Sales Orders
export const mockSalesOrders: SalesOrder[] = [
  {
    id: 'order1',
    quotationId: 'quote1',
    customerId: 'customer1',
    vehicleId: 'vehicle1',
    dealerId: 'dealer1',
    salespersonId: '4',
    orderDate: '2024-01-20T00:00:00Z',
    deliveryDate: '2024-02-15T00:00:00Z',
    status: 'CONFIRMED',
    paymentStatus: 'PARTIAL',
    totalAmount: 43000,
    paidAmount: 15000,
    notes: 'Customer paid deposit, financing approved'
  }
];

// Mock Sales Contracts
export const mockSalesContracts: SalesContract[] = [
  {
    id: 'contract1',
    salesOrderId: 'order1',
    customerId: 'customer1',
    vehicleId: 'vehicle1',
    dealerId: 'dealer1',
    contractDate: '2024-01-20T00:00:00Z',
    contractValue: 43000,
    paymentTerms: 'Monthly installments over 60 months',
    deliveryTerms: 'Delivery within 30 days of payment confirmation',
    warrantyTerms: '3-year comprehensive warranty',
    status: 'SIGNED',
    signedBy: 'John Smith',
    signedAt: '2024-01-20T14:30:00Z'
  }
];

// Mock Test Drives
export const mockTestDrives: TestDrive[] = [
  {
    id: 'testdrive1',
    customerId: 'customer1',
    vehicleId: 'vehicle1',
    dealerId: 'dealer1',
    scheduledDate: '2024-01-25T10:00:00Z',
    duration: 30,
    status: 'COMPLETED',
    notes: 'Customer very interested in the vehicle',
    feedback: 'Great driving experience, smooth acceleration',
    rating: 5
  }
];

// Mock Promotions
export const mockPromotions: Promotion[] = [
  {
    id: 'promo1',
    name: 'New Year Special',
    description: 'Get $3,000 off on all Tesla models',
    type: 'FIXED_AMOUNT',
    value: 3000,
    startDate: '2024-01-01T00:00:00Z',
    endDate: '2024-01-31T23:59:59Z',
    applicableVehicles: ['vehicle1', 'vehicle2'],
    applicableDealers: ['dealer1', 'dealer2'],
    maxUses: 100,
    currentUses: 25,
    status: 'ACTIVE'
  }
];

// Mock Sales Reports
export const mockSalesReports: SalesReport[] = [
  {
    id: 'report1',
    dealerId: 'dealer1',
    salespersonId: '4',
    period: '2024-01',
    totalSales: 43000,
    totalVehicles: 1,
    totalCustomers: 1,
    averageOrderValue: 43000,
    conversionRate: 0.75,
    generatedAt: '2024-02-01T00:00:00Z'
  }
];

// Mock Inventory Reports
export const mockInventoryReports: InventoryReport[] = [
  {
    id: 'inv1',
    dealerId: 'dealer1',
    vehicleId: 'vehicle1',
    currentStock: 15,
    reservedStock: 2,
    availableStock: 13,
    consumptionRate: 2.5,
    reorderPoint: 5,
    lastUpdated: '2024-01-20T00:00:00Z'
  }
];

// Mock Dashboard Stats
export const mockDashboardStats: DashboardStats = {
  totalSales: 1250000,
  totalVehicles: 35,
  totalCustomers: 150,
  totalRevenue: 1250000,
  salesGrowth: 15.5,
  vehicleGrowth: 8.2,
  customerGrowth: 12.3,
  revenueGrowth: 18.7
};

// Mock Chart Data
export const mockSalesChartData: TimeSeriesData[] = [
  { date: '2024-01-01', value: 120000 },
  { date: '2024-01-02', value: 135000 },
  { date: '2024-01-03', value: 150000 },
  { date: '2024-01-04', value: 140000 },
  { date: '2024-01-05', value: 160000 },
  { date: '2024-01-06', value: 175000 },
  { date: '2024-01-07', value: 180000 }
];

export const mockVehicleSalesData: ChartData[] = [
  { name: 'Tesla Model 3', value: 45, sales: 45 },
  { name: 'Tesla Model Y', value: 30, sales: 30 },
  { name: 'BMW i4', value: 15, sales: 15 },
  { name: 'Audi e-tron', value: 10, sales: 10 }
];

export const mockRegionalSalesData: ChartData[] = [
  { name: 'New York', value: 450000, region: 'NY' },
  { name: 'California', value: 380000, region: 'CA' },
  { name: 'Texas', value: 220000, region: 'TX' },
  { name: 'Florida', value: 200000, region: 'FL' }
];
