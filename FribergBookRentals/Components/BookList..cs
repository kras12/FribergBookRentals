using FribergbookRentals.Models;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Mvc;

namespace FribergBookRentals.Components
{
    public partial class BookList : ViewComponent
    {
        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(List<BookViewModel> books, bool enableBookBorrowing = false, bool enableBookReturning = false, bool enableProlongLoan = false)
        {
            return View("BookList", new BookListViewModel(books, enableBookBorrowing: enableBookBorrowing, enableBookReturning: enableBookReturning, enableProlongLoan: enableProlongLoan));
        }

        #endregion
    }
}
