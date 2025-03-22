using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Diagnostics;
using System.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using System.IO;


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

        [Authorize(Roles = "ADMINISTRADOR,COLABORADOR")]
        public IActionResult Principal()
        {
            return View();
        }

        [Authorize(Roles = "ADMINISTRADOR, COLABORADOR")]
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
            ViewBag.ltd = procesos.Listar("LISTARDIVISION");
            ViewBag.ltpro = procesos.Listar("LISTARPROYECTO");
            ViewBag.lcr = procesos.Listar("LISTARCONDICION");
            ViewBag.lor = procesos.Listar("LISTARORIGEN");
            ViewBag.lhab = procesos.Listar("LISTARHABITAD");
            ViewBag.lobl = procesos.Listar("LISTAROBSERVACION");
            ViewBag.lprot = procesos.Listar("LISTARPROTOCOLO");

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
        public IActionResult InsertarImagen(MuestraModel a)
        {
            ImagenModel imagen = new ImagenModel();
            imagen.Nombre = a.Cientifico;
            string n = "";

            if (a.File != null)
            {
                byte[] webpBytes = ConvertToWebP(a.File);

                imagen.Imagen = Convert.ToBase64String(webpBytes);
                n = cnm.UploadImage(imagen);
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
            muestra.Ubicacion = Request.Form["ubicacionSelect"];
            muestra.Coordenada = Request.Form["coordenadasInput"];
            muestra.File = a.File;
            muestra.Division = Request.Form["division"];
            muestra.Proyecto = Request.Form["proyecto"];
            muestra.Adultos = Request.Form["adultos"];
            muestra.Jovenes = Request.Form["jovenes"];
            muestra.Condicion = Request.Form["condicion"];
            muestra.Origen = Request.Form["origen"];
            muestra.Elevacionmin = Request.Form["elevacionmin"];
            muestra.Elevacionmax = Request.Form["elevacionmax"];
            muestra.Habitad = Request.Form["habitad"];
            muestra.ObervacionLocal = Request.Form["observacion"];
            muestra.Catalogo = Request.Form["ncatalogo"];
            muestra.Registro = Request.Form["nregistro"];
            muestra.Autor = Request.Form["autor"];

            ImagenModel imagen = new ImagenModel();
            imagen.Nombre = muestra.Cientifico;
            string n = "";

            if (a.File != null)
            {
                byte[] webpBytes = ConvertToWebP(a.File); // Convertimos a .webp

                imagen.Imagen = Convert.ToBase64String(webpBytes);
                n = cnm.UploadImage(imagen);
            }

            muestra.Imagen = n; // Guardamos la imagen en MongoDB

            List<int> usuariosSeleccionados = Request.Form["usuariosSeleccionados"].Select(int.Parse).ToList();
            int cantidadRecolectores = usuariosSeleccionados.Count;
            muestra.Recolectores = cantidadRecolectores;
            muestra.Ids = string.Join(",", usuariosSeleccionados);

            procesos.CrearMuestra(muestra);

            return RedirectToAction("Principal", "Principal");
        }

        [HttpPost]
        public ActionResult CrearProyecto()
        {
            ProyectoModel a = new ProyectoModel();
            a.Nombre = Request.Form["nombreProyecto"];
            a.Inicio = Request.Form["fechaInicio"];
            a.Fin = Request.Form["fechaFin"];
            a.Esfuerzo = Request.Form["esfuerzoDias"];
            a.Observacion = Request.Form["observaciones"];
            a.Protocolo = Request.Form["selectProtocolo"];
            a.Hinicio = Request.Form["horaInicio"];
            a.Hfin = Request.Form["horaFin"];
            procesos.CrearProyecto(a);

            return RedirectToAction("Principal", "Principal");
        }

        [HttpPost]
        public ActionResult CrearUbicacion() {
            UbicacionModel ubi = new UbicacionModel();
            ubi.Nombre = Request.Form["inputUbicacion"];
            ubi.Tipoubi = Request.Form["selectTipoUbicacion"];
            ubi.Municipio = Request.Form["municipioSelect2"];
            ubi.Cuerpodeagua = Request.Form["selectcuerpodeagua"];
            procesos.CrearUbicacion(ubi);
            return RedirectToAction("GestionMuestra", "Admin");
        }

        [HttpPost]
        public ActionResult CrearParameros() {
            procesos.CrearParametros(Request.Form["selectCaracteristica"], Request.Form["inputCaracteristica"]);
            return RedirectToAction("GestionMuestra", "Admin");
        }

        [Authorize(Roles = "ADMINISTRADOR")]
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

        [Authorize(Roles = "ADMINISTRADOR, COLABORADOR")]
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
                Page = url_pagina
            }
        }
            };

            var archivoPDF = _converter.Convert(pdf);

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

        [HttpPost]
        public IActionResult GenerarInforme(DateTime inicio, DateTime fin)
        {
            // Obtener los datos filtrados por las fechas
            DataTable datos = procesos.ObtenerDatosInforme(inicio, fin);

            // Generar el archivo Excel
            byte[] archivoExcel = procesos.GenerarInformeExcel(datos);

            // Devolver el archivo Excel como un archivo descargable
            return File(archivoExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "InformeDarwinCore.xlsx");
        }

        [HttpPost]
        public IActionResult EditarParametros(string categoria, string opcion, string nuevoValor)
        {
            procesos.EditarParametros(categoria, opcion, nuevoValor);

            return RedirectToAction("GestionMuestra", "Admin");
        }
    }
}
