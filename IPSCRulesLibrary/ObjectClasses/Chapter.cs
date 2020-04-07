using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Chapter
    {
        public Chapter()
        {
            ChapterId = Guid.NewGuid();
            Sections = new List<Section>();
        }

        public Guid ChapterId;
        public Guid DisciplineId;
        public string Numeric;
        public string Name;
        public string Description;

        public List<Section> Sections;
    }
}
