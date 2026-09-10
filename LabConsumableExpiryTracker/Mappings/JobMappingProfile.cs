using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LabConsumableExpiryTracker.DTOs.JobDTOs;
using LabConsumableExpiryTracker.Models;

namespace LabConsumableExpiryTracker.Mappings
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<Job, JobDto>()
            .ForMember(
                destination => destination.JobNumber,
                option =>option.MapFrom(
                source => source.JobNumber
                )
            )
            .ForMember(
                destination => destination.Status,
                option =>option.MapFrom(
                source => source.Status
                )
            )
            .ForMember(
                destination => destination.StartedAt,
                option =>option.MapFrom(
                source => source.StartedAt
                )
            )
            .ForMember(
                destination => destination.CompletedAt,
                option =>option.MapFrom(
                source => source.CompletedAt
                )
            )
            ;
            CreateMap<CreateJobDto, Job>();
            CreateMap<UpdateJobDto, Job>();
        }
    }
}