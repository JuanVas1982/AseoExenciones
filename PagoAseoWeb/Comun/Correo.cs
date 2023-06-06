using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SocialAseoWeb.Comun
{
    class Correo
    {
        MailMessage correos = new MailMessage();
        SmtpClient envios = new SmtpClient();

        public string enviarCorreo(string destinatario, string contenido, string remite, string Adjunto)
        {
            try
            {
                string mensaje = Utils.mensaje;
                string asunto = remite; // Utils.asunto;
                string emisor = Utils.emisor;
                string password = Utils.password;
                correos.To.Clear();
                correos.Body = "";
                correos.Subject = "";
                correos.Body = contenido;
                correos.Subject = asunto;
                correos.IsBodyHtml = true;
                correos.To.Add(destinatario.Trim());

                if (Adjunto.Equals("") == false)
                {
                    System.Net.Mail.Attachment archivo = new System.Net.Mail.Attachment(Adjunto);
                    correos.Attachments.Add(archivo);
                }

                correos.From = new MailAddress(emisor);
                envios.Credentials = new NetworkCredential(emisor, password);

                //Datos importantes no modificables para tener acceso a las cuentas

                //envios.Host = "smtp.gmail.com";
                //envios.Port = 587;
                envios.Host = Utils.Host;
                envios.Port = int.Parse(Utils.Port);
                envios.EnableSsl = bool.Parse(Utils.EnableSsl);

                envios.Send(correos);
                return "OK";
            }
            catch (Exception ex)
            {
                return "NOK";
            }
        }
    }
}