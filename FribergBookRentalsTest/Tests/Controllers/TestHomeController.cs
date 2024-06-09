using AutoMapper;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using AutoMapper;
using FribergBookRentals.Mapper;
using FribergbookRentals.Models;
using Microsoft.AspNetCore.Identity;
using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FribergBookRentals.Models;


namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestHomeController : TestBase
    {
        #region Fields

        IBookRepository _bookRepository;

        IBookLoanRepository _bookLoanRepository;

        IMapper _autoMapper;

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

        [Fact]
        public async Task TestSearchBooks()
        {
            //      SignInManager(UserManager<TUser> userManager,
            //IHttpContextAccessor contextAccessor,
            //IUserClaimsPrincipalFactory<TUser> claimsFactory,
            //IOptions<IdentityOptions> optionsAccessor,
            //ILogger<SignInManager<TUser>> logger,
            //IAuthenticationSchemeProvider schemes,
            //IUserConfirmation<TUser> confirmation)

            // Arrange
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var signingManagerMock = new Mock<SignInManager<User>>(_userManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);
            var userMock = new Mock<ClaimsPrincipal>();
            var claimsPrincipalMock = new Mock<UserClaimsPrincipalFactory<User>>();

            //userMock.Expect(p => p.IsInRole("admin")).Returns(true);            
            contextAccessorMock.SetupGet(ctx => ctx.HttpContext!.User)
                       .Returns(userMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            //controllerContextMock.SetupGet(x => x.HttpContext)
            //                     .Returns(contextMock.Object.HttpContext!);

            

            BookSearchInputViewModel searchInput = new BookSearchInputViewModel();


            var homeController = new HomeController(_bookRepository, _autoMapper, signingManagerMock.Object, _bookLoanRepository);
            homeController.ControllerContext = controllerContextMock.Object;

            // Act
            var result = await homeController.Index(searchInput);
            var viewResult = (ViewResult)result;
            var searchResult = (BookSearchViewModel)viewResult.Model!;

            // Assert

            Assert.True(controllerContextMock.Object.ModelState.IsValid);
            Assert.True(searchResult.HaveSearchedBooks);
            Assert.True(searchResult.Books!.Count > 0);

            //userMock.Verify(p => p.IsInRole("admin"));
            //Assert.Equal("Index", ((ViewResult)result).ViewName);
            //Assert.Equal(StatusCodes.Status200OK, ((ViewResult)result).StatusCode!.Value);
        }

        #endregion
    }
}
