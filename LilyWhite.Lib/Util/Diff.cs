using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Util
{
    public class Diff
    {
        public enum DiffType
        {
            Update,
            Delete,
            Create
        }
        public struct DiffItem
        {
            public DiffType Type { get; set; }
            public string FileName { get; set; }
        }
        public static List<DiffItem> DiffFiles(Dictionary<string, string> newList, Dictionary<string, string> oldList)
        {
            var diff = new List<DiffItem>();
            foreach (var (key, hash) in newList)
            {
                if (!oldList.ContainsKey(key))
                {
                    diff.Add(new DiffItem() { Type = DiffType.Create, FileName = key });
                    continue;
                }
                if (hash == oldList[key])
                {
                    continue;
                }
                diff.Add(new DiffItem() { Type = DiffType.Update, FileName = key });
            }
            foreach (var (key, _) in oldList)
            {
                if (newList.ContainsKey(key))
                {
                    continue;
                }
                diff.Add(new DiffItem() { Type = DiffType.Delete, FileName = key });
            }
            return diff;
        }
        public static string LogDiffList(List<DiffItem> list)
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                var typeStr = "";
                switch (item.Type)
                {
                    case DiffType.Update:
                        typeStr = "* 更新";
                        break;
                    case DiffType.Delete:
                        typeStr = "- 删除";
                        break;
                    case DiffType.Create:
                        typeStr = "+ 新建";
                        break;
                    default:
                        break;
                }
                sb.AppendLine(typeStr + ": " + item.FileName);
            }
            var ret = sb.ToString();
            if (string.Empty.Equals(ret))
            {
                return "内容无变化";
            }
            return ret;
        }
    }
}
