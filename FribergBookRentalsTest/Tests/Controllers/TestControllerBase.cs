using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestControllerBase : TestBase
    {
        #region Methods

        protected Mock<ControllerContext> CreateControllerContextMock(Mock<ClaimsPrincipal> userMock)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = userMock.Object;
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor(), new ModelStateDictionary());

            return new Mock<ControllerContext>(actionContext);
        }

        protected Mock<SignInManager<User>> CreateSigningManagerMock(Mock<ClaimsPrincipal> userMock, bool isUserAuthenticated)
        {
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var signingManagerMock = new Mock<SignInManager<User>>(_userManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

            signingManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(isUserAuthenticated);
            contextAccessorMock.SetupGet(x => x.HttpContext!.User)
                       .Returns(userMock.Object);

            return signingManagerMock;
        }

        protected Mock<ClaimsPrincipal> CreateUserMock(bool isUserAuthenticated, string role)
        {
            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(x => x.IsInRole(role)).Returns(isUserAuthenticated);
            return userMock;
        }

        #endregion
    }
}
