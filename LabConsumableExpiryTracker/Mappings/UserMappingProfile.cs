using AutoMapper;
using LabConsumableExpiryTracker.DTOs.Auth;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;

namespace LabConsumableExpiryTracker.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, ScientistResponseDto>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(_ => UserRole.Scientist.ToString())
            );
    }
}