using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using DinkToPdf;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace HDUA.Controllers
{
    [Authorize]

    public class PrincipalController : Controller{
        Procesos procesos = new Procesos();
        ConexionMongo cnm = new ConexionMongo();

        private readonly ICompositeViewEngine _viewEngine;
        public PrincipalController(ICompositeViewEngine viewEngine)
        {
            _viewEngine = viewEngine;
        }

        public IActionResult Principal()
        {
            return View();
        }
        public IActionResult Avanzada() {
            ViewBag.lorden = procesos.Listar("LISTARORDEN");
            ViewBag.lclase = procesos.Listar("LISTARCLASE");
            ViewBag.lespecie = procesos.Listar("LISTARESPECIE");
            ViewBag.lfamilia = procesos.Listar("LISTARFAMILIA");
            ViewBag.lgenero = procesos.Listar("LISTARGENERO");
            ViewBag.ldepartamento = procesos.Listar("LISTARDEPARTAMENTO");
            ViewBag.ldivision = procesos.Listar("LISTARDIVISION");
            ViewBag.lmun = procesos.Listar("LISTARMUNICIPIO");
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerMunicipiosPorDepartamento(string nombreDepartamento)
        {
            var municipios = procesos.ObtenerMunicipiosPorDepartamento(nombreDepartamento);
            return Json(municipios);
        }

        public IActionResult Resultado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(string nombre, string sorden, string sclase, string sespecie, string sfamilia, string sgenero, string sdepa, string sdivision, string smuni)
        {
            Console.WriteLine("sdepa: " + sdepa);
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
            ViewBag.ldiv = procesos.Listar("LISTARDIVISION");
            ViewBag.lpro = procesos.Listar("LISTARPROYECTO");
            ViewBag.lcr = procesos.Listar("LISTARCONDICION");
            ViewBag.lor = procesos.Listar("LISTARORIGEN");
            ViewBag.lhab = procesos.Listar("LISTARHABITAD");
            ViewBag.lobl = procesos.Listar("LISTAROBSERVACION");

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
                listamuestra = procesos.ResultadoEspecifico(sorden, sclase, sespecie, sfamilia, sgenero, sdepa, sdivision, smuni);
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

        public IActionResult DescargarFichaPdf(int id)
        {
            MuestraModel muestra = procesos.FichaMuestra(id);

            var htmlContent = RenderViewToString("FichaMuestraPDF", muestra);

            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            ColorMode = ColorMode.Color,
            Orientation = Orientation.Portrait,
            PaperSize = PaperKind.A4,
        },
                Objects = {
            new ObjectSettings() {
                PagesCount = true,
                HtmlContent = htmlContent,
                WebSettings = { DefaultEncoding = "utf-8" }
            }
        }
            };

            var pdf = converter.Convert(doc);

            // Retorna el PDF generado como descarga
            return File(pdf, "application/pdf", $"FichaMuestra_{id}.pdf");
        }

        // Método para convertir la vista en cadena HTML
        public string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"No se pudo encontrar la vista {viewName}");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
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

        private byte[] ConvertToWebP(IFormFile file, int quality = 75)
        {
            using (var image = Image.Load(file.OpenReadStream()))
            {
                var encoder = new WebpEncoder { Quality = quality };

                using (var outputStream = new MemoryStream())
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(800, 600)
                    }));

                    image.Save(outputStream, encoder);
                    return outputStream.ToArray();
                }
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
            string division = Request.Form["division"] + "";
            string proyecto = Request.Form["proyecto"] + "";
            string adultos = Request.Form["adultos"] + "";
            string jovenes = Request.Form["jovenes"] + "";
            string condicion = Request.Form["condicion"] + "";
            string origen = Request.Form["origen"] + "";
            string elevacionmin = Request.Form["elevacionmin"] + "";
            string elevacionmax = Request.Form["elevacionmax"] + "";
            string habitad = Request.Form["habitad"] + "";
            string observacion = Request.Form["observacion"] + "";
            string catalogo = Request.Form["catalogo"] + "";
            string registro = Request.Form["registro"] + "";
            string autor = Request.Form["autor"] + "";
            if (a.File != null)
            {
                ImagenModel fotico = new ImagenModel();
                fotico.Nombre = cientifico;

                byte[] webpBytes = ConvertToWebP(a.File);

                fotico.Imagen = Convert.ToBase64String(webpBytes);

                cnm.UpdateImage(fotico, imagen);
            }


            List<int> usuariosSeleccionados = Request.Form["usuariosSeleccionados"].Select(int.Parse).ToList();
            int cantidadRecolectores = usuariosSeleccionados.Count;

            string Ids = string.Join(",", usuariosSeleccionados);

            procesos.EditarMuestra(id, cientifico, vulgar, imagen, coordenada, fecha, altura, clase, orden, familia,
                genero, especie, ubicacion, procedencia, venacion, forma, margen, estado, cantidadRecolectores, Ids,
                division, proyecto, adultos, jovenes, condicion, origen, elevacionmin, elevacionmax, habitad, observacion,
                catalogo, registro, autor);
            return RedirectToAction("Principal", "Principal");
        }

    }
}
