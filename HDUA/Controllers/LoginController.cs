using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using PROYECTO.SERVICIOS;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using HDUA.Helpers;

namespace HDUA.Controllers
{
    public class LoginController : Controller
    {

        Procesos procesos = new Procesos();

        public IActionResult Login(){
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

        public IActionResult LoginChaira()
        {
            string clientId = "wewDSDfgqwGhhCCBbgDefeqWWerHaA";
            string redirectUri = Url.Action("ChairaCallback", "Login", null, Request.Scheme);
            string state = "ok";

            string chairaUrl = $"https://chaira.uniamazonia.edu.co/ChairaApi/oauth2/auth?" +
                               $"response_type=code&client_id={clientId}&redirect_uri={redirectUri}&state={state}";

            return Redirect(chairaUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ChairaCallback(string state, string token_type, string code)
        {
            if (state != "ok" || string.IsNullOrEmpty(code))
            {
                TempData["ErrorMessage"] = "Error en la autenticación con Chaira.";
                return RedirectToAction("Login");
            }

            // Contamos los usuarios registrados como "Visitante Chairá"
            int contadorUsuarios = procesos.ContarUsuariosChairas() + 1;
            string nombreUsuario = $"Visitante Chairá {contadorUsuarios}";

            // Verificamos si ya existe el usuario en la base de datos
            var usuario = procesos.ValidarUsuario(nombreUsuario);

            if (usuario == null) // Si el usuario no existe, lo creamos
            {
                UsuarioModel nuevoUsuario = new UsuarioModel
                {
                    Nombre = nombreUsuario,
                    Correo = null,  // No tenemos correo
                    Contrasenia = null,  // Sin contraseña
                    Genero = null,
                    Tipo = null,
                    Institucion = "Universidad de la Amazonia"
                };

                procesos.RegistrarUsuario(nuevoUsuario);
                usuario = procesos.ValidarUsuario(nombreUsuario);
            }

            if (usuario != null && usuario.Estado)
            {
                // Crear los claims para la sesión
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim("id", usuario.Id.ToString()),
            new Claim(ClaimTypes.Role, usuario.Rol),
            new Claim("Email", "usuario@chaira.com") // Valor simbólico
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Principal", "Principal");
            }

            TempData["ErrorMessage"] = "Usuario no registrado o inhabilitado.";
            return RedirectToAction("Login");
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
            Login(usuario.Correo, usuario.Contrasenia);

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
        public JsonResult RecuperarPword(string correo)
        {
            string contra = GenerateRandomSequence(); // Generamos la nueva contraseña
            bool aux = procesos.BUSCARCOREO(correo, contra); // Se guarda en la DB encriptada

            if (aux)
            {
                ServicioGmail enviar = new ServicioGmail();
                enviar.SendEmailGmail(
                    correo,
                    "Recuperación de contraseña - Herbario HUAZ",
                    "Esta es su nueva contraseña: " + contra +
                    ". Se recomienda usar esta contraseña para ingresar a su perfil y cambiarla desde ahí. " +
                    "Tome el valor '" + contra + "' como una contraseña provisional."
                );

                return Json(new { success = true, message = "Se envió la nueva contraseña al correo " + correo });
            }
            else
            {
                return Json(new { success = false, message = "No se encontró el correo proporcionado." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Login(string NOMBRECORREO, string PWORD)
        {
            UsuarioModel usuario = procesos.ValidarUsuario(NOMBRECORREO);

            if (usuario != null)
            {
                // Verificar si la contraseña es válida
                bool isPasswordValid = PasswordHelper.VerifyPassword(PWORD, usuario.Contrasenia);

                if (isPasswordValid)
                {
                    if (usuario.Estado)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim("id", usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol),
                    new Claim("Email", NOMBRECORREO)
                };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                        return RedirectToAction("Principal", "Principal");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "El usuario está inhabilitado.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "La contraseña es incorrecta.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "El usuario no existe.";
            }

            return RedirectToAction("Login", "Login");
        }



        public async Task<IActionResult> Logout(){
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
