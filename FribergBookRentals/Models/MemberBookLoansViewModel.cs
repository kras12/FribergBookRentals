using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        [BindNever]
        public List<BookLoanViewModel> ActiveLoans { get; set; } = new();

        [BindNever]
        public List<BookLoanViewModel> ClosedLoans { get; set; } = new();

        [BindNever]
        public string? FailureMessage { get; set; } = null;

        [BindNever]
        public string? SuccessMessage { get; set; } = null;

        #endregion
    }
}
