using System;
using System.Collections.Generic;

namespace Models.WebApi.TenantDTOs;

// Request DTOs
public record AttendanceDto(double Latitude, double Longitude, string? Remarks = null);

// Response DTOs for Admin
public record AttendanceResponseDto(
    string Id,
    string UserId,
    string? UserName,
    string? FullName,
    string Type,
    double Latitude,
    double Longitude,
    DateTime Timestamp,
    string? Remarks,
    string? TenantId,
    string? TenantName,
    DateTime CreatedOn
);

// DTO for syncing to tenant API
public record TenantAttendanceSyncDto(
    string UserId,
    string? UserName,
    string? FullName,
    string Type,
    double Latitude,
    double Longitude,
    DateTime Timestamp,
    string? Remarks
);

