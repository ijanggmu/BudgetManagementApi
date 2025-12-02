/**
 * BeemaEdge API TypeScript Types
 * Copy this file to your Next.js project: types/api.ts
 */

// ============================================================================
// Common Types
// ============================================================================

export interface PaginationRequest {
  pageNumber: number;      // Default: 1
  pageSize: number;         // Default: 10, Max: 600
  query?: string;           // Search query
  filters?: string;         // Sieve filters
  sorts?: string;           // Sort order
}

export interface Pagination {
  totalItems: number;
  totalPages: number;
  pageSize: number;
  currentPage: number;
}

export interface ApiResponse<T> {
  isSuccess: boolean;
  data?: T;
  error?: string;
  errorCode?: number;
  pagination?: Pagination;
}

// ============================================================================
// Lead Management Types
// ============================================================================

export interface CreateLeadRequest {
  fullName: string;
  email: string;
  phone?: string;
  productCode: string;
}

export interface UpdateLeadStatusRequest {
  status: LeadStatus;
}

export type LeadStatus = 'New' | 'Qualified' | 'Contacted' | 'Quoted' | 'Won' | 'Lost';

export interface Contact {
  id: string;
  fullName: string;
  email: string;
  phone: string;
}

export interface Prospect {
  id: string;
  primaryContactId: string;
  primaryContact?: Contact;
}

export interface Lead {
  id: string;
  prospectId: string;
  status: LeadStatus;
  source: string;
  ownerUserId?: string;
  createdOn: string; // ISO 8601
  prospect?: Prospect;
}

export interface LeadActivityRequest {
  kind: 'note' | 'call' | 'email';
  notes: string;
}

export interface LeadActivity {
  id: string;
  leadId: string;
  kind: 'note' | 'call' | 'email';
  notes: string;
  when: string; // ISO 8601
}

// ============================================================================
// Quotation Types
// ============================================================================

export type QuotationStatus = 'Draft' | 'Submitted' | 'Approved' | 'Declined' | 'Accepted';

export interface QuotationItemRequest {
  coverageId: string; // UUID
  sumInsured: number;
}

export interface QuotationItem {
  id: string;
  quotationId: string;
  coverageId: string;
  sumInsured: number;
  premium: number;
}

export interface CreateQuotationRequest {
  productId: string; // UUID
  prospectId: string; // UUID
  items: QuotationItemRequest[];
}

export interface UpdateQuotationRequest {
  status?: QuotationStatus;
  totalPremium?: number;
  discountPercent?: number;
  validUntil?: string; // YYYY-MM-DD
  items?: QuotationItemRequest[];
}

export interface Quotation {
  id: string;
  number: string;
  status: QuotationStatus;
  productId: string;
  prospectId: string;
  totalPremium?: number;
  discountPercent?: number;
  validUntil?: string; // YYYY-MM-DD
  pdfUrl?: string;
  createdOn: string; // ISO 8601
  items: QuotationItem[];
}

// ============================================================================
// Premium Calculation Types
// ============================================================================

export interface CalculatePremiumRequest {
  portfolioAlias: string;
  fiscalYear: string;
  effectiveDate?: string; // ISO 8601
  expiryDate?: string; // ISO 8601
  sumInsured?: number;
  fullSumInsured?: number;
  policyPeriodInDays?: number;
  // Add other policy-specific fields as needed
  motorPartial?: any;
  firePartial?: any;
  // ... other partials
}

export interface PremiumCalculationResult {
  sumInsuredAmount: number;
  basicPremium: number;
  grossPremiumAmount: number;
  days: number;
  numberofPassengers?: number;
  ageOfVehicle?: number;
  minbasicPremium?: number;
  actualRecoveryCharge?: number;
  recoveryCharge?: number;
  vehicleCapacity?: number;
  ecoFriendlyDiscountAmount?: number;
  // Add other fields as needed
}

export interface CalculateEndorsementPremiumRequest {
  endorsementModel: CalculatePremiumRequest;
  premiumCalculation: PremiumCalculationResult;
}

// ============================================================================
// Premium Calculation Configuration Types (Admin)
// ============================================================================

export type CalculationEngineType = 'FormulaBased' | 'RuleBased' | 'Legacy';
export type RuleType = 'Formula' | 'Condition' | 'Validation' | 'Discount';

