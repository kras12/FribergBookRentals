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


namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestHomeController : TestBase
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
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var signingManagerMock = new Mock<SignInManager<User>>(_userManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);
            var userMock = new Mock<ClaimsPrincipal>();
            var claimsPrincipalMock = new Mock<UserClaimsPrincipalFactory<User>>();
            var tempDataHelperMock = new Mock<ITempDataHelper>();

            //userMock.Expect(p => p.IsInRole("admin")).Returns(true);            
            contextAccessorMock.SetupGet(x => x.HttpContext!.User)
                       .Returns(userMock.Object);
            //contextAccessorMock.SetupGet(x => x.HttpContext!.User.Identity!.IsAuthenticated)
            //                     .Returns(isUserAuthenticated);

            signingManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(isUserAuthenticated);


            // public ActionContext(
            //        HttpContext httpContext,
            //        RouteData routeData,
            //    ActionDescriptor actionDescriptor)
            //    : this(httpContext, routeData, actionDescriptor, new ModelStateDictionary())

            var httpContext = new DefaultHttpContext();
            httpContext.User = userMock.Object;
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), 
                new ControllerActionDescriptor(), new ModelStateDictionary());
            var controllerContextMock = new Mock<ControllerContext>(actionContext);
            //controllerContextMock.SetupGet(x => x.HttpContext.User.Identity!.IsAuthenticated)
            //                     .Returns(isUserAuthenticated);
            //controllerContextMock.SetupGet(x => x.HttpContext)
            //                     .Returns(new DefaultHttpContext());

            var user = await GetDefaultUser();

            var homeControllerMock = new Mock<HomeController>(_bookRepository, _autoMapper, signingManagerMock.Object, _bookLoanRepository, tempDataHelperMock.Object);
            //homeControllerMock.SetupGet(x => x.ControllerContext).Returns(controllerContextMock.Object);
            homeControllerMock.Setup(x => x.GetUserId()).Returns(user.Id);

            //homeController.ControllerContext = controllerContextMock.Object;

            var homeController = homeControllerMock.Object;
            homeController.ControllerContext = controllerContextMock.Object;
            
            return homeController;
        }

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

            //userMock.Verify(p => p.IsInRole("admin"));
            //Assert.Equal("Index", ((ViewResult)result).ViewName);
            //Assert.Equal(StatusCodes.Status200OK, ((ViewResult)result).StatusCode!.Value);
        }

        #endregion
    }
}
