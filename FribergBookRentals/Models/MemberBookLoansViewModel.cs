using FribergbookRentals.Data.Models;

namespace FribergBookRentals.Models
{
    public class MemberBookLoansViewModel
    {
        #region Constructor
        
        public MemberBookLoansViewModel(List<BookLoanViewModel> activeLoans, List<BookLoanViewModel> closedLoans)
        {
            ActiveLoans = activeLoans;
            ClosedLoans = closedLoans;
        }

        #endregion

        #region Properties

        public List<BookLoanViewModel> ActiveLoans { get; set; } = new();

        public List<BookLoanViewModel> ClosedLoans { get; set; } = new();

        #endregion
    }
}