export interface CreatePremiumCalculationConfigurationRequest {
  portfolioAlias: string;
  fiscalYear: string;
  effectiveFrom: string; // YYYY-MM-DD
  effectiveTo?: string; // YYYY-MM-DD
  description?: string;
  calculationEngineType?: CalculationEngineType;
}

export interface UpdatePremiumCalculationConfigurationRequest {
  effectiveFrom?: string; // YYYY-MM-DD
  effectiveTo?: string; // YYYY-MM-DD
  description?: string;
  calculationEngineType?: CalculationEngineType;
}

export interface PremiumCalculationParameter {
  id: string;
  configurationId: string;
  parameterKey: string;
  parameterName: string;
  dataType: 'decimal' | 'int' | 'bool' | 'string';
  value: string;
  defaultValue?: string;
  minValue?: number;
  maxValue?: number;
  isRequired: boolean;
  displayOrder: number;
  category?: string;
}

export interface CreatePremiumCalculationParameterRequest {
  parameterKey: string;
  parameterName: string;
  dataType: 'decimal' | 'int' | 'bool' | 'string';
  value: string;
  defaultValue?: string;
  minValue?: number;
  maxValue?: number;
  isRequired?: boolean;
  displayOrder?: number;
  category?: string;
}

export interface PremiumCalculationRule {
  id: string;
  configurationId: string;
  ruleName: string;
  ruleType: RuleType;
  expression?: string;
  condition?: string; // JSON string
  priority: number;
  isActive: boolean;
}

export interface CreatePremiumCalculationRuleRequest {
  ruleName: string;
  ruleType: RuleType;
  expression?: string;
  condition?: string; // JSON string
  priority?: number;
  isActive?: boolean;
}

export interface PremiumCalculationRateTable {
  id: string;
  configurationId: string;
  tableName: string;
  schemaJson: string; // JSON string
  dataJson: string; // JSON string (array of objects)
  lookupKey: string;
}

export interface CreatePremiumCalculationRateTableRequest {
  tableName: string;
  schemaJson: string; // JSON string
  dataJson: string; // JSON string (array of objects)
  lookupKey: string;
}

export interface PremiumCalculationConfiguration {
  id: string;
  portfolioAlias: string;
  fiscalYear: string;
  version: number;
  effectiveFrom: string; // YYYY-MM-DD
  effectiveTo?: string; // YYYY-MM-DD
  isActive: boolean;
  description?: string;
  calculationEngineType: CalculationEngineType;
  createdOn: string; // ISO 8601
  lastModifiedOn?: string; // ISO 8601
  parameters: PremiumCalculationParameter[];
  rules: PremiumCalculationRule[];
  rateTables: PremiumCalculationRateTable[];
}

// ============================================================================
// Notification Types
// ============================================================================

export type NotificationChannel = 'Email' | 'SMS' | 'Push';

export interface Notification {
  id: string;
  userId: string;
  title: string;
  body: string;
  channel: NotificationChannel;
  payload?: string; // JSON string
  sentAt: string; // ISO 8601
  readAt?: string; // ISO 8601
  isRead: boolean;
}

// ============================================================================
// Attendance Types
// ============================================================================

export interface AttendanceRequest {
  latitude: number;
  longitude: number;
  remarks?: string;
}

export interface AttendanceRecord {
  id: string;
  userId: string;
  type: 'CheckIn' | 'CheckOut';
  latitude: number;
  longitude: number;
  timestamp: string; // ISO 8601
  remarks?: string;
}

export interface DailyAttendance {
  date: string; // YYYY-MM-DD
  checkIn?: {
    timestamp: string;
    latitude: number;
    longitude: number;
  };
  checkOut?: {
    timestamp: string;
    latitude: number;
    longitude: number;
  };
  totalHours?: number;
}

export interface MonthlyAttendance {
  year: number;
  month: number;
  totalDays: number;
  presentDays: number;
  absentDays: number;
  dailyRecords: DailyAttendance[];
}

// ============================================================================
// Renewal Types
// ============================================================================

export interface RenewalReminder {
  id: string;
  policyId: string;
  dueDate: string; // YYYY-MM-DD
  premiumAmount: number;
  reminderSentAt: string; // ISO 8601
  channel: NotificationChannel;
}

// ============================================================================
// API Client Helper Types
// ============================================================================

export interface ApiClientConfig {
  baseUrl: string;
  getToken: () => string | null;
  onUnauthorized?: () => void;
}

export class ApiError extends Error {
  constructor(
    public message: string,
    public statusCode: number,
    public errorCode?: number
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

