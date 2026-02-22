using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using eVote360.Core.Application.Interface.Common;
using Microsoft.Extensions.Configuration;

namespace eVote360.Infrastructure.Shared.Email
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            var smtpSection = _config.GetSection("Smtp");

            var host = smtpSection["Host"]
                ?? throw new InvalidOperationException("SMTP Host no configurado.");

            var port = smtpSection.GetValue<int>("Port");
            var user = smtpSection["User"]
                ?? throw new InvalidOperationException("SMTP User no configurado.");

            var pass = smtpSection["Pass"]
                ?? throw new InvalidOperationException("SMTP Pass no configurado.");

            var from = smtpSection["From"] ?? user;
            var enableSsl = smtpSection.GetValue<bool>("EnableSsl");

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };

            using var message = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await smtp.SendMailAsync(message, ct);
        }
    }
}
