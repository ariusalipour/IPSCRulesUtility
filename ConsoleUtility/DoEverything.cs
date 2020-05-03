using System;
using System.Collections.Generic;
using System.Text;
using IPSCRulesLibrary.ObjectClasses;
using IPSCRulesLibrary.Services;

namespace ConsoleUtility
{
    public class DoEverything
    {
        public DoEverything()
        {

        }

        public void DoIt()
        {
            Console.WriteLine("Loading Rules Reader Service...");
            var rulesReader = new RulesReader();
            var glossaryReader = new GlossaryReader();
            Console.WriteLine("Rules Reader Service Initialized!");
            Console.WriteLine();
            Console.WriteLine("Please add source files to root of this utility file.");
            Console.WriteLine("Press Enter to Continue...");
            Console.ReadLine();

            var root = AppDomain.CurrentDomain.BaseDirectory;
            string[] filenames = { "ActionAir", "Handgun", "Shotgun", "Rifle", "MiniRifle", "PCC" };
            string[] languageNames = { "English" };

            Console.WriteLine();
            Console.WriteLine("Converting text files to OO Rulebooks");

            var converts = new Dictionary<string, RulesReader.ConversionResult>();
            var glossaries = new Dictionary<string, List<Glossary>>();


            var rootPath = $"{root}/English";

            foreach (var filename in filenames)
            {
                converts.Add(filename, rulesReader.ConvertFromTxtFile($"{rootPath}/{filename}.txt"));
                glossaries.Add(filename, glossaryReader.ConvertFromTxtFile($"{rootPath}/{filename} - Glossary.txt"));
            }


            Console.WriteLine("Conversions complete! Rules now parsed!");
            Console.WriteLine($"WARNINGS:");
            Console.WriteLine($"Rules 3.2.1 and 4.1.1.2 have bullet points which do not translate and need manual entry.");
            Console.WriteLine($"Rules with names that include parantheses will end up in description");
            Console.WriteLine();
            Console.WriteLine("Creating Rules Book from parsings!");

            var disciplines = new List<Discipline>();

            foreach (var convert in converts)
            {
                var glossaryList = glossaries[convert.Key];
                disciplines.Add(rulesReader.CreateRuleChapters(convert.Value, convert.Key, glossaryList));
            }

            Console.WriteLine("Individual rules parsed.");
            Console.WriteLine("");
            Console.WriteLine("Creating Combined rules book...");

            var ruleMerger = new RuleMerger();
            disciplines.Add(ruleMerger.MergeDisciplines(disciplines));

            Console.WriteLine("Rules book parsed. Printing...");
            Console.WriteLine();
            Console.WriteLine("Loading File Writer Service...");
            var fileWriter = new WebsiteCreator();
            Console.WriteLine("File Writer Service Initialized!");
            Console.WriteLine();
            Console.WriteLine("Creating Website Structure...");
            fileWriter.CreateWebsiteFilesDirectory(disciplines);
            Console.WriteLine();
            Console.WriteLine("Website Created!");
            Console.WriteLine();
            Console.WriteLine("Creating CSV and XML Files of Disciplines...");

            var csvParser = new CsvParser();
            var xmlParser = new XmlParser();

            var rulebook = new Rulebook() { Disciplines = disciplines };

            foreach (var discipline in disciplines)
            {
                csvParser.CreateCsvDiscipline(discipline);
                xmlParser.CreateXmlDiscipline(discipline);

                if (glossaries.ContainsKey(discipline.Name))
                {
                    csvParser.CreateCsvGlossary(discipline.Name, glossaries[discipline.Name]);
                    xmlParser.CreateXmlGlossary(discipline.Name, glossaries[discipline.Name]);
                }
            }

            xmlParser.CreateXmlRulebook(rulebook);

            Console.WriteLine("Individual disciplines parsed successfully");
            Console.WriteLine("Creating master file...");
            csvParser.CreateCsvDisciplines(disciplines);
            Console.WriteLine("Master file created!");
            var csvReader = new CsvReader();
            var readDisciplines = csvReader.ReadCsvDisciplines();
        }
    }
}
