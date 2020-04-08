using System;
using System.Collections.Generic;
using System.IO;
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
            filename = filename.Replace("|", "-");

            return filename;
        }

        public static void CreateUpdateFile(string filePath, string filename, string content)
        {
            Directory.CreateDirectory(filePath);

            File.WriteAllText($"{filePath}/{filename}", content, Encoding.Default);
        }
    }
}
