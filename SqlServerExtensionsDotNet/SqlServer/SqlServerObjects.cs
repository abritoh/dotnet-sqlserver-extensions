using Microsoft.SqlServer.Server;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ClusterBRSqlServerObjects
{
    /// <summary>
    /// Provides the implementation of stored procedures for deployment in a SQL Server database. 
    /// The <c>SqlProcedure</c> attribute enables seamless integration of .NET platform languages 
    /// with SQL Server, allowing the addition of advanced custom logic and greatly extending 
    /// the database's native capabilities.
    /// </summary>
    /// <date>2018-02-24</date>
    /// <author>ArBR | arcbrth@gmail</author>  
    public class SP
    {
        #region Public Procedures
        /// <summary>
        /// Sends an email message via custom dotNet SP deployed in SQL-Server database
        /// </summary>
        /// <param name="smtpClient">
        /// An integer specifying the SMTP client for sending the email, which can be one of the following:
        ///     SMTP_GENERAL       = 0;
        ///     SMTP_GMAIL         = 1;
        ///     SMTP_SECURE_SERVER = 2;
        ///     SMTP_EXCHANGE      = 3;
        ///     ...
        /// </param>
        /// <param name="from">
        /// Outgoing email address
        /// </param>
        /// <param name="psw">
        /// Password for the outgoing email account
        /// </param>
        /// <param name="to">
        /// A comma (,) or semicolon (;) separated list of recipient email addresses
        /// </param>
        /// <param name="subject">
        /// Subject of the email
        /// </param>
        /// <param name="body">
        /// Content (message) of the email, which can be in HTML format.
        /// </param>
        /// <param name="outmsg">
        /// String containing the result of the SP execution
        /// </param>
        /// <returns>
        /// 0 if SUCCESS
        /// -1 if FAILURE
        /// </returns>
        /// <date>2018-02-24</date>
        /// <author>ArBR | arcbrth@gmail</author>
        [SqlProcedure]
        public static int SP_System_SendEmail
            (int smtpClient, string host, int port, int enableSSL, int useDefCredential, 
             string from, string psw, string to, string subject, string body, out string outmsg)
        {
            int result = 0;
            try
            {
                MailMessage message = new MailMessage();

                message.From = new MailAddress(from);
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                EmailHelper.ConfigureToEmailList(ref message, to);

                SmtpClient client =  (smtpClient == 0)
                    ? EmailHelper.GetSmtpGeneral(message, from, psw, host, port, enableSSL, useDefCredential) 
                    : EmailHelper.GetSmtpClientById(smtpClient, message, from, psw);

                client.Send(message);

                result = Constants.SUCCESS;
                outmsg = Constants.MSG_SUCCESSFUL_EXECUTION;
            }
            catch (Exception ex)
            {
                outmsg = (ex.InnerException != null) 
                    ? string.Format (Constants.FORMAT_EXCEPTION_INTERNAL,
                        ex.Source, ex.Message, ex.InnerException.Source, ex.InnerException.Message)
                    : string.Format (Constants.FORMAT_EXCEPTION, ex.Source, ex.Message);

                result = Constants.FAILURE;
            }
            return result;
        }

        /// <summary>
        /// Executes the stored procedure <c>SP_Query_Database</c>, 
        /// which queries the status of a SICOSS folio
        /// using the specified service URL and authorization header.
        /// </summary>
        /// <param name="serviceUrl">
        /// The URL of the web service used for the SICOSS query.
        /// </param>
        /// <param name="authorizationHeader">
        /// The HTTP authorization header required for accessing the service.
        /// </param>
        /// <param name="folioNumber">
        /// The unique identifier (folio) of the SICOSS request to query.
        /// </param>
        /// <param name="isCompleted">
        /// An output parameter indicating the completion status of the folio query.
        /// Returns 1 if the process is completed, 0 otherwise.
        /// </param>
        /// <param name="outmsg">
        /// An output parameter containing the resulting message from executing the stored procedure.
        /// </param>
        /// <returns>
        /// Returns 0 if the stored procedure executes successfully.
        /// Returns -1 if the execution fails.
        /// </returns>
        /// <date>2018-02-24</date>
        /// <author>ArBR | arcbrth@gmail</author>
        [SqlProcedure]
        public static int SP_Query_Process_stage
                        (string serviceUrl, string authorizationHeader, string folioNumber, out int isCompleted, out string outmsg)
        {
            int result = 0;

            outmsg = "";
            isCompleted = 0;

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
                httpWebRequest.Method = WebRequestMethods.Http.Get;
                httpWebRequest.Accept = "application/json";

                if (!string.IsNullOrEmpty(authorizationHeader))
                {
                    httpWebRequest.PreAuthenticate = true;
                    httpWebRequest.Headers.Add("Authorization", authorizationHeader);
                }
                httpWebRequest.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                string responseJson = null;
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseJson = sr.ReadToEnd();
                }
                
                if (responseJson != null) {
                    outmsg = responseJson;
                    isCompleted = IsStageCompleted(responseJson);
                    result = Constants.SUCCESS;
                }                
            }
            catch (Exception ex)
            {
                outmsg = (ex.InnerException != null)
                    ? string.Format(Constants.FORMAT_EXCEPTION_INTERNAL,
                        ex.Source, ex.Message, ex.InnerException.Source, ex.InnerException.Message)
                    : string.Format(Constants.FORMAT_EXCEPTION, ex.Source, ex.Message);

                result = Constants.FAILURE;
            }

            return result;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines if the Stage is completed
        /// </summary>
        /// <param name="json">json</param>
        /// <returns>{ 1=true, 0=false} </returns>
        public static int IsStageCompleted(string json)
        {
            string findSituacion = @"""situacion"":""COMPLETADA""";
            return json.Contains(findSituacion) ? 1 : 0;
        }
        #endregion
    }
}
