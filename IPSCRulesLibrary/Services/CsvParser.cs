using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class CsvParser
    {
        private readonly string _separator = ",";
        private readonly string _fileExtension = ".csv";

        public CsvParser() { }

        public void CreateCsvDiscipline(Discipline discipline)
        {
            var csv = new StringBuilder();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var csvPath = $"{rootPath}/csv";

            csv = InsertDisciplineData(discipline, csv);

            csv = InsertChapterData(discipline.Chapters, csv);

            var sections = discipline.Chapters.SelectMany(o => o.Sections);
            csv = InsertSectionData(sections, csv);

            var rules = sections.SelectMany(o => o.Rules);
            csv = InsertRuleData(rules, csv);

            var subRules = rules.SelectMany(o => o.SubRules);
            csv = InsertSubRuleData(subRules, csv);

            UtilityHelper.CreateUpdateFile(csvPath, $"{discipline.Name}{_fileExtension}", csv.ToString());
        }

        public void CreateCsvGlossary(string disciplineName, List<Glossary> glossaries)
        {
            var csv = new StringBuilder();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var csvPath = $"{rootPath}/csv";

            csv = InsertGlossaryData(glossaries);

            UtilityHelper.CreateUpdateFile(csvPath, $"{disciplineName} - Glossary{_fileExtension}", csv.ToString());
        }

        private string CsvFriendlyString(string text)
        {
            return text.Replace(_separator, "~");
        }

        public void CreateCsvDisciplines(List<Discipline> disciplines)
        {
            var csv = new StringBuilder();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var csvPath = $"{rootPath}/csv";

            foreach (var discipline in disciplines)
            {
                csv = InsertDisciplineData(discipline, csv);
            }

            var chapters = disciplines.SelectMany(o => o.Chapters);
            csv = InsertChapterData(chapters, csv);

            var sections = chapters.SelectMany(o => o.Sections);
            csv = InsertSectionData(sections, csv);

            var rules = sections.SelectMany(o => o.Rules);
            csv = InsertRuleData(rules, csv);

            var subRules = rules.SelectMany(o => o.SubRules);
            csv = InsertSubRuleData(subRules, csv);

            UtilityHelper.CreateUpdateFile(csvPath, $"AllDisciplines{_fileExtension}", csv.ToString());
        }

        public StringBuilder InsertGlossaryData(List<Glossary> glossaries)
        {
            var csv = new StringBuilder();

            foreach (var glossary in glossaries)
            {
                csv.AppendLine(
                    $"{glossary.GlossaryId}{_separator}{CsvFriendlyString(glossary.Name)}{_separator}{CsvFriendlyString(glossary.Definition)}");
            }

            return csv;
        }

        public StringBuilder InsertDisciplineData(Discipline discipline, StringBuilder csv)
        {
            // DisciplineId{_separator}DisciplineName
            csv.AppendLine($"DISCIPLINE{_separator}{discipline.DisciplineId}{_separator}{discipline.Name}");

            return csv;
        }

        public StringBuilder InsertChapterData(IEnumerable<Chapter> chapters, StringBuilder csv)
        {
            foreach (var chapter in chapters)
            {
                // ChapterId{_separator}ChapterName{_separator}ChapterNumeric{_separator}ChapterDescription{_separator}DisciplineId
                csv.AppendLine($"CHAPTER{_separator}{chapter.ChapterId}{_separator}{CsvFriendlyString(chapter.Name)}{_separator}{chapter.Numeric}{_separator}{CsvFriendlyString(chapter.Description)}{_separator}{chapter.DisciplineId}");
            }

            return csv;
        }

        public StringBuilder InsertSectionData(IEnumerable<Section> sections, StringBuilder csv)
        {
            foreach (var section in sections)
            {
                // SectionId{_separator}SectionName{_separator}SectionNumeric{_separator}SectionDescription{_separator}ChapterId{_separator}DisciplineId
                csv.AppendLine(
                    $"SECTION{_separator}{section.SectionId}{_separator}{CsvFriendlyString(section.Name)}{_separator}{section.Numeric}{_separator}{CsvFriendlyString(section.Description)}{_separator}{section.ChapterId}{_separator}{section.DisciplineId}");
            }

            return csv;
        }

        public StringBuilder InsertRuleData(IEnumerable<Rule> rules, StringBuilder csv)
        {
            foreach (var rule in rules)
            {
                // RuleId{_separator}RuleName{_separator}RuleNumeric{_separator}RuleDescription{_separator}SectionId{_separator}DisciplineId
                csv.AppendLine(
                    $"RULE{_separator}{rule.RuleId}{_separator}{CsvFriendlyString(rule.Name)}{_separator}{rule.Numeric}{_separator}{CsvFriendlyString(rule.Description)}{_separator}{rule.SectionId}{_separator}{rule.DisciplineId}");
            }

            return csv;
        }

        public StringBuilder InsertSubRuleData(IEnumerable<SubRule> subRules, StringBuilder csv)
        {
            foreach (var subRule in subRules)
            {
                // SubRuleId{_separator}SubRuleName{_separator}SubRuleNumeric{_separator}SubRuleDescription{_separator}RuleId{_separator}DisciplineId
                csv.AppendLine(
                    $"SUBRULE{_separator}{subRule.SubRuleId}{_separator}{CsvFriendlyString(subRule.Name)}{_separator}{subRule.Numeric}{_separator}{CsvFriendlyString(subRule.Description)}{_separator}{subRule.RuleId}{_separator}{subRule.DisciplineId}");
            }

            return csv;
        }
    }
}
