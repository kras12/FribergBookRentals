using FribergbookRentals.Data.Models;

namespace FribergbookRentals.Data.Repositories
{
	public interface IBookLoanRepository
	{
		public Task<BookLoan> AddAsync(BookLoan bookLoan);

        public Task<BookLoan> AddAsync(DateTime startTime, TimeSpan newEndTimeOffset, string userId, int bookId);

        public Task<BookLoan?> GetBookLoanByIdAsync(int id);

        public Task<BookLoan?> GetBookLoanByUserIdAsync(string userId);

		public Task<List<BookLoan>> GetAllBookLoansAsync();

        public Task<List<BookLoan>> GetActiveBookLoansAsync(string userid);

        public Task<List<BookLoan>> GetClosedBookLoansAsync(string userid);

        public Task<List<BookLoan>> GetBookLoansByUserIdAsync(string userId);

		public Task<BookLoan> CloseLoanAsync(BookLoan bookLoan);

        public Task CloseLoanAsync(string userId, int loanId);

        public Task ProlongLoanAsync(string userId, int loanId, TimeSpan newEndTimeOffset);        

        public Task<BookLoan> ProlongBookLoanAsync(BookLoan bookLoan, TimeSpan newEndTimeOffset);

        public Task<int> CloseExpiredBookLoans();

		public Task<bool> IsBookBorrowedAsync(string userId, int bookId);
	}
}