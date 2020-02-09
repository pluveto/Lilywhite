using LilyWhite.Lib.Runtime;
using LilyWhite.Lib.Type;
using LilyWhite.Lib.Util;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LilyWhite.Lib.Renderer
{
    public class LayoutRenderer
    {
        private static int isRecRendering = 0;
        /// <summary>
        /// <b>布局递归渲染器.</b><br />
        ///     例如, 给定一个 ScriptObject, 一个布局模板文件(解析过meta 的)<br />
        ///     本函数将把 ScriptObject 渲染至其中, 然后读取该布局文件的上层布局文件,<br />
        ///     在此把 ScriptObject 渲染至其中, 重复进行这一过程, 直到没有上层布局文件, 最后返回渲染结果 html<br />
        /// </summary>
        /// <returns></returns>
        public static string Render(ScriptObject pageModel, ScriptObject layoutPageModel)
        {


            var store = Engine.App.Store;

            var layoutContext = new TemplateContext() { TemplateLoader = store.TemplateLoader };
            var paginatorModel = Converter.ObjectToScriptObject(
                new
                {
                    Page = 1,
                    Previous_Page = 0,
                    Next_Page = 2,
                    Total_Pages = 10
                }
                );
            // 0. 导入 pipe 函数            
            layoutContext.BuiltinObject.Import(typeof(PipeFunctions));
            // 1. 将全局数据推送给模板
            layoutContext.PushGlobal(Converter.WrapScriptObject("site", store.SiteModel));
            layoutContext.PushGlobal(Converter.WrapScriptObject("content", pageModel["_content"]));
            layoutContext.PushGlobal(Converter.WrapScriptObject("page", new[]
            {
                pageModel,      // 2. 将内容的模板数据推送给模板
                layoutPageModel // 3. 将布局自身的模板数据推送给模板
            }));

            // 4. 对于分页的处理
            layoutContext.PushGlobal(Converter.WrapScriptObject("paginator", paginatorModel));
            var layoutTemplate = Template.Parse(layoutPageModel.GetSafeValue<string>("_rawText"));
            var layoutResultHtml = layoutTemplate.Render(layoutContext);
            Logger.Info("* 渲染结束");
            // 判断是否需要递归渲染
            if (!layoutPageModel.ContainsKey("layout") || layoutPageModel.GetSafeValue<string>("layout") == "null")
            {
                return layoutResultHtml;
            }
            var outLayoutShortName = layoutPageModel.GetSafeValue<string>("layout");
            Logger.Info("发现上级布局, 进行递归渲染. 布局文件为: " + layoutPageModel.GetSafeValue<string>("layout"));
            var outLayout = store.GetLayoutFromCache(outLayoutShortName);
            //Logger.Info(Logger.Indent(isRecRendering) + "递归渲染中, 当前层数: " + isRecRendering);
            isRecRendering++;
            pageModel["_content"] = layoutResultHtml;
            var html = Render(pageModel, outLayout);
            isRecRendering--;
            Logger.Info(Logger.Indent(isRecRendering) + "递归渲染结束, 层数: " + isRecRendering);
            return html;
        }
    }
}
