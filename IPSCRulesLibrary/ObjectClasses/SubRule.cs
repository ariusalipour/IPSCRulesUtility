using System;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class SubRule
    {
        public SubRule()
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
