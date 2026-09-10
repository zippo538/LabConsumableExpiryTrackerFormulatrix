using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs.JobDTOs;

namespace LabConsumableExpiryTracker.Services.Interfaces
{
    public interface IJobService
    {
        Task<ServiceResult<JobDto>> CreateAsync(CreateJobDto dto, CancellationToken ct = default);
        Task<ServiceResult<JobDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<ServiceResult<IReadOnlyList<JobDto>>> GetAllAsync(CancellationToken ct = default);
        Task<ServiceResult<JobDto>> StartAsync(Guid id, CancellationToken ct = default);
        Task<ServiceResult<JobDto>> CompleteAsync(Guid id, CancellationToken ct = default);
        Task<ServiceResult<JobDto>> UpdateStatusAsync(Guid id, UpdateJobDto dto, CancellationToken ct = default);
    }
}