using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Section
    {
        public Section()
        {
            SectionId = Guid.NewGuid();
            Rules = new List<Rule>();
        }

        public Guid SectionId;
        public string Numeric;
        public string Name;
        public string Description;
        public Guid ChapterId;
        public Guid DisciplineId;
        public string SearchableString;
        public List<Rule> Rules;
    }
}
