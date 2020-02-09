using LilyWhite.Lib.Runtime;
using LilyWhite.Lib.Util;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LilyWhite.Lib.Renderer
{
    public class DocumentRenderer
    {

        public enum DocumentFormat
        {
            Markdown,
            Html
        }
        public enum DocumentType
        {
            Post,
            Page
        }
        public static string GetTypeDirPrefix(DocumentType type, bool FirstLetterUpperCase = false)
        {
            string ret = null;
            switch (type)
            {
                case DocumentType.Post:
                    ret = "posts";
                    break;
                case DocumentType.Page:
                    ret = "pages";
                    break;
                default:
                    break;
            }
            if (FirstLetterUpperCase)
            {
                ret = ret[0].ToString().ToUpper() + ret.Substring(1);
            }
            return ret;
        }
        public static void PreRenderDocuments(DocumentType type, List<ScriptObject> saveTo, string[] documentFiles)
        {
            Logger.Info($" ~ {type.ToString()} 预处理开始 ~ ");

            var store = Engine.App.Store;
            var searchDir = store.InputDir + "/posts";
            saveTo.Clear();
            for (int i = 0; i < documentFiles.Length; i++)
            {
                var rawFilePath = documentFiles[i];
                var uuid = System.Guid.NewGuid().ToString();
                var docPageModel = Converter.MappingToScriptObject(Parser.ExtractMeta(rawFilePath));

                if (type == DocumentType.Post && docPageModel.ContainsKey("tags"))
                {
                    var tagsOfPost = docPageModel.GetSafeValue<string[]>("tags");
                    Inbox.MergeValue(store.TagsContainer, uuid, tagsOfPost);
                }

                Logger.Info("预处理文件: " + rawFilePath);


                // 计算文章的相对 url 和输出文件的路径
                var (url, outPath) = PathHelper.CalculateUrl(searchDir, store.OutputDir, rawFilePath, GetTypeDirPrefix(type) + "/");
                var src = docPageModel.GetSafeValue<string>("_rawText");
                var content = RawTextRenderer.BuildPostRawText(rawFilePath.EndsWith(".md") ? DocumentFormat.Markdown : DocumentFormat.Html,
                    src);
                docPageModel.Add("url", url);
                docPageModel.Add("uuid", uuid);
                docPageModel.Add("content", content);
                docPageModel.Add("_content", content);
                docPageModel.Add("_outPath", outPath);
                docPageModel.Add("_rawFilePath", rawFilePath);
                saveTo.Add(docPageModel);
            }
            saveTo.Sort((x, y) => y.GetSafeValue<DateTime>("date").CompareTo(x.GetSafeValue<DateTime>("date")));

            Logger.Info("Posts 预处理完毕\n\n");
        }
        public static void RenderDocuments(DocumentType type, List<ScriptObject> docModels)
        {
            Logger.Info($" ~ {type.ToString()} 渲染开始 ~ ");
            var store = Engine.App.Store;
            var count = docModels.Count;
            for (int i = 0; i < count; i++)
            {
                docModels[i].Add("id", i + 1);
                RenderSingleDocument(docModels, i);
                Engine.App.BuildProgressCallback?.Invoke(Engine.App, new Type.BuildProgressEventArgs(
                    Type.BuildStatus.Compiling, (double)(i + 1) / count, $"Rendering {type.ToString()} ({i + 1}/{count})"));
            }
            Logger.Info($"{type.ToString()} 渲染完毕\n\n");

        }


        public static void RenderSingleDocument(List<ScriptObject> models, int index)
        {
            var store = Engine.App.Store;
            var model = models[index];
            var layout = model.GetSafeValue<string>("layout");

            var prevPostMetaMap = index > 0 ? models[index - 1] : null;
            var nextPostMetaMap = index + 1 < models.Count ? models[index + 1] : null;
            model.Add("previous", prevPostMetaMap);
            model.Add("next", nextPostMetaMap);

            var rawFilePath = model.GetSafeValue<string>("_rawFilePath");
            var outPath = model.GetSafeValue<string>("_outPath");
            var layoutPageModel = store.GetLayoutFromCache(layout);

            // 渲染文章到其布局
            Logger.Info(string.Format("布局递归渲染器开始工作, 当前文件为: {0}" +
                " \n当前布局为: {1}", rawFilePath, layout));
            var finalPostHtml = LayoutRenderer.Render(model, layoutPageModel);
            // 将最终文章渲染结果写入文件
            File.WriteAllText(outPath, finalPostHtml);
            Logger.Info("完成渲染: " + Path.GetFileName(rawFilePath) + "\n");

            // TODO: 增加对 category 的支持
        }


    }
}
