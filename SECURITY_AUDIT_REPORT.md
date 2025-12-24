# Security Audit Report: XSS, CSRF, and SSRF Vulnerabilities

**Date:** $(Get-Date)  
**Scope:** BeemaEdge API Codebase  
**Vulnerabilities Checked:** XSS, CSRF, SSRF

---

## Executive Summary

This audit identified **1 CRITICAL SSRF vulnerability** and several **security configuration issues** that should be addressed. The application has good security foundations but needs improvements in URL validation and security header enforcement.

**Risk Summary:**
- 🔴 **CRITICAL:** 1 SSRF vulnerability
- 🟡 **MEDIUM:** 2 CSRF configuration issues
- 🟡 **MEDIUM:** 2 XSS configuration issues

---

## 1. SSRF (Server-Side Request Forgery) Vulnerabilities

### 🔴 **CRITICAL: SSRF in QuotationPdfService.DownloadImageAsync()**

**Location:** `src/Business/Business.Common/PdfGeneration/QuotationPdfService.cs:310-327`

**Vulnerability:**
The `DownloadImageAsync()` method accepts a user-controlled URL (`logoUrl`) and makes an HTTP GET request without proper validation. The `logoUrl` comes from `CompanyBranding.LogoUrl`, which can be set by users via the branding update API.

**Vulnerable Code:**
```csharp
private async Task<byte[]?> DownloadImageAsync(string url)
{
    try
    {
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        var response = await httpClient.GetAsync(url);  // ⚠️ No URL validation
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
    catch
    {
        return null;
    }
    return null;
}
```

**Attack Scenario:**
1. Attacker updates tenant branding with malicious URL: `http://169.254.169.254/latest/meta-data/` (AWS metadata)
2. Attacker generates a quotation PDF
3. Server fetches the malicious URL, potentially exposing internal services or cloud metadata

**Impact:**
- Access to internal services (localhost, private IPs)
- Cloud metadata exposure (AWS, Azure, GCP)
- Port scanning of internal network
- Information disclosure

**Recommendation:**
```csharp
private async Task<byte[]?> DownloadImageAsync(string url)
{
    try
    {
        // Validate URL scheme (only allow https)
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            return null;
            
        if (uri.Scheme != "https")
            return null;
            
        // Whitelist allowed domains (CDN, blob storage, etc.)
        var allowedDomains = new[] { "cdn.example.com", "storage.blob.core.windows.net", "s3.amazonaws.com" };
        if (!allowedDomains.Any(domain => uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase)))
            return null;
            
        // Block private/internal IP addresses
        if (IsPrivateIp(uri.Host))
            return null;
            
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.Timeout = TimeSpan.FromSeconds(10);
        
        // Prevent redirects to internal URLs
        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = false
        };
        httpClient = new HttpClient(handler);
        
        var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            // Validate content type is an image
            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (!contentType?.StartsWith("image/") ?? true)
                return null;
                
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
    catch
    {
        return null;
    }
    return null;
}

private bool IsPrivateIp(string host)
{
    // Check if host resolves to private IP
    // Implementation: Resolve DNS and check IP ranges
    // 10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16, 127.0.0.0/8, 169.254.0.0/16
    return false; // Placeholder - implement proper check
}
```

**Alternative (Safer):**
- Store logo images in your own blob storage (MinIO/S3)
- Only allow URLs from your own CDN/storage
- Use signed URLs with expiration

---

## 2. CSRF (Cross-Site Request Forgery) Vulnerabilities

### 🟡 **MEDIUM: Missing CSRF Protection on API Endpoints**

**Location:** API Controllers (all endpoints)

**Vulnerability:**
The application uses JWT tokens stored in HTTP-only cookies, which provides some CSRF protection via `SameSite=Strict`. However, there's no explicit anti-forgery token validation for state-changing operations.

**Current Protection:**
- ✅ JWT tokens in HTTP-only cookies
- ✅ `SameSite=Strict` for cookies (in production)
- ✅ `SameSite=Lax` for localhost (development)
- ❌ No `ValidateAntiForgeryToken` attributes on controllers
- ❌ No explicit CSRF token validation

**Risk:**
- Lower risk for API endpoints using JWT in cookies with SameSite
- Higher risk if cookies are accessible via JavaScript (XSS)
- State-changing operations (POST, PUT, DELETE) should have additional protection

**Recommendation:**
1. **For API endpoints:** Since you're using JWT in cookies, ensure:
   - All state-changing operations require authentication
   - Consider adding CSRF tokens for sensitive operations (tenant creation, admin creation, etc.)
   - Use `[ValidateAntiForgeryToken]` for cookie-based authentication endpoints

2. **For cookie-based sessions:** Add anti-forgery tokens:
```csharp
// In Program.cs
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.Secure = true;
});

// On sensitive endpoints
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
{
    // ...
}
```

3. **Current Status:** Hangfire dashboard correctly ignores anti-forgery tokens (line 52 in `Extensions.cs`)

---

### 🟡 **MEDIUM: CORS Configuration Allows Credentials**

**Location:** `src/Web/Extensions/Cors/Extensions.cs`

**Vulnerability:**
CORS policy allows credentials (`AllowCredentials()`) with origin validation. While origin validation exists, the configuration allows any method and any header, which could be exploited if origin validation fails.

**Current Configuration:**
```csharp
policy.AllowAnyMethod()      // ⚠️ Allows all HTTP methods
      .AllowAnyHeader()      // ⚠️ Allows all headers
      .SetIsOriginAllowed(origin => { /* validation */ })
      .AllowCredentials()    // ⚠️ Allows credentials
```

**Risk:**
- If origin validation has bugs, attackers could make cross-origin requests with credentials
- Overly permissive headers could allow custom headers that bypass security

