using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Discipline
    {
        public Discipline()
        {
            DisciplineId = Guid.NewGuid();
            Chapters = new List<Chapter>();
            GlossaryList = new List<Glossary>();
        }

        public string Language;
        public Guid DisciplineId;
        public string Name;
        public List<Glossary> GlossaryList;

        public List<Chapter> Chapters;
    }
}
