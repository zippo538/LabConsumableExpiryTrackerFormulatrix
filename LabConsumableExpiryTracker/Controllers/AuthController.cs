using FluentValidation;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs.Auth;
using LabConsumableExpiryTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabConsumableExpiryTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResult<AuthResponseDto>>> Login(
        LoginRequestDto request)
    {
        var response = await _identityService.LoginAsync(request);

        if (!response.Success)
            return Unauthorized(response);

        return Ok(response);
    }

    
}