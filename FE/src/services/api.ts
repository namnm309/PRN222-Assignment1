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
  TimeSeriesData,
  LoginForm,
  VehicleForm,
  CustomerForm,
  ApiResponse
} from '../types';
import {
  mockUsers,
  mockVehicles,
  mockCustomers,
  mockQuotations,
  mockSalesOrders,
  mockSalesContracts,
  mockDealers,
  mockTestDrives,
  mockPromotions,
  mockSalesReports,
  mockInventoryReports,
  mockDashboardStats,
  mockSalesChartData,
  mockVehicleSalesData,
  mockRegionalSalesData
} from './mockData';

// Simulate API delay
const delay = (ms: number) => new Promise(resolve => setTimeout(resolve, ms));

// Authentication API
export const authApi = {
  login: async (credentials: LoginForm): Promise<ApiResponse<User>> => {
    await delay(1000);
    
    const user = mockUsers.find(u => u.email === credentials.email);
    if (user && credentials.password === 'password') {
      return {
        data: user,
        message: 'Login successful',
        success: true
      };
    }
    
    throw new Error('Invalid credentials');
  },

  logout: async (): Promise<ApiResponse<null>> => {
    await delay(500);
    return {
      data: null,
      message: 'Logout successful',
      success: true
    };
  },

  getCurrentUser: async (): Promise<ApiResponse<User>> => {
    await delay(500);
    const user = mockUsers[0]; // Return first user as current
    return {
      data: user,
      message: 'User retrieved successfully',
      success: true
    };
  }
};

// Vehicle API
export const vehicleApi = {
  getVehicles: async (dealerId?: string): Promise<ApiResponse<Vehicle[]>> => {
    await delay(800);
    let vehicles = mockVehicles;
    if (dealerId) {
      vehicles = vehicles.filter(v => v.dealerId === dealerId);
    }
    return {
      data: vehicles,
      message: 'Vehicles retrieved successfully',
      success: true
    };
  },

  getVehicle: async (id: string): Promise<ApiResponse<Vehicle>> => {
    await delay(500);
    const vehicle = mockVehicles.find(v => v.id === id);
    if (!vehicle) {
      throw new Error('Vehicle not found');
    }
    return {
      data: vehicle,
      message: 'Vehicle retrieved successfully',
      success: true
    };
  },

  createVehicle: async (vehicle: VehicleForm): Promise<ApiResponse<Vehicle>> => {
    await delay(1000);
    const newVehicle: Vehicle = {
      id: `vehicle${Date.now()}`,
      ...vehicle,
      images: [],
      dealerId: 'dealer1'
    };
    return {
      data: newVehicle,
      message: 'Vehicle created successfully',
      success: true
    };
  },

  updateVehicle: async (id: string, vehicle: Partial<VehicleForm>): Promise<ApiResponse<Vehicle>> => {
    await delay(1000);
    const existingVehicle = mockVehicles.find(v => v.id === id);
    if (!existingVehicle) {
      throw new Error('Vehicle not found');
    }
    const updatedVehicle = { ...existingVehicle, ...vehicle };
    return {
      data: updatedVehicle,
      message: 'Vehicle updated successfully',
      success: true
    };
  },

  deleteVehicle: async (_id: string): Promise<ApiResponse<null>> => {
    await delay(800);
    return {
      data: null,
      message: 'Vehicle deleted successfully',
      success: true
    };
  }
};

// Customer API
export const customerApi = {
  getCustomers: async (dealerId?: string): Promise<ApiResponse<Customer[]>> => {
    await delay(800);
    let customers = mockCustomers;
    if (dealerId) {
      customers = customers.filter(c => c.dealerId === dealerId);
    }
    return {
      data: customers,
      message: 'Customers retrieved successfully',
      success: true
    };
  },

  getCustomer: async (id: string): Promise<ApiResponse<Customer>> => {
    await delay(500);
    const customer = mockCustomers.find(c => c.id === id);
    if (!customer) {
      throw new Error('Customer not found');
    }
    return {
      data: customer,
      message: 'Customer retrieved successfully',
      success: true
    };
  },

  createCustomer: async (customer: CustomerForm): Promise<ApiResponse<Customer>> => {
    await delay(1000);
    const newCustomer: Customer = {
      id: `customer${Date.now()}`,
      ...customer,
      dealerId: 'dealer1',
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    };
    return {
      data: newCustomer,
      message: 'Customer created successfully',
      success: true
    };
  },

  updateCustomer: async (id: string, customer: Partial<CustomerForm>): Promise<ApiResponse<Customer>> => {
    await delay(1000);
    const existingCustomer = mockCustomers.find(c => c.id === id);
    if (!existingCustomer) {
      throw new Error('Customer not found');
    }
    const updatedCustomer = { 
      ...existingCustomer, 
      ...customer,
      updatedAt: new Date().toISOString()
    };
    return {
      data: updatedCustomer,
      message: 'Customer updated successfully',
      success: true
    };
  }
};

