using ReStore.Application.Interfaces;
using ReStore.Application.Models;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using ReStore.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using ReStore.Domain.Utils;

namespace ReStore.Application.Implementations
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        private readonly ILogger<MailService> _logger;

        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(Order order, User user)
        {
            try
            {
                MailMessage message = new();

                SmtpClient smtp = new();

                EmailSettings mailSettings = new()
                {
                    DisplayName = _configuration["EmailSettings:DisplayName"],
                    Host = _configuration["EmailSettings:Host"],
                    Mail = _configuration["EmailSettings:Mail"],
                    Password = _configuration["EmailSettings:Password"],
                    Port = int.Parse(_configuration["EmailSettings:Port"])
                };

                message.From = new MailAddress(mailSettings.Mail, mailSettings.DisplayName);
                message.To.Add(new MailAddress(user.Email));
                message.Subject = "ReStore API - Order received!";

                message.IsBodyHtml = true;
                message.Body = EmailTemplates.OrderReceived(order,user);
                smtp.Port = mailSettings.Port;
                smtp.Host = mailSettings.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(mailSettings.Mail, mailSettings.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
