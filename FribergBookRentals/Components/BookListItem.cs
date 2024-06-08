using FribergbookRentals.Models;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace FribergBookRentals.Components
{
    public partial class BookListItem : ViewComponent
    {
        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(BookListItemViewModel book)
        {
            return View("BookListItem", book);
        }

        #endregion
    }
}
