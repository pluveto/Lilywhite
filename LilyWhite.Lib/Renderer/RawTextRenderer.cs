using LilyWhite.Lib.Util;
using Markdig;
using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Renderer
{
    public class RawTextRenderer
    {

        public static string BuildPostRawText(DocumentRenderer.DocumentFormat format, string rawText)
        {
            if (format == DocumentRenderer.DocumentFormat.Markdown)
            {
                // Build Markdown
                var markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
                var mdRendered = Markdown.ToHtml(rawText, markdownPipeline);
                Logger.Info("Markdown 编译完成");
                return mdRendered;
            }
            // 对于 html 文件直接渲染
            else
            {
                Logger.Info("Html文件, 已跳过编译直接返回");
                return rawText;
            }
        }
    }
}
