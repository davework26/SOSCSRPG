using System;
using System.IO;

namespace Engine.Services
{
    public static class LoggingService
    {
        private const string LOG_FILE_DIRECTORY = "Logs";

        static LoggingService()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_FILE_DIRECTORY);

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public static void Log(Exception exception, bool isInnerExeception = false)
        {
            using (StreamWriter sw = new StreamWriter(LogFileName(), true))
            {
                sw.WriteLine(isInnerExeception ? "INNER EXCEPTION" : $"EXCEPTION: {DateTime.Now}");
                sw.WriteLine(new String(isInnerExeception ? '-' : '=', 40));
                sw.WriteLine($"{exception.Message}");
                sw.WriteLine($"{exception.StackTrace}");

                sw.WriteLine();
            }

            if (exception.InnerException != null)
            {
                Log(exception.InnerException, true);
            }
        }

        private static string LogFileName()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LOG_FILE_DIRECTORY,
                $"SOSCSRPG_{DateTime.Now:yyyyMMdd}.log");
        }
    }
}
