using AutoMapper;
using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using FribergBookRentals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FribergBookRentals.Controllers.Member
{
    public class MemberController : Controller
    {
        #region Fields

        IMapper _autoMapper;

        IBookLoanRepository _bookLoanRepository;

        #endregion

        #region Constructors

        public MemberController(IBookLoanRepository bookLoanRepository, IMapper autoMapper)
        {
            _bookLoanRepository = bookLoanRepository;
            _autoMapper = autoMapper;
        }

        #endregion

        // GET: MemberController
        [Authorize(Roles = ApplicationUserRoles.Member)]
        public async Task<IActionResult> Index()
        {
            List<BookLoanViewModel> activeLoans = _autoMapper.Map<List<BookLoanViewModel>>(await _bookLoanRepository.GetActiveBookLoansAsync(User.Claims.Single(x => x.Type == ApplicationUserClaims.UserId).Value));
            List<BookLoanViewModel> closedLoans = _autoMapper.Map<List<BookLoanViewModel>>(await _bookLoanRepository.GetClosedBookLoansAsync(User.Claims.Single(x => x.Type == ApplicationUserClaims.UserId).Value));
            //activeLoans = closedLoans;
            var result = new MemberBookLoansViewModel(activeLoans, closedLoans);

            return View(result);
        }
    }
}
