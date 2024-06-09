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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
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

        private async Task<MemberController> CreateMemberController(bool isUserLoggedIn)
        {
            // User
            var user = await GetDefaultUser();
            ClaimsPrincipal claimsPrincipal = CreateClaimsPrincipal(user, isUserLoggedIn);

            // Controller context
            var controllerContextMock = CreateControllerContextMock(claimsPrincipal);

            // Member controller
            var tempDataHelperMock = new Mock<ITempDataHelper>();
            var controller = new MemberController(_bookLoanRepository, _autoMapper, _bookRepository, tempDataHelperMock.Object);
            controller.ControllerContext = controllerContextMock.Object;    

            return controller;
        }

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestListBookLoans(bool isUserAuthenticated)
        {
            // Arrange
            var user = await GetDefaultUser();
            var memberController = await CreateMemberController(isUserAuthenticated);
            var books = await _bookRepository.GetBooksAsync();
            int activeLoans = 10;
            int closedLoans = 8;

            await BookLoanHelper.CreateBookLoans(_bookLoanRepository, user, books.Take(activeLoans).ToList(), createActiveLoans: true, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));
            await BookLoanHelper.CreateBookLoans(_bookLoanRepository, user, books.Skip(activeLoans).Take(closedLoans).ToList(), createActiveLoans: false, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));

            // Act
            var result = await memberController.Index();

            // Assert
            if (isUserAuthenticated)
            {
                var viewResult = result as ViewResult;
                Assert.NotNull(viewResult);

                var model = viewResult!.Model as MemberBookLoansViewModel;
                Assert.NotNull(model);
                Assert.True(model.ActiveLoans.Count == activeLoans, "Failed active loans count");
                Assert.True(model.ClosedLoans.Count == closedLoans, "Failed closed loans count");
                Assert.All(model.ActiveLoans, x => Assert.Null(x.ClosedTime));
                Assert.All(model.ClosedLoans, x => Assert.NotNull(x.ClosedTime));
            }
            else
            {
                Assert.IsType<UnauthorizedResult>(result);
            }            
        }

        #endregion
    }
}
