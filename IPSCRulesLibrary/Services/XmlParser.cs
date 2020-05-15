using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using IPSCRulesLibrary.ObjectClasses;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace IPSCRulesLibrary.Services
{
    public class XmlParser
    {
        public void CreateXmlRulebook(Rulebook rulebook)
        {
            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            var xmlPath = $"{rootPath}/xml";

            var settings = new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}; 
            var serialisedText = JsonConvert.SerializeObject(rulebook, Newtonsoft.Json.Formatting.Indented, settings);

            using (var xmlFile = GenerateStreamFromString(serialisedText))
            {
                UtilityHelper.CreateUpdateFile(xmlPath, "Rulebook.data", Encoding.UTF8.GetString(xmlFile.ToArray()));
            }
        }

        public MemoryStream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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
