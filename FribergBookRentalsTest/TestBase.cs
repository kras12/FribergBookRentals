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
using FribergBookRentalsTest.Data;
using FribergbookRentals.Data.Repositories;

namespace FribergBookRentalsTest
{
	public abstract class TestBase
	{
		#region Fields

		protected ApplicationDbContext _dbContext;


		private DbContextOptions<ApplicationDbContext> _options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase("TestingMemoryDb")
			.Options;


		#endregion

		#region Constructors

		protected TestBase()
		{
			_dbContext = new ApplicationDbContext(_options);
			_dbContext.Database.EnsureCreated();
			SeedBooks();
			SeedUsers();
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

		private void SeedBooks()
		{
			var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "BookList.txt"));
			var mockBooks = JsonSerializer.Deserialize<List<Book>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
			
			IBookRepository bookRepository = new BookRepository(_dbContext);
			bookRepository.AddBooks(mockBooks!).Wait();
		}

		private void SeedUsers()
		{
			User newUser = new User("Kajsa", "Anka", "kajsa@ankeborg.com");
			UserManager<User> userManager = CreateUserManagager();
			userManager.CreateAsync(newUser, "Ab1slkdjflksdfjlksd").Wait();
		}

		#endregion
	}
}
