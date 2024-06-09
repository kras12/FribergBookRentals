using AutoMapper;
using FribergbookRentals.Data.Dto;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Models;

namespace FribergBookRentals.Mapper
{
    public class DtoToEntityAutoMapperProfile : Profile
    {
        public DtoToEntityAutoMapperProfile()
        {
            CreateMap<SeedBookDto, Book>()
                .ForMember(dest => dest.NumberOfPages, opt => opt.MapFrom(src => src.Pages))
                .ForMember(dest => dest.InformationUrl, opt => opt.MapFrom(src => src.Link))
                .ForMember(dest => dest.ImageName, opt => opt.MapFrom(src => src.ImageLink.Replace("images/", "")));
        }
    }
}
