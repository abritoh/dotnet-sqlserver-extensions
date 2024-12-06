using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace ClusterBRSqlServerObjects
{
    /// <summary>
    /// EmailHelper
    /// </summary>
    /// <date>2018-02-24</date>    
    /// <author>ArBR | arcbrth@gmail</author>
    internal static class EmailHelper
    {

        /// <summary>
        /// Factory method to create and configure an instance of <c>SmtpClient</c>.
        /// This method provides flexibility for handling multiple email sending configurations 
        /// by selecting the appropriate <c>SmtpClient</c> setup based on the provided identifier.
        /// </summary>
        /// <param name="smtpClient">
        /// The ID representing the specific <c>SmtpClient</c> configuration to use. 
        /// Valid values are defined in <c>Constantes</c>, such as:
        /// <list type="bullet">
        /// <item><description><c>Constantes.SMTP_GMAIL</c> - Gmail SMTP server</description></item>
        /// <item><description><c>Constantes.SMTP_SECURE_SERVER</c> - Secure server configuration</description></item>
        /// <item><description><c>Constantes.SMTP_EXCHANGE</c> - Microsoft Exchange SMTP server</description></item>
        /// <item><description><c>Constantes.SMTP_COMPANY</c> - Custom company SMTP server</description></item>
        /// </list>
        /// </param>
        /// <param name="message">
        /// The email message to be sent using the configured <c>SmtpClient</c> instance.
        /// </param>
        /// <param name="from">
        /// The email address of the sender (outgoing email address).
        /// </param>
        /// <param name="psw">
        /// The password for the sender's email account.
        /// </param>
        /// <returns>
        /// An instance of <c>SmtpClient</c> pre-configured and ready to send the specified email.
        /// Returns <c>null</c> if no matching configuration is found.
        /// </returns>
        /// <date>2018-02-24</date>    
        /// <author>ArBR | arcbrth@gmail</author>
        internal static SmtpClient GetSmtpClientById(int smtpClient, MailMessage message, string from, string psw)
        {
            SmtpClient result = null;

            switch (smtpClient)
            {
                case Constants.SMTP_GMAIL:
                    result = GetSmtpGmail(message, from, psw);
                    break;

                case Constants.SMTP_SECURE_SERVER:
                    result = GetSmtpSecureServer(message, from, psw);
                    break;

                case Constants.SMTP_EXCHANGE:
                    result = GetSmtpServer1(message, from, psw);
                    break;

                case Constants.SMTP_COMPANY:
                    result = GetSmtpServer2(message, from, psw);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Configures and returns an instance of <c>SmtpClient</c> for sending emails via Gmail's SMTP server.
        /// This method sets up the client with the necessary credentials, host, port, and SSL settings.
        /// </summary>
        /// <param name="message">
        /// The email message to be sent using the configured <c>SmtpClient</c> instance.
        /// </param>
        /// <param name="from">
        /// The email address of the sender (outgoing email address).
        /// </param>
        /// <param name="psw">
        /// The password for the sender's email account.
        /// </param>
        /// <returns>
        /// An instance of <c>SmtpClient</c> configured for Gmail's SMTP server, ready for sending emails.
        /// </returns>
        /// <date>2018-02-24</date>    
        /// <author>ArBR | arcbrth@gmail</author>
        internal static SmtpClient GetSmtpGmail(MailMessage message, string from, string psw)
        {
            // Create a new instance of SmtpClient.
            SmtpClient result = new SmtpClient();
            result.Credentials = new NetworkCredential(from, psw);
            result.Port = 587;
            result.Host = "smtp.gmail.com";
            result.EnableSsl = true;
            return result;
        }

        internal static SmtpClient GetSmtpSecureServer(MailMessage message, string from, string psw)
        {
            SmtpClient result = new SmtpClient();
            result.Credentials = new NetworkCredential(from, psw);
            result.Port = 25;
            result.Host = "relay-hosting.secureserver.net";
            result.DeliveryMethod = SmtpDeliveryMethod.Network;
            return result;
        }

        internal static SmtpClient GetSmtpServer1(MailMessage message, string from, string psw)
        {
            SmtpClient result = new SmtpClient();
            result.Credentials = new NetworkCredential(from, psw);
            result.Port = 25;
            result.Host = "mail.server1.org.mx";
            result.DeliveryMethod = SmtpDeliveryMethod.Network;
            return result;
        }

        internal static SmtpClient GetSmtpServer2(MailMessage message, string from, string psw)
        {
            SmtpClient result = new SmtpClient();
            result.Credentials = new NetworkCredential(from, psw);
            result.Port = 25;
            result.Host = "mail.server2.com";
            result.DeliveryMethod = SmtpDeliveryMethod.Network;
            return result;
        }

        internal static SmtpClient GetSmtpGeneral
            (MailMessage message, string from, string psw, string host, int port, int enableSSL, int useDefCredential)
        {
            SmtpClient result = new SmtpClient();

            result.Host = host;
            result.Port = port;            
            result.DeliveryMethod = SmtpDeliveryMethod.Network;
            result.EnableSsl = (enableSSL > 0);
            result.Timeout = 300000; /* 5 minutos = 300000 ms */

            if (useDefCredential > 0)
            {
                result.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                result.UseDefaultCredentials = true;
            }
            else {
                result.Credentials = new NetworkCredential(from, psw);
            }

            return result;
        }

        /// <summary>
        /// Configures the recipient list for a <c>MailMessage</c> instance by parsing a 
        /// comma-or-semicolon separated string of email addresses.
        /// Adds the first address as the primary recipient and the rest as BCC addresses,
        /// ensuring no duplicate entries.
        /// </summary>
        /// <param name="message">
        /// A reference to the <c>MailMessage</c> object to configure.
        /// </param>
        /// <param name="to">
        /// A string containing a list of email addresses separated by commas or semicolons.
        /// </param>
        /// <date>2018-02-24</date>    
        /// <author>ArBR | arcbrth@gmail</author>
        internal static void ConfigureToEmailList(ref MailMessage message, string to)
        {
            string item = null;
            List<string> list = new List<string>();
            string[] toArray = to.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (toArray.Length == 1)
            {
                message.To.Add (new MailAddress(toArray[0]));
            }
            else if (toArray.Length > 1)
            {
                for (int index = 0; index < toArray.Length; index++)
                {
                    if (!string.IsNullOrEmpty(toArray[index]))
                    {
                        item = toArray[index];
                        
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                            message.Bcc.Add(new MailAddress(item));
                        }
                    }
                }
            }
        }

    }
}
