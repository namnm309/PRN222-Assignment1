// User and Authentication Types
export interface User {
  id: string;
  email: string;
  name: string;
  role: UserRole;
  dealerId?: string;
  avatar?: string;
}

export type UserRole = 'DEALER_STAFF' | 'DEALER_MANAGER' | 'EVM_STAFF' | 'ADMIN';

// Vehicle Types
export interface Vehicle {
  id: string;
  model: string;
  version: string;
  color: string;
  year: number;
  basePrice: number;
  features: string[];
  specifications: VehicleSpecifications;
  images: string[];
  stock: number;
  dealerId?: string;
}

export interface VehicleSpecifications {
  batteryCapacity: string;
  range: string;
  chargingTime: string;
  power: string;
  acceleration: string;
  topSpeed: string;
  weight: string;
  dimensions: string;
}

// Customer Types
export interface Customer {
  id: string;
  name: string;
  email: string;
  phone: string;
  address: string;
  dateOfBirth: string;
  licenseNumber: string;
  dealerId: string;
  createdAt: string;
  updatedAt: string;
}

// Sales Types
export interface Quotation {
  id: string;
  customerId: string;
  vehicleId: string;
  dealerId: string;
  salespersonId: string;
  basePrice: number;
  discount: number;
  finalPrice: number;
  status: 'DRAFT' | 'SENT' | 'ACCEPTED' | 'REJECTED';
  validUntil: string;
  createdAt: string;
  notes?: string;
}

export interface SalesOrder {
  id: string;
  quotationId: string;
  customerId: string;
  vehicleId: string;
  dealerId: string;
  salespersonId: string;
  orderDate: string;
  deliveryDate: string;
  status: 'PENDING' | 'CONFIRMED' | 'IN_PRODUCTION' | 'SHIPPED' | 'DELIVERED' | 'CANCELLED';
  paymentStatus: 'PENDING' | 'PARTIAL' | 'FULL';
  totalAmount: number;
  paidAmount: number;
  notes?: string;
}

export interface SalesContract {
  id: string;
  salesOrderId: string;
  customerId: string;
  vehicleId: string;
  dealerId: string;
  contractDate: string;
  contractValue: number;
  paymentTerms: string;
  deliveryTerms: string;
  warrantyTerms: string;
  status: 'DRAFT' | 'SIGNED' | 'EXECUTED';
  signedBy: string;
  signedAt?: string;
}

// Dealer Types
export interface Dealer {
  id: string;
  name: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  phone: string;
  email: string;
  managerId: string;
  salesTarget: number;
  currentSales: number;
  debt: number;
  status: 'ACTIVE' | 'INACTIVE' | 'SUSPENDED';
  createdAt: string;
}

// Test Drive Types
export interface TestDrive {
  id: string;
  customerId: string;
  vehicleId: string;
  dealerId: string;
  scheduledDate: string;
  duration: number; // in minutes
  status: 'SCHEDULED' | 'COMPLETED' | 'CANCELLED' | 'NO_SHOW';
  notes?: string;
  feedback?: string;
  rating?: number;
}

// Promotion Types
export interface Promotion {
  id: string;
  name: string;
  description: string;
  type: 'PERCENTAGE' | 'FIXED_AMOUNT';
  value: number;
  startDate: string;
  endDate: string;
  applicableVehicles: string[];
  applicableDealers: string[];
  maxUses?: number;
  currentUses: number;
  status: 'ACTIVE' | 'INACTIVE' | 'EXPIRED';
}

// Report Types
export interface SalesReport {
  id: string;
  dealerId: string;
  salespersonId?: string;
  period: string;
  totalSales: number;
  totalVehicles: number;
  totalCustomers: number;
  averageOrderValue: number;
  conversionRate: number;
  generatedAt: string;
}

export interface InventoryReport {
  id: string;
  dealerId: string;
  vehicleId: string;
  currentStock: number;
  reservedStock: number;
  availableStock: number;
  consumptionRate: number;
  reorderPoint: number;
  lastUpdated: string;
}

// API Response Types
export interface ApiResponse<T> {
  data: T;
  message: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  limit: number;
  totalPages: number;
}

// Form Types
export interface LoginForm {
  email: string;
  password: string;
}

export interface VehicleForm {
  model: string;
  version: string;
  color: string;
  year: number;
  basePrice: number;
  features: string[];
  specifications: VehicleSpecifications;
  stock: number;
}

export interface CustomerForm {
  name: string;
  email: string;
  phone: string;
  address: string;
  dateOfBirth: string;
  licenseNumber: string;
}

// Dashboard Types
export interface DashboardStats {
  totalSales: number;
  totalVehicles: number;
  totalCustomers: number;
  totalRevenue: number;
  salesGrowth: number;
  vehicleGrowth: number;
  customerGrowth: number;
  revenueGrowth: number;
}

// Chart Data Types
export interface ChartData {
  name: string;
  value: number;
  [key: string]: any;
}

export interface TimeSeriesData {
  date: string;
  value: number;
  [key: string]: any;
}
