﻿using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public IActionResult DesactivarPerfil(int id)
        {
            try
            {
                procesos.DesactivarPerfil(id);
                // Cerrar sesión después de desactivar el perfil
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Json(new { success = true, message = "Perfil desactivado correctamente." });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
