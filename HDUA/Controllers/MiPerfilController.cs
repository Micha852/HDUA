using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HDUA.Helpers;

namespace HDUA.Controllers
{
    [Authorize]
    public class MiPerfilController : Controller
    {
        Procesos procesos = new Procesos();
        public IActionResult MiPerfil() 
        {
            ViewBag.lgu = procesos.Listar("LISTARGENEROUSUARIO");
            ViewBag.li = procesos.Listar("LISTARINSTITUCION");
            ViewBag.ltu = procesos.Listar("LISTARTIPOUSUARIO");
            UsuarioModel model = new UsuarioModel();
            var claimsPrincipal = User;
            int traerID = Convert.ToInt32(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "id").Value);

            model = procesos.DatosMiPerfil(traerID);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditarMiPerfil()
        {
            int id = Convert.ToInt32(Request.Form["input-invisible"] + "");
            string namee = Request.Form["inputNombreUsuario"] + "";
            string correoo = Request.Form["inputCorreo"] + "";
            string contraa = Request.Form["inputContrasenia"] + "";
            string generouu = Request.Form["selectGenero"] + "";
            string tipouu = Request.Form["selectTipoUsuario"] + "";
            string instuu = Request.Form["selectInstitucion"] + "";

            procesos.EditarMiPerfil(id, namee, correoo, contraa, generouu, tipouu, instuu);

            return RedirectToAction("MiPerfil");
        }

        [HttpPost]
        public JsonResult VerificarContrasenia(string contrasenia)
        {
            int userId = Convert.ToInt32(User.FindFirst("id")?.Value);
            UsuarioModel usuario = procesos.ValidarUsuarioPorId(userId);

            if (usuario != null)
            {
                bool isPasswordValid = PasswordHelper.VerifyPassword(contrasenia, usuario.Contrasenia);

                return Json(new { success = isPasswordValid });
            }

            return Json(new { success = false });
        }



        [HttpPost]
        public IActionResult DesactivarPerfil(int id)
        {
            try
            {
                procesos.DesactivarPerfil(id);
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Json(new { success = true, message = "Perfil desactivado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}