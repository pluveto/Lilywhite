using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LilyWhite.Lib.Util
{
    public class Parser
    {
        private static Dictionary<string, Dictionary<string, string>> cache = new Dictionary<string, Dictionary<string, string>>();
        /// <summary>
        /// 该函数扫描模板文件的内容, 提取 meta 信息到映射表, 将其余正文(body)放入映射表的 rawText 字段.
        /// </summary>
        /// <param name="templatePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ExtractMeta(string templatePath)
        {
            if (cache.ContainsKey(templatePath))
            {
                return cache[templatePath];
            }
            // 返回的映射表
            var templateMeta = new Dictionary<string, string>();
            StringBuilder body = new StringBuilder();
            using (StreamReader reader = new StreamReader(templatePath))
            {
                try
                {
                    ReadMetaAndBodyFromStream(templateMeta, body, reader);
                }
                // 如果读取不到 meta 或者读取出错, 就返回整个文件内容, 该内容放在 _rawText 中
                catch (NullReferenceException e)
                {
                    Logger.Info("跳过了该文件的 meta: " + templatePath + "\n" + e);
                    reader.DiscardBufferedData();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    templateMeta.Add("_rawFilePath", templatePath);
                    templateMeta.Add("_rawText", reader.ReadToEnd());
                    goto exitFunction;
                }
            }
            templateMeta.Add("_rawFilePath", templatePath);
            templateMeta.Add("_rawText", body.ToString());

            exitFunction: cache[templatePath] = templateMeta;
            return templateMeta;
        }

        private static void ReadMetaAndBodyFromStream(Dictionary<string, string> templateMeta, StringBuilder body, StreamReader reader)
        {
            var line = reader.ReadLine();
            // 扫描作为开头的 ---
            while (!line.StartsWith("---"))
            {
                body.AppendLine(line);
                line = reader.ReadLine();
            }
            // 丢弃作为开头的 --- 行
            line = reader.ReadLine();
            // 读取开头的 --- 行之后的行, 直到遇到作为结束的 --- 行
            while (!line.StartsWith("---"))
            {
                var spl = line.Split(':', 2);
                // 跳过无法分割的行
                if (spl.Length != 2)
                {
                    continue;
                }
                templateMeta.Add(spl[0].Trim(), spl[1].Trim());
                line = reader.ReadLine();
            }
            body.Append(reader.ReadToEnd());
        }
    }
}
