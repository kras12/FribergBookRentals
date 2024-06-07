using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApplication1.Areas.Identity.Pages.Account;

namespace FribergBookRentalsTest.Tests.Pages
{
    public class TestLoginPage : TestBase
    {
        #region Methods

        [Fact]
        public async Task TestSuccessfulLoginRedirect()
        {
            //Arrange

            var signingManagerMock = new Mock<SignInManager<User>>(_userManager.Object, new HttpContextAccessor(), new Mock<IUserClaimsPrincipalFactory<User>>().Object, 
                null, null, null, null);
            signingManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync())
                .Returns(Task.FromResult(new List<AuthenticationScheme>().AsEnumerable()));
            signingManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));
            signingManagerMock.CallBase = true;

            var loginInput = new LoginModel.InputModel();
            loginInput.Email = _defaultSeedUserData.Email;
            loginInput.Password = DefaultUserPassword;
            loginInput.RememberMe = false;

            var newLoginPage = new LoginModel(signingManagerMock.Object);
            newLoginPage.Input = loginInput;


            // Act

            var result = await newLoginPage.OnPostAsync("ReturnUrlPlaceHolder");

            // Assert
            Assert.IsType<LocalRedirectResult>(result);
            Assert.True(((LocalRedirectResult)result).Url == "ReturnUrlPlaceHolder");
        }

        #endregion
    }
}
