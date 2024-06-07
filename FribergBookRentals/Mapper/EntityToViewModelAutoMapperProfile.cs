using AutoMapper;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Models;

namespace FribergBookRentals.Mapper
{
    public class EntityToViewModelAutoMapperProfile : Profile
    {
        public EntityToViewModelAutoMapperProfile()
        {
            CreateMap<Book, BookViewModel>();
            CreateMap<BookLoan, BookLoanViewModel>();
        }
    }
}
