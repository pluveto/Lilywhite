using LilyWhite.Lib.Type;
using LilyWhite.Lib.Util;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Runtime
{
    /// <summary>
    /// 状态管理中介, 储存被广泛调用的数据.
    /// </summary>
    public class Store
    {
        /// <summary>
        /// 当前主题的目录, 渲染模板数据源
        /// </summary>
        public string ThemeDir { get; internal set; }
        public string ThemeLayoutDir { get { return ThemeDir + "/_layouts"; }}
        public string ThemeIncludeDir { get { return ThemeDir + "/_includes"; } }
        /// <summary>
        /// 渲染内容数据源
        /// </summary>
        public string InputDir { get; internal set; }
        /// <summary>
        /// 渲染结果输出目录
        /// </summary>
        public string OutputDir { get; internal set; }
        // 模板加载器, 用于为模板中 include 函数提供基准路径
        public TemplateLoader TemplateLoader { get; internal set; }
        /// <summary>
        /// Site 模板数据
        /// </summary>
        public ScriptObject SiteModel { get; internal set; }
        /// <summary>
        /// 标签容器, PostsRC 渲染时对其赋值, 结构
        /// 标签[(标签名, [标签下的文章id])]
        /// </summary>
        public Dictionary<string, List<string>> TagsContainer { get; internal set; } = new Dictionary<string, List<string>>();
        public List<ScriptObject> PostModels { get; internal set; }
        public List<ScriptObject> PagesModels { get; internal set; }
        public Dictionary<string, string> FileMD5Map { get; internal set; }
        public Dictionary<string, ScriptObject> LayoutModels { get; internal set; }

        public ScriptObject GetLayoutFromCache(string layout, string ext = ".html")
        {
            ScriptObject layoutPageModel;
            if (!this.LayoutModels.ContainsKey(layout))
            {
                var layoutMetaMap = Parser.ExtractMeta(this.ThemeLayoutDir + "/" + layout + ext);
                layoutPageModel = Converter.MappingToScriptObject(layoutMetaMap);
                this.LayoutModels.Add(layout, layoutPageModel);
            }
            else
            {
                layoutPageModel = this.LayoutModels[layout];
                Logger.Debug("通过缓存读取到了布局");
            }
            return layoutPageModel;
        }
    }
}
