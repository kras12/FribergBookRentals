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

		private readonly ApplicationDbContext applicationDbContext;

        #endregion

        #region Constructors

        public BookLoanRepository(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        #endregion

        #region Methods

        public async Task<BookLoan> AddAsync(BookLoan bookLoan)
        {
            applicationDbContext.BookLoans.Add(bookLoan);
			await applicationDbContext.SaveChangesAsync();
            return bookLoan;
        }

		public async Task<BookLoan> CloseLoanAsync(BookLoan bookLoan)
		{
			bookLoan.ClosedTime = DateTime.Now;
			await applicationDbContext.SaveChangesAsync();
			return bookLoan;
        }

		public async Task<BookLoan?> GetBookLoanByUserIdAsync(string userId)
		{
            return await applicationDbContext.BookLoans.FirstOrDefaultAsync(b => b.User.Id == userId);
			
        }

		public async Task<List<BookLoan>> GetBookLoansByUserIdAsync(string userId)
		{
            return await applicationDbContext.BookLoans.Where(b => b.User.Id == userId).ToListAsync();
        }

		public async Task<BookLoan> ProlongBookLoanAsync(BookLoan bookLoan, DateTime newEndTime)
		{
			if (bookLoan.ClosedTime.HasValue)
			{
				throw new BookLoanExpiredException("Can't prolong a book loan that has been closed.");
			}

			applicationDbContext.Attach(bookLoan);
			bookLoan.EndTime = newEndTime;
			await applicationDbContext.SaveChangesAsync();
			return bookLoan;
		}

		#endregion
	}
}
