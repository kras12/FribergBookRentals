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
using FribergbookRentals.Data.Constants;

namespace FribergBookRentalsTest.Tests.Controllers
{
    public class TestControllerBase : TestBase
    {
        #region Methods

        protected Mock<ControllerContext> CreateControllerContextMock(ClaimsPrincipal user)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = user;
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor(), new ModelStateDictionary());

            return new Mock<ControllerContext>(actionContext);
        }        

        #endregion
    }
}
