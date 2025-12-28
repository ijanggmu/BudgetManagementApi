using Data.Context;
using Data.Entities.Identity;
using Data.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SharedKernel.Constant.Roles;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Infrastructure;

/// <summary>
/// Service for resolving tenant information with multi-level caching.
/// Implements L1 (HttpContext) and L2 (MemoryCache) caching strategy.
/// </summary>
public class TenantResolutionService : ITenantResolutionService
{
    private readonly ApplicationDataContext _db;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<TenantResolutionService> _logger;
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

    // Cache key prefixes
    private const string TenantBySlugCacheKeyPrefix = "tenant:slug:";
    private const string TenantByIdCacheKeyPrefix = "tenant:id:";
    private const string UserRolesCacheKeyPrefix = "user:roles:";
    private const string UserTenantIdCacheKeyPrefix = "user:tenantid:";

    // Cache TTLs
    private static readonly TimeSpan TenantCacheTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan UserCacheTtl = TimeSpan.FromMinutes(1);

    private static readonly MemoryCacheEntryOptions TenantCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TenantCacheTtl,
        SlidingExpiration = TimeSpan.FromMinutes(2)
    };

    private static readonly MemoryCacheEntryOptions UserCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = UserCacheTtl,
        SlidingExpiration = TimeSpan.FromSeconds(30)
    };

    public TenantResolutionService(
        ApplicationDataContext db,
        IMemoryCache memoryCache,
        ILogger<TenantResolutionService> logger,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _memoryCache = memoryCache;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Tenant?> ResolveTenantBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return null;

        // L1 Cache: Check HttpContext.Items (request-level cache)
        var httpContext = _httpContextAccessor.HttpContext;
        var l1CacheKey = $"tenant_slug_{slug}";
        if (httpContext?.Items.TryGetValue(l1CacheKey, out var cachedTenant) == true && cachedTenant is Tenant tenant)
        {
            _logger.LogDebug("Tenant resolved from L1 cache (HttpContext): {Slug}", slug);
            return tenant;
        }

        // L2 Cache: Check MemoryCache
        var cacheKey = $"{TenantBySlugCacheKeyPrefix}{slug}";
        if (!_memoryCache.TryGetValue(cacheKey, out Tenant? result))
        {
            _logger.LogDebug("Tenant not in cache, querying database: {Slug}", slug);
            result = await _db.Set<Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == slug && t.IsActive);
            
            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, TenantCacheOptions);
            }
        }
        else
        {
            _logger.LogDebug("Tenant resolved from L2 cache (MemoryCache): {Slug}", slug);
        }

        // Store in L1 cache for this request
        if (httpContext != null && result != null)
        {
            httpContext.Items[l1CacheKey] = result;
        }

        return result;
    }

    public async Task<Tenant?> ResolveTenantByIdAsync(string tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            return null;

        // L1 Cache: Check HttpContext.Items
        var httpContext = _httpContextAccessor.HttpContext;
        var l1CacheKey = $"tenant_id_{tenantId}";
        if (httpContext?.Items.TryGetValue(l1CacheKey, out var cachedTenant) == true && cachedTenant is Tenant tenant)
        {
            _logger.LogDebug("Tenant resolved from L1 cache (HttpContext): {TenantId}", tenantId);
            return tenant;
        }

        // L2 Cache: Check MemoryCache
        var cacheKey = $"{TenantByIdCacheKeyPrefix}{tenantId}";
        if (!_memoryCache.TryGetValue(cacheKey, out Tenant? result))
        {
            _logger.LogDebug("Tenant not in cache, querying database: {TenantId}", tenantId);
            result = await _db.Set<Tenant>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId && t.IsActive);
            
            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, TenantCacheOptions);
            }
        }
        else
        {
            _logger.LogDebug("Tenant resolved from L2 cache (MemoryCache): {TenantId}", tenantId);
        }

        // Store in L1 cache for this request
        if (httpContext != null && result != null)
        {
            httpContext.Items[l1CacheKey] = result;
        }

        return result;
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return new List<string>();

        // L1 Cache: Check HttpContext.Items
        var httpContext = _httpContextAccessor.HttpContext;
        var l1CacheKey = $"user_roles_{userId}";
        if (httpContext?.Items.TryGetValue(l1CacheKey, out var cachedRoles) == true && cachedRoles is List<string> roles)
        {
            _logger.LogDebug("User roles resolved from L1 cache (HttpContext): {UserId}", userId);
            return roles;
        }

        // L2 Cache: Check MemoryCache
        var cacheKey = $"{UserRolesCacheKeyPrefix}{userId}";
        if (!_memoryCache.TryGetValue(cacheKey, out List<string>? result))
        {
            _logger.LogDebug("User roles not in cache, querying database: {UserId}", userId);
            result = await _db.UserRoles
                .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                .Join(_db.Roles.Where(r => !r.IsDeleted),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .ToListAsync();
            
            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, UserCacheOptions);
            }
        }
        else
        {
            _logger.LogDebug("User roles resolved from L2 cache (MemoryCache): {UserId}", userId);
        }

        // Store in L1 cache for this request
        if (httpContext != null && result != null)
        {
            httpContext.Items[l1CacheKey] = result;
        }

        return result ?? new List<string>();
    }

    public async Task<string?> GetUserTenantIdAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return null;

        // L1 Cache: Check HttpContext.Items
        var httpContext = _httpContextAccessor.HttpContext;
        var l1CacheKey = $"user_tenantid_{userId}";
        if (httpContext?.Items.TryGetValue(l1CacheKey, out var cachedTenantId) == true && cachedTenantId is string tenantId)
        {
            _logger.LogDebug("User tenant ID resolved from L1 cache (HttpContext): {UserId}", userId);
            return tenantId;
        }

        // L2 Cache: Check MemoryCache
        var cacheKey = $"{UserTenantIdCacheKeyPrefix}{userId}";
        if (!_memoryCache.TryGetValue(cacheKey, out string? result))
        {
            _logger.LogDebug("User tenant ID not in cache, querying database: {UserId}", userId);
            result = await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && !u.IsDeleted && !u.IsDisabled)
                .Select(u => u.TenantId)
                .FirstOrDefaultAsync();
            
            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, UserCacheOptions);
            }
        }
        else
        {
            _logger.LogDebug("User tenant ID resolved from L2 cache (MemoryCache): {UserId}", userId);
        }

        // Store in L1 cache for this request
        if (httpContext != null && result != null)
        {
            httpContext.Items[l1CacheKey] = result;
        }

        return result;
    }

    public Task InvalidateTenantCacheAsync(string tenantId, string? slug = null)
    {
        _memoryCache.Remove($"{TenantByIdCacheKeyPrefix}{tenantId}");

        if (!string.IsNullOrWhiteSpace(slug))
        {
            _memoryCache.Remove($"{TenantBySlugCacheKeyPrefix}{slug}");
        }

        _logger.LogInformation("Invalidated tenant cache: {TenantId}, {Slug}", tenantId, slug);
        return Task.CompletedTask;
    }

    public Task InvalidateUserCacheAsync(string userId)
    {
        _memoryCache.Remove($"{UserRolesCacheKeyPrefix}{userId}");
        _memoryCache.Remove($"{UserTenantIdCacheKeyPrefix}{userId}");
        _logger.LogInformation("Invalidated user cache: {UserId}", userId);
        return Task.CompletedTask;
    }
}

