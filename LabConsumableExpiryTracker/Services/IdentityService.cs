using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.Configurations;
using LabConsumableExpiryTracker.DTOs.Auth;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using LabConsumableExpiryTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LabConsumableExpiryTracker.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly IMapper _mapper;

    public IdentityService(
        UserManager<User> userManager,
        IOptions<JwtSettings> jwtSettings,
        IMapper mapper)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _mapper = mapper;
    }

    public async Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByNameAsync(
            request.Username
        );

        if (user == null || !user.IsActive)
        {
            return ServiceResult<AuthResponseDto>.ErrorResult(
                "User not found or inactive."
            );
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(
            user,
            request.Password
        );

        if (!isPasswordValid)
        {
            return ServiceResult<AuthResponseDto>.ErrorResult(
                "Invalid password."
            );
        }

        string tSuccessResulten = await GenerateJwtTSuccessResultenAsync(user);

        var response = new AuthResponseDto(
            tSuccessResulten
        );

        return ServiceResult<AuthResponseDto>.SuccessResult(response);
    }

    public async Task<ServiceResult<ScientistResponseDto>> RegisterScientistAsync(
        CreateScientistRequestDto createScientistRequest)
    {
        var user = new User(
            createScientistRequest.Username,
            createScientistRequest.Email
        );

        var result = await _userManager.CreateAsync(
            user,
            createScientistRequest.Password
        );

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToList();

            return ServiceResult<ScientistResponseDto>.ErrorResult(
                "Failed to create scientist.",
                errors
            );
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            UserRole.Scientist.ToString()
        );

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            var errors = roleResult.Errors
                .Select(e => e.Description)
                .ToList();

            return ServiceResult<ScientistResponseDto>.ErrorResult(
                "Failed to assign scientist role.",
                errors
            );
        }

        var response = _mapper.Map<ScientistResponseDto>(user);

        return ServiceResult<ScientistResponseDto>.SuccessResult(response);
    }

    public async Task<ServiceResult<List<ScientistResponseDto>>> GetAllScientistsAsync()
    {
        var scientists = await _userManager.GetUsersInRoleAsync(UserRole.Scientist.ToString());

        var response = _mapper.Map<List<ScientistResponseDto>>(scientists);

        return ServiceResult<List<ScientistResponseDto>>.SuccessResult(response);
    }

    public async Task<ServiceResult<ScientistResponseDto>> UpdateScientistAsync(Guid id,
        UpdateScientistRequestDto request)
    {
        var scientist = await _userManager.FindByIdAsync(id.ToString());

        if (scientist is null || !await _userManager.IsInRoleAsync(scientist, UserRole.Scientist.ToString()))
        {
            return ServiceResult<ScientistResponseDto>.ErrorResult(
                "Scientist not found."
            );
        }

        scientist.UserName = request.Username;
        scientist.Email = request.Email;

        var result = await _userManager.UpdateAsync(scientist);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToList();

            return ServiceResult<ScientistResponseDto>.ErrorResult(
                "ErrorResulted to update scientist detail.",
                errors
            );
        }

        var response = _mapper.Map<ScientistResponseDto>(scientist);

        return ServiceResult<ScientistResponseDto>.SuccessResult(response);
    }

    public async Task<ServiceResult<bool>> SetScientistActiveStatusAsync(
        Guid id,
        bool isActive)
    {
        var scientist = await _userManager.FindByIdAsync(id.ToString());

        if (scientist is null ||
            !await _userManager.IsInRoleAsync(
                scientist,
                UserRole.Scientist.ToString()))
        {
            return ServiceResult<bool>.ErrorResult(
                "Scientist not found."
            );
        }

        scientist.IsActive = isActive;

        var result = await _userManager.UpdateAsync(scientist);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToList();

            return ServiceResult<bool>.ErrorResult(
                "ErrorResulted to update scientist status.",
                errors
            );
        }

        return ServiceResult<bool>.SuccessResult(true);
    }

    public async Task<ServiceResult<bool>> DeleteScientistAsync(Guid id)
    {
        var scientist = await _userManager.FindByIdAsync(id.ToString());

        if (scientist is null || !await _userManager.IsInRoleAsync(scientist, UserRole.Scientist.ToString()))
        {
            return ServiceResult<bool>.ErrorResult(
                "Scientist tidak ditemukan."
            );
        }

        var result = await _userManager.DeleteAsync(scientist);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToList();

            return ServiceResult<bool>.ErrorResult(
                "ErrorResulted to delete scientist account.",
                errors
            );
        }

        return ServiceResult<bool>.SuccessResult(true);
    }

    private async Task<string> GenerateJwtTSuccessResultenAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);


        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret)
        );

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );


        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiryMinutes
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}