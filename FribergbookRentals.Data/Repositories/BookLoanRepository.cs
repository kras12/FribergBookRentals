using FribergbookRentals.Data.Exceptions;
using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _applicationDbContext.BookLoans.Add(bookLoan);
			await _applicationDbContext.SaveChangesAsync();
            return bookLoan;
        }

		public async Task<BookLoan> CloseLoanAsync(BookLoan bookLoan)
		{
			bookLoan.ClosedTime = DateTime.Now;
			await _applicationDbContext.SaveChangesAsync();
			return bookLoan;
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

		public async Task<BookLoan> ProlongBookLoanAsync(BookLoan bookLoan, DateTime newEndTime)
		{
			if (bookLoan.ClosedTime.HasValue)
			{
				throw new BookLoanExpiredException("Can't prolong a book loan that has been closed.");
			}

			_applicationDbContext.Attach(bookLoan);
			bookLoan.EndTime = newEndTime;
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

		#endregion
	}
}
