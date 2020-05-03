using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IPSCRulesLibrary.ObjectClasses;

namespace IPSCRulesLibrary.Services
{
    public class XmlParser
    {
        public void CreateXmlRulebook(Rulebook rulebook)
        {
            var xmlFile = new MemoryStream();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = $"{rootPath}/xml";

            var serializer = new XmlSerializer(rulebook.GetType());

            serializer.Serialize(xmlFile, rulebook);

            UtilityHelper.CreateUpdateFile(xmlPath, "Rulebook.data", Encoding.UTF8.GetString(xmlFile.ToArray()));
        }

        public void CreateXmlDiscipline(Discipline discipline)
        {
            var xmlFile = new MemoryStream();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = $"{rootPath}/xml";

            var serializer = new XmlSerializer(discipline.GetType());

            serializer.Serialize(xmlFile, discipline);

            UtilityHelper.CreateUpdateFile(xmlPath, $"{discipline.Name}.rules", Encoding.UTF8.GetString(xmlFile.ToArray()));
        }

        public void CreateXmlGlossary(string disciplineName, List<Glossary> glossaries)
        {
            var xmlFile = new MemoryStream();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = $"{rootPath}/xml";

            var serializer = new XmlSerializer(glossaries.GetType());

            serializer.Serialize(xmlFile, glossaries);

            UtilityHelper.CreateUpdateFile(xmlPath, $"{disciplineName}.glossary", Encoding.ASCII.GetString(xmlFile.ToArray()));
        }
    }
}
