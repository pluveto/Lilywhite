using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LilyWhite.Lib.Type
{
    public class TemplateLoader : ITemplateLoader
    {
        private string baseDir;
        public TemplateLoader(string dirForParts)
        {
            baseDir = dirForParts;
        }
        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            throw new NotImplementedException();
        }

        string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(baseDir, templateName);
        }

        string ITemplateLoader.GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(baseDir, templateName);
        }

        string ITemplateLoader.Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            return File.ReadAllText(templatePath);
        }
    }
}
