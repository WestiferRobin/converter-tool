using System;
using System.IO;

namespace ConverterTool.Logger
{
    public static class Log
    {
        public static string LogFileLocation = string.Empty;

        private static void WriteToLog(string message)
        {
            if (!string.IsNullOrEmpty(LogFileLocation))
            {
                using (StreamWriter file = new StreamWriter(LogFileLocation, true))
                {
                    file.WriteLine(message);
                }
            }
        }

        public static void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ResetColor();
            WriteToLog(message);
        }

        public static void Success(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
            WriteToLog(message);
        }

        public static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            WriteToLog(message);
        }

        public static void Warn(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
            WriteToLog(message);
        }
    }
}
