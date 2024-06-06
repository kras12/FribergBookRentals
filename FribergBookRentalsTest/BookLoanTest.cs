using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergBookRentalsTest
{
	public class BookLoanTest : TestBase
	{
        #region Fields

        IBookRepository bookRepository = new BookRepository(_dbContext);
        IBookLoanRepository bookLoanRepository = new BookLoanRepository(_dbContext);

        #endregion
        #region Methods

        [Fact]
		public async Task TestAddBookLoan()
		{
			BookLoan bookLoan = await CreateLoan();

            var result = await bookLoanRepository.AddAsync(bookLoan);
			var bookLoanUserResult = await bookLoanRepository.GetBookLoanByUserId(result.User.Id);

			Assert.NotNull(result);
			Assert.NotNull(bookLoanUserResult);
			Assert.Equal(bookLoan.User.Id, bookLoanUserResult.User.Id);
			Assert.Equal(bookLoan.StartTime, bookLoanUserResult.StartTime);
        }

		public async Task TestListActiveBookLoans()
		{
			await CreateLoan(10, onlyActiveLoans: false);
			List<BookLoan> bookLoans = await GetBookLoans();

			Assert.True(bookLoans.Count > 0);
		}

		public async Task TestListClosedBookLoans()
		{
			await CreateLoan(10, onlyActiveLoans: false);
			List<BookLoan> bookLoans = await GetBookLoans();


		}

		private Task CreateLoan()
		{
			return CreateLoan(1);
		}


		private async Task CreateLoan(int numberOfLoans, bool onlyActiveLoans = true)
		{
			// Arrange

			UserManager<User> userManager = CreateUserManagager();
			User user = userManager.Users.First();
			Book book = await bookRepository.GetBookById(1);
			BookLoan bookLoan = new BookLoan(DateTime.Now, DateTime.Now.AddDays(14), user, book);
			

			//Act
			

			//Assert
			
		}

		private async Task<List<BookLoan>> GetBookLoans()
		{
			UserManager<User> userManager = CreateUserManagager();
			User user = userManager.Users.First();
			IBookLoanRepository bookLoanRepository = new BookLoanRepository(_dbContext);
			List<BookLoan> bookLoans = await bookLoanRepository.GetBookLoansByUserId(user.Id);
			return bookLoans;
		}

		#endregion
	}
}
