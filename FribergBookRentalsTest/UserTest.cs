using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribergBookRentalsTest
{
	public class UserTest : TestBase
	{
		#region Methods

		[Fact]
		public async Task TestUserRegistration()
		{
			//Arrange

			User newUser = new User("Kalle", "Anka", "kalle@ankeborg.com");
			UserManager<User> userManager = CreateUserManagager();

			//Act

			var createResult = await userManager.CreateAsync(newUser, "Ab1slkdjflksdfjlksd");
			var fetchedUser = await userManager.FindByEmailAsync("kalle@ankeborg.com");
			var passwordTest = await userManager.CheckPasswordAsync(fetchedUser!, "Ab1slkdjflksdfjlksd");

			//Assert

			Assert.True(createResult.Succeeded);
			Assert.True(fetchedUser != null);
			Assert.True(fetchedUser.FirstName == "Kalle");
			Assert.True(fetchedUser.LastName == "Anka");
			Assert.True(fetchedUser.Email == "kalle@ankeborg.com");
			Assert.True(passwordTest);
		}

		[Fact]
		public async Task TestUserLogin()
		{
            //Arrange

            User newUser = new User("Kalle", "Anka", "kalle@ankeborg.com");
			UserManager<User> userManager = CreateUserManagager();

			//Act
			var createResult = await userManager.CreateAsync(newUser, "Ab1slkdjflksdfjlksd");
            var fetchedUser = await userManager.FindByEmailAsync("kalle@ankeborg.com");
            var passwordTest = await userManager.CheckPasswordAsync(fetchedUser!, "Ab1slkdjflksdfjlksd");

            //Assert

            Assert.True(createResult.Succeeded);
            Assert.True(fetchedUser != null);
            Assert.True(fetchedUser.Email == "kalle@ankeborg.com");
            Assert.True(passwordTest);
        }

		[Fact]
		public async Task TestUserLogout()
		{
            User newUser = new User("Kalle", "Anka", "kalle@ankeborg.com");
            UserManager<User> userManager = CreateUserManagager();

            var createResult = await userManager.CreateAsync(newUser, "Ab1slkdjflksdfjlksd");
            var fetchedUser = await userManager.FindByEmailAsync("kalle@ankeborg.com");

            // Inne i en controller:
            // User.Identity.IsAuthenticated
        }


		#endregion
	}
}
