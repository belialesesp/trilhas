using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Trilhas.Data;
using Trilhas.Data.Model.Notifications;

namespace Trilhas.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public NotificationService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Send email alert about upcoming courses needing staff
        /// </summary>
        public void EnviarEmailAlertaContratacao(string emailDestino, string nomeUsuario, List<AlertaContratacao> alertas)
        {
            try
            {
                var smtpConfig = _configuration.GetSection("SmtpSettings");
                var remetente = smtpConfig["From"];
                var host = smtpConfig["Host"];
                var port = int.Parse(smtpConfig["Port"]);
                var usuario = smtpConfig["Username"];
                var senha = smtpConfig["Password"];
                var useSsl = bool.Parse(smtpConfig["UseSsl"] ?? "true");

                using (var client = new SmtpClient(host, port))
                {
                    client.EnableSsl = useSsl;
                    client.Credentials = new NetworkCredential(usuario, senha);

                    var message = new MailMessage
                    {
                        From = new MailAddress(remetente),
                        Subject = "⚠️ ALERTA: Cursos necessitam contratação urgente",
                        IsBodyHtml = true
                    };

                    message.To.Add(emailDestino);
                    message.Body = GerarCorpoEmailAlerta(nomeUsuario, alertas);

                    client.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw - notification failures shouldn't break the system
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
            }
        }

        /// <summary>
        /// Generate HTML email body for hiring alerts
        /// </summary>
        private string GerarCorpoEmailAlerta(string nomeUsuario, List<AlertaContratacao> alertas)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }");
            sb.AppendLine(".container { max-width: 800px; margin: 0 auto; padding: 20px; }");
            sb.AppendLine(".header { background-color: #dc3545; color: white; padding: 20px; border-radius: 5px; }");
            sb.AppendLine(".alert-box { background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 10px 0; }");
            sb.AppendLine(".curso-item { margin: 15px 0; padding: 15px; background-color: #f8f9fa; border-radius: 5px; }");
            sb.AppendLine(".urgente { color: #dc3545; font-weight: bold; }");
            sb.AppendLine(".info { color: #6c757d; font-size: 0.9em; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='container'>");
            
            sb.AppendLine("<div class='header'>");
            sb.AppendLine("<h2>⚠️ Alerta de Contratações Pendentes</h2>");
            sb.AppendLine("</div>");

            sb.AppendLine($"<p>Olá, <strong>{nomeUsuario}</strong>,</p>");
            sb.AppendLine($"<p>Existem <strong>{alertas.Count} curso(s)</strong> que iniciam em até 15 dias e ainda possuem vagas de profissionais não preenchidas:</p>");

            foreach (var alerta in alertas.OrderBy(a => a.DiasRestantes))
            {
                sb.AppendLine("<div class='curso-item'>");
                
                if (alerta.DiasRestantes <= 5)
                {
                    sb.AppendLine("<p class='urgente'>⚠️ URGENTE - Inicia em " + alerta.DiasRestantes + " dia(s)!</p>");
                }
                else
                {
                    sb.AppendLine("<p><strong>Inicia em " + alerta.DiasRestantes + " dia(s)</strong></p>");
                }

                sb.AppendLine("<p><strong>Curso:</strong> " + alerta.Curso + "</p>");
                sb.AppendLine("<p><strong>Demandante:</strong> " + alerta.Demandante + "</p>");
                sb.AppendLine("<p><strong>Categoria:</strong> " + alerta.Categoria + "</p>");
                sb.AppendLine("<p><strong>Vagas restantes:</strong> " + alerta.VagasRestantes + "</p>");
                sb.AppendLine("<p><strong>Data do curso:</strong> " + alerta.DataOferta.ToString("dd/MM/yyyy") + "</p>");
                
                sb.AppendLine("</div>");
            }

            sb.AppendLine("<div class='alert-box'>");
            sb.AppendLine("<p><strong>⚡ Ação necessária:</strong></p>");
            sb.AppendLine("<p>Por favor, acesse o sistema e tome as providências necessárias o quanto antes para garantir que os cursos tenham o quadro de profissionais completo.</p>");
            sb.AppendLine("</div>");

            sb.AppendLine("<p class='info'>Este é um email automático do Sistema de Gestão de Trilhas - CBMES.</p>");
            sb.AppendLine("<p class='info'>Data/hora: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + "</p>");

            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        /// <summary>
        /// Create in-app notification for users
        /// </summary>
        public void CriarNotificacaoInterna(string userId, List<AlertaContratacao> alertas)
        {
            try
            {
                var mensagem = $"{alertas.Count} curso(s) iniciam em até 15 dias e ainda precisam de contratações.";
                var dados = System.Text.Json.JsonSerializer.Serialize(alertas);

                var notificacao = new Notification
                {
                    UserId = userId,
                    Title = "⚠️ Contratações Pendentes",
                    Message = mensagem,
                    Type = "ALERT",
                    Data = dados,
                    Read = false,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notificacao);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar notificação interna: {ex.Message}");
            }
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        public void MarcarComoLida(long notificationId)
        {
            var notification = _context.Notifications.Find(notificationId);
            if (notification != null)
            {
                notification.Read = true;
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Get unread notifications for a user
        /// </summary>
        public List<Notification> ObterNotificacoesNaoLidas(string userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId && !n.Read)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }
    }
}