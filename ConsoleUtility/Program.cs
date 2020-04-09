using System;
using System.Collections.Generic;
using IPSCRulesLibrary.ObjectClasses;
using IPSCRulesLibrary.Services;

namespace ConsoleUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading Rules Reader Service...");
            var rulesReader = new RulesReader();
            Console.WriteLine("Rules Reader Service Initialized!");
            Console.WriteLine();
            Console.WriteLine("Please add source files to root of this utility file.");
            Console.WriteLine("Press Enter to Continue...");
            Console.ReadLine();

            var rootPath = AppDomain.CurrentDomain.BaseDirectory;
            string[] filenames = {"Action Air.txt", "Handgun.txt", "Shotgun.txt", "Rifle.txt", "Mini-Rifle.txt", "PCC.txt"};

            Console.WriteLine();
            Console.WriteLine("Converting text files to OO Rulebooks");

            var converts = new Dictionary<string, RulesReader.ConversionResult>();

            foreach (var filename in filenames)
            {
                converts.Add(filename, rulesReader.ConvertFromTxtFile($"{rootPath}/{filename}"));
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
                disciplines.Add(rulesReader.CreateRuleChapters(convert.Value, convert.Key));
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
            Console.WriteLine("Creating CSV Files of Disciplines...");
            var csvParser = new CsvParser();
            foreach (var discipline in disciplines)
            {
                csvParser.CreateCsvDiscipline(discipline);
            }
            Console.WriteLine("Individual disciplines parsed successfully");
            Console.WriteLine("Creating master file...");
            csvParser.CreateCsvDisciplines(disciplines);
            Console.WriteLine("Master file created!");
            var csvReader = new CsvReader();
            var readDisciplines = csvReader.ReadCsvDisciplines();
        }
    }
}
