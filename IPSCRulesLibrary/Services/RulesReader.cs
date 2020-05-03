using IPSCRulesLibrary.ObjectClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace IPSCRulesLibrary.Services
{
    public class RulesReader
    {
        private Regex _chapterRegex = new Regex(@"^CHAPTER\s(?<numeric>\d+):\s(?<name>.+)");
        private Regex _sectionRegex = new Regex(@"^(?<numeric>\d+\.\d+)\s(?<name>.+)");
        private Regex _ruleRegex = new Regex(@"^(?<numeric>\d+\.\d+\.\d+)\s((?<name>((\w+)\s)+)(-|–|�)\s)?(?<description>.+)");
        private Regex _subRuleRegex = new Regex(@"^(?<numeric>\d+\.\d+\.\d+\.\d+)\s((?<name>((\w+)\s)+)(-|–|�)\s)?(?<description>.+)");
        private Regex _numericRegex = new Regex(@"^(?<chapter>\d+)(\.(?<section>\d+)(\.(?<rule>\d+)(\.(?<subrule>\d+))?)?)?");
        
        public RulesReader()
        {

        }

        public Discipline CreateRuleChapters(ConversionResult input, string disciplineName, List<Glossary> glossaryList)
        {
            var discipline = new Discipline()
            {
                Name = disciplineName.Replace(".txt", ""),
                Language = "English",
                GlossaryList = glossaryList
            };

            var subRules = input.SubRules;
            var rulesDictionary = input.Rules.ToDictionary(o => o.Numeric, p => p);
            var sectionsDictionary = input.Sections.ToDictionary(o => o.Numeric, p => p);
            var chaptersDictionary = input.Chapters.ToDictionary(o => o.Numeric, p => p);

            foreach (var subRule in subRules)
            {
                var numericRegex = _numericRegex.Match(subRule.Numeric);

                var ruleNumeric = $"{numericRegex.Groups["chapter"].Value}.{numericRegex.Groups["section"].Value}.{numericRegex.Groups["rule"].Value}";

                subRule.DisciplineId = discipline.DisciplineId;
                subRule.RuleId = rulesDictionary[ruleNumeric].RuleId;

                rulesDictionary[ruleNumeric].SubRules.Add(subRule);
            }

            foreach (var rule in rulesDictionary.Values)
            {
                var numericRegex = _numericRegex.Match(rule.Numeric);

                var sectionNumeric = $"{numericRegex.Groups["chapter"].Value}.{numericRegex.Groups["section"].Value}";

                rule.DisciplineId = discipline.DisciplineId;
                rule.SectionId = sectionsDictionary[sectionNumeric].SectionId;

                sectionsDictionary[sectionNumeric].Rules.Add(rule);
            }

            foreach (var section in sectionsDictionary.Values)
            {
                var numericRegex = _numericRegex.Match(section.Numeric);

                var chapterNumeric = $"{numericRegex.Groups["chapter"].Value}";

                section.DisciplineId = discipline.DisciplineId;
                section.ChapterId = chaptersDictionary[chapterNumeric].ChapterId;

                chaptersDictionary[chapterNumeric].Sections.Add(section);
            }

            foreach (var chapter in chaptersDictionary.Values)
            {
                chapter.DisciplineId = discipline.DisciplineId;
                discipline.Chapters.Add(chapter);
            }

            return discipline;
        }

        public ConversionResult ConvertFromTxtFile(string filePath)
        {
            var fileArray = UtilityHelper.ReadFromFile(filePath);

            var result = ConvertToRules(fileArray);

            return result;
        }

        private ConversionResult ConvertToRules(string[] fileArray)
        {

            var extractedChapters = new List<Chapter>();
            var extractedSections = new List<Section>();
            var extractedRules = new List<Rule>();
            var extractedSubRules = new List<SubRule>();

            var lastLineWasChapter = false;
            var lastLineWasSection = false;

            foreach (var line in fileArray)
            {
                // Check for Empty Line

                if (string.IsNullOrWhiteSpace(line)) continue;

                // Check for Chapter Line

                var chapterRegex = _chapterRegex.Match(line);

                if (chapterRegex.Success)
                {
                    extractedChapters.Add(new Chapter()
                    {
                        Numeric = chapterRegex.Groups["numeric"].Value.Trim(),
                        Name = chapterRegex.Groups["name"].Value.Trim(),
                        Description = chapterRegex.Groups["description"].Value.Trim()
                    });

                    lastLineWasChapter = true;
                    lastLineWasSection = false;

                    continue;
                }

                // Check for Section Line

                var sectionRegex = _sectionRegex.Match(line);

                if (sectionRegex.Success)
                {
                    extractedSections.Add(new Section()
                    {
                        Numeric = sectionRegex.Groups["numeric"].Value.Trim(),
                        Name = sectionRegex.Groups["name"].Value.Trim(),
                        Description = sectionRegex.Groups["description"].Value.Trim()
                    });

                    lastLineWasChapter = false;
                    lastLineWasSection = true;

                    continue;
                }

                // Check for Rule Line

                var ruleRegex = _ruleRegex.Match(line);

                if (ruleRegex.Success)
                {
                    extractedRules.Add(new Rule()
                    {
                        Numeric = ruleRegex.Groups["numeric"].Value.Trim(),
                        Name = ruleRegex.Groups["name"].Value.Trim(),
                        Description = ruleRegex.Groups["description"].Value.Trim()
                    });

                    lastLineWasChapter = false;
                    lastLineWasSection = false;

                    continue;
                }

                // Check for SubRule Line

                var subRuleRegex = _subRuleRegex.Match(line);

                if (subRuleRegex.Success)
                {
                    extractedSubRules.Add(new SubRule()
                    {
                        Numeric = subRuleRegex.Groups["numeric"].Value.Trim(),
                        Name = subRuleRegex.Groups["name"].Value.Trim(),
                        Description = subRuleRegex.Groups["description"].Value.Trim()
                    });

                    lastLineWasChapter = false;
                    lastLineWasSection = false;

                    continue;
                }
                
                // Check for Chapter Description

                if (lastLineWasChapter)
                {
                    extractedChapters.Last().Description = line;

                    lastLineWasChapter = false;
                    lastLineWasSection = false;
                }

                // Check for Section Description

                if (lastLineWasSection)
                {
                    extractedSections.Last().Description = line;

                    lastLineWasChapter = false;
                    lastLineWasSection = false;
                }
            }


            return new ConversionResult()
            {
                Chapters = extractedChapters,
                Sections = extractedSections,
                Rules = extractedRules,
                SubRules = extractedSubRules
            };
        }

        public class ConversionResult
        {
            public List<Chapter> Chapters;
            public List<Section> Sections;
            public List<Rule> Rules;
            public List<SubRule> SubRules;
        }
    }
}
