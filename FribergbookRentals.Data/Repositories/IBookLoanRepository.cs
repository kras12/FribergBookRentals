using FribergbookRentals.Data.Models;

namespace FribergbookRentals.Data.Repositories
{
	public interface IBookLoanRepository
	{
		public Task<BookLoan> AddAsync(BookLoan bookLoan);

		public Task<BookLoan?> GetBookLoanByUserIdAsync(string userId);

		public Task<List<BookLoan>> GetAllBookLoansAsync();

        public Task<List<BookLoan>> GetActiveBookLoansAsync(string userid);

        public Task<List<BookLoan>> GetClosedBookLoansAsync(string userid);

        public Task<List<BookLoan>> GetBookLoansByUserIdAsync(string userId);

		public Task<BookLoan> CloseLoanAsync(BookLoan bookLoan);

		public Task<BookLoan> ProlongBookLoanAsync(BookLoan bookLoan, DateTime newEndTime);

		public Task<int> CloseExpiredBookLoans();
	}
}