using System;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Glossary
    {
        public Glossary()
        {
            GlossaryId = Guid.NewGuid();
        }

        public Guid GlossaryId;
        public Guid DisciplineId;
        public string Name;
        public string Definition;
    }
}
