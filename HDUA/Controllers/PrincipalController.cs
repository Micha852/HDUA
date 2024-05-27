using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HDUA.Controllers
{
    public class PrincipalController : Controller{
        Procesos procesos = new Procesos();
        public IActionResult Principal()
        {
            return View();
        }
        public IActionResult Avanzada(){
            ViewBag.lorden = procesos.Listar("LISTARORDEN");
            ViewBag.lclase = procesos.Listar("LISTARCLASE");
            ViewBag.lespecie = procesos.Listar("LISTARESPECIE");
            ViewBag.lfamilia = procesos.Listar("LISTARFAMILIA");
            ViewBag.lgenero = procesos.Listar("LISTARGENERO");
            ViewBag.ldepartamento = procesos.Listar("LISTARDEPARTAMENTO");
            return View();
        }
        public IActionResult Resultado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(string nombre, string sorden, string sclase, string sespecie, string sfamilia, string sgenero, string sdepa)
        {
            List<MuestraModel> listamuestra = new List<MuestraModel>();
            if (Request.Form["btnCientifico"].Count > 0)
            {
                listamuestra = procesos.ResultadoCientifico(nombre);
            }
            else if (Request.Form["btnVulgar"].Count > 0)
            {
                listamuestra = procesos.ResultadoVulgar(nombre);
            }else if (Request.Form["btnParametros"].Count > 0)
            {
                listamuestra = procesos.ResultadoEspecifico(sorden, sclase, sespecie, sfamilia, sgenero, sdepa);
            }
            return View(listamuestra);
        }


        public IActionResult FichaMuestra() {
            return View();
        }

        [HttpPost]
        public IActionResult FichaMuestra(int id)
        {
            int muestraId = id;

            MuestraModel muestra = procesos.FichaMuestra(muestraId);
            List<ComentarioModel> comentarios = new List<ComentarioModel>();
            comentarios = procesos.ComentariosMuestra(muestraId);
            muestra.comentarios = comentarios;
            return View(muestra);
        }

        [HttpPost]
        public ActionResult CrearComentario(string commentInput, int hiddenInputMuestra)
        {
            ComentarioModel com = new ComentarioModel();
            com.Texto = commentInput;
            com.Muestra = hiddenInputMuestra+"";

            var claimsPrincipal = HttpContext.User.Identity as ClaimsIdentity;
            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                com.Usuario = userId+"";
            }
            else
            {
                return RedirectToAction("Principal", "Principal");
            }
            List<ComentarioModel> lista = procesos.CrearComentario(com);
            return Json(lista);
        }

    }
}
