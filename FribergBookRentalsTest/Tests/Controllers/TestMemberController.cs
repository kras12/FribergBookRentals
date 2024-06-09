using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Controllers;
using FribergBookRentals.Controllers.Member;
using FribergBookRentals.Mapper;
using FribergBookRentals.Models;
using FribergBookRentals.Services;
using FribergBookRentalsTest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestMemberController : TestControllerBase
    {
        #region Constants

        private const int BookLoanTime = 14;

        #endregion

        #region Fields

        IBookRepository _bookRepository;

        IBookLoanRepository _bookLoanRepository;        

        #endregion

        #region Constructors

        public TestMemberController()
        {
            _bookRepository = new BookRepository(_dbContext);
            _bookLoanRepository = new BookLoanRepository(_dbContext);
        }

        #endregion

        #region Methods

        [Fact]
        public async Task TestListBookLoans()
        {
            // Arrange
            var user = await _userManager.Object.FindByEmailAsync(_defaultSeedUserData.Email!);
            var tempDataHelperMock = new Mock<ITempDataHelper>();

            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ApplicationUserClaims.UserId, user.Id),
                    new Claim(ApplicationUserClaims.UserRole, ApplicationUserRoles.Member)
                })
            });

            var httpContextMock = new HttpContextMoq.HttpContextMock();
            httpContextMock.User = claimsPrincipal;

            var memberController = new MemberController(_bookLoanRepository, _autoMapper, _bookRepository, tempDataHelperMock.Object);
            memberController.ControllerContext.HttpContext = httpContextMock;
            memberController.TempData = new TempDataDictionary(httpContextMock, Mock.Of<ITempDataProvider>());

            var books = await _bookRepository.GetBooksAsync();

            int activeLoans = 10;
            int closedLoans = 8;

            await BookLoanHelper.CreateBookLoans(_bookLoanRepository, user, books.Take(activeLoans).ToList(), createActiveLoans: true, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));
            await BookLoanHelper.CreateBookLoans(_bookLoanRepository, user, books.Skip(activeLoans).Take(closedLoans).ToList(), createActiveLoans: false, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));

            // Act
            var result = (await memberController.Index()) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = result!.Model as MemberBookLoansViewModel;

            Assert.NotNull(model);
            Assert.True(model.ActiveLoans.Count == activeLoans, "Failed active loans count");
            Assert.True(model.ClosedLoans.Count == closedLoans, "Failed closed loans count");
            Assert.All(model.ActiveLoans, x => Assert.Null(x.ClosedTime));
            Assert.All(model.ClosedLoans, x => Assert.NotNull(x.ClosedTime));
        }

        #endregion
    }
}
