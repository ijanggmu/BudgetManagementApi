using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Data.Entities.Identity;
using Infrastructure.Common.PaginationAndFilter.Sieve;
using Infrastructure.Common.UserProfile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models.Common;
using Models.WebApi.TenantDTOs;
using SharedKernel.Operation;

namespace Business.Common.TenantDomain;

public class UserService : IUserService
{
    private readonly ApplicationDataContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISieveExtension _sieveExtension;
    private readonly IUserProfileService _userProfileService;

    public UserService(ApplicationDataContext db, UserManager<ApplicationUser> userManager, ISieveExtension sieveExtension, IUserProfileService userProfileService)
    {
        _db = db;
        _userManager = userManager;
        _sieveExtension = sieveExtension;
        _userProfileService = userProfileService;
    }

    private static UserResponseDto MapToDto(ApplicationUser user)
    {
        return new UserResponseDto(
            user.Id,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.EmailConfirmed,
            user.PhoneNumberConfirmed,
            user.IsDisabled,
            user.TenantId
        );
    }

    public async Task<Result<UserResponseDto>> CreateAsync(CreateUserDto dto)
    {
        if (await _userManager.FindByNameAsync(dto.UserName) != null)
            return Result<UserResponseDto>.Failed("Username already exists.");

        if (!string.IsNullOrEmpty(dto.Email) && await _userManager.FindByEmailAsync(dto.Email) != null)
            return Result<UserResponseDto>.Failed("Email already exists.");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return Result<UserResponseDto>.Failed(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (!string.IsNullOrEmpty(dto.Role))
        {
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        return Result<UserResponseDto>.Success(MapToDto(user));
    }

    public async Task<Result<List<UserResponseDto>>> ListAsync(CommonPaginationRequestModel requestModel)
    {
        var query = _db.Users.AsNoTracking().Where(u => !u.IsDeleted);

        var (result, totalCount, totalPage) = await _sieveExtension.ApplySieve(query, requestModel);
        var users = await result.ToListAsync();

        var userDtos = users.Select(MapToDto).ToList();

        var pagination = new Pagination
        {
            TotalItems = totalCount,
            TotalPages = totalPage,
            PageSize = requestModel.PageSize,
            CurrentPage = requestModel.PageNumber
        };

        return Result<List<UserResponseDto>>.Success(userDtos, pagination);
    }

    public async Task<Result<UserResponseDto>> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.IsDeleted)
            return Result<UserResponseDto>.Failed("User not found.");

        return Result<UserResponseDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserResponseDto>> UpdateAsync(string id, UpdateUserDto dto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.IsDeleted)
            return Result<UserResponseDto>.Failed("User not found.");

        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return Result<UserResponseDto>.Failed("Email already exists.");

            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        if (dto.IsDisabled.HasValue)
            user.IsDisabled = dto.IsDisabled.Value;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<UserResponseDto>.Failed(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result<UserResponseDto>.Success(MapToDto(user));
    }

    public async Task<Result<bool>> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null || user.IsDeleted)
            return Result<bool>.Failed("User not found.");

        user.IsDeleted = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result<bool>.Failed(string.Join(", ", result.Errors.Select(e => e.Description)));

        return Result<bool>.Success(true);
    }
}

