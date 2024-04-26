using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;

namespace HDUA.Controllers
{
    public class LoginController : Controller
    {

        Procesos procesos = new Procesos();

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Reestaurar()
        {
            return View();
        }

        public IActionResult Registrarme()
        {
            ViewBag.lgu = procesos.Listar("LISTARGENEROUSUARIO");
            ViewBag.li = procesos.Listar("LISTARINSTITUCION");
            ViewBag.ltu = procesos.Listar("LISTARTIPOUSUARIO");
            return View();
        }


        [HttpPost]
        public ActionResult Registrar()
        {
            UsuarioModel usuario = new UsuarioModel();
            usuario.Nombre = Request.Form["nombre"];
            usuario.Tipo = Request.Form["tipoUsuario"];
            usuario.Institucion = Request.Form["institucion"];
            usuario.Genero = Request.Form["genero"];
            usuario.Correo = Request.Form["correo"];
            usuario.Contrasenia = Request.Form["contrasenia"];

            procesos.RegistrarUsuario(usuario);

            return RedirectToAction("Principal", "Principal");
        }
    }
}
