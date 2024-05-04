using HDUA.DATA;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.Json;


namespace HDUA.Controllers
{
    public class AdminController : Controller
    {
        Procesos procesos = new Procesos();
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

            return View();
        }

        public IActionResult GestionUsuario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerMunicipiosPorDepartamento(string nombreDepartamento)
        {
            var municipios = procesos.ObtenerMunicipiosPorDepartamento(nombreDepartamento);
            return Json(municipios);
        }


    }
}
