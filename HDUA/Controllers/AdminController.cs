using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;


namespace HDUA.Controllers
{

    public class AdminController : Controller
    {
        private readonly IConverter _converter;

        public AdminController(IConverter converter)
        {
            _converter = converter;
        }
        Procesos procesos = new Procesos();
        ConexionMongo cnm = new ConexionMongo();
        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult Principal()
        {
            return View();
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult GestionMuestra()
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

            return View();
        }

        [HttpPost]
        public ActionResult ObtenerMunicipiosPorDepartamento(string nombreDepartamento)
        {
            var municipios = procesos.ObtenerMunicipiosPorDepartamento(nombreDepartamento);
            return Json(municipios);
        }

        [HttpPost]
        public ActionResult ObtenerTiposUbicacionPorMunicipio(string nombreMunicipio)
        {
            var tiposUbicacion = procesos.ObtenerTiposUbicacionPorMunicipio(nombreMunicipio);
            return Json(tiposUbicacion);
        }

        [HttpPost]
        public ActionResult ObtenerUbicacionesPorTipoYMunicipio(string nombreTipo, string nombreMunicipio)
        {
            var ubicaciones = procesos.ObtenerUbicacionesPorTipoYMunicipio(nombreTipo, nombreMunicipio);
            return Json(ubicaciones);
        }

        [HttpPost]
        public IActionResult InsertarImagen(MuestraModel a)
        {
            ImagenModel imagen = new ImagenModel();
            imagen.Nombre = a.Cientifico;
            string n = "";
            byte[] bytes;
            if (a.File != null)
            {
                using (Stream fs = a.File.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                        imagen.Imagen = Convert.ToBase64String(bytes, 0, bytes.Length);
                        n = cnm.UploadImage(imagen);
                    }
                }
            }
            return RedirectToAction("GestionMuestra", "Admin");
        }

        [HttpPost]
        public ActionResult CrearMuestra(MuestraModel a)
        {
            MuestraModel muestra = new MuestraModel();
            muestra.Cientifico = Request.Form["cientifico"];
            muestra.Vulgar = Request.Form["vulgar"];
            muestra.Fecha = Request.Form["fecharecolecccion"];
            muestra.Venacion = Request.Form["venacion"];
            muestra.Forma = Request.Form["forma"];
            muestra.Margen = Request.Form["margen"];
            muestra.Procedencia = Request.Form["procedencia"];
            muestra.Altura = Request.Form["altura"];
            muestra.Clase = Request.Form["clase"];
            muestra.Especie = Request.Form["especie"];
            muestra.Genero = Request.Form["genero"];
            muestra.Orden = Request.Form["orden"];
            muestra.Familia = Request.Form["familia"];
            muestra.Orden = Request.Form["orden"];
            muestra.Ubicacion = Request.Form["ubicacionSelect"];
            muestra.Coordenada = Request.Form["coordenadasInput"];
            muestra.File = a.File;

            ImagenModel imagen = new ImagenModel();
            imagen.Nombre = muestra.Cientifico;
            string n = "";
            byte[] bytes;
            if (a.File != null)
            {
                using (Stream fs = a.File.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                        imagen.Imagen = Convert.ToBase64String(bytes, 0, bytes.Length);
                        n = cnm.UploadImage(imagen);
                    }
                }
            }
            muestra.Imagen = n;

            List<int> usuariosSeleccionados = Request.Form["usuariosSeleccionados"].Select(int.Parse).ToList();
            int cantidadRecolectores = usuariosSeleccionados.Count;
            muestra.Recolectores = cantidadRecolectores;

            muestra.Ids = string.Join(",", usuariosSeleccionados);

            procesos.CrearMuestra(muestra);

            return RedirectToAction("Principal", "Principal");
        }

        [HttpPost]
        public ActionResult CrearUbicacion() {
            UbicacionModel ubi = new UbicacionModel();
            ubi.Nombre = Request.Form["inputUbicacion"];
            ubi.Tipoubi = Request.Form["selectTipoUbicacion"];
            ubi.Municipio = Request.Form["municipioSelect2"];
            procesos.CrearUbicacion(ubi);
            return RedirectToAction("GestionMuestra", "Admin");
        }

        [HttpPost]
        public ActionResult CrearParameros() {
            procesos.CrearParametros(Request.Form["selectCaracteristica"], Request.Form["inputCaracteristica"]);
            return RedirectToAction("GestionMuestra", "Admin");
        }

        public IActionResult GestionUsuario()
        {
            ViewBag.mostrarusuarios = procesos.ListarUsuarios();
            return View();
        }

        [HttpPost]
        public ActionResult EditarUser()
        {
            bool rec;
            if (Request.Form["selectRecolector"] == "Si") {
                rec = true;
            } else {
                rec = false;
            }

            bool est;
            if (Request.Form["selectEstado"] == "Activo") {
                est = true;
            } else {
                est = false;
            }

            procesos.EditarUser(Convert.ToInt32(Request.Form["input-invisible"] + ""), (Request.Form["selectRol"]) + "", rec, est);

            return RedirectToAction("GestionUsuario");
        }

        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult GenerarInformes()
        {
            return View();
        }

        public IActionResult VistaParaPDF1()
        {
            ViewBag.informe = procesos.InformeMuestrasRecolector1();
            return View();
        }

        public IActionResult DescargarPDF1()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Admin/VistaParaPDF1";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        Page=url_pagina
                    }
                }
            };
            var archivoPDF = _converter.Convert(pdf);

            MemoryStream pdfStream = new MemoryStream(archivoPDF);
            pdfStream.Position = 0;

            string nombrePDF = "reporte_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";


            return File(archivoPDF, "application/pdf", nombrePDF);
        }

        public IActionResult DescargarEXCEL1(){
            List<MuestraPorRecolector> informe1 = procesos.InformeMuestrasRecolector1();

            using (var workbook = new XLWorkbook())
            {
                foreach (var recolector in informe1)
                {
                    var worksheet = workbook.Worksheets.Add(recolector.nombre);

                    worksheet.Cell(1, 1).Value = "Recolector";
                    worksheet.Cell(1, 2).Value = recolector.nombre;

                    worksheet.Cell(2, 1).Value = "Nombre de las muestras";
                    worksheet.Cell(2, 2).Value = "Fecha de asignación";

                    for (int i = 0; i < recolector.datos.Count; i++)
                    {
                        worksheet.Cell(i + 3, 1).Value = recolector.datos[i].Item1;
                        worksheet.Cell(i + 3, 2).Value = recolector.datos[i].Item2;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InformeMuestrasRecolector.xlsx");
                }
            }
        }

    }
}
