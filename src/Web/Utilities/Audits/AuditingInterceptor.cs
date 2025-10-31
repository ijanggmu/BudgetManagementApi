using Data.Entities.BaseEntity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Security.Claims;
using SharedKernel.Constant;
using Data.Entities.Audit;

namespace BeemaEdgeApi.Utilities.Audits;

public class AuditingInterceptor : ISaveChangesInterceptor
{
    private SaveChangesAudit _audit;
    private readonly Channel<SaveChangesAudit> _channel;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditingInterceptor(Channel<SaveChangesAudit> channel, IHttpContextAccessor httpContextAccessor)
    {
        _channel = channel;
        _httpContextAccessor = httpContextAccessor;
    }

    public async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _audit = CreateAudit(eventData.Context);
        await _channel.Writer.WriteAsync(_audit, cancellationToken);
        return result;
    }

    public InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        _audit = CreateAudit(eventData.Context);
        _ = _channel.Writer.WriteAsync(_audit);

        return result;
    }

    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {


        if (_audit is not null)
        {
            _audit.Succeeded = true;
            _audit.EndAt = DateTime.UtcNow;
        }

        _ = _channel.Writer.WriteAsync(_audit);

        return result;
    }

    public async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (_audit is not null)
        {
            _audit.Succeeded = true;
            _audit.EndAt = DateTime.UtcNow;
        }

        await _channel.Writer.WriteAsync(_audit, cancellationToken);

        return result;
    }

    public void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        if (_audit is not null)
        {
            _audit.Succeeded = false;
            _audit.EndAt = DateTime.UtcNow;
            _audit.ErrorMessage = eventData.Exception.Message;
        }


        _ = _channel.Writer.WriteAsync(_audit);

    }

    public async Task SaveChangesFailedAsync(
        DbContextErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (_audit is not null)
        {

            _audit.Succeeded = false;
            _audit.EndAt = DateTime.UtcNow;
            _audit.ErrorMessage = eventData.Exception.InnerException?.Message;
        }

        await _channel.Writer.WriteAsync(_audit, cancellationToken);
    }

    private SaveChangesAudit CreateAudit(DbContext context)
    {
        var remoteIpAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var userId = _httpContextAccessor?.HttpContext?.User.Claims.Where(x => x.Type == TokenKey.UserId)?.FirstOrDefault()?.Value;
        var userName = _httpContextAccessor?.HttpContext?.User.Claims.Where(x => x.Type == ClaimTypes.Name)?.FirstOrDefault()?.Value;

        var audit = new SaveChangesAudit
        {
            UserId = userId,
            UserName = userName,
            RemoteIpAddress = remoteIpAddress,
            StartAt = DateTimeOffset.UtcNow,
        };

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            var state = entry.State;

            if (state == EntityState.Detached || state == EntityState.Unchanged)
            {
                break;
            }

            var isModified = state == EntityState.Modified;

            var auditEntry = new AuditEntry(entry)
            {
                TableName = entry.Metadata.GetTableName()
            };


            foreach (var property in entry.Properties)
            {
                var propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (state)
                {
                    case EntityState.Added:
                        auditEntry.State = state;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.State = state;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                }

                if (isModified && property.IsModified)
                {
                    auditEntry.ChangedColumns.Add(propertyName);
                    auditEntry.State = state;
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                }
            }

            audit.Entities.Add(auditEntry.ToAudit());
        }

        return audit;
    }
}
