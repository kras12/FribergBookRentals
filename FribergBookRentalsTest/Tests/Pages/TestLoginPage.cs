using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using WebApplication1.Areas.Identity.Pages.Account;

namespace FribergBookRentalsTest.Tests.Pages
{
    public class TestLoginPage : TestPageBase
    {
        #region Methods

        [InlineData(true)]
        [InlineData(false)]
        [Theory]
        public async Task TestSuccessfulLoginRedirect(bool isUserLoggedIn)
        {
            //Arrange
            var user = await GetDefaultUser();
            var claimsPrincipal = CreateClaimsPrincipal(user, isUserLoggedIn);
            var signinManager = CreateSigningManagerMock(claimsPrincipal, isUserLoggedIn);

            var loginInput = new LoginModel.InputModel();
            loginInput.Email = _defaultSeedUserData.Email;
            loginInput.Password = DefaultUserPassword;
            loginInput.RememberMe = false;

            var newLoginPage = new LoginModel(signinManager.Object);
            newLoginPage.Input = loginInput;
            var pageContextMock = CreatePageContextMock(claimsPrincipal);
            newLoginPage.PageContext = pageContextMock.Object;

            // Act

            var result = await newLoginPage.OnPostAsync("ReturnUrlPlaceHolder");

            // Assert
            if (isUserLoggedIn)
            {
                Assert.IsType<RedirectToActionResult>(result);                
            }
            else
            {
                Assert.IsType<LocalRedirectResult>(result);
                Assert.True(newLoginPage.ModelState.IsValid);
                Assert.True(((LocalRedirectResult)result).Url == "ReturnUrlPlaceHolder");
            }           
        }

        #endregion
    }
}
