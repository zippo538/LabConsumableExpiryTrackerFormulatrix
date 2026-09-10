using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LabConsumableExpiryTracker.Controllers.Helper;

public static class JobResponseHelper
{
    public static (int StatusCode, ServiceResult<JobDto> Response)
        HandleJobResponse(ServiceResult<JobDto> response)
    {
        if (response.Success)
        {
            return (
                StatusCodes.Status200OK,
                response
            );
        }
        if (response.Message.Contains(
                "not found",
                StringComparison.OrdinalIgnoreCase))
        {
            return (
                StatusCodes.Status404NotFound,
                response
            );
        }
        if (response.Message.Contains(
                "already exists",
                StringComparison.OrdinalIgnoreCase))
        {
            return (
                StatusCodes.Status409Conflict,
                response
            );
        }
        return (
            StatusCodes.Status400BadRequest,
            response
        );
    }
}