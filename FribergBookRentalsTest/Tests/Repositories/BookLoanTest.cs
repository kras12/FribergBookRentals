using FribergbookRentals.Data.Exceptions;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
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

            // Act
            BookLoan bookLoan = await CreateBookLoan(user, createActiveLoans: true);

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
            BookLoan bookLoan = await CreateBookLoan(user, createActiveLoans: true);

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
            await CreateBookLoans(user, loanCount, createActiveLoans: true);

            // Act
            List<BookLoan> bookLoans = await GetBookLoans(user);

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
            await CreateBookLoans(user, loanCount, createActiveLoans: false);

            // Act
            List<BookLoan> bookLoans = await GetBookLoans(user);

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
            BookLoan bookLoan = await CreateBookLoan(user, createActiveLoans: true, overrideStartTime: DateTime.Now.AddDays(offsetDays));

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
            BookLoan bookLoan = await CreateBookLoan(user, createActiveLoans: false, overrideStartTime: DateTime.Now.AddDays(offsetDays));

            // Act
            // Assert
            await Assert.ThrowsAsync<BookLoanExpiredException>(async () => await bookLoanRepository.ProlongBookLoanAsync(bookLoan, DateTime.Now.AddDays(BookLoanTime - 1)));
        }

        [Fact]
        public async Task TestCloseExpiredBookLoans()
        {
            // Arrange
            int numberOfActiveLoans = 15;
            int numberOfExpiredLoans = 10;
            var user = await GetDefaultUser();
            await CreateBookLoans(user, numberOfActiveLoans, createActiveLoans: true, overrideStartTime: DateTime.Now.AddDays(-BookLoanTime / 2).Date);
            await CreateBookLoans(user, numberOfExpiredLoans, createActiveLoans: true, overrideStartTime: DateTime.Now.AddDays(-BookLoanTime).Date);

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

        private async Task<BookLoan> CreateBookLoan(User user, bool createActiveLoans, DateTime? overrideStartTime = null)
        {
            return (await CreateBookLoans(user, 1, createActiveLoans, overrideStartTime: overrideStartTime)).First();
        }

        private async Task<List<BookLoan>> CreateBookLoans(User user, int numberOfLoans, bool createActiveLoans, DateTime? overrideStartTime = null)
        {
            // Arrange
            DateTime startTime = overrideStartTime ?? DateTime.Now;
            List<Book> books = await bookRepository.GetBooksAsync();
            List<BookLoan> result = new();

            // Act
            for (int i = 0; i < numberOfLoans; i++)
            {
                int bookIndex = i < books.Count ? i : books.Count - 1;
                DateTime? closedTime = createActiveLoans ? null : startTime;

                BookLoan bookLoan = new BookLoan(startTime, startTime.AddDays(BookLoanTime - 1), user, books[bookIndex]) { ClosedTime = closedTime };
                await bookLoanRepository.AddAsync(bookLoan);
                result.Add(bookLoan);
            }

            return result;
        }

        private async Task<List<BookLoan>> GetBookLoans(User user)
        {
            IBookLoanRepository bookLoanRepository = new BookLoanRepository(_dbContext);
            List<BookLoan> bookLoans = await bookLoanRepository.GetBookLoansByUserIdAsync(user.Id);
            return bookLoans;
        }

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
