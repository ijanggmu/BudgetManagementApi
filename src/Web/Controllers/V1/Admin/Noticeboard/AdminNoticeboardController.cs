using System.Threading;
using System.Threading.Tasks;
using BeemaEdgeApi.Controllers.V1.BaseController;
using BeemaEdgeApi.Filters.AuthorizationFilters;
using Business.AdminPortalApi.Noticeboard;
using Microsoft.AspNetCore.Mvc;
using Models.Common;
using Models.WebApi.Noticeboard;
using SharedKernel.Constant.Permission;

namespace BeemaEdgeApi.Controllers.V1.Admin.Noticeboard;

public class AdminNoticeboardController : BaseAdminApiController
{
    private readonly INoticeboardService _noticeboardService;

    public AdminNoticeboardController(INoticeboardService noticeboardService)
    {
        _noticeboardService = noticeboardService;
    }

    /// <summary>
    /// Get all noticeboard entries with pagination and filtering
    /// </summary>
    [HttpPost("list")]
    [Permission(MenuPermissionConstant.NoticeBoardView)]
    public async Task<IActionResult> GetAllNoticeboardsAsync(
        [FromBody] CommonPaginationRequestModel? requestModel = null,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await _noticeboardService.GetAllNoticeboardsAsync(requestModel, cancellationToken));
    }

    /// <summary>
    /// Get a noticeboard entry by ID
    /// </summary>
    [HttpGet("{id}")]
    [Permission(MenuPermissionConstant.NoticeBoardView)]
    public async Task<IActionResult> GetNoticeboardByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await _noticeboardService.GetNoticeboardByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Create a new noticeboard entry
    /// </summary>
    [HttpPost]
    [Permission(MenuPermissionConstant.NoticeBoardCreate)]
    public async Task<IActionResult> CreateNoticeboardAsync(
        [FromBody] CreateNoticeboardDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await _noticeboardService.CreateNoticeboardAsync(dto, cancellationToken));
    }

    /// <summary>
    /// Update an existing noticeboard entry
    /// </summary>
    [HttpPut("{id}")]
    [Permission(MenuPermissionConstant.NoticeBoardUpdate)]
    public async Task<IActionResult> UpdateNoticeboardAsync(
        string id,
        [FromBody] UpdateNoticeboardDto dto,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await _noticeboardService.UpdateNoticeboardAsync(id, dto, cancellationToken));
    }

    /// <summary>
    /// Delete a noticeboard entry (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Permission(MenuPermissionConstant.NoticeBoardDelete)]
    public async Task<IActionResult> DeleteNoticeboardAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        return HandleResult(await _noticeboardService.DeleteNoticeboardAsync(id, cancellationToken));
    }
}
