using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using LabConsumableExpiryTracker.Services.Interfaces;

namespace LabConsumableExpiryTracker.Services
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;
        private readonly TimeProvider _timeProvider;
        private readonly IMapper _mapper;
        public JobService(
            IJobRepository jobRepository,
            TimeProvider timeProvider,
            IMapper mapper
            )
        {
            _jobRepository = jobRepository;
            _timeProvider = timeProvider;
            _mapper = mapper;
        }



        public async Task<ServiceResult<JobDto>> CreateAsync(CreateJobDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.JobNumber))
            {
                ServiceResult<JobDto>.ErrorResult("Job number is required.");
            }

            var existing = await _jobRepository.GetByJobNumberAsync(dto.JobNumber, ct);
            if (existing is not null)
            {
                ServiceResult<JobDto>.ErrorResult($"Job number '{dto.JobNumber.Trim()}' already exists.");
            }

            var job = new Job(Guid.NewGuid(), dto.JobNumber);
            var created = await _jobRepository.AddAsync(job, ct);
            var response = _mapper.Map<JobDto>(created);
            return ServiceResult<JobDto>.SuccessResult(
                response,
                "Job Create successfully");
        }

        public async Task<ServiceResult<IReadOnlyList<JobDto>>> GetAllAsync(CancellationToken ct = default)
        {
            var jobs = await _jobRepository.GetAllAsync(ct);
            var response = _mapper.Map<IReadOnlyList<JobDto>>(jobs);
            return ServiceResult<JobDto>.SuccessResult(
                response,
                "Job retrieved successfully.");
        }

        public async Task<ServiceResult<JobDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            if (id == Guid.Empty)
            {
                ServiceResult<JobDto>.ErrorResult("Id is null");
            }
            var create = await _jobRepository.GetByIdAsync(id, ct);
            var response = _mapper.Map<JobDto>(create);
            return ServiceResult<JobDto>.SuccessResult(
                response);

        }
        public async Task<ServiceResult<JobDto>> UpdateStatusAsync(Guid id, UpdateJobDto dto, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(dto);
            return dto.Status switch
            {
                JobStatus.InProgress => await StartAsync(id, ct),
                JobStatus.Completed => await CompleteAsync(id, ct),
                _ => throw new InvalidOperationException("Only InProgress and Completed are valid lifecycle updates.")
            };

        }

        public async Task<ServiceResult<JobDto>> StartAsync(Guid id, CancellationToken ct = default)
        {
            return await TransitionAsync(
               id,
               job => job.Start(_timeProvider.GetUtcNow()),
               ct);
        }
        public async Task<ServiceResult<JobDto>> CompleteAsync(Guid id, CancellationToken ct = default)
        {
            return await TransitionAsync(
                id,
                job => job.Complete(_timeProvider.GetUtcNow()),
                ct);
        }
        private async Task<ServiceResult<JobDto>> TransitionAsync(Guid id, Action<Job> transition, CancellationToken ct)
        {
            if (id == Guid.Empty)
            {
                ServiceResult<JobDto>.ErrorResult("Job id is null");

            }
            var job = await _jobRepository.GetByIdAsync(id, ct);
            if (job is null)
            {
                return ServiceResult<JobDto>.ErrorResult($"Job with id '{id}' not found.");
            }
            transition(job);
            var create = await _jobRepository.UpdateAsync(job, ct);
            var response = _mapper.Map<JobDto>(create);
            return ServiceResult<JobDto>.SuccessResult(response);
        }


    }
}