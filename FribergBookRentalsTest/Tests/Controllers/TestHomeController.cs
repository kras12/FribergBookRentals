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

        public static IEnumerable<object[]> _bookSearchInputData =>
        new List<object[]>
        {
            new object[] { new BookSearchInputViewModel() },

            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart") },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Chinua Achebe") },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart Chinua Achebe") },


            new object[] { new BookSearchInputViewModel(language: "English") },
            new object[] { new BookSearchInputViewModel(year: 1958) },
            new object[] { new BookSearchInputViewModel(language: "English", year: 1958) },

            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart", language: "English") },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart", year: 1958) },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart", language: "English", year: 1958) },

            new object[] { new BookSearchInputViewModel(searchPhrase: "Chinua Achebe", language: "English") },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Chinua Achebe", year: 1958) },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Chinua Achebe", language: "English", year: 1958) },

            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart Chinua Achebe", language: "English") },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart Chinua Achebe", year: 1958) },
            new object[] { new BookSearchInputViewModel(searchPhrase: "Things Fall Apart Chinua Achebe", language: "English", year: 1958) },
        };

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
            var user = await GetDefaultUser();
            var userClaims = CreateUserClaims(user);

            // Signing manager
            var signinManagerMock = CreateSigningManagerMock(userClaims, isUserAuthenticated);
            
            // Controller context
            var controllerContextMock = CreateControllerContextMock(userClaims);

            // Home controller
            var tempDataHelperMock = new Mock<ITempDataHelper>();
            var homeController = new HomeController(_bookRepository, _autoMapper, signinManagerMock.Object, _bookLoanRepository, tempDataHelperMock.Object);
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

        [Theory]
        [MemberData(nameof(_bookSearchInputData))]
        public async Task TestSearchBooks(BookSearchInputViewModel searchInput)
        {
            // Arrange
            var homeController = await CreateHomeController(false);

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
