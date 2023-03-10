using Domain.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCase.Clientes.Command
{
    public  class ClienteCommandUseCase : IClienteCommandUseCase
    {
        public async Task EnviarNotificacionPorEmail(ClienteRequest cliente)
        {
            string fromMail = "cuentapruebasenterprise@gmail.com";
            string fromPassword = "kpzb ncmt djev jqpw";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = cliente.Nombres + " " + cliente.Apellidos;
            message.To.Add(new MailAddress(cliente.CorreoElectronico));
            message.Body = $"<html><body>{cliente.Nombres}, tu usuario ha sido creado </body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.SendAsync(message, "d");   
        }
    }
}
