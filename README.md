# Sql Server Objects for Advanced SQL Development with .NET

## Introduction

SQL Server objects developed in .NET platform provide a powerful means to extend SQL Server functionality by allowing to write stored procedures using .NET languages such as C#. This approach leverages the robustness of the .NET framework to introduce advanced custom logic and seamlessly integrate it into SQL Server databases. 

## Benefits

1. **Advanced Custom Logic**: Integrate complex business logic, advanced computations, and extensive error handling directly within SQL Server using C#.
2. **Code Reusability**: Leverage object-oriented principles to write reusable and maintainable code, reducing redundancy and improving efficiency.
3. **Enhanced Development Environment**: Utilize Visual Studio's powerful debugging and development tools, providing a comprehensive environment for building and testing stored procedures.
4. **Seamless Integration**: Ensure smooth interaction between .NET methods and SQL Server, creating a cohesive and efficient system.

## Summary

Implementing SqlServer objects via the .NET platform offers a powerful and flexible approach to extend SQL Server capabilities while providing a robust mechanism to address advanced business requirements. By utilizing the advanced features of the .NET framework, this method enables the creation of sophisticated logic that enhances SQL Server databases. This approach not only improves code maintainability and reusability but also ensures seamless integration between .NET and SQL Server functionalities.
___


```csharp

        [SqlProcedure]
        public static int SP_System_SendEmail
            (int smtpClient, string host, int port, 
            int enableSSL, int useDefCredential, 
            string from, string psw, string to, string subject, 
            string body, out string outmsg)
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
                    : string.Format (Constants.FORMAT_EXCEPTION, ex.Source, ex.Message);

                result = Constants.FAILURE;
            }
            return result;
        }
```