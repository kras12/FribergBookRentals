using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
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
            return bookLoan;
        }

        #endregion
    }
}
