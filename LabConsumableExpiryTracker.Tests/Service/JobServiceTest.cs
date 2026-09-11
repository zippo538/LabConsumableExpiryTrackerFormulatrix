using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using LabConsumableExpiryTracker.Repositories.Interfaces;
using LabConsumableExpiryTracker.Services;
using LabConsumableExpiryTracker.Services.Interfaces;
using Moq;

namespace LabConsumableExpiryTracker.Tests.Service
{
    public class JobServiceTests
    {
        private Mock<IJobRepository> _repository;
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IMapper> _mapper;

        private JobService _service;


        [SetUp]
        public void Setup()
        {
            _repository = new();
            _unitOfWork = new();
            _mapper = new();


            _service = new JobService(
                _repository.Object,
                TimeProvider.System,
                _mapper.Object,
                _unitOfWork.Object
            );
        }



        [Test]
        public async Task CreateAsync_Should_Return_Error_When_JobNumber_Empty()
        {
            var dto = new CreateJobDto
            {
                JobNumber = ""
            };

            var result = await _service.CreateAsync(dto);
            result.Success.Should()
                .BeFalse();
            result.Message.Should()
                .Contain("required");
        }
        [Test]
        public async Task CreateAsync_Should_Return_Error_When_JobNumber_Exists()
        {
            var dto = new CreateJobDto
            {
                JobNumber = "JOB-001"
            };
            _repository
                .Setup(x => x.ExistsByJobNumberAsync(
                    "JOB-001",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var result = await _service.CreateAsync(dto);
            result.Success.Should()
                .BeFalse();
            result.Message.Should()
                .Contain("already exists");
        }
        [Test]
        public async Task CreateAsync_Should_Create_New_Job()
        {
            var dto = new CreateJobDto
            {
                JobNumber = "JOB-001"
            };
            var job =
                new Job(
                    Guid.NewGuid(),
                    "JOB-001");
            _repository
                .Setup(x => x.ExistsByJobNumberAsync(
                    "JOB-001",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _repository
                .Setup(x => x.AddAsync(
                    It.IsAny<Job>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(job);
            _mapper
                .Setup(x => x.Map<JobDto>(
                    It.IsAny<Job>()))
                .Returns(new JobDto
                {
                    Id = job.Id,
                    JobNumber = "JOB-001"
                });
            var result =
                await _service.CreateAsync(dto);
            result.Success.Should()
                .BeTrue();
            _unitOfWork.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}