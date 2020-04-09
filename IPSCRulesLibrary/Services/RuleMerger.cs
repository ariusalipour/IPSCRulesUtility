using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class RuleMerger
    {
        private Dictionary<string, Guid> _disciplineGuids;
        private Dictionary<Guid, string> _disciplineNames;

        public RuleMerger()
        {
            _disciplineGuids = new Dictionary<string, Guid>();
            _disciplineNames = new Dictionary<Guid, string>();
        }

        public Discipline MergeDisciplines(List<Discipline> disciplines)
        {
            var mergedDiscipline = new Discipline()
            {
                DisciplineId = Guid.Empty,
                Name = "Combined"
            };

            _disciplineGuids = disciplines.ToDictionary(o => o.Name, o => o.DisciplineId);
            _disciplineNames = disciplines.ToDictionary(o => o.DisciplineId, o => o.Name);

            var allChapters = disciplines.SelectMany(o => o.Chapters);

            var allChaptersLookup = allChapters.ToLookup(o => o.Name, o => o);

            foreach (var combinedChapters in allChaptersLookup)
            {
                mergedDiscipline.Chapters.Add(MergeChapters(combinedChapters, mergedDiscipline.DisciplineId));
            }

            return mergedDiscipline;
        }

        public Chapter MergeChapters(IGrouping<string, Chapter> combinedChapters, Guid disciplineId)
        {
            var mergedChapter = new Chapter()
            {
                Description = combinedChapters.First().Description,
                Numeric = combinedChapters.First().Numeric,
                DisciplineId = disciplineId
            };

            var disciplineCount = _disciplineGuids.Count;
            var chapterCount = combinedChapters.Count();
            
            if (disciplineCount == chapterCount)
            {
                mergedChapter.Name = combinedChapters.Key;
            }
            else
            {
                var chapterOwners = combinedChapters.Select(o => _disciplineNames[o.DisciplineId]);
                var chapterNames = new StringBuilder();

                chapterNames.Append("(");

                foreach (var owner in chapterOwners)
                {
                    chapterNames.Append(owner);

                    if (owner != chapterOwners.Last())
                    {
                        chapterNames.Append(" | ");
                    }
                }

                chapterNames.Append(" only)");

                mergedChapter.Name = $"{combinedChapters.Key} {chapterNames}";
            }

            var allSections = combinedChapters.SelectMany(o => o.Sections);

            var allSectionsLookup = allSections.ToLookup(o => o.Name, o => o);

            foreach (var combinedSections in allSectionsLookup)
            {
                mergedChapter.Sections.Add(MergeSections(combinedSections, mergedChapter.ChapterId));
            }

            return mergedChapter;
        }

        public Section MergeSections(IGrouping<string, Section> combinedSections, Guid chapterId)
        {
            var mergedSection = new Section()
            {
                Description = combinedSections.First().Description,
                Numeric = combinedSections.First().Numeric,
                ChapterId = chapterId
            };

            var disciplineCount = _disciplineGuids.Count;
            var sectionCount = combinedSections.Count();

            if (disciplineCount == sectionCount)
            {
                mergedSection.Name = combinedSections.Key;
            }
            else
            {
                var sectionOwners = combinedSections.Select(o => _disciplineNames[o.DisciplineId]);
                var sectionNames = new StringBuilder();

                sectionNames.Append("(");

                
                foreach (var owner in sectionOwners)
                {
                    sectionNames.Append(owner);

                    if (owner != sectionOwners.Last())
                    {
                        sectionNames.Append(" | ");
                    }
                }

                sectionNames.Append(" only)");

                mergedSection.Name = $"{combinedSections.Key} {sectionNames}";
            }

            var allRules = combinedSections.SelectMany(o => o.Rules);

            var allRulesLookup = allRules.ToLookup(o => o.Description, o => o);

            foreach (var combinedRules in allRulesLookup)
            {
                mergedSection.Rules.Add(MergeRules(combinedRules, mergedSection.SectionId));
            }

            return mergedSection;
        }

        public Rule MergeRules(IGrouping<string, Rule> combinedRules, Guid sectionId)
        {
            var mergedRule = new Rule()
            {
                Numeric = combinedRules.First().Numeric,
                Description = combinedRules.Key,
                SectionId = sectionId
            };

            var disciplineCount = _disciplineGuids.Count;
            var ruleCount = combinedRules.Count();

            if (disciplineCount == ruleCount)
            {
                mergedRule.Name = combinedRules.First().Name;
            }
            else
            {
                var ruleOwners = combinedRules.Select(o => _disciplineNames[o.DisciplineId]);
                var ruleNames = new StringBuilder();

                ruleNames.Append("(");

                foreach (var owner in ruleOwners)
                {
                    ruleNames.Append(owner);

                    if (owner != ruleOwners.Last())
                    {
                        ruleNames.Append(" | ");
                    }
                }

                ruleNames.Append(" only)");

                mergedRule.Name = $"{mergedRule.Name} {ruleNames}";
            }

            var allSubRules = combinedRules.SelectMany(o => o.SubRules);

            var allSubRulesLookup = allSubRules.ToLookup(o => o.Description, o => o);

            foreach (var combinedSubRules in allSubRulesLookup)
            {
                mergedRule.SubRules.Add(MergeSubRules(combinedSubRules, mergedRule.RuleId));
            }

            return mergedRule;
        }

        public SubRule MergeSubRules(IGrouping<string, SubRule> combinedRules, Guid ruleId)
        {
            var mergedSubRule = new SubRule()
            {
                Numeric = combinedRules.First().Numeric,
                Description = combinedRules.Key,
                RuleId = ruleId
            };

            var disciplineCount = _disciplineGuids.Count;
            var subRuleCount = combinedRules.Count();

            if (disciplineCount == subRuleCount)
            {
                mergedSubRule.Name = combinedRules.First().Name;
            }
            else
            {
                var subRuleOwners = combinedRules.Select(o => _disciplineNames[o.DisciplineId]);
                var subRuleNames = new StringBuilder();

                subRuleNames.Append("(");

                foreach (var owner in subRuleOwners)
                {
                    subRuleNames.Append(owner);

                    if (owner != subRuleOwners.Last())
                    {
                        subRuleNames.Append(" | ");
                    }
                }

                subRuleNames.Append(" only)");

                mergedSubRule.Name = $"{mergedSubRule.Name} {subRuleNames}";
            }

            return mergedSubRule;
        }
    }
}
