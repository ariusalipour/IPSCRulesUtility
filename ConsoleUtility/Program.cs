using System;
using System.Collections.Generic;
using System.IO;
using IPSCRulesLibrary.ObjectClasses;
using IPSCRulesLibrary.Services;

namespace ConsoleUtility
{
    class Program
    {
        static void Main(string[] args)
        {
           
            var rulesReader = new RulesReader();
            var glossaryReader = new GlossaryReader();
            var xmlParser = new XmlParser();

            var rulebook = new Rulebook() { Disciplines = new List<Discipline>() };

            var root = AppDomain.CurrentDomain.BaseDirectory;
            string[] filenames = {"ActionAir", "Handgun", "Shotgun", "Rifle", "MiniRifle", "PCC"};
            string[] languageNames = {"English", "Brazilian"};


            var converts = new Dictionary<string, RulesReader.ConversionResult>();
            var glossaries = new Dictionary<string, List<Glossary>>();

            foreach (var languageName in languageNames)
            {
                var rootPath = $"{root}/{languageName}";

                if (!Directory.Exists(rootPath))
                    continue;

                foreach (var filename in filenames)
                {
                    if (File.Exists($"{rootPath}/{filename}.txt"))
                        converts.Add(filename, rulesReader.ConvertFromTxtFile($"{rootPath}/{filename}.txt"));

                    if (File.Exists($"{rootPath}/{filename} - Glossary.txt"))
                        glossaries.Add(filename, glossaryReader.ConvertFromTxtFile($"{rootPath}/{filename} - Glossary.txt"));
                }

                var disciplines = new List<Discipline>();

                foreach (var convert in converts)
                {
                    var glossaryList = glossaries[convert.Key];
                    disciplines.Add(rulesReader.CreateRuleChapters(convert.Value, convert.Key, glossaryList));
                }

                rulebook.Disciplines.AddRange(disciplines);
            }

            xmlParser.CreateXmlRulebook(rulebook);
        }
    }
}
