using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LilyWhite.Lib.Type
{
    /// <summary>
    /// 配置文件对象管理器
    /// </summary>
    public class Config

    {
        /// <summary>
        /// 缓存
        /// </summary>
        private Dictionary<string,JObject> cache =  new Dictionary<string, JObject>();

        public string BaseDir { get; private set; }
        public Config(string baseDir = "./config")
        {
            this.BaseDir = baseDir;
        }

        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="path">路径, 例如: test.myItem1.myItem2 表示返回 test.json 文件中 myItem1 下的 myItem2 的值</param>
        /// <returns></returns>
        public T Get<T>(string path)
        {
            var explode = path.Split(".", 2);
            var fileName = explode[0] + ".json";
            var configPath = explode[1];
            JObject myJObject = GetConfigObject(fileName);
            var ret =  myJObject.SelectToken(configPath);
            return ret == null ? default(T) : ret.Value<T>();
        }
        /// <summary>
        /// 获取配置文件对象的映射形式
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public JObject GetObjMap(string fileName)
        {
            fileName += (fileName.EndsWith(".json") ? "" : ".json");
            JObject myJObject = GetConfigObject(fileName);
            return myJObject;       
        }
        /// <summary>
        /// 获取某文件的配置文件对象, 先查询内存中是否有缓存.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private JObject GetConfigObject(string fileName)
        {
            JObject myJObject;
            if (cache.ContainsKey(fileName))
            {
                myJObject = cache[fileName];
            }
            else
            {
                var myJsonString = File.ReadAllText(this.BaseDir + "/" + fileName);
                myJObject = JObject.Parse(myJsonString);
            }
            return myJObject;
        }

        /// <summary>
        /// 获取文件夹的完整字符串表示, 其中文件夹名称从配置文件中读取, 以 site.dir 作为键进行搜索.
        /// 例如, 获取 inputDir 的路径, 则查找 site.inputDir 项, 得值, 前拼接 AppDir, 得值, 递交之.
        /// </summary>
        /// <param name="config">配置管理器</param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public string GetDirSetting(string dir)
        {
            return Path.GetFullPath(this.Get<string>("site." + dir));
        }


    }
}
