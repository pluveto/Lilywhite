using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Util
{
    public static class Logger
    {
        private static void Print(string type, string text)
        {
            var appDir = PathHelper.GetAppDir("",false);
            var appDirUnix = PathHelper.GetAppDir("", true);
            Console.WriteLine("[{0}] {1}: {2}", DateTime.Now.ToString("HH:mm:ss.fff"), type,
                text
                .Replace("\n","\n" + Indent(17+ type.Length, " "))
                .Replace(appDir, "<AppDir>")
                .Replace(appDirUnix, "<AppDir>"));
        }

        public static void Debug(string text)
        {
            var type = "Debug";
            Print(type, text);
        }
        public static void Info(string text)
        {
            var type = "Info";
            Print(type, text);
        }
        public static void BoxInfo(string text)
        {
            var type = "Info";
            var appDir = PathHelper.GetAppDir("", false);
            var appDirUnix = PathHelper.GetAppDir("", true);
            Console.WriteLine("[{0}] {1}: {2}", DateTime.Now.ToString("HH:mm:ss.fff"), type,
                "\n" + Indent(17 + type.Length, "-") + "----------------------------------------------\n" +
                text
                .Replace("\n", "\n" + Indent(17 + type.Length, " "))
                .Replace(appDir, "<AppDir>")
                .Replace(appDirUnix, "<AppDir>")+
                "\n" + Indent(17 + type.Length, "-") + "----------------------------------------------\n");
        }
        public static void Error(string text)
        {
            var type = "Error";
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!");
            Print(type, text);
        }
        public static string Indent(int count, string str = "*")
        {
            var sb = new StringBuilder();
            while (count-- > 0)
            {
                sb.Append(str);
            }
            return sb.ToString();
        }
    }
}
