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
using Microsoft.EntityFrameworkCore;
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
            return ServiceResult<AuthResponseDto>.Fail(
                "User not found or inactive."
            );
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(
            user,
            request.Password
        );

        if (!isPasswordValid)
        {
            return ServiceResult<AuthResponseDto>.Fail(
                "Invalid password."
            );
        }

        string token = await GenerateJwtTokenAsync(user);

        var response = new AuthResponseDto(
            token
        );

        return ServiceResult<AuthResponseDto>.Ok(response);
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
                .ToArray();

            return ServiceResult<ScientistResponseDto>.Fail(errors);
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
                .ToArray();

            return ServiceResult<ScientistResponseDto>.Fail(errors);
        }

        var response = _mapper.Map<ScientistResponseDto>(user);

        return ServiceResult<ScientistResponseDto>.Ok(response);
    }

    public async Task<ServiceResult<List<ScientistResponseDto>>> GetAllScientistsAsync()
    {
        var scientists = await _userManager.GetUsersInRoleAsync(UserRole.Scientist.ToString());

        var response = _mapper.Map<List<ScientistResponseDto>>(scientists);

        return ServiceResult<List<ScientistResponseDto>>.Ok(response);
    }

    public async Task<ServiceResult<ScientistResponseDto>> UpdateScientistAsync(Guid id,
        UpdateScientistRequestDto request)
    {
        var scientist = await _userManager.FindByIdAsync(id.ToString());

        if (scientist is null || !await _userManager.IsInRoleAsync(scientist, UserRole.Scientist.ToString()))
        {
            return ServiceResult<ScientistResponseDto>.Fail(
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
                .ToArray();

            return ServiceResult<ScientistResponseDto>.Fail(errors);
        }

        var response = _mapper.Map<ScientistResponseDto>(scientist);

        return ServiceResult<ScientistResponseDto>.Ok(response);
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
            return ServiceResult<bool>.Fail(
                "Scientist not found."
            );
        }

        scientist.IsActive = isActive;

        var result = await _userManager.UpdateAsync(scientist);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToArray();

            return ServiceResult<bool>.Fail(errors);
        }

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteScientistAsync(Guid id)
    {
        var scientist = await _userManager.FindByIdAsync(id.ToString());

        if (scientist is null || !await _userManager.IsInRoleAsync(scientist, UserRole.Scientist.ToString()))
        {
            return ServiceResult<bool>.Fail(
                "Scientist tidak ditemukan."
            );
        }

        var result = await _userManager.DeleteAsync(scientist);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => e.Description)
                .ToArray();

            return ServiceResult<bool>.Fail(errors);
        }

        return ServiceResult<bool>.Ok(true);
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
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