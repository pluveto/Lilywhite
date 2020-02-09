using LilyWhite.Lib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LilyWhite.Lib.Type;
using Scriban;
using Scriban.Runtime;
using Markdig;
using System.IO;
using System.Web;
using System.Text;
using LilyWhite.Lib.Util;
using LilyWhite.Lib.Renderer;
using Newtonsoft.Json.Linq;

namespace LilyWhite.Lib.Runtime
{
    public class Engine
    {
        // App 单例
        public static Engine App = new Engine();

        public Config Config { get; private set; }
        public Store Store { get; private set; }
        // 引擎初始化方法
        public void Init()
        {
            this.Config = new Config();
            this.Store = new Store();
            this.Store.ThemeDir = PathHelper.GetAppDir("themes") + "/" + this.Config.Get<string>("site.theme");
            this.Store.InputDir = this.Config.GetDirSetting("inputDir");
            this.Store.OutputDir = this.Config.GetDirSetting("outputDir");
            this.Store.TemplateLoader = new TemplateLoader(this.Store.ThemeDir + "/_includes");
            this.Store.SiteModel = Converter.JObjectMappingToScriptObject(this.Config.GetObjMap("site"));
            this.Store.LayoutModels = new Dictionary<string, ScriptObject>();


            PathHelper.CloneDirectory(this.Store.ThemeDir + "/assets", this.Store.OutputDir + "/assets");
            this.Store.FileMD5Map = FileListMD5(this.Store.OutputDir);
        }

        // 引擎启动方法
        public void Start()
        {

            MonitorDirectory(this.Store.InputDir);
        }
        public static Dictionary<string, string> FileListMD5(string dir)
        {
            var ret = new Dictionary<string, string>();
            var list = PathHelper.GetFilesToDepth(dir, 5, null);
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                foreach (var fileName in list)
                {

                    using (var stream = File.OpenRead(fileName))
                    {
                        byte[] checksum = md5.ComputeHash(stream);
                        ret.Add(fileName, BitConverter.ToString(checksum));
                    }
                }
            }
            return ret;
        }
        public void SetOutput(TextWriter textWriter)
        {
            //Console.OutputEncoding = Encoding.UTF8;
            Console.SetOut(textWriter);
        }
        public BuildProgressHandler BuildProgressCallback { get; internal set; }

        public void SetBuildCallback(BuildProgressHandler eventHandler)
        {
            this.BuildProgressCallback = eventHandler;
        }
        private static void MonitorDirectory(string path)

        {

            FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = path;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.EnableRaisingEvents = true;

        }

        private static void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        // 文件变动时执行
        // TODO: LOG
        public void Update()
        {
            var timeStart = DateTime.Now;
            try
            {
                new RenderController.PostsRC().Start();
                new RenderController.PagesRC().Start();
                new RenderController.HomeRC().Start();
                // var ls1 = new string[] { "author.html", "author.xml", "tag.html", "tag.xml" };
                var ls = new string[] { "author.html", "tag.html" };
                foreach (var tag in this.Store.TagsContainer)
                {
                    new RenderController.LoopRC(
                        this.Store.PostModels.FindAll(post => tag.Value.Contains(post.GetSafeValue<string>("uuid"))),
                        this.Store.ThemeLayoutDir + "/tag.html",
                        this.Store.OutputDir + "/tag/" + tag.Key,
                        new ScriptObject() { { "tag", tag.Key } })
                    .Start();
                }
                foreach (var author in this.Config.Get<JArray>("site.data.authors"))
                {
                    var username = author.SelectToken("username").Value<string>();
                    new RenderController.LoopRC(
                         this.Store.PostModels.FindAll(x => x.GetSafeValue<string>("author").Equals(username)),
                         this.Store.ThemeLayoutDir + "/author.html",
                        this.Store.OutputDir + "/author/" + username,
                        new ScriptObject() { { "author", username } }).Start();
                }
                Logger.Debug("tags: " + string.Join(", ", this.Store.TagsContainer.Keys));

            }
            catch (Scriban.Syntax.ScriptRuntimeException e)
            {
                Logger.Error(e.Message + "\n" + e.StackTrace + "\n");
                Logger.BoxInfo("编译失败:" + e.Message + "\n" + e.StackTrace + "\n");
                this.BuildProgressCallback.Invoke(this, new BuildProgressEventArgs(Type.BuildStatus.Failed, 1.0d, "编译失败"));
            }
            var timeEnd = DateTime.Now;
            var deltaTime = Math.Round((timeEnd - timeStart).TotalSeconds, 2);
            this.BuildProgressCallback.Invoke(this, new BuildProgressEventArgs(Type.BuildStatus.Success, 1.0d, "编译完成"));
            Logger.BoxInfo(
                $"\n编译完成, 耗时 {deltaTime}s\n");

            var newHashes = FileListMD5(this.Store.OutputDir);
            var diffFiles =  Util.Diff.DiffFiles(newHashes, this.Store.FileMD5Map);
            this.Store.FileMD5Map = newHashes;
            Logger.Info(Diff.LogDiffList(diffFiles));
        }


        public void Reload()
        {
            this.Init();
            this.Update();
        }
    }
}
