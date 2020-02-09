using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Util
{
    public static class Inbox
    {
        /// <summary>
        /// 将 map 中的指定 key 添加上值 value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <param name="value"></param>
        /// <param name="keys">指定的key[]</param>
        public static void MergeValue<T>(Dictionary<string, List<T>> map, T value, string[] keys)
        {
            foreach (var key in keys)
            {
                if (map.ContainsKey(key))
                {
                    map[key].Add(value);
                }
                else
                {
                    map.Add(key, new List<T>() { value });
                }
            }
        }
    }
}