// Sales API
export const salesApi = {
  getQuotations: async (dealerId?: string): Promise<ApiResponse<Quotation[]>> => {
    await delay(800);
    let quotations = mockQuotations;
    if (dealerId) {
      quotations = quotations.filter(q => q.dealerId === dealerId);
    }
    return {
      data: quotations,
      message: 'Quotations retrieved successfully',
      success: true
    };
  },

  createQuotation: async (quotation: Omit<Quotation, 'id' | 'createdAt'>): Promise<ApiResponse<Quotation>> => {
    await delay(1000);
    const newQuotation: Quotation = {
      id: `quote${Date.now()}`,
      ...quotation,
      createdAt: new Date().toISOString()
    };
    return {
      data: newQuotation,
      message: 'Quotation created successfully',
      success: true
    };
  },

  getSalesOrders: async (dealerId?: string): Promise<ApiResponse<SalesOrder[]>> => {
    await delay(800);
    let orders = mockSalesOrders;
    if (dealerId) {
      orders = orders.filter(o => o.dealerId === dealerId);
    }
    return {
      data: orders,
      message: 'Sales orders retrieved successfully',
      success: true
    };
  },

  createSalesOrder: async (order: Omit<SalesOrder, 'id'>): Promise<ApiResponse<SalesOrder>> => {
    await delay(1000);
    const newOrder: SalesOrder = {
      id: `order${Date.now()}`,
      ...order
    };
    return {
      data: newOrder,
      message: 'Sales order created successfully',
      success: true
    };
  },

  getSalesContracts: async (dealerId?: string): Promise<ApiResponse<SalesContract[]>> => {
    await delay(800);
    let contracts = mockSalesContracts;
    if (dealerId) {
      contracts = contracts.filter(c => c.dealerId === dealerId);
    }
    return {
      data: contracts,
      message: 'Sales contracts retrieved successfully',
      success: true
    };
  }
};

// Dealer API
export const dealerApi = {
  getDealers: async (): Promise<ApiResponse<Dealer[]>> => {
    await delay(800);
    return {
      data: mockDealers,
      message: 'Dealers retrieved successfully',
      success: true
    };
  },

  getDealer: async (id: string): Promise<ApiResponse<Dealer>> => {
    await delay(500);
    const dealer = mockDealers.find(d => d.id === id);
    if (!dealer) {
      throw new Error('Dealer not found');
    }
    return {
      data: dealer,
      message: 'Dealer retrieved successfully',
      success: true
    };
  },

  updateDealer: async (id: string, dealer: Partial<Dealer>): Promise<ApiResponse<Dealer>> => {
    await delay(1000);
    const existingDealer = mockDealers.find(d => d.id === id);
    if (!existingDealer) {
      throw new Error('Dealer not found');
    }
    const updatedDealer = { ...existingDealer, ...dealer };
    return {
      data: updatedDealer,
      message: 'Dealer updated successfully',
      success: true
    };
  }
};

// Test Drive API
export const testDriveApi = {
  getTestDrives: async (dealerId?: string): Promise<ApiResponse<TestDrive[]>> => {
    await delay(800);
    let testDrives = mockTestDrives;
    if (dealerId) {
      testDrives = testDrives.filter(td => td.dealerId === dealerId);
    }
    return {
      data: testDrives,
      message: 'Test drives retrieved successfully',
      success: true
    };
  },

  createTestDrive: async (testDrive: Omit<TestDrive, 'id'>): Promise<ApiResponse<TestDrive>> => {
    await delay(1000);
    const newTestDrive: TestDrive = {
      id: `testdrive${Date.now()}`,
      ...testDrive
    };
    return {
      data: newTestDrive,
      message: 'Test drive scheduled successfully',
      success: true
    };
  }
};

// Promotion API
export const promotionApi = {
  getPromotions: async (): Promise<ApiResponse<Promotion[]>> => {
    await delay(800);
    return {
      data: mockPromotions,
      message: 'Promotions retrieved successfully',
      success: true
    };
  },

  createPromotion: async (promotion: Omit<Promotion, 'id' | 'currentUses'>): Promise<ApiResponse<Promotion>> => {
    await delay(1000);
    const newPromotion: Promotion = {
      id: `promo${Date.now()}`,
      ...promotion,
      currentUses: 0
    };
    return {
      data: newPromotion,
      message: 'Promotion created successfully',
      success: true
    };
  }
};

// Report API
export const reportApi = {
  getSalesReports: async (dealerId?: string): Promise<ApiResponse<SalesReport[]>> => {
    await delay(800);
    let reports = mockSalesReports;
    if (dealerId) {
      reports = reports.filter(r => r.dealerId === dealerId);
    }
    return {
      data: reports,
      message: 'Sales reports retrieved successfully',
      success: true
    };
  },

  getInventoryReports: async (dealerId?: string): Promise<ApiResponse<InventoryReport[]>> => {
    await delay(800);
    let reports = mockInventoryReports;
    if (dealerId) {
      reports = reports.filter(r => r.dealerId === dealerId);
    }
    return {
      data: reports,
      message: 'Inventory reports retrieved successfully',
      success: true
    };
  },

  getDashboardStats: async (_dealerId?: string): Promise<ApiResponse<DashboardStats>> => {
    await delay(800);
    return {
      data: mockDashboardStats,
      message: 'Dashboard stats retrieved successfully',
      success: true
    };
  },

  getSalesChartData: async (_dealerId?: string): Promise<ApiResponse<TimeSeriesData[]>> => {
    await delay(800);
    return {
      data: mockSalesChartData,
      message: 'Sales chart data retrieved successfully',
      success: true
    };
  },

  getVehicleSalesData: async (): Promise<ApiResponse<ChartData[]>> => {
    await delay(800);
    return {
      data: mockVehicleSalesData,
      message: 'Vehicle sales data retrieved successfully',
      success: true
    };
  },

  getRegionalSalesData: async (): Promise<ApiResponse<ChartData[]>> => {
    await delay(800);
    return {
      data: mockRegionalSalesData,
      message: 'Regional sales data retrieved successfully',
      success: true
    };
  }
};
