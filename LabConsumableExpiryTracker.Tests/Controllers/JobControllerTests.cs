using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LabConsumableExpiryTracker.Commons.Result;
using LabConsumableExpiryTracker.Controllers;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using LabConsumableExpiryTracker.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LabConsumableExpiryTracker.Tests.Controllers
{
    public class JobControllerTests
    {
        private Mock<IJobService> _service;
        private JobController _controller;
        [SetUp]
        public void Setup()
        {
            _service = new Mock<IJobService>();

            var createValidator =
                new Mock<IValidator<CreateJobDto>>();

            createValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<CreateJobDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ValidationResult());


            var updateValidator =
                new Mock<IValidator<UpdateJobDto>>();

            updateValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<UpdateJobDto>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new ValidationResult());


            _controller = new JobController(
                _service.Object,
                createValidator.Object,
                updateValidator.Object);
        }
        [Test]
        public async Task GetAll_Should_Return_Ok_When_Success()
        {
            IReadOnlyList<JobDto> jobs =
                new List<JobDto>
                {
            new JobDto
            {
                Id = Guid.NewGuid(),
                JobNumber = "JOB001"
            }
                };


            _service
                .Setup(x => x.GetAllAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<IReadOnlyList<JobDto>>
                    .SuccessResult(jobs)
                );


            var result =
                await _controller.GetAll(
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<OkObjectResult>();
        }
        [Test]
        public async Task GetById_Should_Return_Ok_When_Exists()
        {
            var id = Guid.NewGuid();


            _service
                .Setup(x => x.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .SuccessResult(
                        new JobDto
                        {
                            Id = id
                        }));


            var result =
                await _controller.GetById(
                    id,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<OkObjectResult>();
        }
        [Test]
        public async Task GetById_Should_Return_NotFound_When_Missing()
        {
            var id = Guid.NewGuid();


            _service
                .Setup(x => x.GetByIdAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .ErrorResult(
                        "Job not found."));


            var result =
                await _controller.GetById(
                    id,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<NotFoundObjectResult>();
        }
        [Test]
        public async Task Create_Should_Return_Created_When_Success()
        {
            var dto = new CreateJobDto
            {
                JobNumber = "JOB001"
            };


            var job =
                new JobDto
                {
                    Id = Guid.NewGuid(),
                    JobNumber = "JOB001"
                };


            _service
                .Setup(x => x.CreateAsync(
                    dto,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .SuccessResult(job));


            var result =
                await _controller.Create(
                    dto,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<CreatedAtActionResult>();
        }
        [Test]
        public async Task Create_Should_Return_Conflict_When_Duplicate()
        {
            var dto = new CreateJobDto
            {
                JobNumber = "JOB001"
            };


            _service
                .Setup(x => x.CreateAsync(
                    dto,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .ErrorResult(
                        "Job number already exists."));


            var result =
                await _controller.Create(
                    dto,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<ConflictObjectResult>();
        }
        [Test]
        public async Task Update_Should_Return_Ok_When_Success()
        {
            var id = Guid.NewGuid();


            var dto = new UpdateJobDto();


            _service
                .Setup(x => x.UpdateStatusAsync(
                    id,
                    dto,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .SuccessResult(
                        new JobDto()));


            var result =
                await _controller.Update(
                    id,
                    dto,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<OkObjectResult>();
        }
        [Test]
        public async Task Start_Should_Return_Ok_When_Success()
        {
            var id = Guid.NewGuid();


            _service
                .Setup(x => x.StartAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .SuccessResult(
                        new JobDto()));


            var result =
                await _controller.Start(
                    id,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<OkObjectResult>();
        }
        [Test]
        public async Task Complete_Should_Return_Ok_When_Success()
        {
            var id = Guid.NewGuid();


            _service
                .Setup(x => x.CompleteAsync(
                    id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ServiceResult<JobDto>
                    .SuccessResult(
                        new JobDto()));


            var result =
                await _controller.Complete(
                    id,
                    CancellationToken.None);


            result.Result
                .Should()
                .BeOfType<OkObjectResult>();
        }
    }
}