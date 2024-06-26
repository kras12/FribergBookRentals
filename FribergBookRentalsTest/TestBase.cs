﻿using FribergbookRentals.Data.Models;
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
using AutoMapper;
using FribergBookRentals.Mapper;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace FribergBookRentalsTest
{
	public abstract class TestBase
	{
        #region Fields

        protected ApplicationDbContext _dbContext;

		protected User _defaultSeedUserData = new User("Kalle", "Anka", "kalle@ankeborg.com");

		protected const string DefaultUserPassword = "Aa!12345678";

        private DbContextOptions<ApplicationDbContext> _options = new DbContextOptionsBuilder<ApplicationDbContext>()
			.UseInMemoryDatabase($"TestingMemoryDb-{Guid.NewGuid()}")
			.Options;

		protected readonly Mock<UserManager<User>> _userManager;

		protected readonly UserStore<User> _userStore;

        protected IMapper _autoMapper;

        #endregion

        #region Constructors

        protected TestBase()
		{
			_dbContext = new ApplicationDbContext(_options);
            _dbContext.Database.EnsureCreated();
			
			_userStore = CreateUserStore();
			_userManager = CreateUserManager();

            MapperConfiguration config = new MapperConfiguration(config =>
            {
                config.AddProfile(new EntityToViewModelAutoMapperProfile());
                config.AddProfile(new ViewModelToEntityMapperProfile());
            });

            _autoMapper = new Mapper(config);

            SeedBooks();
            SeedDefaultUser().Wait();
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
            userManager.Setup(x => x.GetClaimsAsync(It.IsAny<User>())).CallBase();
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

		private async Task SeedDefaultUser()
		{
            _userManager.Object.CreateAsync(_defaultSeedUserData, DefaultUserPassword).Wait();
			var user = await GetDefaultUser();
			await _userManager.Object.AddToRoleAsync(user, ApplicationUserRoles.Member);
        }

        protected Mock<SignInManager<User>> CreateSigningManagerMock(ClaimsPrincipal user, bool isUserAuthenticated)
        {
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var signingManagerMock = new Mock<SignInManager<User>>(_userManager.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

            signingManagerMock.Setup(x => x.IsSignedIn(It.IsAny<ClaimsPrincipal>())).Returns(isUserAuthenticated);
            contextAccessorMock.SetupGet(x => x.HttpContext!.User)
                       .Returns(user);

            signingManagerMock.Setup(x => x.GetExternalAuthenticationSchemesAsync())
                .Returns(Task.FromResult(new List<AuthenticationScheme>().AsEnumerable()));

            signingManagerMock.Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

            return signingManagerMock;   
        }

        protected ClaimsPrincipal CreateClaimsPrincipal(User user, bool isUserLoggedIn)
        {
            var mock = new Mock<ClaimsPrincipal>();
            mock.SetupGet(x => x.Identity!.IsAuthenticated).Returns(isUserLoggedIn);
            mock.Setup(x => x.AddIdentity(It.IsAny<ClaimsIdentity>())).CallBase();
            mock.CallBase = true;
            ClaimsPrincipal result = mock.Object;

            if (isUserLoggedIn)
            {
                result.AddIdentity(new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ApplicationUserClaims.UserId, user.Id),
                    new Claim(ApplicationUserClaims.UserRole, ApplicationUserRoles.Member)
                }));
            }

            return result;
        }
        #endregion
    }
}
