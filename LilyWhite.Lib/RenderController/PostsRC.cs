using LilyWhite.Lib.Renderer;
using LilyWhite.Lib.Runtime;
using LilyWhite.Lib.Util;
using Scriban.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace LilyWhite.Lib.RenderController
{

    /// <summary>
    /// 文章渲染控制器
    /// </summary>
    public class PostsRC
    {
        /// <summary>
        /// 遍历 posts 文件夹下的所有 markdown / html 文件, 对于每个文件, 进行 meta 解析, 并渲染到 meta 中申明的 layout, 输出文件.
        /// 遍历的过程中, 会得到 Tags, Categories 等信息.
        /// 最后返回的是所有 Posts.
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            Engine.App.Store.PostModels = new List<ScriptObject>();

            var store = Engine.App.Store;                        
            var models = store.PostModels;
            var type = DocumentRenderer.DocumentType.Post;

            var searchDir = store.InputDir + "/" + DocumentRenderer.GetTypeDirPrefix(type) + "/";
            var postFiles = PathHelper.GetFilesToDepth(searchDir, 3, new[]{ ".md",".html"});
            DocumentRenderer.PreRenderDocuments(type, models, postFiles.ToArray());
            DocumentRenderer.RenderDocuments(type, models);
        }


    }
}
