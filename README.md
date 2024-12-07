# SQL Server Objects for Reinforced SQL Development with .NET

## Introduction

**SQL Server objects developed on the .NET platform** provide a powerful method to extend SQL Server functionality by **enabling the creation of stored procedures, functions, triggers, views, and other database objects using .NET languages such as C#**. This approach leverages the robustness of the .NET framework to introduce advanced custom logic, integrate complex business rules, and enhance SQL Server’s built-in features. By effectively integrating the capabilities of .NET with SQL Server, this method not only improves the maintainability and reusability of database objects but also allows for the development of sophisticated solutions that can better meet business and technical requirements.

## Benefits

1. **Advanced Custom Logic**: Integrate complex business logic, advanced computations, and extensive error handling directly within SQL Server using C#.
2. **Code Reusability**: Leverage object-oriented principles to write reusable and maintainable code, reducing redundancy and improving efficiency.
3. **Enhanced Development Environment**: Utilize Visual Studio's powerful debugging and development tools, providing a comprehensive environment for building and testing stored procedures.
4. **Seamless Integration**: Ensure smooth interaction between .NET methods and SQL Server, creating a cohesive and efficient system.

## Summary

Implementing SqlServer objects via the .NET platform offers a powerful and flexible approach to extend SQL Server features while providing a robust mechanism to address advanced business requirements. By utilizing the capabilities of the .NET framework, this method enables the creation of sophisticated logic that enhances SQL Server databases. This approach not only improves code maintainability and reusability but also ensures cohesive integration between .NET and SQL Server functionalities.
___


```csharp
namespace ClusterBRSqlServerObjects
{
    public class SP
    {
        [SqlProcedure]
        public static int SP_System_SendEmail
            (int smtpClient, string host, int port, 
            int enableSSL, int useDefCredential, 
            string from, string psw, string to, 
            string subject, string body, out string outmsg)
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
                    ? EmailHelper.GetSmtpGeneral(
                        message, from, psw, host, port, enableSSL, useDefCredential) 
                    : EmailHelper.GetSmtpClientById(
                        smtpClient, message, from, psw);

                client.Send(message);

                result = Constants.SUCCESS;
                outmsg = Constants.MSG_SUCCESSFUL_EXECUTION;
            }
            catch (Exception ex)
            {
                outmsg = (ex.InnerException != null) 
                    ? string.Format (Constants.FORMAT_EXCEPTION_INTERNAL,
                        ex.Source, ex.Message, 
                        ex.InnerException.Source, ex.InnerException.Message)
                    : string.Format (Constants.FORMAT_EXCEPTION, 
                        ex.Source, ex.Message);

                result = Constants.FAILURE;
            }
            return result;
        }
    }
}
```