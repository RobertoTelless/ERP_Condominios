using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public static class CommunicationPackage
    {
        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static Int32 SendEmail(Email email)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                mensagem.To.Add(email.EMAIL_DESTINO);
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                smtp.Send(mensagem);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static Int32 SendEmailCollection(Email email, MailAddressCollection col)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                foreach (var item in col)
                {
                    mensagem.To.Add(item);
                }
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                smtp.Send(mensagem);
                return 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Sends the email assync.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static async Task SendEmailAssync(Email email)
        {
            try
            {
                MailMessage mensagem = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                mensagem.From = new MailAddress(email.EMAIL_EMISSOR, email.NOME_EMISSOR);
                mensagem.To.Add(email.EMAIL_DESTINO);
                mensagem.Subject = email.ASSUNTO;
                mensagem.IsBodyHtml = true;
                mensagem.Body = email.CORPO;
                mensagem.Priority = email.PRIORIDADE;
                mensagem.IsBodyHtml = true;
                if (email.ATTACHMENT != null)
                {
                    foreach (var attachment in email.ATTACHMENT)
                    {
                        mensagem.Attachments.Add(attachment);
                    }
                }
                smtp.EnableSsl = email.ENABLE_SSL;
                smtp.Port = Convert.ToInt32(email.PORTA);
                smtp.Host = email.SMTP;
                smtp.UseDefaultCredentials = email.DEFAULT_CREDENTIALS;
                smtp.Credentials = new System.Net.NetworkCredential(email.EMAIL_EMISSOR, email.SENHA_EMISSOR);
                await smtp.SendMailAsync(mensagem);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
