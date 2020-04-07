using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Discipline
    {
        public Guid DisciplineId;
        public string Name;

        public List<Chapter> Chapters;
    }
}
