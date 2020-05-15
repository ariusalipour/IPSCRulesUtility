using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class XmlFix
    {
        private static List<Discipline> _allDisciplines = new List<Discipline>();
        private static List<Section> _allSections = new List<Section>();
        private static List<Rule> _allRules = new List<Rule>();
        private static List<SubRule> _allSubRules = new List<SubRule>();
        private static List<Glossary> _allGlossaries = new List<Glossary>();
        private static List<Section> _sections = new List<Section>();
        private static List<Glossary> _glossaries = new List<Glossary>();
        private static List<Rule> _rules = new List<Rule>();
        private static List<SubRule> _subRules = new List<SubRule>();

        public Rulebook ConfigureRulebooks()
        {
            var rulebook = new Rulebook();
            
            // Load Rulebook.data file
            var xmlReader = new XmlReader();
            _allDisciplines = xmlReader.ReadXmlDisciplines();
            _allSections = _allDisciplines.SelectMany(o => o.Chapters).SelectMany(o => o.Sections).ToList();
            _allRules = _allSections.SelectMany(o => o.Rules).ToList();
            _allSubRules = _allRules.SelectMany(o => o.SubRules).ToList();

            // Load New Properties
            foreach (var discipline in _allDisciplines)
            {
                Console.WriteLine($"{discipline.Name} has started compiling...");
                // Add Glossaries (DisciplineId needs populating)
                discipline.GlossaryList.ForEach(o => o.DisciplineId = discipline.DisciplineId);
                _allGlossaries.AddRange(discipline.GlossaryList);

                foreach (var chapter in discipline.Chapters)
                {
                    Console.WriteLine($"--{chapter.Name} has started compiling...");

                    foreach (var section in chapter.Sections)
                    {
                        Console.WriteLine($"-- >>{section.Name} has started compiling...");

                        section.SearchableString = UtilityHelper.CreateSearchableString(section);

                        foreach (var rule in section.Rules)
                        {
                            rule.SearchableString = UtilityHelper.CreateSearchableString(rule);

                            rule.SpanList = CreateSpanList(rule.Description, rule.DisciplineId);

                            foreach (var subRule in rule.SubRules)
                            {
                                subRule.SearchableString = UtilityHelper.CreateSearchableString(subRule);

                                subRule.SpanList = CreateSpanList(subRule.Description, rule.DisciplineId);
                            }
                        }

                        Console.WriteLine($"-- >>{section.Name} has finished compiling...");
                    }

                    Console.WriteLine($"--{chapter.Name} has finished compiling...");
                }

                Console.WriteLine($"{discipline.Name} has finished compiling...");
            }

            rulebook.Disciplines = _allDisciplines;

            return rulebook;
        }

        private static List<RuleSpan> CreateSpanList(string description, Guid disciplineId)
        {
            var spanList = new List<RuleSpan>();

            var sectionList = _allSections.Where(o => o.DisciplineId == disciplineId);
            var glossaryList = _allGlossaries.Where(o => o.DisciplineId == disciplineId);
            var ruleList = _allRules.Where(o => o.DisciplineId == disciplineId);
            var subRuleList = _allSubRules.Where(o => o.DisciplineId == disciplineId);

            foreach (var subRule in subRuleList.Reverse())
            {
                var regex = new Regex($"(?<=\\s|^)({subRule.Numeric.Replace(".", "\\.")})(?=\\s|\\S\\s|\\)|\\S?$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                var match = regex.Match(description);

                if (match.Success)
                {
                    description = regex.Replace(description, $"|{match.Value}|");
                }
            }

            foreach (var rule in ruleList.Reverse())
            {
                if (string.IsNullOrWhiteSpace(rule.Numeric))
                    continue;

                var regex = new Regex($"(?<=\\s|^)({rule.Numeric.Replace(".", "\\.")})(?=\\s|\\S\\s|\\)|\\S?$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                var match = regex.Match(description);

                if (match.Success)
                {
                    description = regex.Replace(description, $"|{match.Value}|");
                }
            }

            foreach (var glossaryItem in glossaryList)
            {
                var regex = new Regex($"(?<=\\W|^)((({glossaryItem.Name.Replace("(s)", "(s?)").Replace(" (", "))|(((").Replace(" / ", ")|(").Trim()})))(?=\\W|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                var match = regex.Match(description);

                if (match.Success)
                {
                    description = regex.Replace(description, $"|{match.Value}|");
                }
            }

            foreach (var section in sectionList.Reverse())
            {
                var regex = new Regex($"(?<=\\s|^)({section.Numeric.Replace(".", "\\.")})(?=\\s|\\S\\s|\\)|\\S?$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                var match = regex.Match(description);

                if (match.Success)
                {
                    description = regex.Replace(description, $"|{match.Value}|");
                }
            }
            
            var descriptionArray = description.Split('|');

            foreach (var split in descriptionArray)
            {
                var splitAdded = false;
                
                foreach (var subRule in subRuleList.Reverse())
                {
                    var regex = new Regex($"(?<=\\s|^)({subRule.Numeric.Replace(".", "\\.")})(?=\\s|\\S\\s|\\)|\\S?$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                    var match = regex.Match(split);

                    if (match.Success)
                    {
                        spanList.Add(new RuleSpan()
                        {
                            Text = split,
                            SpanType = RuleSpan.SpanTypeEnum.SubRuleReference,
                            SubRuleId = subRule.SubRuleId
                        });

                        splitAdded = true;
                        break;
                    }
                }

                if (splitAdded)
                    continue;

                foreach (var rule in ruleList.Reverse())
                {
                    if (string.IsNullOrWhiteSpace(rule.Numeric))
                        continue;

                    var regex = new Regex($"(?<=\\s|^)({rule.Numeric.Replace(".", "\\.")})(?=\\s|\\S\\s|\\)|\\S?$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                    var match = regex.Match(split);

                    if (match.Success)
                    {
                        spanList.Add(new RuleSpan()
                        {
                            Text = split,
                            SpanType = RuleSpan.SpanTypeEnum.RuleReference,
                            RuleId = rule.RuleId
                        });

                        splitAdded = true;
                        break;
                    }
                }

                if (splitAdded)
                    continue;

                foreach (var section in sectionList.Reverse())
                {
                    if (string.IsNullOrWhiteSpace(section.Numeric))
                        continue;

                    var regex = new Regex($"(?<=\\s|^)({section.Numeric.Replace(".", "\\.")})(?=\\s|\\S\\s|\\)|\\S?$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                    var match = regex.Match(split);

                    if (match.Success)
                    {
                        spanList.Add(new RuleSpan()
                        {
                            Text = split,
                            SpanType = RuleSpan.SpanTypeEnum.SectionReference,
                            RuleId = section.SectionId
                        });

                        splitAdded = true;
                        break;
                    }
                }

                if (splitAdded)
                    continue;

                foreach (var glossaryItem in glossaryList)
                {
                    var regex = new Regex($"(?<=\\W|^)((({glossaryItem.Name.Replace("(s)", "(s?)").Replace(" (", "))|(((").Replace(" / ", ")|(").Trim()})))(?=\\W|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

                    var match = regex.Match(split);

                    if (match.Success)
                    {
                        spanList.Add(new RuleSpan()
                        {
                            Text = split,
                            SpanType = RuleSpan.SpanTypeEnum.GlossaryTooltip,
                            GlossaryId = glossaryItem.GlossaryId
                        });

                        splitAdded = true;
                        break;
                    }
                }

                if (splitAdded)
                    continue;

                // NO Reference Found
                spanList.Add(new RuleSpan()
                {
                    Text = split,
                    SpanType = RuleSpan.SpanTypeEnum.Normal
                });
            }

            return spanList;
        }
    }
}
