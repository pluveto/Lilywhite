using LilyWhite.Lib.Type;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace LilyWhite.Lib.Util
{
    public static class PathHelper
    {
        /// <summary>
        /// 获取程序所在目录的完整字符串表示, 返回值不以路径分隔符结尾.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string GetAppDir(string dir = "", bool unixStyle = true)
        {
            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            if (unixStyle)
            {
                baseDir = baseDir.Replace("\\", "/");
            }
            if (dir == string.Empty)
            {
                return baseDir;
            }
            return baseDir + "/" + dir.TrimEnd('/');
        }


        public static IList<string> GetFilesToDepth(string path, int depth, string[] exts)
        {
            var files = Directory.EnumerateFiles(path).ToList();
            if (depth == 0)
            {
                return files;
            }
            var folders = Directory.EnumerateDirectories(path);
            foreach (var folder in folders)
            {
                if (folder.StartsWith("_"))
                {
                    continue;
                }
                files.AddRange(GetFilesToDepth(folder, depth - 1, exts));
            }
            if (exts == null)
            {
                return files;
            }
            var valid = new List<string>();
            Array.ForEach(exts, (ext) =>
           {
               valid.AddRange(files.FindAll(x => x.EndsWith(ext)));
           });

            return valid;
        }


        /// <summary>
        /// 为目标路径创建文件夹并计算文件的相对 Url      
        /// </summary>
        /// <param name="baseDir">源文件基础目录</param>
        /// <param name="outDir">输出基础目录</param>
        /// <param name="rawFilePath">源文件路径</param>
        /// <param name="prefix">前缀, 以`/`结尾</param>
        /// <returns></returns>
        public static (string url, string outpath) CalculateUrl(string baseDir, string outDir, string rawFilePath, string prefix = "/")
        {
            var ext = rawFilePath.EndsWith(".md")?"html":Path.GetExtension(rawFilePath);
            var relPath = Path.GetRelativePath(baseDir, rawFilePath);
            var dirName = Path.GetDirectoryName(rawFilePath);
            var relDir = Path.GetRelativePath(baseDir, dirName);
            if (relDir == ".")
            {
                relDir = "";
            }
            var outputDir = outDir + "/" + prefix + "/" + relDir;
            // mkdir if !exists
            Directory.CreateDirectory(outputDir);
            var url = prefix + relDir + "/" + HttpUtility.UrlEncode(Path.GetFileNameWithoutExtension(relPath)) + ext;
            url = url.Replace("//", "/");
            return (url, outputDir + "/" + Path.GetFileNameWithoutExtension(relPath) + ext);
        }

        public static void CloneDirectory(string root, string dest, bool overwrite = false)
        {
            foreach (var directory in Directory.GetDirectories(root))
            {
                string dirName = Path.GetFileName(directory);
                if (!Directory.Exists(Path.Combine(dest, dirName)))
                {
                    Directory.CreateDirectory(Path.Combine(dest, dirName));
                }
                CloneDirectory(directory, Path.Combine(dest, dirName));
            }

            foreach (var file in Directory.GetFiles(root))
            {
                var targetName = Path.Combine(dest, Path.GetFileName(file));
                if (File.Exists(targetName))
                {
                    continue;
                }
                File.Copy(file, targetName);
            }
        }

    }
}
