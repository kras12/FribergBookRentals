using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using WebApplication1.Areas.Identity.Pages.Account;

namespace FribergBookRentalsTest.Tests.Pages
{
    public class TestRegisterPage : TestBase
    {
        #region Methods

        [Fact]
        public async Task TestUserRegistration()
        {
            //Arrange

            var userStore = new UserStore<User>(_dbContext);
            var signingManagerMock = new Mock<SignInManager<User>>(_userManager.Object, new HttpContextAccessor(), new Mock<IUserClaimsPrincipalFactory<User>>().Object, null, null, null, null);
            var emailSender = new Mock<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender>();

            signingManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync()).Returns(async () => { return new List<AuthenticationScheme>(); });
            signingManagerMock.Setup(x => x.SignInAsync(It.IsAny<User>(), It.IsAny<AuthenticationProperties>(), null)).Returns(Task.CompletedTask);
            emailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var newUserInput = new RegisterModel.InputModel();
            newUserInput.FirstName = "Kalle";
            newUserInput.LastName = "Anka";
            newUserInput.Email = "kalle@ankeborg.com";
            newUserInput.Password = "Aa!12345678";
            newUserInput.ConfirmPassword = "Aa!12345678";

            var registerPage = new Mock<RegisterModel>(_userManager.Object, userStore, signingManagerMock.Object, emailSender.Object);
            registerPage.Setup(x => x.GetCallBackUrl(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
            registerPage.Setup(x => x.OnPostAsync(It.IsAny<string>())).CallBase();
            registerPage.SetupGet(x => x.Input).CallBase();
            registerPage.CallBase = true;
            registerPage.Object.Input = newUserInput;

            //Act

            await registerPage.Object.OnPostAsync("/");
            var fetchedUser = await _userManager.Object.FindByEmailAsync("kalle@ankeborg.com");
            var passwordTest = await _userManager.Object.CheckPasswordAsync(fetchedUser!, "Aa!12345678");
            var userRoles = await _userManager.Object.GetRolesAsync(fetchedUser!);          
            
            //Assert

            Assert.True(registerPage.Object.ModelState.IsValid, "Model state is not valid.");
            Assert.NotNull(fetchedUser);
            Assert.True(fetchedUser.FirstName == "Kalle", "First name is invalid.");
            Assert.True(fetchedUser.LastName == "Anka", "Last name is invalid.");
            Assert.True(fetchedUser.Email == "kalle@ankeborg.com", "Email is invalid.");
            Assert.True(passwordTest, "Password validation failed.");
            Assert.True(userRoles.Contains(ApplicationUserRoles.Member), "Invalid user roles.");
        }

        #endregion
    }
}
