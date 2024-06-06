using AutoMapper;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergbookRentals.Models;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FribergBookRentals.Controllers
{
	public class HomeController : Controller
	{
        #region Fields

        IMapper _autoMapper;

        IBookRepository _bookRepository;

        #endregion

        #region Constructors

        public HomeController(IBookRepository bookRepository, IMapper autoMapper)
        {
            _bookRepository = bookRepository;
            _autoMapper = autoMapper;
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
            return View(new List<BookViewModel>());
        }

        [HttpPost]
        public async Task<IActionResult> Index(BookSearchInputViewModel searchInput)
        {
            List<BookViewModel> books = new();

            if (ModelState.IsValid)
            {
                var searchInputDto = _autoMapper.Map<BookSearchInputDto>(searchInput);
                books = _autoMapper.Map<List<BookViewModel>>(await _bookRepository.SearchBooksAsync(searchInputDto));
            }

            return View(books);
        }

        #endregion
    }
}
