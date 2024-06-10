using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FribergBookRentalsTest.Tests.Pages
{
    public class TestPageBase : TestBase
    {
        #region Methods

        protected Mock<PageContext> CreatePageContextMock(ClaimsPrincipal user)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = user;
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(),
                new PageActionDescriptor(), new ModelStateDictionary());

            return new Mock<PageContext>(actionContext);
        }

        #endregion
    }
}
