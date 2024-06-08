using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergbookRentals.Models;
using FribergBookRentals.Constants;
using FribergBookRentals.Data;
using FribergBookRentals.Helpers;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Reflection.Metadata.BlobBuilder;

namespace FribergBookRentals.Controllers
{
	public class HomeController : Controller
	{
        #region Constants

        private const string BorrowBookAfterLoginKey = "HomeControllerBorrowBookAfterLoginKey";

        private const string BookAlreadyBorrowedKey = "HomeControllerBookAlreadyBorrowedKey";

        private const string BookBorrowedSuccessfullyKey = "HomeControllerBookBorrowedSuccessfullyKey";

        #endregion

        #region Fields

        IMapper _autoMapper;

        IBookRepository _bookRepository;

        IBookLoanRepository _bookLoanRepository;

        private readonly SignInManager<User> _signInManager;

        #endregion

        #region Constructors

        public HomeController(IBookRepository bookRepository, IMapper autoMapper, SignInManager<User> signInManager, IBookLoanRepository bookLoanRepository)
        {
            _bookRepository = bookRepository;
            _autoMapper = autoMapper;
            _signInManager = signInManager;
            _bookLoanRepository = bookLoanRepository;
        }

        #endregion

        #region Actions

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            if (TempDataHelper.TryGet(TempData, BorrowBookAfterLoginKey, out int bookId))
            {            
                if (_signInManager.IsSignedIn(User) && User.IsInRole(ApplicationUserRoles.Member))
                {
                    await TryBorrowBook(bookId);
                }
            }

            int bookid;
            var languages = await _bookRepository.GetBookLanguages();
            var viewModel = new BookSearchViewModel(languages);
            Book? borrowedBook;

            if (TempDataHelper.TryGet(TempData, BookAlreadyBorrowedKey, out bookid))
            {
                borrowedBook = await _bookRepository.GetBookByIdAsync(bookid);
                viewModel.FailureMessage = $"Du har redan ett lån på boken: {borrowedBook!.Title}";
            }
            else if (TempDataHelper.TryGet(TempData, BookBorrowedSuccessfullyKey, out bookid))
            {
                borrowedBook = await _bookRepository.GetBookByIdAsync(bookid);
                viewModel.SuccessMessage = $"Du har nu lånat boken: {borrowedBook!.Title}";
            }
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BookSearchInputViewModel searchInput)
        {
            var languages = await _bookRepository.GetBookLanguages();
            List<BookViewModel> books = new();

            if (ModelState.IsValid)
            {
                var searchInputDto = _autoMapper.Map<BookSearchInputDto>(searchInput);
                books = _autoMapper.Map<List<BookViewModel>>(await _bookRepository.SearchBooksAsync(searchInputDto));

                if (_signInManager.IsSignedIn(User) && User.IsInRole(ApplicationUserRoles.Member))
                {
                    string userId = User.Claims.First(x => x.Type == ApplicationUserClaims.UserId).Value;
                    var borrowedBooks = await _bookLoanRepository.GetActiveBookLoansAsync(userId);

                    foreach (var book in books)
                    {
                        if (borrowedBooks.Any(x => x.Book.BookId == book.BookId))
                        {
                            book.IsBorrowedByUser = true;
                        }
                        else
                        {
                            book.IsBorrowedByUser = false;
                        }
                    }
                }
            }            

            return View(new BookSearchViewModel(languages, searchInput, books, haveSearchedBooks: true));
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook([FromForm] int id)
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempDataHelper.Set(TempData, BorrowBookAfterLoginKey, id);
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = "/" });
            }

            await TryBorrowBook(id);
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region OtherMethods

        private async Task<bool> TryBorrowBook(int id)
        {
            if (await HaveBorrowedBook(id))
            {
                TempDataHelper.Set(TempData, BookAlreadyBorrowedKey, id);
                return false;
            }
            else
            {
                await CreateBookLoan(id);
                TempDataHelper.Set(TempData, BookBorrowedSuccessfullyKey, id);
                return true;
            }
        }

        private async Task<bool> HaveBorrowedBook(int id)
        {
            string userId = User.Claims.First(x => x.Type == ApplicationUserClaims.UserId).Value;

            if (await _bookLoanRepository.IsBookBorrowedAsync(userId, id))
            {
                return true;
            }

            return false;
        }

        private async Task CreateBookLoan(int id)
        {
            string userId = User.Claims.First(x => x.Type == ApplicationUserClaims.UserId).Value;
            await _bookLoanRepository.AddAsync(DateTime.Now, DateTime.Now.AddDays(BookLoanData.BookLoanDurationInDays), userId, id);
        }

        #endregion
    }
}
