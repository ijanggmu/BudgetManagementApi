using Models.Common;
using Models.WebApi.Noticeboard;
using SharedKernel.Operation;

namespace Business.AdminPortalApi.Noticeboard;

/// <summary>
/// Service interface for managing noticeboard entries
/// </summary>
public interface INoticeboardService
{
    /// <summary>
    /// Get all noticeboard entries with pagination and filtering
    /// </summary>
    Task<Result<List<NoticeboardResponseDto>>> GetAllNoticeboardsAsync(
        CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a noticeboard entry by ID
    /// </summary>
    Task<Result<NoticeboardResponseDto>> GetNoticeboardByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new noticeboard entry
    /// </summary>
    Task<Result<NoticeboardResponseDto>> CreateNoticeboardAsync(
        CreateNoticeboardDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing noticeboard entry
    /// </summary>
    Task<Result<NoticeboardResponseDto>> UpdateNoticeboardAsync(
        string id,
        UpdateNoticeboardDto dto,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a noticeboard entry (soft delete)
    /// </summary>
    Task<Result<MessageResponseModel>> DeleteNoticeboardAsync(
        string id,
        CancellationToken cancellationToken = default);
}
