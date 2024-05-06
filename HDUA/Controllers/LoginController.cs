using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using PROYECTO.SERVICIOS;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace HDUA.Controllers
{
    public class LoginController : Controller
    {

        Procesos procesos = new Procesos();

        public IActionResult Login()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToAction("Principal", "Principal");
            }
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

        static string GenerateRandomSequence()
        {
            Random random = new Random();
            string sequence = string.Empty;
            for (int i = 0; i < 7; i++)
            {
                int randomNumber = random.Next(1, 10); // Genera un número aleatorio entre 1 y 9.
                sequence += randomNumber.ToString();
            }
            return sequence;
        }

        [HttpPost]
        public ActionResult RecuperarPword(String correo)
        {
            String contra = GenerateRandomSequence();
            Boolean aux = procesos.BUSCARCOREO(correo, contra);
            if (aux)
            {
                ServicioGmail enviar = new ServicioGmail();
                enviar.SendEmailGmail(correo, "Recuperación de contraseña", "Esta es su nueva contraseña: " + contra);
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View("Reestaurar");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Login(String NOMBRECORREO, String PWORD){
            UsuarioModel usuario = procesos.ValidarUsuario(NOMBRECORREO, PWORD);
            if(usuario.Nombre != null){
                var claims = new List<Claim>{
                    new Claim (ClaimTypes.Name, usuario.Nombre),
                    new Claim (ClaimTypes.Role, usuario.Rol)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Principal", "Principal");
            }
            return RedirectToAction("Login", "Login");
        }

        public async Task<IActionResult> Logout(){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
