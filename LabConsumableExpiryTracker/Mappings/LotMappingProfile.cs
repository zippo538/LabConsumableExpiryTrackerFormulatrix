using AutoMapper;
using LabConsumableExpiryTracker.Models;
using LabConsumableExpiryTracker.Models.Enums;
using LabConsumableExpiryTracker.DTOs;

namespace LabConsumableExpiryTracker.Mappings
{
    public class LotMappingProfile : Profile
    {
        public LotMappingProfile()
        {
            CreateMap<Lot, LotDto>();


            CreateMap<CreateLotDto, Lot>()
            .ConstructUsing(source => new Lot(
                Guid.NewGuid(),
                source.ItemId,
                source.LotNumber,
                source.SupplierLotNumber,
                new DateTimeOffset(source.ReceivedAt),
                source.SupplierName,
                source.InitialQuantity,
                source.RemainingQuantity,
                source.ExpiryDate,
                source.StorageLocation,
                LotStatus.Active));

            CreateMap<UpdateLotDto, Lot>();
        }


    }
}