**Recommendation:**
```csharp
policy.WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
      .WithHeaders("Content-Type", "Authorization", "X-Requested-With", "X-CSRF-TOKEN")
      .SetIsOriginAllowed(origin => { /* existing validation */ })
      .AllowCredentials()
```

---

## 3. XSS (Cross-Site Scripting) Vulnerabilities

### 🟡 **MEDIUM: Security Headers Disabled**

**Location:** `src/Web/Program.cs:103`

**Vulnerability:**
Security headers middleware is commented out, which disables important XSS protection headers.

**Vulnerable Code:**
```csharp
//app.UseSecurityHeaders(app.Configuration, app.Environment);  // ⚠️ COMMENTED OUT
```

**Impact:**
- No Content Security Policy (CSP)
- No X-Frame-Options
- No X-Content-Type-Options
- No X-XSS-Protection
- No HSTS

**Recommendation:**
```csharp
app.UseSecurityHeaders(app.Configuration, app.Environment);  // ✅ ENABLE THIS
```

---

### 🟡 **MEDIUM: Weak Content Security Policy**

**Location:** `src/Web/Extensions/SecurityHeaders/Extensions.cs:45-46`

**Vulnerability:**
CSP includes `'unsafe-inline'` and `'unsafe-eval'`, which significantly reduces XSS protection.

**Vulnerable Code:**
```csharp
var csp = "default-src 'self'; " +
          "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +  // ⚠️ UNSAFE
          "style-src 'self' 'unsafe-inline'; " +                 // ⚠️ UNSAFE
          // ...
```

**Risk:**
- `'unsafe-inline'` allows inline scripts, making XSS easier
- `'unsafe-eval'` allows `eval()`, `Function()`, etc., which can execute injected code

**Recommendation:**
1. **Remove unsafe-inline/eval:**
```csharp
var csp = "default-src 'self'; " +
          "script-src 'self'; " +           // ✅ Remove unsafe-inline
          "style-src 'self'; " +             // ✅ Remove unsafe-inline
          "img-src 'self' data: https:; " +
          "font-src 'self' data:; " +
          "connect-src 'self' https:; " +
          "frame-ancestors 'none'; " +
          "base-uri 'self'; " +
          "form-action 'self';";
```

2. **Use nonces for inline scripts (if needed):**
```csharp
// Generate nonce per request
var nonce = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
context.Items["CSPNonce"] = nonce;

var csp = $"script-src 'self' 'nonce-{nonce}'; ...";
```

3. **If you must use unsafe-inline (temporary):**
   - Document why it's needed
   - Plan to remove it
   - Add additional XSS protection layers

---

## 4. Additional Security Observations

### ✅ **Good Security Practices Found:**

1. **JWT Token Security:**
   - HTTP-only cookies ✅
   - SameSite protection ✅
   - Secure flag in production ✅

2. **Input Validation:**
   - FluentValidation used ✅
   - DTO validation ✅

3. **Authentication:**
   - Proper JWT validation ✅
   - Role-based authorization ✅

4. **HTTPS:**
   - HTTPS redirection enabled ✅
   - HSTS configured (when security headers enabled) ✅

### ⚠️ **Areas for Improvement:**

1. **URL Validation:**
   - No URL validation utility
   - No whitelist/blacklist for external URLs

2. **Error Handling:**
   - Some catch blocks swallow exceptions (could hide security issues)
   - Consider logging security-relevant errors

3. **Rate Limiting:**
   - Rate limiting is configured ✅
   - Verify it's applied to all sensitive endpoints

---

## 5. Priority Recommendations

### **Immediate (Critical):**
1. ✅ **Fix SSRF in QuotationPdfService** - Add URL validation and whitelist
2. ✅ **Enable Security Headers** - Uncomment `UseSecurityHeaders()`

### **Short-term (High Priority):**
3. ✅ **Strengthen CSP** - Remove `unsafe-inline` and `unsafe-eval`
4. ✅ **Add CSRF Protection** - For sensitive operations
5. ✅ **Tighten CORS** - Specify allowed methods and headers

### **Medium-term:**
6. ✅ **Implement URL Validation Utility** - Reusable for all external URLs
7. ✅ **Add Security Logging** - Log SSRF attempts, failed validations
8. ✅ **Security Testing** - Add automated tests for SSRF/XSS/CSRF

---

## 6. Testing Recommendations

### **SSRF Testing:**
```bash
# Test with internal IPs
curl -X POST /api/v1/admin/branding/update \
  -H "Authorization: Bearer <token>" \
  -d '{"logoUrl": "http://169.254.169.254/latest/meta-data/"}'

# Test with localhost
curl -X POST /api/v1/admin/branding/update \
  -H "Authorization: Bearer <token>" \
  -d '{"logoUrl": "http://localhost:8080/admin"}'
```

### **CSRF Testing:**
```html
<!-- Test CSRF protection -->
<form action="https://api.example.com/api/v1/admin/tenant" method="POST">
  <input type="hidden" name="name" value="Evil Tenant">
  <input type="submit" value="Submit">
</form>
```

### **XSS Testing:**
- Test all user input fields for XSS
- Check if user input is properly encoded in responses
- Verify CSP is working

---

## 7. Conclusion

The codebase has a **solid security foundation** but requires **immediate attention** to the SSRF vulnerability and security header configuration. The identified issues are fixable with proper URL validation and security header enforcement.

**Overall Security Rating:** 🟡 **MEDIUM** (with 1 Critical issue)

**Next Steps:**
1. Fix SSRF vulnerability immediately
2. Enable and strengthen security headers
3. Add CSRF protection for sensitive operations
4. Implement comprehensive security testing

---

*Report generated by security audit*

