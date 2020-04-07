using System;
using System.Collections.Generic;
using System.Text;

namespace IPSCRulesLibrary.Services
{
    public static class UtilityHelper
    {
        public static string CreateFriendlyName(string filename)
        {
            filename = filename.Replace("/", "");
            filename = filename.Replace("(", "");
            filename = filename.Replace(")", "");
            filename = filename.Replace("\"", "");
            filename = filename.Replace("�", "-");

            return filename;
        }
    }
}
