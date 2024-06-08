using FribergbookRentals.Data.Models;
using FribergbookRentals.Models;
using FribergBookRentals.Components;

namespace FribergBookRentals.Models
{
    public class BookListViewModel : IBookListSettings
    {
        #region Constructors

        public BookListViewModel(List<BookViewModel> books, bool enableBookBorrowing, bool enableBookReturning, bool enableProlongLoan)
        {
            Books = books.Select(x => new BookListItemViewModel(this, x)).ToList();
            EnableBookBorrowing = enableBookBorrowing;
            EnableCloseLoan = enableBookReturning;
            EnableProlongLoan = enableProlongLoan;
        }

        public BookListViewModel(List<BookLoanViewModel> bookLoans, bool enableBookBorrowing, bool enableBookReturning, bool enableProlongLoan)
        {
            Books = bookLoans.Select(x => new BookListItemViewModel(this, x)).ToList();
            EnableBookBorrowing = enableBookBorrowing;
            EnableCloseLoan = enableBookReturning;
            EnableProlongLoan = enableProlongLoan;
        }

        #endregion

        #region Properties

        public List<BookListItemViewModel> Books { get; set; }

        public bool EnableBookBorrowing { get; set; }

        public bool EnableCloseLoan { get; set; }

        public bool EnableProlongLoan { get; set; }

        #endregion
    }
}
