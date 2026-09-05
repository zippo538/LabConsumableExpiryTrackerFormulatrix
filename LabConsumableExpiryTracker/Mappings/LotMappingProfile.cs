using AutoMapper;
using LabConsumableExpireTracker.Models;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Mappings
{
    public class LotMappingProfile : Profile
    {
        public LotMappingProfile()
        {
            CreateMap<Lot, LotDTO>();

             CreateMap<CreateLotDTO, Lot>()
            .ConstructUsing(source => new Lot(
                Guid.NewGuid(),
                source.ItemId,
                source.LotNumber,
                source.SupplierLotNumber,
                new DateTimeOffset(source.ReceivedAt),
                source.SupplierName));

        CreateMap<UpdateLotDTO, Lot>();
        }

        
    }
}