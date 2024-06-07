using AutoMapper;
using FribergbookRentals.Data.Repositories;
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
        public ActionResult Index()
        {
            return View();
        }
    }
}
