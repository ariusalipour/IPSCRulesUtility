using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public static class UtilityHelper
    {
        public static string[] ReadFromFile(string filePath)
        {
            var fileArray = File.ReadAllLines(filePath, Encoding.UTF8);

            return fileArray;
        }

        public static Stream ReadStreamFromFile(string fileName)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = $"{rootPath}/xml";

            return File.OpenRead($"{xmlPath}/{fileName}");
        }

        public static string CreateSearchableString(Section section)
        {
            StringBuilder searchableString = new StringBuilder();

            searchableString.AppendLine(section.Numeric);
            searchableString.AppendLine(section.Name);
            searchableString.AppendLine(section.Description);

            foreach (var rule in section.Rules)
            {
                searchableString.AppendLine(rule.Numeric);
                searchableString.AppendLine(rule.Name);
                searchableString.AppendLine(rule.Description);

                foreach (var subRule in rule.SubRules)
                {
                    searchableString.AppendLine(subRule.Numeric);
                    searchableString.AppendLine(subRule.Name);
                    searchableString.AppendLine(subRule.Description);
                }
            }

            return searchableString.ToString().ToLowerInvariant();
        }

        public static string CreateSearchableString(Rule rule)
        {
            StringBuilder searchableString = new StringBuilder();

            searchableString.AppendLine(rule.Numeric);
            searchableString.AppendLine(rule.Name);
            searchableString.AppendLine(rule.Description);

            return searchableString.ToString().ToLowerInvariant();
        }

        public static string CreateSearchableString(SubRule subRule)
        {
            StringBuilder searchableString = new StringBuilder();

            searchableString.AppendLine(subRule.Numeric);
            searchableString.AppendLine(subRule.Name);
            searchableString.AppendLine(subRule.Description);

            return searchableString.ToString().ToLowerInvariant();
        }

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
