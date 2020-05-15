using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class Rule : RuleBase
    {
        public Rule()
        {
            RuleId = Guid.NewGuid();
            SubRules = new List<SubRule>();
        }

        public List<RuleSpan> SpanList;
        public string SearchableString;
        public List<SubRule> SubRules;
    }

    public class RuleBase
    {
        public RuleBase()
        {
            RuleId = Guid.NewGuid();
        }

        public Guid RuleId;
        public string Numeric;
        public string Name;
        public string Description;
        public Guid SectionId;
        public Guid DisciplineId;
    }
}
