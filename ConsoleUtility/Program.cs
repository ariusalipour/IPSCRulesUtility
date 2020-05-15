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
            var tasks = new DoEverything();
            tasks.DoIt3();
        }
    }
}
