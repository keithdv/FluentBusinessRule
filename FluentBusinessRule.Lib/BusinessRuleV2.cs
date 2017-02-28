using Csla;
using Csla.Core;
using Csla.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBusinessRule.Lib
{
    public class BusinessRuleV2 : Csla.Rules.BusinessRule
    {

        public static BusinessRuleV2 NewRule(IPropertyInfo primaryProperty)
        {
            return new BusinessRuleV2(primaryProperty);
        }

        public BusinessRuleV2(IPropertyInfo primary) : base(primary)
        {
        }

        public List<Func<RuleContext, bool>> When { get; set; } = new List<Func<RuleContext, bool>>();

        public List<Func<RuleContext, string>> ErrorCondition { get; set; } = new List<Func<RuleContext, string>>();

        public Action<RuleContext> Set { get; set; }

        protected override void Execute(RuleContext context)
        {
            base.Execute(context);

            var cont = true;
            When.ForEach(x =>
            {
                if (cont)
                {
                    cont = x(context) && cont;
                }
            });

            if (cont)
            {
                ErrorCondition.ForEach(e =>
                {
                    if (cont)
                    {
                        var result = e(context);

                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            context.AddErrorResult(result);
                            cont = false;
                        }
                    }
                });
            }

            if (cont && Set != null)
            {
                Set(context);
            }

            context.Complete();

        }

    }
}
