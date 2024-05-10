using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;


namespace HDUA.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class AdminController : Controller
    {
        Procesos procesos = new Procesos();
        ConexionMongo cnm = new ConexionMongo();
        public IActionResult Principal()
        {
            return View();
        }

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
            if (a.File!=null)
            {
                using(Stream fs = a.File.OpenReadStream())
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                        imagen.Imagen = Convert.ToBase64String(bytes, 0, bytes.Length);
                        n = cnm.UploadImage(imagen);
                    }
                }
            }
            return RedirectToAction("GestionMuestra","Admin");
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

        public IActionResult GestionUsuario()
        {
            ViewBag.mostrarusuarios = procesos.ListarUsuarios();
            return View();
        }

        [HttpPost]
        public ActionResult EditarUser()
        {
            bool rec;
            if (Request.Form["selectRecolector"] == "Si"){
                rec = true;
            }else{
                rec = false;
            }

            bool est;
            if (Request.Form["selectEstado"] == "Activo"){
                est = true;
            }else{
                est = false;
            }

                procesos.EditarUser(Convert.ToInt32(Request.Form["input-invisible"] + ""), (Request.Form["selectRol"])+"", rec, est);

            return RedirectToAction("GestionUsuario");
        }

    }
}
