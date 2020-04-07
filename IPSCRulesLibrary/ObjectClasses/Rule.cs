using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Rule
    {
        public Rule()
        {
            RuleId = Guid.NewGuid();
            SubRules = new List<SubRule>();
        }

        public Guid RuleId;
        public string Numeric;
        public string Name;
        public string Description;
        public Guid SectionId;
        public Guid DisciplineId;

        public List<SubRule> SubRules;
    }
}
