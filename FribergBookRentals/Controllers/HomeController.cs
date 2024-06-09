using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergbookRentals.Models;
using FribergBookRentals.Constants;
using FribergBookRentals.Data;
using FribergBookRentals.Models;
using FribergBookRentals.Services;
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

        private readonly IMapper _autoMapper;

        private readonly IBookRepository _bookRepository;

        private readonly IBookLoanRepository _bookLoanRepository;

        private readonly SignInManager<User> _signInManager;

        private readonly ITempDataHelper _tempDataHelper;

        #endregion

        #region Constructors

        public HomeController(IBookRepository bookRepository, IMapper autoMapper, SignInManager<User> signInManager, 
            IBookLoanRepository bookLoanRepository, ITempDataHelper tempDataHelper)
        {
            _bookRepository = bookRepository;
            _autoMapper = autoMapper;
            _signInManager = signInManager;
            _bookLoanRepository = bookLoanRepository;
            _tempDataHelper = tempDataHelper;
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
            if (_tempDataHelper.TryGet(TempData, BorrowBookAfterLoginKey, out int bookId))
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

            if (_tempDataHelper.TryGet(TempData, BookAlreadyBorrowedKey, out bookid))
            {
                borrowedBook = await _bookRepository.GetBookByIdAsync(bookid);
                viewModel.FailureMessage = $"Du har redan ett lån på boken: {borrowedBook!.Title}";
            }
            else if (_tempDataHelper.TryGet(TempData, BookBorrowedSuccessfullyKey, out bookid))
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
            if (!_signInManager.IsSignedIn(User))
            {
                _tempDataHelper.Set(TempData, BorrowBookAfterLoginKey, id);
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = "/" });
            }

            if (!User.IsInRole(ApplicationUserRoles.Member))
            {
                return Unauthorized();
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
                _tempDataHelper.Set(TempData, BookAlreadyBorrowedKey, id);
                return false;
            }
            else
            {
                await CreateBookLoan(id);
                _tempDataHelper.Set(TempData, BookBorrowedSuccessfullyKey, id);
                return true;
            }
        }

        public virtual string GetUserId()
        {
            return User.Claims.First(x => x.Type == ApplicationUserClaims.UserId).Value;
        }

        private async Task<bool> HaveBorrowedBook(int id)
        {
            if (await _bookLoanRepository.IsBookBorrowedAsync(GetUserId(), id))
            {
                return true;
            }

            return false;
        }

        private async Task CreateBookLoan(int id)
        {
            await _bookLoanRepository.AddAsync(DateTime.Now, DateTime.Now.AddDays(BookLoanData.BookLoanDurationInDays), GetUserId(), id);
        }

        #endregion
    }
}
