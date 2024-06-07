using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Controllers;
using FribergBookRentals.Controllers.Member;
using FribergBookRentals.Mapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestMemberController : TestBase
    {
        #region Fields

        IBookLoanRepository _bookLoanRepository;

        IMapper _autoMapper;

        #endregion

        #region Constructors

        public TestMemberController()
        {
            _bookLoanRepository = new BookLoanRepository(_dbContext);

            MapperConfiguration config = new MapperConfiguration(config =>
            {
                config.AddProfile(new EntityToViewModelAutoMapperProfile());
                config.AddProfile(new ViewModelToEntityMapperProfile());
            });

            _autoMapper = new Mapper(config);
        }

        #endregion

        #region Methods

        public async Task TestListBookLoans()
        {
            var memberController = new MemberController(_bookLoanRepository, _autoMapper);
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Expect(p => p.IsInRole(ApplicationUserRoles.Member)).Returns(true);
        }

        #endregion
    }
}
