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
        }

        public Guid DisciplineId;
        public string Name;

        public List<Chapter> Chapters;
    }
}
