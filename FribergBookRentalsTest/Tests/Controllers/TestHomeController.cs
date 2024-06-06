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


namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestHomeController : TestBase
    {
        #region Fields

        IBookRepository _bookRepository;

        IMapper _autoMapper;

        #endregion

        #region Constructors

        public TestHomeController()
        {
            _bookRepository = new BookRepository(_dbContext);

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
            var homeController = new HomeController(_bookRepository, _autoMapper);

            var userMock = new Mock<ClaimsPrincipal>();
            //userMock.Expect(p => p.IsInRole("admin")).Returns(true);

            var contextMock = new Mock<IHttpContextAccessor>();
            contextMock.SetupGet(ctx => ctx.HttpContext!.User)
                       .Returns(userMock.Object);

            var controllerContextMock = new Mock<ControllerContext>();
            //controllerContextMock.SetupGet(x => x.HttpContext)
            //                     .Returns(contextMock.Object.HttpContext!);

            homeController.ControllerContext = controllerContextMock.Object;


            BookSearchInputViewModel searchInput = new BookSearchInputViewModel();

            var result = await homeController.Search(searchInput);
            var viewResult = (ViewResult)result;
            var foundBooks = (List<BookViewModel>)viewResult.Model!;

            Assert.True(controllerContextMock.Object.ModelState.IsValid);
            Assert.True(foundBooks.Count > 0);

            //userMock.Verify(p => p.IsInRole("admin"));
            //Assert.Equal("Index", ((ViewResult)result).ViewName);
            //Assert.Equal(StatusCodes.Status200OK, ((ViewResult)result).StatusCode!.Value);
        }

        #endregion
    }
}
