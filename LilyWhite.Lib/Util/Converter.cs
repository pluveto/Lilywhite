using Newtonsoft.Json.Linq;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace LilyWhite.Lib.Util
{
    /// <summary>
    /// 转换工具静态类
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// 将普通对象转换为 ScriptObject
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ScriptObject ObjectToScriptObject(object obj)
        {
            ScriptObject sobj = new ScriptObject();
            var type = obj.GetType();
            var properties = type.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var prop = properties[i];
                var name = prop.Name.ToLower();
                var val = prop.GetValue(obj);
                sobj.Add(name, val);
            }
            return sobj;
        }
        /// <summary>
        /// 将纯文本键值对映射为模板数据对象. 并根据键/值内容自动判断数据类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ScriptObject MappingToScriptObject(this IDictionary<string, string> source)
        {
            var someObject = new ScriptObject();

            foreach (var item in source)
            {
                var key = item.Key.Trim();
                var value = item.Value.Trim().ToLower();
                if (item.Value.StartsWith("cs::"))
                {
                    var realValue = item.Value.Substring("cs::".Length);
                    if (realValue == "time.now")
                    {
                        someObject[key] = DateTime.Now;
                    }
                }
                else if (key == "tags")
                {
                    someObject[key] = (item.Value).Trim().Split(',').Select(x => x.Trim()).ToArray();
                }
                else if (key == "date")
                {
                    someObject[key] = DateTime.Parse((item.Value));
                }
                else if (value == "true" || value == "false")
                {
                    someObject[key] = bool.Parse(item.Value);
                }
                else if (item.Value.StartsWith("\'") && item.Value.EndsWith("\'"))
                {
                    someObject[key] = item.Value.Trim('\'');
                }
                else if (item.Value.StartsWith("\"") && item.Value.EndsWith("\""))
                {
                    someObject[key] = item.Value.Trim('\"');
                }
                else if (int.TryParse(item.Value, out int n))
                {
                    someObject[key] = n;
                }
                else
                {
                    someObject[key] = item.Value;
                }

            }

            return someObject;
        }
        public static ScriptObject JObjectMappingToScriptObject(JObject source)
        {
            var someObject = new ScriptObject();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                var key = item.Key.Trim().ToLower();
                if (item.Value.GetType() != typeof(string))
                {
                    someObject[key] = item.Value;
                    continue;
                }
                var value = ((string)item.Value).Trim().ToLower();
                if (value.StartsWith("cs::"))
                {
                    var realValue = value.Substring("cs::".Length);
                    if (realValue == "time.now")
                    {
                        someObject[key] = DateTime.Now;
                    }
                }
                else if (key == "tags")
                {
                    someObject[key] = (value).Trim().Split(',').Select(email => email.Trim()).ToArray();
                }
                else if (key == "date")
                {
                    someObject[key] = DateTime.Parse((value));
                }
                else if (value == "true" || value == "false")
                {
                    someObject[key] = bool.Parse(value);
                }
                else if (value.StartsWith("\'") && value.EndsWith("\'"))
                {
                    someObject[key] = value.Trim('\'');
                }
                else if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    someObject[key] = value.Trim('\"');
                }
                else if (int.TryParse(value, out int n))
                {
                    someObject[key] = n;
                }
                else
                {
                    someObject[key] = item.Value;
                }

            }

            return someObject;
        }
        public static ScriptObject WrapScriptObject(string key, object obj)
        {
            var wrapper = new ScriptObject();
            wrapper.Add(key, obj);
            return wrapper;
        }
        public static ScriptObject WrapScriptObject(string key, ScriptObject obj)
        {
            var wrapper = new ScriptObject();
            wrapper.Add(key, obj);
            return wrapper;
        }
        public static ScriptObject WrapScriptObject(string key, ScriptObject[] objs)
        {
            var wrapper = new ScriptObject();
            var objFirst = objs[0];
            for (int i = 1; i < objs.Length; i++)
            {
                var obj = objs[i];
                objFirst.Import(obj);
            }
            wrapper.Add(key, objFirst);
            return wrapper;
        }
    }
}
