using System;
using System.Collections.Generic;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class SubRule : SubRuleBase
    {
        public SubRule()
        {
            SubRuleId = Guid.NewGuid();
        }

        public string SearchableString;
        public List<RuleSpan> SpanList;
    }

    public class SubRuleBase
    {
        public SubRuleBase()
        {
            SubRuleId = Guid.NewGuid();
        }

        public Guid SubRuleId;
        public string Numeric;
        public string Name;
        public string Description;
        public Guid RuleId;
        public Guid DisciplineId;
    }
}
