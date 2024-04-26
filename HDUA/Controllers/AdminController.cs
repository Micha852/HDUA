using Microsoft.AspNetCore.Mvc;

namespace HDUA.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Principal()
        {
            return View();
        }

        public IActionResult GestionMuestra()
        {
            return View();
        }
    }
}
