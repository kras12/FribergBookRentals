using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergBookRentalsTest.Helpers
{
    public static class BookLoanHelper
    {
        #region Methods

        public static async Task<BookLoan> CreateBookLoan(IBookLoanRepository bookLoanRepository, User user, Book book, bool createActiveLoans, DateTime startTime, DateTime endTime)
        {
            return (await CreateBookLoans(bookLoanRepository, user, new List<Book>() { book }, createActiveLoans, startTime, endTime)).First();
        }

        public static async Task<List<BookLoan>> CreateBookLoans(IBookLoanRepository bookLoanRepository, User user, List<Book> books, bool createActiveLoans, DateTime startTime, DateTime endTime)
        {
            List<BookLoan> result = new();

            foreach (var book in books)
            {
                DateTime? closedTime = createActiveLoans ? null : startTime;

                BookLoan bookLoan = new BookLoan(startTime, endTime, user, book) { ClosedTime = closedTime };
                await bookLoanRepository.AddAsync(bookLoan);
                result.Add(bookLoan);
            }

            return result;
        }

        #endregion
    }
}
