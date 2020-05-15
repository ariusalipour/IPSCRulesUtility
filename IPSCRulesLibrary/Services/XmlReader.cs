using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class XmlReader
    {
        public List<Discipline> ReadXmlDisciplines()
        {
            var file = UtilityHelper.ReadStreamFromFile("OldRulebook.data");
            var rulebook = new Rulebook();

            var serializer = new XmlSerializer(rulebook.GetType());

            rulebook = (Rulebook)serializer.Deserialize(file);

            foreach (var rulebookDiscipline in rulebook.Disciplines)
            {
                var glossarySection = rulebookDiscipline.Chapters.SelectMany(o => o.Sections)
                    .First(o => o.Name == "Glossary");

                var glossary = rulebookDiscipline.GlossaryList;

                foreach (var glossaryItem in glossary)
                {
                    glossarySection.Rules.Add(new Rule()
                    {
                        RuleId = glossaryItem.GlossaryId,
                        Name = glossaryItem.Name,
                        Description = glossaryItem.Definition,
                        DisciplineId = rulebookDiscipline.DisciplineId
                    });
                }
            }

            return rulebook.Disciplines;
        }
    }
}
