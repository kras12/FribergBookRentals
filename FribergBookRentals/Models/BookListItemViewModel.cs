using FribergbookRentals.Data.Models;
using FribergbookRentals.Models;
using FribergBookRentals.Components;

namespace FribergBookRentals.Models
{
    public class BookListItemViewModel
    {
        #region Constructors

        public BookListItemViewModel(IBookListSettings listSettings, BookViewModel book)
        {
            Book = book;
            ListSettings = listSettings;
        }

        public BookListItemViewModel(IBookListSettings listSettings, BookLoanViewModel bookLoan)
        {
            BookLoan = bookLoan;
            Book = bookLoan.Book;
            ListSettings = listSettings;
        }

        #endregion

        #region Properties

        public BookViewModel Book { get; }

        public BookLoanViewModel? BookLoan { get; }

        public IBookListSettings ListSettings { get; }

        #endregion
    }
}


