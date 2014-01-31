using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace SharedSource.SegmentPicker.Conditions
{
    public class IsInSegment<T> : WhenCondition<T> where T : RuleContext
    {
        public string SegmentName { get; set; }

        internal ID Segments = new ID("{FE04B19D-4D0C-487B-BE7D-A04F3842CDA7}");

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object)ruleContext, "ruleContext");
            return EvaluateRule();
        }

        protected bool EvaluateRule()
        {
            var ruleContext = new RuleContext();
            var item = Sitecore.Context.Database.GetItem(Segments).Axes.GetChild(SegmentName);


            foreach (Rule<RuleContext> rule in RuleFactory.GetRules<RuleContext>(new[] {item}, "Rule").Rules)
            {
                if (rule.Condition != null)
                {
                    var stack = new RuleStack();
                    rule.Condition.Evaluate(ruleContext, stack);

                    if (ruleContext.IsAborted)
                    {
                        continue;
                    }
                    
                    if((stack.Count != 0) && ((bool)stack.Pop()))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}