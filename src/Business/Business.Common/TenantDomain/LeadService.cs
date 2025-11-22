using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Tenant;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class LeadService : ILeadService
{
    private readonly ApplicationDataContext _db;
    private readonly ISieveExtension _sieveExtension;
    private readonly ILogger<LeadService> _logger;

    public LeadService(ApplicationDataContext db, ISieveExtension sieveExtension, ILogger<LeadService> logger)
    {
        _db = db;
        _sieveExtension = sieveExtension;
        _logger = logger;
    }

    public async Task<Result<Lead>> CreateLeadAsync(CreateLeadPublicDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var contact = new Contact 
            { 
                FullName = dto.FullName, 
                Email = dto.Email, 
                Phone = dto.Phone ?? string.Empty 
            };
            
            await _db.Contacts.AddAsync(contact);
            await _db.SaveChangesAsync(); // Save to get contact ID
            
            var prospect = new Prospect
            {
                PrimaryContactId = contact.Id
            };
            
            await _db.Prospects.AddAsync(prospect);
            await _db.SaveChangesAsync(); // Save to get prospect ID
            
            var lead = new Lead
            {
                ProspectId = prospect.Id,
                Status = LeadStatus.New,
                Source = "Web"
            };
            
            await _db.Leads.AddAsync(lead);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            // Reload lead with related entities for response
            await _db.Entry(lead).Reference(l => l.Prospect).LoadAsync();
            await _db.Entry(lead.Prospect).Reference(p => p.PrimaryContact).LoadAsync();
            
            return Result<Lead>.Success(lead);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error creating lead for contact {Email}: {Message}", dto.Email, ex.Message);
            throw;
        }
    }

    public async Task<Result<LeadActivity>> AddActivityAsync(string leadId, LeadActivityDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var lead = await _db.Set<Lead>().FirstOrDefaultAsync(l => l.Id == leadId);
            if (lead == null)
                return Result<LeadActivity>.Failed("Lead not found.");

            var act = new LeadActivity 
            { 
                LeadId = leadId, 
                Kind = dto.Kind, 
                Notes = dto.Notes,
                When = DateTimeOffset.UtcNow
            };
            
            await _db.LeadActivities.AddAsync(act);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return Result<LeadActivity>.Success(act);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error adding activity to lead {LeadId}: {Message}", leadId, ex.Message);
            throw;
        }
    }

    public async Task<Result<List<Lead>>> ListAsync(CommonPaginationRequestModel requestModel, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _db.Set<Lead>()
            .Include(l => l.Prospect)
            .ThenInclude(p => p.PrimaryContact)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeadStatus>(status, true, out var statusEnum))
        {
            query = query.Where(l => l.Status == statusEnum);
        }

        if (from.HasValue)
        {
            query = query.Where(l => l.CreatedOn >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(l => l.CreatedOn <= to.Value);
        }

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
        var leads = await result.ToListAsync();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<Lead>>.Success(leads, pagination);
    }

    public async Task<Result<Lead>> GetByIdAsync(string id)
    {
        var lead = await _db.Set<Lead>()
            .Include(l => l.Prospect)
            .ThenInclude(p => p.PrimaryContact)
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lead == null)
            return Result<Lead>.Failed("Lead not found.");

        return Result<Lead>.Success(lead);
    }

    public async Task<Result<Lead>> UpdateStatusAsync(string id, string newStatus)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var lead = await _db.Set<Lead>().FirstOrDefaultAsync(l => l.Id == id);
            if (lead == null)
                return Result<Lead>.Failed("Lead not found.");

            if (!Enum.TryParse<LeadStatus>(newStatus, true, out var statusEnum))
            {
                return Result<Lead>.Failed($"Invalid status: {newStatus}");
            }

            // Simple state machine validation
            if (lead.Status == LeadStatus.Lost && statusEnum == LeadStatus.Contacted)
            {
                return Result<Lead>.Failed("Cannot move from Lost to Contacted directly.");
            }

            lead.Status = statusEnum;
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            // Reload with related entities
            await _db.Entry(lead).Reference(l => l.Prospect).LoadAsync();
            await _db.Entry(lead.Prospect).Reference(p => p.PrimaryContact).LoadAsync();
            
            return Result<Lead>.Success(lead);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating status for lead {LeadId}: {Message}", id, ex.Message);
            throw;
        }
    }

    public async Task<Result<List<LeadActivity>>> GetActivitiesAsync(string leadId)
    {
        var lead = await _db.Set<Lead>().AsNoTracking().FirstOrDefaultAsync(l => l.Id == leadId);
        if (lead == null)
            return Result<List<LeadActivity>>.Failed("Lead not found.");

        var activities = await _db.Set<LeadActivity>()
            .Where(a => a.LeadId == leadId)
            .OrderByDescending(a => a.When)
            .ToListAsync();

        return Result<List<LeadActivity>>.Success(activities);
    }
}
