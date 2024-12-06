
namespace ClusterBRSqlServerObjects
{
    /// <summary>
    /// A static class containing constants used throughout the application.
    /// Provides a centralized repository for status codes, SMTP client identifiers, 
    /// and reusable string messages or formats for exceptions and success notifications.
    /// </summary>
    /// <date>2018-02-24</date>    
    /// <author>ArBR | arcbrth@gmail</author>    
    public static class Constants
    {
        // General status codes
        internal const int SUCCESS = 0;
        internal const int FAILURE = -1;

        // Identifiers for SMTP client configurations
        internal const int SMTP_GENERAL = 0;
        internal const int SMTP_GMAIL = 1;
        internal const int SMTP_SECURE_SERVER = 2;
        internal const int SMTP_EXCHANGE = 3;
        internal const int SMTP_COMPANY = 4;

        // Messages
        internal const string MSG_SUCCESSFUL_EXECUTION = "Successful execution";

        // Formats for exception messages
        internal const string FORMAT_EXCEPTION =
            "==Exception:== \r\n Source: {0} \r\n Message: {1}";

        internal const string FORMAT_EXCEPTION_INTERNAL =
            "==Exception:== \r\n Source: {0} \r\n Message: {1} \r\n ===InnerException== \r\n Source: {2}. \r\n Message: {3}";
    }

}
