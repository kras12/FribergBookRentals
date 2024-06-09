using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Constants;
using FribergBookRentals.Models;
using FribergBookRentals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FribergBookRentals.Controllers.Member
{
    public class MemberController : Controller
    {
        #region Constants

        private const string BookLoanClosedResultKey = "MemberControllerBookLoanClosedResultKey";

        private const string BookLoanProlongedResultKey = "MemberControllerBookLoanProlongedResultKey";        

        #endregion

        #region Fields

        IMapper _autoMapper;

        IBookRepository _bookRepository;

        IBookLoanRepository _bookLoanRepository;

        private readonly ITempDataHelper _tempDataHelper;

        #endregion

        #region Constructors

        public MemberController(IBookLoanRepository bookLoanRepository, IMapper autoMapper, IBookRepository bookRepository, ITempDataHelper tempDataHelper)
        {
            _bookLoanRepository = bookLoanRepository;
            _autoMapper = autoMapper;
            _bookRepository = bookRepository;
            _tempDataHelper = tempDataHelper;
        }

        #endregion

        #region Actions

        // GET: MemberController
        [Authorize(Roles = ApplicationUserRoles.Member)]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated || !User.IsInRole(ApplicationUserRoles.Member))
            {
                return Unauthorized();
            }

            List<BookLoanViewModel> activeLoans = _autoMapper.Map<List<BookLoanViewModel>>(await _bookLoanRepository.GetActiveBookLoansAsync(User.Claims.Single(x => x.Type == ApplicationUserClaims.UserId).Value));
            List<BookLoanViewModel> closedLoans = _autoMapper.Map<List<BookLoanViewModel>>(await _bookLoanRepository.GetClosedBookLoansAsync(User.Claims.Single(x => x.Type == ApplicationUserClaims.UserId).Value));
            //activeLoans = closedLoans;
            var viewModel = new MemberBookLoansViewModel(activeLoans, closedLoans);

            if (_tempDataHelper.TryGet(TempData, BookLoanClosedResultKey, out bool isLoanClosed))
            {
                if (isLoanClosed)
                {
                    viewModel.SuccessMessage = "Lånet avslutades.";
                }
                else
                {
                    viewModel.SuccessMessage = "Ett fel inträffade. Vänligen kontakta supporten.";
                }
            }

            if (_tempDataHelper.TryGet(TempData, BookLoanProlongedResultKey, out bool isLoanProlonged))
            {
                if (isLoanProlonged)
                {
                    viewModel.SuccessMessage = "Lånet förlängdes.";
                }
                else
                {
                    viewModel.SuccessMessage = "Ett fel inträffade. Vänligen kontakta supporten.";
                }
            }

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = ApplicationUserRoles.Member)]
        public async Task<IActionResult> CloseLoan(int bookLoanId)
        {
            if (!User.Identity!.IsAuthenticated || !User.IsInRole(ApplicationUserRoles.Member))
            {
                return Unauthorized();
            }

            if (await TryCloseBookLoan(bookLoanId))
            {
                _tempDataHelper.Set(TempData, BookLoanClosedResultKey, true);
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = ApplicationUserRoles.Member)]
        public async Task<IActionResult> ProlongLoan(int bookLoanId)
        {
            if (!User.Identity!.IsAuthenticated || !User.IsInRole(ApplicationUserRoles.Member))
            {
                return Unauthorized();
            }

            if (await TryProlongBookLoan(bookLoanId))
            {
                _tempDataHelper.Set(TempData, BookLoanProlongedResultKey, true);
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Methods

        private async Task<bool> TryCloseBookLoan(int bookLoanId)
        {
            string userId = User.Claims.First(x => x.Type == ApplicationUserClaims.UserId).Value;
            return await _bookLoanRepository.TryCloseLoanAsync(userId, bookLoanId);
        }

        private async Task<bool> TryProlongBookLoan(int bookLoanId)
        {
            string userId = User.Claims.First(x => x.Type == ApplicationUserClaims.UserId).Value;
            return await _bookLoanRepository.TryProlongLoanAsync(userId, bookLoanId, TimeSpan.FromDays(BookLoanData.BookLoanDurationInDays - 1));
        }

        #endregion
    }
}
