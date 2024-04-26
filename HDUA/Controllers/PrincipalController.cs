using Microsoft.AspNetCore.Mvc;

namespace HDUA.Controllers
{
    public class PrincipalController : Controller
    {
        public IActionResult Principal()
        {
            return View();
        }
        public IActionResult Avanzada()
        {
            return View();
        }
    }
}
