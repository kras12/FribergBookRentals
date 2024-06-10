using FribergbookRentals.Data.Exceptions;
using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
using Microsoft.EntityFrameworkCore;

namespace FribergbookRentals.Data.Repositories
{
	public class BookLoanRepository : IBookLoanRepository
	{
		#region Fields

		private readonly ApplicationDbContext _applicationDbContext;

        #endregion

        #region Constructors

        public BookLoanRepository(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }

        #endregion

        #region Methods

        public async Task<BookLoan> AddAsync(BookLoan bookLoan)
        {
			if (await IsBookBorrowedAsync(bookLoan.User.Id, bookLoan.Book.BookId))
			{
				throw new BookAlreadyBorrowedException($"The user ({bookLoan.User.Id}) has already borrowed the book ({bookLoan.Book.BookId}).");
			}

            _applicationDbContext.BookLoans.Add(bookLoan);
			await _applicationDbContext.SaveChangesAsync();
            return bookLoan;
        }

        public Task<BookLoan> AddAsync(DateTime startTime, TimeSpan newEndTimeOffset, string userId, int bookId)
		{
			var user = _applicationDbContext.Users.Find(userId) ?? throw new UserNotFoundException("User not found");
			var book = _applicationDbContext.Books.Find(bookId) ?? throw new BookNotFoundException("Book not found");
			var newBookLoan = new BookLoan(startTime, startTime.Add(newEndTimeOffset), user, book);
			return AddAsync(newBookLoan);
        }

        public async Task<BookLoan> CloseLoanAsync(BookLoan bookLoan)
		{
			//bookLoan.ClosedTime = DateTime.Now;

            if (bookLoan.ClosedTime != null)
            {
                throw new BookLoanClosedException();
            }

            bookLoan.ClosedTime = DateTime.Now;
			await _applicationDbContext.SaveChangesAsync();
			return bookLoan;
        }

        public async Task CloseLoanAsync(string userId, int loanId)
        {
			var loan = await _applicationDbContext.BookLoans.Where(x => x.User.Id == userId && x.Id == loanId).SingleOrDefaultAsync();

			if (loan == null)
			{
				throw new BookLoanNotFoundException();
			}

            if (loan.ClosedTime != null)
			{
				throw new BookLoanClosedException();
			}

			await CloseLoanAsync(loan);
		}

        public async Task ProlongLoanAsync(string userId, int loanId, TimeSpan newEndTimeOffset)
        {
            var loan = await _applicationDbContext.BookLoans.Where(x => x.User.Id == userId && x.Id == loanId).SingleOrDefaultAsync();

            if (loan == null)
            {
                throw new BookLoanNotFoundException();
            }

            if (loan.ClosedTime != null)
            {
                throw new BookLoanClosedException();
            }

			await ProlongBookLoanAsync(loan, newEndTimeOffset);
        }

        public async Task<BookLoan?> GetBookLoanByIdAsync(int id)
		{
			return await _applicationDbContext.BookLoans.FirstOrDefaultAsync(x => x.Id == id);
		}

        public async Task<BookLoan?> GetBookLoanByUserIdAsync(string userId)
		{
            return await _applicationDbContext.BookLoans.FirstOrDefaultAsync(b => b.User.Id == userId);
			
        }

		public async Task<List<BookLoan>> GetAllBookLoansAsync()
		{
			return await _applicationDbContext.BookLoans.ToListAsync();
		}

        public Task<List<BookLoan>> GetActiveBookLoansAsync(string userId)
		{
			return _applicationDbContext.BookLoans.Where(x => x.User.Id == userId && x.ClosedTime == null).ToListAsync();
		}

        public Task<List<BookLoan>> GetClosedBookLoansAsync(string userId)
		{
            return _applicationDbContext.BookLoans.Where(x => x.User.Id == userId && x.ClosedTime != null).ToListAsync();
        }

        public async Task<List<BookLoan>> GetBookLoansByUserIdAsync(string userId)
		{
            return await _applicationDbContext.BookLoans.Where(b => b.User.Id == userId).ToListAsync();
        }

		public async Task<BookLoan> ProlongBookLoanAsync(BookLoan bookLoan, TimeSpan newEndTimeOffset)
		{
			if (bookLoan.ClosedTime.HasValue)
			{
				throw new BookLoanClosedException("Can't prolong a book loan that has been closed.");
			}

			_applicationDbContext.Attach(bookLoan);
			bookLoan.EndTime = DateTime.Now.Add(newEndTimeOffset);
			await _applicationDbContext.SaveChangesAsync();
			return bookLoan;
		}

        public async Task<int> CloseExpiredBookLoans()
		{
			var loans = await _applicationDbContext.BookLoans
				.Where(x => x.ClosedTime == null && x.EndTime.Date < DateTime.Now.Date)
				.ToListAsync();

			loans.ForEach(x => x.ClosedTime = DateTime.Now);

			return await _applicationDbContext.SaveChangesAsync();				
		}

        public Task<bool> IsBookBorrowedAsync(string userId, int bookId)
        {
			return _applicationDbContext.BookLoans.AnyAsync(x => x.User.Id == userId && x.Book.BookId == bookId && x.ClosedTime == null);
		}

        #endregion
    }
}
