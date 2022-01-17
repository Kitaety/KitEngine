using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitEngine.Common
{
    public static class Log
    {
        private const ConsoleColor InfoColor = ConsoleColor.White;
        private const ConsoleColor WarningColor = ConsoleColor.Yellow;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private const ConsoleColor SuccsessColor = ConsoleColor.Green;
        public static void Info(object message)
        {
            Console.ForegroundColor = InfoColor;
            Print(message);
        }
        public static void Warn(object message)
        {
            Console.ForegroundColor = WarningColor;
            Print(message, prefix:"Warning:");
        }
        public static void Error(object message)
        {
            Console.ForegroundColor = ErrorColor;
            Print(message, prefix: "Error:");
        }
        public static void Success(object message)
        {
            Console.ForegroundColor = SuccsessColor;
            Print(message, postfix:"Successful");
        }
        private static void Print(object message, string prefix = "", string postfix = "")
        {
            Console.WriteLine($"{GetTimestamp()} {prefix} {message} {postfix}");
        }
        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("[dd-MM-yyyy HH:mm:ss.fff] ");
        }
    }
}
