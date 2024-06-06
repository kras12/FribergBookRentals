using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FribergBookRentals.Controllers.Member
{
    public class MemberController : Controller
    {
        // GET: MemberController
        public ActionResult Index()
        {
            return View();
        }
    }
}
