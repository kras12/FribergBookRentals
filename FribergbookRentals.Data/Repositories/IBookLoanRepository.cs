using FribergbookRentals.Data.Models;

namespace FribergbookRentals.Data.Repositories
{
	public interface IBookLoanRepository
	{
		public Task<BookLoan> AddAsync(BookLoan bookLoan);
		public Task<BookLoan> GetBookLoanByUserId(string userId);
		public Task<List<BookLoan>> GetBookLoansByUserId(string userId);
	}
}