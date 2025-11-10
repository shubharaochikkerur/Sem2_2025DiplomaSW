using Microsoft.AspNetCore.Mvc;

namespace StudentInfoLoginRoles.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
