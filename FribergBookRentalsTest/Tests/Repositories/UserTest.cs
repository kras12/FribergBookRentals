using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribergBookRentalsTest.Tests.Repositories
{
    public class UserTest : TestBase
    {
        #region Methods

        [Fact]
        public async Task TestUserRegistration()
        {
            //Arrange

            User newUser = new User("Kajsa", "Anka", "kajsa@ankeborg.com");

            //Act

            var createResult = await _userManager.Object.CreateAsync(newUser, DefaultUserPassword);
            var fetchedUser = await _userManager.Object.FindByEmailAsync(newUser.Email!);
            var passwordTest = await _userManager.Object.CheckPasswordAsync(fetchedUser!, DefaultUserPassword);

            //Assert

            Assert.True(createResult.Succeeded);
            Assert.True(fetchedUser != null);
            Assert.True(fetchedUser.FirstName == newUser.FirstName);
            Assert.True(fetchedUser.LastName == newUser.LastName);
            Assert.True(fetchedUser.Email == newUser.Email);
            Assert.True(passwordTest);
        }

        [Fact]
        public async Task TestUserLogin()
        {
            //Arrange

            //Act
            var fetchedUser = await _userManager.Object.FindByEmailAsync(_defaultSeedUserData.Email!);
            var passwordTest = await _userManager.Object.CheckPasswordAsync(fetchedUser!, DefaultUserPassword);

            //Assert

            Assert.True(fetchedUser != null, "User not found in database.");
            Assert.True(passwordTest, "Password check failed.");
        }

        #endregion
    }
}
