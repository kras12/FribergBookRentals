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

namespace FribergBookRentalsTest
{
	public abstract class TestBase
	{
		#region Fields

		protected ApplicationDbContext _dbContext;

		private User _defaultSeedUserData = new User("Kajsa", "Anka", "kajsa@ankeborg.com");

		private DbContextOptions<ApplicationDbContext> _options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase($"TestingMemoryDb-{Guid.NewGuid()}")
			.Options;

		#endregion

		#region Constructors

		protected TestBase()
		{
			_dbContext = new ApplicationDbContext(_options);
			_dbContext.Database.EnsureCreated();
			SeedBooks();
			SeedDefaultUser();
		}


		#endregion

		#region Methods

		protected UserManager<User> CreateUserManagager()
		{
			IUserStore<User> store = new UserStore<User>(_dbContext);
			UserManager<User> userManager = new UserManager<User>(store, null, new PasswordHasher<User>(), null, null, null, null, null, null);
			return userManager;
		}

		public void Dispose()
		{
			_dbContext.Database.EnsureDeleted();
		}

		protected async Task<User> GetDefaultUser()
		{
			UserManager<User> userManager = CreateUserManagager();
			return await userManager.FindByEmailAsync(_defaultSeedUserData.Email!) ?? throw new Exception("Default user not found");
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
			UserManager<User> userManager = CreateUserManagager();
			userManager.CreateAsync(_defaultSeedUserData, "Ab1slkdjflksdfjlksd").Wait();
		}

		#endregion
	}
}
