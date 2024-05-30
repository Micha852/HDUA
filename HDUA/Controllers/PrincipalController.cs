using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;

namespace HDUA.Controllers
{
    [Authorize]

    public class PrincipalController : Controller{
        Procesos procesos = new Procesos();
        ConexionMongo cnm = new ConexionMongo();
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
        public IActionResult Resultado(string nombre, string sorden, string sclase, string sespecie, string sfamilia, string sgenero, string sdepa, int page = 1, int pageSize = 4)
        {
            ViewBag.ltv = procesos.Listar("LISTARTIPOVENACION");
            ViewBag.ltf = procesos.Listar("LISTARTIPOFORMA");
            ViewBag.ltM = procesos.Listar("LISTARTIPOMARGEN");
            ViewBag.ltp = procesos.Listar("LISTARPROCEDENCIA");
            ViewBag.ltc = procesos.Listar("LISTARCLASE");
            ViewBag.lte = procesos.Listar("LISTARESPECIE");
            ViewBag.ltg = procesos.Listar("LISTARGENERO");
            ViewBag.lto = procesos.Listar("LISTARORDEN");
            ViewBag.ltfami = procesos.Listar("LISTARFAMILIA");
            ViewBag.ldep = procesos.Listar("LISTARDEPARTAMENTO");
            ViewBag.lmun = procesos.Listar("LISTARMUNICIPIO");
            ViewBag.ltipoubi = procesos.Listar("LISTARTIPOUBICACION");
            ViewBag.ltipoubi2 = procesos.Listar("LISTARTIPOUBI2");
            ViewBag.mostrarrecolector = procesos.ListarRecolector();

            List<MuestraModel> listamuestra = new List<MuestraModel>();
            if (Request.Form["btnCientifico"].Count > 0)
            {
                listamuestra = procesos.ResultadoCientifico(nombre);
            }
            else if (Request.Form["btnVulgar"].Count > 0)
            {
                listamuestra = procesos.ResultadoVulgar(nombre);
            }
            else if (Request.Form["btnParametros"].Count > 0)
            {
                listamuestra = procesos.ResultadoEspecifico(sorden, sclase, sespecie, sfamilia, sgenero, sdepa);
            }

            int totalItems = listamuestra.Count;
            var muestraPaginada = listamuestra.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(muestraPaginada);
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

        [HttpPost]
        public IActionResult EliminarComentario(int id)
        {
            try
            {
                procesos.EliminarComentario(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult EditarComentario(int id, string newComentario)
        {
            try
            {
                procesos.EditarComentario(id, newComentario);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditarMuestra(MuestraModel a) {
            int id = Convert.ToInt32(Request.Form["idMuestra1"] + "");
            string cientifico = Request.Form["cientifico"] + "";
            string vulgar = Request.Form["vulgar"] + "";
            string imagen = Request.Form["ParaImagen"] + "";
            string coordenada = Request.Form["coordenada"] + "";
            string fecha = Request.Form["fecharecoleccion"] + "";
            string altura = Request.Form["altura"] + "";    
            string clase = Request.Form["clase"] + "";
            string orden = Request.Form["orden"] + "";
            string familia = Request.Form["familia"] + "";
            string genero = Request.Form["genero"] + "";
            string especie = Request.Form["especie"] + "";
            string ubicacion = Request.Form["ubicacionSelect"] + "";
            string procedencia = Request.Form["procedencia"] + "";
            string venacion = Request.Form["venacion"] + "";
            string forma = Request.Form["forma"] + "";
            string margen = Request.Form["margen"] + "";
            int estado = Request.Form.ContainsKey("estadoCheckbox") ? 1 : 0;
            if(a.File != null)
            {
                ImagenModel fotico = new ImagenModel();
                fotico.Nombre = cientifico;
                byte[] bytes;
                using(Stream ps = a.File.OpenReadStream())
                {
                    using(BinaryReader br = new(ps))
                    {
                        bytes = br.ReadBytes((int)ps.Length);
                        fotico.Imagen = Convert.ToBase64String(bytes, 0, bytes.Length);
                        cnm.UpdateImage(fotico, imagen);
                    }
                } 
            }
            procesos.EditarMuestra(id, cientifico, vulgar, imagen, coordenada, fecha, altura, clase, orden, familia, genero, especie, ubicacion, procedencia, venacion, forma, margen, estado);
            return RedirectToAction("Principal", "Principal");
        }

    }
}
