using Microsoft.AspNetCore.Mvc;
using PROYECTO.SERVICIOS;

namespace HDUA.Controllers
{
    public class CorreoController : Controller
    {
        [HttpPost]
        public JsonResult EnviarComentario(string mensaje)
        {
            if (!string.IsNullOrEmpty(mensaje))
            {
                string userEmail = User.FindFirst("Email")?.Value;
                ServicioGmail enviar = new ServicioGmail();
                string receptor = "herbario@uniamazonia.edu.co";
                string asunto = "Nuevo Comentario/Sugerencia en HDUA";
                string cuerpo = $"Se ha recibido un nuevo comentario o sugerencia por parte de {userEmail}:<br><br>{mensaje}";

                try
                {
                    enviar.SendEmailGmail(receptor, asunto, cuerpo);
                    return Json(new { success = true, message = "Comentario enviado exitosamente." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al enviar el comentario: " + ex.Message });
                }
            }
            else
            {
                return Json(new { success = false, message = "El comentario no puede estar vacío." });
            }
        }
    }
}
