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
    public static class BusinessRuleExtensions
    {

        public static BusinessRule<Nullable<T>> NewBusinessRule<T>(PropertyInfo<Nullable<T>> primaryProperty) where T : struct
        {
            var rule = new BusinessRule<Nullable<T>>(primaryProperty);

            return rule;
        }

        public static BusinessRule<Nullable<T1>, Nullable<T2>> DependsOn<T1, T2>(this BusinessRule<Nullable<T1>> rule, PropertyInfo<Nullable<T2>> dependentProperty, bool IsRequired = true) where T1 : struct where T2 : struct
        {

            var rule2 = new BusinessRule<Nullable<T1>, Nullable<T2>>((PropertyInfo<Nullable<T1>>)rule.PrimaryProperty, dependentProperty);

            rule.ExecuteMethods.ForEach(m => rule2.ExecuteMethods.Add((context, t1, t2) => m(context, t1)));

            if (IsRequired)
            {
                rule2.ExecuteMethods.Add(new Action<Csla.Rules.RuleContext, Nullable<T1>, Nullable<T2>>((context, t1, t2) =>
                {
                    if (!t2.HasValue)
                    {
                        context.AddErrorResult("Missing value");
                    }
                }));
            }

            return rule2;
        }


        public static BusinessRule<Nullable<T1>, Nullable<T2>, Nullable<T3>> DependsOn<T1, T2, T3>(this BusinessRule<Nullable<T1>, Nullable<T2>> rule, PropertyInfo<Nullable<T3>> dependentProperty, bool IsRequired = true) where T1 : struct where T2 : struct where T3 : struct
        {

            var rule3 = new BusinessRule<Nullable<T1>, Nullable<T2>, Nullable<T3>>((PropertyInfo<Nullable<T1>>)rule.PrimaryProperty, (PropertyInfo<Nullable<T2>>)rule.InputProperties[1], dependentProperty);

            rule.ExecuteMethods.ForEach(m => rule3.ExecuteMethods.Add((context, t1, t2, t3) => m(context, t1, t2)));

            if (IsRequired)
            {
                rule3.ExecuteMethods.Add(new Action<Csla.Rules.RuleContext, Nullable<T1>, Nullable<T2>, Nullable<T3>>((context, t1, t2, t3) =>
                {
                    if (!t3.HasValue)
                    {
                        context.AddErrorResult("Missing value");
                    }
                }));
            }

            return rule3;
        }

        public static BusinessRule<decimal?> Required(this BusinessRule<decimal?> rule)
        {
            rule.ExecuteMethods.Add((context, t1) =>
            {
                if (!t1.HasValue)
                {
                    context.AddErrorResult("Value is required");
                }
            });

            return rule;
        }

        public static BusinessRule<decimal?, decimal?> AllRequired(this BusinessRule<decimal?, decimal?> rule)
        {
            rule.ExecuteMethods.Add((context, t1, t2) =>
            {
                if (!t1.HasValue || !t2.HasValue)
                {
                    context.AddErrorResult("Value is required");
                }
            });

            return rule;
        }

        public static BusinessRule<decimal?, decimal?, decimal?> AllRequired(this BusinessRule<decimal?, decimal?, decimal?> rule)
        {
            rule.ExecuteMethods.Add((context, t1, t2, t3) =>
            {
                if (!t1.HasValue || !t2.HasValue || !t3.HasValue)
                {
                    context.AddErrorResult("Value is required");
                }
            });

            return rule;
        }


        public static BusinessRule<decimal?, decimal?, decimal?> SumDependants(this BusinessRule<decimal?, decimal?, decimal?> rule)
        {

            rule.ExecuteMethods.Add(new Action<Csla.Rules.RuleContext, decimal?, decimal?, decimal?>((context, val1, val2, val3) =>
            {
                context.AddOutValue(rule.PrimaryProperty, (val2 ?? 0) + (val3 ?? 0));
            }));

            return rule;
        }


        /// <summary>
        /// If a non-null string is returned assumed to be an error
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="rule"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BusinessRule<T1> ErrorCondition<T1>(this BusinessRule<T1> rule, Func<T1, string> action)
        {

            rule.ExecuteMethods.Add((context, t1) =>
            {
                var s = action(t1);
                if (!string.IsNullOrEmpty(s))
                {
                    context.AddErrorResult(s);
                }
            });

            return rule;
        }

        public static BusinessRule<T1, T2> ErrorCondition<T1, T2>(this BusinessRule<T1, T2> rule, Func<T1, T2, string> action)
        {

            rule.ExecuteMethods.Add((context, t1, t2) =>
            {
                var s = action(t1, t2);
                if (!string.IsNullOrEmpty(s))
                {
                    context.AddErrorResult(s);
                }
            });

            return rule;
        }


        public static BusinessRule<T1, T2, T3> ErrorCondition<T1, T2, T3>(this BusinessRule<T1, T2, T3> rule, Func<T1, T2, T3, string> action)
        {

            rule.ExecuteMethods.Add((context, t1, t2, t3) =>
            {
                var s = action(t1, t2, t3);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    context.AddErrorResult(s);
                }
            });

            return rule;
        }

        public static BusinessRule<decimal?, decimal?, decimal?> Calculate(this BusinessRule<decimal?, decimal?, decimal?> rule, Func<decimal?, decimal?, decimal?> calc)
        {
            rule.ExecuteMethods.Add((context, v1, v2, v3) =>
            {
                if (v2.HasValue && v3.HasValue) // hmmmm
                {
                    context.AddOutValue(calc(v2, v3));
                }
            });

            return rule;
        }

    }
}
