using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Text.Json;
using FribergbookRentals.Data.Repositories;
using Moq;
using FribergbookRentals.Data.Migrations;
using Microsoft.Extensions.Logging;
using FribergbookRentals.Data.Constants;

namespace FribergBookRentalsTest
{
	public abstract class TestBase
	{
        #region Fields

        protected ApplicationDbContext _dbContext;

		protected User _defaultSeedUserData = new User("Kajsa", "Anka", "kajsa@ankeborg.com");

		protected const string DefaultUserPassword = "Aa!12345678";

        private DbContextOptions<ApplicationDbContext> _options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase($"TestingMemoryDb-{Guid.NewGuid()}")
			.Options;

		protected readonly Mock<UserManager<User>> _userManager;

		protected readonly UserStore<User> _userStore;

        #endregion

        #region Constructors

        protected TestBase()
		{
			_dbContext = new ApplicationDbContext(_options);
            _dbContext.Database.EnsureCreated();
			
			_userStore = CreateUserStore();
			_userManager = CreateUserManager();

            SeedBooks();
            SeedDefaultUser();
        }

        #endregion

        #region Methods

        private Mock<UserManager<User>> CreateUserManager()
		{
			if (_userStore == null)
			{
				throw new NullReferenceException(nameof(_userStore));
			}

			var mockedLogger = new Mock<ILogger<UserManager<User>>>();

            Mock<UserManager<User>> userManager = new Mock<UserManager<User>>(_userStore, null, 
				new PasswordHasher<User>(), null, new List<PasswordValidator<User>>() { new PasswordValidator<User>() }, 
                new UpperInvariantLookupNormalizer(), null, null, mockedLogger.Object);

			userManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).Returns(Task.FromResult("x"));
			userManager.Setup(x => x.CreateAsync(It.IsAny<User>())).CallBase();
			userManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).CallBase();
            userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).CallBase();
			userManager.Setup(x => x.GetRolesAsync(It.IsAny<User>())).CallBase();
            userManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>())).CallBase();
            userManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).CallBase();
            userManager.CallBase = true;

            return userManager;
		}

		private UserStore<User> CreateUserStore()
		{
			return new UserStore<User>(_dbContext);
        }

		public void Dispose()
		{
			_dbContext.Database.EnsureDeleted();
		}

		protected async Task<User> GetDefaultUser()
		{
			
			return await _userManager.Object.FindByEmailAsync(_defaultSeedUserData.Email!) ?? throw new Exception("Default user not found");
		}

		private void SeedBooks()
		{
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "BookList.txt"));
			var mockBooks = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			
			IBookRepository bookRepository = new BookRepository(_dbContext);
			bookRepository.AddBooksAsync(mockBooks!).Wait();
		}

		private void SeedDefaultUser()
		{
            _userManager.Object.CreateAsync(_defaultSeedUserData, DefaultUserPassword).Wait();
			var user = _userManager.Object.FindByEmailAsync(_defaultSeedUserData.Email!).Result;
			_userManager.Object.AddToRoleAsync(user!, ApplicationUserRoles.Member);
        }

		#endregion
	}
}
