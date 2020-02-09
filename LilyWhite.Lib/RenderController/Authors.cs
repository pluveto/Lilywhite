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
    /// 首页渲染控制器
    /// </summary>
    public class AuthorsRC
    {
        public void Start()
        {
            var store = Engine.App.Store;

            var pageSize = Engine.App.Config.Get<int>("site.pagination.pageSize");
            var totalPages = System.Math.Ceiling((double)store.PostModels.Count / pageSize);


            var rawFilePath = Engine.App.Store.ThemeDir + $"/index.html";
            var indexModel = Converter.MappingToScriptObject(Parser.ExtractMeta(rawFilePath));

            //PaginationRenderer.Render(indexModel, totalPages, store.OutputDir, pageSize);
        }
        

    }
}
