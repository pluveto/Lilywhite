using System;
using System.Collections.Generic;
using System.Text;

namespace LilyWhite.Lib.Type
{
    public static class PipeFunctions
    {
        public static string DateToString(DateTime dateTime)
        {
            //23 Mar 2016
            return dateTime.ToString("MMMM dd, yyyy");
        }

        public static string ExpandUrls(string htmlbody, string baseUrl)
        {
            //var regex = "<a\\s+(?:[^>]*?\\s+)?href=([\"'])(.*?)\\1";
            return htmlbody;
        }
    }
}
