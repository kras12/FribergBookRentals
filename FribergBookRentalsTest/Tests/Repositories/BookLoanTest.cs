using FribergbookRentals.Data.Exceptions;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentalsTest.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergBookRentalsTest.Tests.Repositories
{
    public class BookLoanTest : TestBase
    {
        #region Constants

        private const int BookLoanTime = 14;

        #endregion

        #region Fields

        IBookRepository bookRepository;
        IBookLoanRepository bookLoanRepository;

        #endregion

        #region Constructors

        public BookLoanTest() : base()
        {
            bookRepository = new BookRepository(_dbContext);
            bookLoanRepository = new BookLoanRepository(_dbContext);
        }

        #endregion

        #region Tests

        [Fact]
        public async Task TestCreateBookLoan()
        {
            // Arrange
            var user = await GetDefaultUser();
            var book = (await bookRepository.GetBooksAsync()).First(); 

            // Act
            BookLoan bookLoan = await BookLoanHelper.CreateBookLoan(bookLoanRepository, user, book, createActiveLoans: true, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));

            // Assert
            var bookLoanUserResult = await bookLoanRepository.GetBookLoanByUserIdAsync(user.Id);

            Assert.NotNull(bookLoanUserResult);
            Assert.Equal(bookLoan.User.Id, bookLoanUserResult.User.Id);
            Assert.Equal(bookLoan.StartTime, bookLoanUserResult.StartTime);
            Assert.Equal(bookLoan.EndTime, bookLoanUserResult.EndTime);
        }

        [Fact]
        public async Task TestCloseBookLoan()
        {
            // Arrange
            var user = await GetDefaultUser();
            var book = (await bookRepository.GetBooksAsync()).First();
            BookLoan bookLoan = await BookLoanHelper.CreateBookLoan(bookLoanRepository, user, book, createActiveLoans: true, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));

            // Act
            var result = await bookLoanRepository.CloseLoanAsync(bookLoan);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(bookLoan.ClosedTime);
            Assert.Equal(bookLoan.ClosedTime.Value.Date, DateTime.Now.Date);
        }

        [Fact]
        public async Task TestListActiveBookLoans()
        {
            // Arrange
            int loanCount = 10;
            var user = await GetDefaultUser();
            var books = (await bookRepository.GetBooksAsync()).Take(loanCount).ToList();
            await BookLoanHelper.CreateBookLoans(bookLoanRepository, user, books, createActiveLoans: true, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));

            // Act
            List<BookLoan> bookLoans = await bookLoanRepository.GetBookLoansByUserIdAsync(user.Id);

            // Assert
            Assert.True(bookLoans.Count == loanCount);
            Assert.All(bookLoans, x => Assert.Equal(user.Id, x.User.Id));
            Assert.All(bookLoans, x => Assert.False(x.ClosedTime.HasValue));
        }

        [Fact]
        public async Task TestListClosedBookLoans()
        {
            // Arrange
            int loanCount = 10;
            var user = await GetDefaultUser();
            var books = (await bookRepository.GetBooksAsync()).Take(loanCount).ToList();
            await BookLoanHelper.CreateBookLoans(bookLoanRepository, user, books, createActiveLoans: false, DateTime.Now, DateTime.Now.AddDays(BookLoanTime - 1));

            // Act
            List<BookLoan> bookLoans = await bookLoanRepository.GetBookLoansByUserIdAsync(user.Id);

            // Assert
            Assert.True(bookLoans.Count == loanCount);
            Assert.All(bookLoans, x => Assert.Equal(user.Id, x.User.Id));
            Assert.All(bookLoans, x => Assert.True(x.ClosedTime.HasValue));
        }

        [MemberData(nameof(GetTestProlongActiveBookLoanInputData))]
        [Theory]
        public async Task TestProlongActiveBookLoan(int offsetDays)
        {
            // Arrange
            var user = await GetDefaultUser();
            var book = (await bookRepository.GetBooksAsync()).First();
            BookLoan bookLoan = await BookLoanHelper.CreateBookLoan(bookLoanRepository, user, book, createActiveLoans: true, 
                startTime: DateTime.Now.AddDays(offsetDays), endTime: DateTime.Now.AddDays(offsetDays).AddDays(BookLoanTime - 1));

            // Act
            BookLoan prolongedBookLoan = await bookLoanRepository.ProlongBookLoanAsync(bookLoan, DateTime.Now.AddDays(BookLoanTime - 1));

            // Assert
            Assert.Null(prolongedBookLoan.ClosedTime);
            Assert.True(prolongedBookLoan.EndTime.Date == DateTime.Now.Date.AddDays(BookLoanTime - 1));
            Assert.Equal(bookLoan.StartTime, prolongedBookLoan.StartTime);
            Assert.Equal(bookLoan.User.Id, prolongedBookLoan.User.Id);
            Assert.Equal(bookLoan.Book.BookId, prolongedBookLoan.Book.BookId);
        }

        [MemberData(nameof(GetTestProlongClosedBookLoanInputData))]
        [Theory]
        public async Task TestProlongClosedBookLoan(int offsetDays)
        {
            // Arrange
            var user = await GetDefaultUser();
            var book = (await bookRepository.GetBooksAsync()).First();
            BookLoan bookLoan = await BookLoanHelper.CreateBookLoan(bookLoanRepository, user, book, createActiveLoans: false,
                startTime: DateTime.Now.AddDays(offsetDays), endTime: DateTime.Now.AddDays(offsetDays).AddDays(BookLoanTime - 1));

            // Act
            // Assert
            await Assert.ThrowsAsync<BookLoanClosedException>(async () => await bookLoanRepository.ProlongBookLoanAsync(bookLoan, TimeSpan.FromDays(BookLoanTime - 1)));
        }

        [Fact]
        public async Task TestCloseExpiredBookLoans()
        {
            // Arrange
            int numberOfActiveLoans = 15;
            int numberOfExpiredLoans = 10;
            var user = await GetDefaultUser();
            var books = await bookRepository.GetBooksAsync();
            DateTime startTimeActiveLoans = DateTime.Now.AddDays(-BookLoanTime / 2);
            DateTime startTimeClosedLoans = DateTime.Now.AddDays(-BookLoanTime);

            await BookLoanHelper.CreateBookLoans(bookLoanRepository, user, books.Take(numberOfActiveLoans).ToList(), createActiveLoans: true, 
                startTime: startTimeActiveLoans, endTime: startTimeActiveLoans.AddDays(BookLoanTime - 1));
            await BookLoanHelper.CreateBookLoans(bookLoanRepository, user, books.Take(numberOfExpiredLoans).ToList(), createActiveLoans: true, 
                startTime: startTimeClosedLoans, endTime: startTimeClosedLoans.AddDays(BookLoanTime - 1));

            // Act
            await bookLoanRepository.CloseExpiredBookLoans();

            // Assert
            var loans = await bookLoanRepository.GetAllBookLoansAsync();
            Assert.Equal(numberOfActiveLoans + numberOfExpiredLoans, loans.Count);
            Assert.Equal(numberOfActiveLoans, loans.Count(x => x.ClosedTime == null));
            Assert.Equal(numberOfExpiredLoans, loans.Count(x => x.ClosedTime != null));
        }

        #endregion

        #region Methods        

        public static IEnumerable<object[]> GetTestProlongActiveBookLoanInputData()
        {
            List<object[]> data = new List<object[]>();

            for (int i = -BookLoanTime + 1; i <= 0; i++)
            {
                data.Add(new object[] { i });
            }

            return data;
        }

        public static IEnumerable<object[]> GetTestProlongClosedBookLoanInputData()
        {
            List<object[]> data = new List<object[]>();

            for (int i = -BookLoanTime; i >= -BookLoanTime - 2; i--)
            {
                data.Add(new object[] { i });
            }

            return data;
        }

        #endregion
    }
}
