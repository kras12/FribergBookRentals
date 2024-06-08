using FribergbookRentals.Data.Models;
using FribergbookRentals.Models;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Mvc;

namespace FribergBookRentals.Components
{
    public partial class BookLoanList : ViewComponent
    {
        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(List<BookLoanViewModel> books, bool enableBookBorrowing = false, bool enableBookReturning = false, bool enableProlongLoan = false)
        {
            return View("BookLoanList", new BookListViewModel(books, enableBookBorrowing: enableBookBorrowing, enableBookReturning: enableBookReturning, enableProlongLoan: enableProlongLoan));
        }

        #endregion
    }
}
