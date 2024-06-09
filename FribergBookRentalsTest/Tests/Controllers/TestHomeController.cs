using AutoMapper;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using FribergBookRentals.Mapper;
using FribergbookRentals.Models;
using Microsoft.AspNetCore.Identity;
using FribergbookRentals.Data.Models;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Controllers;
using FribergBookRentals.Services;
using FribergbookRentals.Data.Constants;


namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestHomeController : TestControllerBase
    {
        #region Fields

        IBookRepository _bookRepository;

        IBookLoanRepository _bookLoanRepository;

        #endregion

        #region Constructors

        public TestHomeController()
        {
            _bookRepository = new BookRepository(_dbContext);
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

        private async Task<HomeController> CreateHomeController(bool isUserAuthenticated)
        {
            // User
            var userMock = CreateUserMock(isUserAuthenticated, ApplicationUserRoles.Member);

            // Signing manager
            var signinManagerMock = CreateSigningManagerMock(userMock, isUserAuthenticated);
            
            // Controller context
            var controllerContextMock = CreateControllerContextMock(userMock);

            // Home controller mock
            var user = await GetDefaultUser();
            var tempDataHelperMock = new Mock<ITempDataHelper>();
            var homeControllerMock = new Mock<HomeController>(_bookRepository, _autoMapper, signinManagerMock.Object, _bookLoanRepository, tempDataHelperMock.Object);
            homeControllerMock.Setup(x => x.GetUserId()).Returns(user.Id);

            // Home controller
            var homeController = homeControllerMock.Object;
            homeController.ControllerContext = controllerContextMock.Object;
            
            return homeController;
        }

        #endregion

        #region Tests

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestBorrowBook(bool isUserLoggedIn)
        {
            // Arrange
            var book = _bookRepository.GetBookByIdAsync(1);
            var homeController = await CreateHomeController(isUserLoggedIn);
            var user = await GetDefaultUser();

            // Act
            var result = await homeController.BorrowBook(book.Id);
            var bookLoans = await _bookLoanRepository.GetActiveBookLoansAsync(user.Id);

            // Assert
            if (isUserLoggedIn)
            {
                Assert.Contains(bookLoans, x => x.Book.BookId == book.Id && x.User.Id == user.Id);
            }
            else
            {
                Assert.DoesNotContain(bookLoans, x => x.Book.BookId == book.Id && x.User.Id == user.Id);
            }            
        }

        [Fact]
        public async Task TestSearchBooks()
        {
            // Arrange
            var homeController = await CreateHomeController(false);
            BookSearchInputViewModel searchInput = new BookSearchInputViewModel();

            // Act
            var result = await homeController.Index(searchInput);
            var viewResult = (ViewResult)result;
            var searchResult = (BookSearchViewModel)viewResult.Model!;

            // Assert

            Assert.True(homeController.ModelState.IsValid);
            Assert.True(searchResult.HaveSearchedBooks);
            Assert.True(searchResult.Books!.Count > 0);
        }

        #endregion
    }
}
