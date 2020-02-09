using LilyWhite.Lib.Renderer;
using LilyWhite.Lib.Runtime;
using LilyWhite.Lib.Type;
using LilyWhite.Lib.Util;
using Scriban;
using Scriban.Runtime;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LilyWhite.Lib.RenderController
{

    /// <summary>
    /// 通用循环渲染控制器
    /// </summary>
    public class LoopRC
    {
        private string outputDir;
        public ScriptObject Model { get; set; }
        public List<ScriptObject> Posts { get; set; }
        public LoopRC(List<ScriptObject> posts, string modelPath, string outputDir, ScriptObject attach = null)
        {
            this.Posts = posts;
            this.Model = Converter.MappingToScriptObject(Parser.ExtractMeta(modelPath)); ;
            this.outputDir = outputDir;

            if (attach != null)
            {
                foreach (var (key,value) in attach)
                {
                    this.Model[key] = value;
                }
            }

        }
        public void Start()
        {
            var store = Engine.App.Store;

            var pageSize = Engine.App.Config.Get<int>("site.pagination.pageSize");
            var totalPages = (int)System.Math.Ceiling((double)this.Posts.Count / pageSize);
            PaginationRenderer.Render(Model, Posts, totalPages, outputDir, pageSize);
        }
        

    }
}
