using Csla.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Rules;
using Csla;

namespace FluentBusinessRule.Lib
{
    //public class BusinessRule : Csla.Rules.BusinessRule
    //{

    //    public BusinessRule(IPropertyInfo primaryProperty) : base(primaryProperty)
    //    {

    //    }

    //    public Action<RuleContext> ExecuteMethod { get; set; } = new Action<RuleContext>(rc => { });

    //    protected override void Execute(RuleContext context)
    //    {
    //        base.Execute(context);

    //        ExecuteMethod(context);

    //    }

    //}


    public class BusinessRule<T1> : Csla.Rules.BusinessRule
    {

        public BusinessRule(PropertyInfo<T1> primaryProperty) : base(primaryProperty)
        {
            InputProperties.Add(primaryProperty);
        }

        public List<Action<RuleContext, T1>> ExecuteMethods { get; set; } = new List<Action<RuleContext, T1>>();


        protected override void Execute(RuleContext context)
        {
            base.Execute(context);

            T1 t1 = context.GetInputValue<T1>(PrimaryProperty);

            ExecuteMethods.ForEach(m => m(context, t1));

            context.Complete();

        }

    }


    public class BusinessRule<T1, T2> : Csla.Rules.BusinessRule
    {

        public BusinessRule(PropertyInfo<T1> primaryProperty, PropertyInfo<T2> t2) : base(primaryProperty)
        {
            InputProperties.Add(primaryProperty);
            InputProperties.Add(t2);
        }

        public List<Action<RuleContext, T1, T2>> ExecuteMethods { get; set; } = new List<Action<RuleContext, T1, T2>>();


        protected override void Execute(RuleContext context)
        {
            base.Execute(context);

            T1 t1 = context.GetInputValue<T1>(PrimaryProperty);
            T2 t2 = context.GetInputValue<T2>(InputProperties[1]);

            ExecuteMethods.ForEach(m => m(context, t1, t2));

            context.Complete();

        }

    }

    public class BusinessRule<T1, T2, T3> : Csla.Rules.BusinessRule
    {

        public BusinessRule(PropertyInfo<T1> primaryProperty, PropertyInfo<T2> t2, PropertyInfo<T3> t3) : base(primaryProperty)
        {
            InputProperties.Add(primaryProperty);
            InputProperties.Add(t2);
            InputProperties.Add(t3);
        }

        public List<Action<RuleContext, T1, T2, T3>> ExecuteMethods { get; set; } = new List<Action<RuleContext, T1, T2, T3>>();

        protected override void Execute(RuleContext context)
        {
            base.Execute(context);

            T1 t1 = context.GetInputValue<T1>(PrimaryProperty);
            T2 t2 = context.GetInputValue<T2>(InputProperties[1]);
            T3 t3 = context.GetInputValue<T3>(InputProperties[2]);

            ExecuteMethods.ForEach(m => m(context, t1, t2, t3));

            context.Complete();

        }

    }
}
