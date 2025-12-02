/**
 * Next.js API Client Example
 * Copy this to your Next.js project: lib/api-client.ts
 * 
 * Usage:
 * import { apiClient } from '@/lib/api-client';
 * const leads = await apiClient.leads.list();
 */

import type {
  ApiResponse,
  PaginationRequest,
  Lead,
  CreateLeadRequest,
  UpdateLeadStatusRequest,
  LeadActivity,
  LeadActivityRequest,
  Quotation,
  CreateQuotationRequest,
  UpdateQuotationRequest,
  PremiumCalculationResult,
  CalculatePremiumRequest,
  CalculateEndorsementPremiumRequest,
  PremiumCalculationConfiguration,
  CreatePremiumCalculationConfigurationRequest,
  UpdatePremiumCalculationConfigurationRequest,
  PremiumCalculationParameter,
  CreatePremiumCalculationParameterRequest,
  PremiumCalculationRule,
  CreatePremiumCalculationRuleRequest,
  PremiumCalculationRateTable,
  CreatePremiumCalculationRateTableRequest,
  Notification,
  AttendanceRequest,
  AttendanceRecord,
  DailyAttendance,
  MonthlyAttendance,
  RenewalReminder,
} from '@/types/api';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://api.beemaedge.com/api/v1';

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<ApiResponse<T>> {
    const token = typeof window !== 'undefined' 
      ? localStorage.getItem('access_token')
      : null;

    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...(token && { Authorization: `Bearer ${token}` }),
        ...options.headers,
      },
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({ error: 'Network error' }));
      throw new Error(error.error || `HTTP ${response.status}`);
    }

    return response.json();
  }

  // ============================================================================
  // Lead Management API
  // ============================================================================

  leads = {
    create: async (data: CreateLeadRequest): Promise<ApiResponse<Lead>> => {
      return this.request<Lead>('/leads', {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },

    list: async (
      params?: PaginationRequest & {
        status?: string;
        from?: string;
        to?: string;
      }
    ): Promise<ApiResponse<Lead[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Lead[]>(`/leads?${queryParams.toString()}`);
    },

    getById: async (id: string): Promise<ApiResponse<Lead>> => {
      return this.request<Lead>(`/leads/${id}`);
    },

    updateStatus: async (
      id: string,
      data: UpdateLeadStatusRequest
    ): Promise<ApiResponse<Lead>> => {
      return this.request<Lead>(`/leads/${id}/status`, {
        method: 'PATCH',
        body: JSON.stringify(data),
      });
    },

    addActivity: async (
      id: string,
      data: LeadActivityRequest
    ): Promise<ApiResponse<LeadActivity>> => {
      return this.request<LeadActivity>(`/leads/${id}/activities`, {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },

    getActivities: async (id: string): Promise<ApiResponse<LeadActivity[]>> => {
      return this.request<LeadActivity[]>(`/leads/${id}/activities`);
    },

    getQuotations: async (leadId: string): Promise<ApiResponse<Quotation[]>> => {
      return this.request<Quotation[]>(`/leads/${leadId}/quotations`);
    },
  };

  // Admin Lead API
  adminLeads = {
    list: async (
      params?: PaginationRequest & {
        status?: string;
        from?: string;
        to?: string;
      }
    ): Promise<ApiResponse<Lead[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Lead[]>(`/admin/leads?${queryParams.toString()}`);
    },

    getById: async (id: string): Promise<ApiResponse<Lead>> => {
      return this.request<Lead>(`/admin/leads/${id}`);
    },

    getByTenantId: async (
      tenantId: string,
      params?: PaginationRequest & {
        status?: string;
        from?: string;
        to?: string;
      }
    ): Promise<ApiResponse<Lead[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Lead[]>(
        `/admin/leads/tenant/${tenantId}?${queryParams.toString()}`
      );
    },
  };

  // ============================================================================
  // Quotation API
  // ============================================================================

  quotations = {
    create: async (data: CreateQuotationRequest): Promise<ApiResponse<Quotation>> => {
      return this.request<Quotation>('/quotations', {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },

    list: async (params?: PaginationRequest): Promise<ApiResponse<Quotation[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Quotation[]>(`/quotations?${queryParams.toString()}`);
    },

    getById: async (id: string): Promise<ApiResponse<Quotation>> => {
      return this.request<Quotation>(`/quotations/${id}`);
    },

    update: async (
      id: string,
      data: UpdateQuotationRequest
    ): Promise<ApiResponse<Quotation>> => {
      return this.request<Quotation>(`/quotations/${id}`, {
        method: 'PATCH',
        body: JSON.stringify(data),
      });
    },

    delete: async (id: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(`/quotations/${id}`, {
        method: 'DELETE',
      });
    },

    generatePdf: async (id: string): Promise<ApiResponse<string>> => {
      return this.request<string>(`/quotations/${id}/pdf`);
    },
  };

  // Admin Quotation API
  adminQuotations = {
    list: async (
      params?: PaginationRequest & {
        status?: string;
        from?: string;
        to?: string;
      }
    ): Promise<ApiResponse<Quotation[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Quotation[]>(`/admin/quotations?${queryParams.toString()}`);
    },

    getById: async (id: string): Promise<ApiResponse<Quotation>> => {
      return this.request<Quotation>(`/admin/quotations/${id}`);
    },

    getByTenantId: async (
      tenantId: string,
      params?: PaginationRequest & {
        status?: string;
        from?: string;
        to?: string;
      }
    ): Promise<ApiResponse<Quotation[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Quotation[]>(
        `/admin/quotations/tenant/${tenantId}?${queryParams.toString()}`
      );
    },
  };

  // ============================================================================
  // Premium Calculation API
  // ============================================================================

  premiums = {
    calculate: async (
      data: CalculatePremiumRequest
    ): Promise<ApiResponse<PremiumCalculationResult>> => {
      return this.request<PremiumCalculationResult>('/premiums/calculate', {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },

    calculateEndorsement: async (
      data: CalculateEndorsementPremiumRequest
    ): Promise<ApiResponse<PremiumCalculationResult>> => {
      return this.request<PremiumCalculationResult>('/premiums/calculate-endorsement', {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },
  };

  // Admin Premium Calculation Configuration API
  adminPremiumConfigs = {
    list: async (params?: {
      portfolioAlias?: string;
      fiscalYear?: string;
    }): Promise<ApiResponse<PremiumCalculationConfiguration[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<PremiumCalculationConfiguration[]>(
        `/admin/premium-calculation/configurations?${queryParams.toString()}`
      );
    },

    getById: async (
      id: string
    ): Promise<ApiResponse<PremiumCalculationConfiguration>> => {
      return this.request<PremiumCalculationConfiguration>(
        `/admin/premium-calculation/configurations/${id}`
      );
    },

    getByPortfolioAndFiscalYear: async (
      portfolioAlias: string,
      fiscalYear: string,
      effectiveDate?: string
    ): Promise<ApiResponse<PremiumCalculationConfiguration>> => {
      const queryParams = effectiveDate
        ? `?effectiveDate=${effectiveDate}`
        : '';
      return this.request<PremiumCalculationConfiguration>(
        `/admin/premium-calculation/configurations/portfolio/${portfolioAlias}/fiscal-year/${fiscalYear}${queryParams}`
      );
    },

    create: async (
      data: CreatePremiumCalculationConfigurationRequest
    ): Promise<ApiResponse<PremiumCalculationConfiguration>> => {
      return this.request<PremiumCalculationConfiguration>(
        '/admin/premium-calculation/configurations',
        {
          method: 'POST',
          body: JSON.stringify(data),
        }
      );
    },

    update: async (
      id: string,
      data: UpdatePremiumCalculationConfigurationRequest
    ): Promise<ApiResponse<PremiumCalculationConfiguration>> => {
      return this.request<PremiumCalculationConfiguration>(
        `/admin/premium-calculation/configurations/${id}`,
        {
          method: 'PUT',
          body: JSON.stringify(data),
        }
      );
    },

    delete: async (id: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(
        `/admin/premium-calculation/configurations/${id}`,
        {
          method: 'DELETE',
        }
      );
    },

    activate: async (id: string, fiscalYear: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(
        `/admin/premium-calculation/configurations/${id}/activate`,
        {
          method: 'POST',
          body: JSON.stringify(fiscalYear),
        }
      );
    },

    clone: async (id: string, newFiscalYear: string): Promise<ApiResponse<PremiumCalculationConfiguration>> => {
      return this.request<PremiumCalculationConfiguration>(
        `/admin/premium-calculation/configurations/${id}/clone`,
        {
          method: 'POST',
          body: JSON.stringify(newFiscalYear),
        }
      );
    },

    validate: async (id: string): Promise<ApiResponse<{ isValid: boolean; errors: string[] }>> => {
      return this.request<{ isValid: boolean; errors: string[] }>(
        `/admin/premium-calculation/configurations/${id}/validate`,
        {
          method: 'POST',
        }
      );
    },
  };

  // Admin Premium Calculation Parameters API
  adminPremiumParameters = {
    getByConfiguration: async (
      configurationId: string
    ): Promise<ApiResponse<PremiumCalculationParameter[]>> => {
      return this.request<PremiumCalculationParameter[]>(
        `/admin/premium-calculation/parameters/configuration/${configurationId}`
      );
    },

    getById: async (id: string): Promise<ApiResponse<PremiumCalculationParameter>> => {
      return this.request<PremiumCalculationParameter>(
        `/admin/premium-calculation/parameters/${id}`
      );
    },

    create: async (
      configurationId: string,
      data: CreatePremiumCalculationParameterRequest
    ): Promise<ApiResponse<PremiumCalculationParameter>> => {
      return this.request<PremiumCalculationParameter>(
        `/admin/premium-calculation/parameters/configuration/${configurationId}`,
        {
          method: 'POST',
          body: JSON.stringify(data),
        }
      );
    },

    update: async (
      id: string,
      data: CreatePremiumCalculationParameterRequest
    ): Promise<ApiResponse<PremiumCalculationParameter>> => {
      return this.request<PremiumCalculationParameter>(
        `/admin/premium-calculation/parameters/${id}`,
        {
          method: 'PUT',
          body: JSON.stringify(data),
        }
      );
    },

    delete: async (id: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(`/admin/premium-calculation/parameters/${id}`, {
        method: 'DELETE',
      });
    },

    bulkUpdate: async (
      configurationId: string,
      data: CreatePremiumCalculationParameterRequest[]
    ): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(
        `/admin/premium-calculation/parameters/configuration/${configurationId}/bulk`,
        {
          method: 'POST',
          body: JSON.stringify(data),
        }
      );
    },
  };

  // Admin Premium Calculation Rules API
  adminPremiumRules = {
    getByConfiguration: async (
      configurationId: string
    ): Promise<ApiResponse<PremiumCalculationRule[]>> => {
      return this.request<PremiumCalculationRule[]>(
        `/admin/premium-calculation/rules/configuration/${configurationId}`
      );
    },

    getById: async (id: string): Promise<ApiResponse<PremiumCalculationRule>> => {
      return this.request<PremiumCalculationRule>(
        `/admin/premium-calculation/rules/${id}`
      );
    },

    create: async (
      configurationId: string,
      data: CreatePremiumCalculationRuleRequest
    ): Promise<ApiResponse<PremiumCalculationRule>> => {
      return this.request<PremiumCalculationRule>(
        `/admin/premium-calculation/rules/configuration/${configurationId}`,
        {
          method: 'POST',
          body: JSON.stringify(data),
        }
      );
    },

    update: async (
      id: string,
      data: CreatePremiumCalculationRuleRequest
    ): Promise<ApiResponse<PremiumCalculationRule>> => {
      return this.request<PremiumCalculationRule>(
        `/admin/premium-calculation/rules/${id}`,
        {
          method: 'PUT',
          body: JSON.stringify(data),
        }
      );
    },

    delete: async (id: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(`/admin/premium-calculation/rules/${id}`, {
        method: 'DELETE',
      });
    },
  };

  // Admin Premium Calculation Rate Tables API
  adminPremiumRateTables = {
    getByConfiguration: async (
      configurationId: string
    ): Promise<ApiResponse<PremiumCalculationRateTable[]>> => {
      return this.request<PremiumCalculationRateTable[]>(
        `/admin/premium-calculation/rate-tables/configuration/${configurationId}`
      );
    },

    getById: async (id: string): Promise<ApiResponse<PremiumCalculationRateTable>> => {
      return this.request<PremiumCalculationRateTable>(
        `/admin/premium-calculation/rate-tables/${id}`
      );
    },

    create: async (
      configurationId: string,
      data: CreatePremiumCalculationRateTableRequest
    ): Promise<ApiResponse<PremiumCalculationRateTable>> => {
      return this.request<PremiumCalculationRateTable>(
        `/admin/premium-calculation/rate-tables/configuration/${configurationId}`,
        {
          method: 'POST',
          body: JSON.stringify(data),
        }
      );
    },

    update: async (
      id: string,
      data: CreatePremiumCalculationRateTableRequest
    ): Promise<ApiResponse<PremiumCalculationRateTable>> => {
      return this.request<PremiumCalculationRateTable>(
        `/admin/premium-calculation/rate-tables/${id}`,
        {
          method: 'PUT',
          body: JSON.stringify(data),
        }
      );
    },

    delete: async (id: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(`/admin/premium-calculation/rate-tables/${id}`, {
        method: 'DELETE',
      });
    },
  };

  // ============================================================================
  // Notification API
  // ============================================================================

  notifications = {
    list: async (params?: PaginationRequest): Promise<ApiResponse<Notification[]>> => {
      const queryParams = new URLSearchParams();
      if (params) {
        Object.entries(params).forEach(([key, value]) => {
          if (value !== undefined && value !== null) {
            queryParams.append(key, String(value));
          }
        });
      }
      return this.request<Notification[]>(`/notifications?${queryParams.toString()}`);
    },

    markAsRead: async (id: string): Promise<ApiResponse<boolean>> => {
      return this.request<boolean>(`/notifications/${id}/read`, {
        method: 'PATCH',
      });
    },
  };

  // ============================================================================
  // Attendance API
  // ============================================================================

  attendance = {
    checkIn: async (data: AttendanceRequest): Promise<ApiResponse<AttendanceRecord>> => {
      return this.request<AttendanceRecord>('/attendance/check-in', {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },

    checkOut: async (data: AttendanceRequest): Promise<ApiResponse<AttendanceRecord>> => {
      return this.request<AttendanceRecord>('/attendance/check-out', {
        method: 'POST',
        body: JSON.stringify(data),
      });
    },

    getDaily: async (date?: string): Promise<ApiResponse<DailyAttendance>> => {
      const queryParams = date ? `?date=${date}` : '';
      return this.request<DailyAttendance>(`/attendance/daily${queryParams}`);
    },

    getMonthly: async (
      year?: number,
      month?: number
    ): Promise<ApiResponse<MonthlyAttendance>> => {
      const queryParams = new URLSearchParams();
      if (year) queryParams.append('year', String(year));
      if (month) queryParams.append('month', String(month));
      const query = queryParams.toString();
      return this.request<MonthlyAttendance>(
        `/attendance/monthly${query ? `?${query}` : ''}`
      );
    },
  };

  // ============================================================================
  // Renewal API
  // ============================================================================

  renewals = {
    triggerReminders: async (): Promise<ApiResponse<{ message: string }>> => {
      return this.request<{ message: string }>('/renewals/trigger-reminders', {
        method: 'POST',
      });
    },

    getMyReminders: async (): Promise<ApiResponse<RenewalReminder[]>> => {
      return this.request<RenewalReminder[]>('/renewals/my-reminders');
    },
  };
}

// Export singleton instance
export const apiClient = new ApiClient();

// Export class for custom instances
export { ApiClient };

