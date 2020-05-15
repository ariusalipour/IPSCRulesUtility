using System;
using System.Collections.Generic;
using System.Text;

namespace IPSCRulesLibrary.ObjectClasses
{
    public class RuleSpan
    {
        public RuleSpan() { }

        public string Text;
        public SpanTypeEnum SpanType;
        public Guid GlossaryId;
        public Guid RuleId;
        public Guid SubRuleId;
        public Guid SectionId;

        public enum SpanTypeEnum
        {
            Normal,
            GlossaryTooltip,
            RuleReference,
            SubRuleReference,
            SectionReference
        }
    }
}
