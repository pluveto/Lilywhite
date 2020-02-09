using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {            
            var inFilePath = @"D:\in.html";
            var template = Template.Parse(System.IO.File.ReadAllText(inFilePath));
            var obj = new ScriptObject();
            var context = new TemplateContext();
            context.TemplateLoader = new MyIncludeFromDisk();
            Console.WriteLine(template.Render(context));
            Console.ReadKey();
        }
    }
    public class MyIncludeFromDisk : ITemplateLoader
    {
        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            throw new NotImplementedException();
        }

        string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(@"D:\", templateName);
        }

        string ITemplateLoader.GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
        {
            return Path.Combine(@"D:\", templateName);
        }

        string ITemplateLoader.Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            return File.ReadAllText(templatePath);
        }
    }
}
