using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class HtmlParser
    {
        private bool _UseFullHtml;

        public HtmlParser(bool useFullHtml = true)
        {
            _UseFullHtml = useFullHtml;
        }

        private string ParseStringBuilder(StringBuilder builder)
        {
            if (_UseFullHtml)
            {
                // CSS
                var css = CreateCssStyling();

                // Prepend
                builder.Insert(0, $"<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<style>{css}</style>\r\n</head>\r\n<body>\r\n");

                // Append
                builder.AppendLine("\r\n</body>\r\n</html>");
            }
            
            return builder.ToString().Replace('�', '-');
        }

        public string CreateCssStyling()
        {
            var cssBuilder = new StringBuilder();

            cssBuilder.AppendLine(
                "h1 {\r\n    color: #54595f;\r\n    font-family: \"Roboto\";\r\n    font-weight: 600;\r\n}" +
                "h5 {\r\n    color: #54595f;\r\n    font-family: \"Roboto\";\r\n    font-weight: 600;\r\n    font-size: 24px;\r\n}" +
                "h6 {\r\n    color: #54595f;\r\n    font-family: \"Roboto\";\r\n    font-weight: 600;\r\n    font-size: 22px;\r\n}" +
                "body {\r\n    color: #7a7a7a;\r\n    font-family: \"Roboto\", Sans-serif;\r\n    font-weight: 400;\r\n    font-size: 16px;\r\n}");

            return cssBuilder.ToString();
        }

        public string CreateSectionHtmlPage(Section section)
        {
            var html = new StringBuilder();

            html.AppendLine($"<h1>{section.Numeric} {section.Name}</h1>");
            html.AppendLine($"{section.Description}");

            foreach (var sectionRule in section.Rules)
            {
                html.AppendLine($"<h5>{sectionRule.Numeric} {sectionRule.Name}</h5>");
                html.AppendLine($"{sectionRule.Description}");

                foreach (var ruleSubRule in sectionRule.SubRules)
                {
                    html.AppendLine($"<h6>{ruleSubRule.Numeric} {ruleSubRule.Name}</h6>");
                    html.AppendLine($"{ruleSubRule.Description}");
                }
            }

            return ParseStringBuilder(html);
        }

        public string CreateChapterHtmlPage(Chapter chapter)
        {
            var html = new StringBuilder();

            html.AppendLine($"<h1>{chapter.Numeric} {chapter.Name}</h1>");
            html.AppendLine($"{chapter.Description}");

            foreach (var chapterSection in chapter.Sections)
            {
                html.AppendLine($"<h5>{chapterSection.Numeric} {chapterSection.Name}</h5>");
                html.AppendLine($"{chapterSection.Description}");
                html.AppendLine($"<a href=\"{UtilityHelper.CreateFriendlyName(chapter.Name)}/{UtilityHelper.CreateFriendlyName(chapterSection.Name)}.html\">Read More...</a>");
            }

            return ParseStringBuilder(html);
        }

        public string CreateDisciplineHtmlPage(Discipline discipline)
        {
            var html = new StringBuilder();

            html.AppendLine($"<h1>{discipline.Name} IPSC Rules</h1>");

            foreach (var chapter in discipline.Chapters)
            {
                html.AppendLine($"<h5>{chapter.Numeric} {chapter.Name}</h5>");
                html.AppendLine($"<a href=\"{UtilityHelper.CreateFriendlyName(chapter.Name)}.html\">Read More...</a>");
            }

            return ParseStringBuilder(html);
        }

        public string CreateIndexHtmlPage(List<Discipline> disciplines)
        {
            var html = new StringBuilder();

            html.AppendLine($"<h1>IPSC Disciplines</h1>");
            html.AppendLine($"WARNING!: <br>");
            html.AppendLine($"Rules 3.2.1 and 4.1.1.2 have bullet points which do not translate and need manual entry. <br>");
            html.AppendLine($"Rules with names that include parantheses will end up in description.");

            foreach (var discipline in disciplines)
            {
                html.AppendLine($"<h5>{discipline.Name}</h5>");
                html.AppendLine($"<a href=\"{UtilityHelper.CreateFriendlyName(discipline.Name)}/{UtilityHelper.CreateFriendlyName(discipline.Name)}.html\">Read More...</a>");
            }

            return ParseStringBuilder(html);
        }
    }
}
