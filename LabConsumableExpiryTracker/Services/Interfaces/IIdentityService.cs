using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs.Auth;

namespace LabConsumableExpiryTracker.Services.Interfaces;

public interface IIdentityService
{
    Task<ServiceResult<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ServiceResult<ScientistResponseDto>> RegisterScientistAsync(CreateScientistRequestDto createScientistRequest);
    Task<ServiceResult<List<ScientistResponseDto>>> GetAllScientistsAsync();
    Task<ServiceResult<ScientistResponseDto>> UpdateScientistAsync(Guid id, UpdateScientistRequestDto request);
    Task<ServiceResult<bool>> DeleteScientistAsync(Guid id);
    Task<ServiceResult<bool>> SetScientistActiveStatusAsync(Guid id, bool inactive);
}