using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class CsvReader
    {
        private readonly string _separator = ",";
        private readonly string _fileExtension = ".csv";

        public CsvReader() { }

        private string CleanFriendlyString(string text)
        {
            return text.Replace("~", _separator);
        }

        public List<Discipline> ReadCsvDisciplines(string disciplineName = "AllDisciplines")
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var csvPath = $"{rootPath}/csv";
            var csvFile = $"{disciplineName}{_fileExtension}";

            var file = UtilityHelper.ReadFromFile($"{csvPath}/{csvFile}");

            var disciplines = new List<Discipline>();
            var chapters = new List<Chapter>();
            var sections = new List<Section>();
            var rules = new List<Rule>();
            var subRules = new List<SubRule>();

            // Get object classes
            foreach (var line in file)
            {
                var lineSegment = line.Split(_separator);

                switch (lineSegment[0])
                {
                    case "DISCIPLINE":
                    {
                        var discipline = new Discipline
                        {
                            DisciplineId = new Guid(lineSegment[1]), 
                            Name = lineSegment[2]
                        };


                        disciplines.Add(discipline);

                        break;
                    }
                    case "CHAPTER":
                    {
                        var chapter = new Chapter
                        {
                            ChapterId = new Guid(lineSegment[1]),
                            Name = CleanFriendlyString(lineSegment[2]),
                            Numeric = lineSegment[3],
                            Description = CleanFriendlyString(lineSegment[4]),
                            DisciplineId = new Guid(lineSegment[5])
                        };


                        chapters.Add(chapter);

                        break;
                    }
                    case "SECTION":
                    {
                        var section = new Section
                        {
                            SectionId = new Guid(lineSegment[1]),
                            Name = CleanFriendlyString(lineSegment[2]),
                            Numeric = lineSegment[3],
                            Description = CleanFriendlyString(lineSegment[4]),
                            ChapterId = new Guid(lineSegment[5]),
                            DisciplineId = new Guid(lineSegment[6])
                        };


                        sections.Add(section);    

                        break;
                    }
                    case "RULE":
                    {
                        var rule = new Rule()
                        {
                            RuleId = new Guid(lineSegment[1]),
                            Name = CleanFriendlyString(lineSegment[2]),
                            Numeric = lineSegment[3],
                            Description = CleanFriendlyString(lineSegment[4]),
                            SectionId = new Guid(lineSegment[5]),
                            DisciplineId = new Guid(lineSegment[6])
                        };

                        rules.Add(rule);

                        break;
                    }
                    case "SUBRULE":
                    {
                        var subRule = new SubRule()
                        {
                            SubRuleId = new Guid(lineSegment[1]),
                            Name = CleanFriendlyString(lineSegment[2]),
                            Numeric = lineSegment[3],
                            Description = CleanFriendlyString(lineSegment[4]),
                            RuleId = new Guid(lineSegment[5]),
                            DisciplineId = new Guid(lineSegment[6])
                        };

                        subRules.Add(subRule);
                        
                        break;
                    }
                }
            }

            // Sort object classes
            foreach (var discipline in disciplines)
            {
                var subRulesLookup = subRules.Where(o => o.DisciplineId == discipline.DisciplineId)
                    .ToLookup(o => o.RuleId, o => o);

                foreach (var subRulesGroup in subRulesLookup)
                {
                    var rule = rules.First(o => o.RuleId == subRulesGroup.Key);
                    rule.SubRules = subRulesGroup.ToList();
                }

                var rulesLookup = rules.Where(o => o.DisciplineId == discipline.DisciplineId)
                    .ToLookup(o => o.SectionId, o => o);

                foreach (var rulesGroup in rulesLookup)
                {
                    var section = sections.First(o => o.SectionId == rulesGroup.Key);
                    section.Rules = rulesGroup.ToList();
                }

                var sectionLookup = sections.Where(o => o.DisciplineId == discipline.DisciplineId)
                    .ToLookup(o => o.ChapterId, o => o);

                foreach (var sectionsGroup in sectionLookup)
                {
                    var chapter = chapters.First(o => o.ChapterId == sectionsGroup.Key);
                    chapter.Sections = sectionsGroup.ToList();
                }

                discipline.Chapters = chapters.Where(o => o.DisciplineId == discipline.DisciplineId).ToList();
            }
            

            return disciplines;
        }
    }
}
