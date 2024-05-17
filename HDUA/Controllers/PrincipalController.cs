using HDUA.DATA;
using HDUA.Models;
using Microsoft.AspNetCore.Mvc;

namespace HDUA.Controllers
{
    public class PrincipalController : Controller
    {
        Procesos procesos = new Procesos();
        public IActionResult Principal()
        {
            return View();
        }
        public IActionResult Avanzada()
        {
            return View();
        }
        public IActionResult Resultado()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Resultado(string nombre)
        {
            List<MuestraModel> listamuestra = new List<MuestraModel>();
            if (Request.Form["btnCientifico"].Count > 0)
            {
                listamuestra = procesos.ResultadoCientifico(nombre);
            }
            else if (Request.Form["btnVulgar"].Count > 0)
            {
                listamuestra = procesos.ResultadoVulgar(nombre);
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

            return View(muestra);
        }
    }
}
