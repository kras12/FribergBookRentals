using AutoMapper;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Models;

namespace FribergBookRentals.Mapper
{
    public class ViewModelToEntityMapperProfile : Profile
    {
        public ViewModelToEntityMapperProfile()
        {
            CreateMap<BookViewModel, Book>();
            CreateMap<BookSearchInputViewModel, BookSearchInputDto>();
        }
    }
}
