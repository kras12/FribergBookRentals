using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Areas.Identity.Pages.Account;

namespace FribergBookRentalsTest.Tests.Pages
{
    public class TestLogoutPage : TestPageBase
    {
        [InlineData("ReturnUrlPlaceHolder")]
        [InlineData(null)]
        [Theory]
        public async Task TestLogout(string? returnUrl)
        {
            // Arrange
            var user = await GetDefaultUser();
            var claimsPrincipal = CreateClaimsPrincipal(user, false);
            var signinManager = CreateSigningManagerMock(claimsPrincipal, false);
            var pageContextMock = CreatePageContextMock(claimsPrincipal);
            var logoutPage = new LogoutModel(signinManager.Object);
            logoutPage.PageContext = pageContextMock.Object;

            // Act
            var result = await logoutPage.OnPost(returnUrl);

            // Assert
            if (returnUrl == null)
            {
                Assert.IsType<RedirectToPageResult>(result);
            }
            else
            {
                Assert.IsType<LocalRedirectResult>(result);
                Assert.True(((LocalRedirectResult)result).Url == returnUrl);
            }

        }
    }
}
