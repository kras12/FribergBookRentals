using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Constants;
using FribergBookRentals.Helpers;
using FribergBookRentals.Models;
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

        #endregion

        #region Constructors

        public MemberController(IBookLoanRepository bookLoanRepository, IMapper autoMapper, IBookRepository bookRepository)
        {
            _bookLoanRepository = bookLoanRepository;
            _autoMapper = autoMapper;
            _bookRepository = bookRepository;
        }

        #endregion

        #region Actions

        // GET: MemberController
        [Authorize(Roles = ApplicationUserRoles.Member)]
        public async Task<IActionResult> Index()
        {
            List<BookLoanViewModel> activeLoans = _autoMapper.Map<List<BookLoanViewModel>>(await _bookLoanRepository.GetActiveBookLoansAsync(User.Claims.Single(x => x.Type == ApplicationUserClaims.UserId).Value));
            List<BookLoanViewModel> closedLoans = _autoMapper.Map<List<BookLoanViewModel>>(await _bookLoanRepository.GetClosedBookLoansAsync(User.Claims.Single(x => x.Type == ApplicationUserClaims.UserId).Value));
            //activeLoans = closedLoans;
            var viewModel = new MemberBookLoansViewModel(activeLoans, closedLoans);

            if (TempDataHelper.TryGet(TempData, BookLoanClosedResultKey, out bool isLoanClosed))
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

            if (TempDataHelper.TryGet(TempData, BookLoanProlongedResultKey, out bool isLoanProlonged))
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
            if (await TryCloseBookLoan(bookLoanId))
            {
                TempDataHelper.Set(TempData, BookLoanClosedResultKey, true);
            }
            else
            {
                TempDataHelper.Set(TempData, BookLoanClosedResultKey, false);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = ApplicationUserRoles.Member)]
        public async Task<IActionResult> ProlongLoan(int bookLoanId)
        {
            if (await TryProlongBookLoan(bookLoanId))
            {
                TempDataHelper.Set(TempData, BookLoanProlongedResultKey, true);
            }
            else
            {
                TempDataHelper.Set(TempData, BookLoanProlongedResultKey, false);
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
            return await _bookLoanRepository.TryProlongLoanAsync(userId, bookLoanId, BookLoanData.BookLoanDurationInDays);
        }

        #endregion
    }
}
