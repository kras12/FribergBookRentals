using FribergbookRentals.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FribergBookRentals.Models
{
    public class BookSearchViewModel
    {
        #region Constants

        public const string AllLanguagesChoice = "Alla";

        #endregion

        #region Constructors

        public BookSearchViewModel(List<string> languages)
        {
            Languages.Add(new SelectListItem(AllLanguagesChoice, ""));
            Languages.AddRange(languages.Select(x => new SelectListItem(x, x)).ToList());
        }

        public BookSearchViewModel(List<string> languages, BookSearchInputViewModel searchInput, List<BookViewModel> books, bool haveSearchedBooks) : this(languages)
        {
            SearchInput = searchInput;
            Books = books;
            HaveSearchedBooks = haveSearchedBooks;
        }

        #endregion

        #region Properties

        [BindNever]
        public bool HaveSearchedBooks { get; set; }

        public BookSearchInputViewModel SearchInput { get; set; } = new();

        [BindNever]
        public List<BookViewModel>? Books { get; set; } = null;

        [BindNever]
        public List<SelectListItem> Languages { get; set; } = new();

        [BindNever]
        public string? FailureMessage { get; set; } = null;

        [BindNever]
        public string? SuccessMessage { get; set; } = null;

        #endregion
    }
}
