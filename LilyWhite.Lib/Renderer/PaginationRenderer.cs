using LilyWhite.Lib.Runtime;
using LilyWhite.Lib.Type;
using LilyWhite.Lib.Util;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LilyWhite.Lib.Renderer
{
    public class PaginationRenderer
    {
        public static void Render(ScriptObject model, List<ScriptObject> posts, int totalPages, string outputDir, int PageSize)
        {
            var store = Engine.App.Store;

            for (int i = 0; i < totalPages; i++)
            {
                var currentPage = i + 1;
                var outPath = outputDir + $"/page/{currentPage}/index.html";

                var curPageModel = model.Clone(true) as ScriptObject;
                var template = curPageModel.GetSafeValue<string>("_rawText");
                var content = renderRaw(template, posts, curPageModel, currentPage, PageSize);
                curPageModel.Add("_content", content);

                var layout = curPageModel.GetSafeValue<string>("layout");
                var layoutPageModel = store.GetLayoutFromCache(layout);
                var finalPostHtml = LayoutRenderer.Render(curPageModel, layoutPageModel);
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllText(outPath, finalPostHtml);
            }
            File.Copy(outputDir + "/page/1/index.html",
                outputDir + "/index.html", true);
        }

        private static string renderRaw(string template, List<ScriptObject> posts, ScriptObject pageModel, int currentPage, int pageSize)
        {
            var store = Engine.App.Store;
            var layoutContext = new TemplateContext() { TemplateLoader = store.TemplateLoader };            
            var totalPages = (int)System.Math.Ceiling((double)posts.Count / pageSize);
            var paginatorModel = new ScriptObject();
            paginatorModel["posts"] = posts.GetRange((currentPage - 1) * pageSize,
                currentPage == totalPages ? posts.Count - (currentPage - 1) * pageSize : pageSize);
            paginatorModel["page"] = currentPage;
            if (currentPage > 1)
            {
                paginatorModel["previousPage"] = currentPage - 1;
            }
            if (currentPage < totalPages)
            {
                paginatorModel["nextPage"] = currentPage + 1;
            }
            paginatorModel["totalPages"] = totalPages;

            
            layoutContext.BuiltinObject.Import(typeof(PipeFunctions));
            layoutContext.PushGlobal(Converter.WrapScriptObject("site", store.SiteModel));
            layoutContext.PushGlobal(Converter.WrapScriptObject("page", pageModel));
            layoutContext.PushGlobal(Converter.WrapScriptObject("paginator", paginatorModel));
            var layoutTemplate = Template.Parse(template);
            return layoutTemplate.Render(layoutContext);
        }
    }
}
