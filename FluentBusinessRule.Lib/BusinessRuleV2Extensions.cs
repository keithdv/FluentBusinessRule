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
    public static class BusinessRuleV2Extensions
    {

        public static BusinessRuleV2 Required<T>(this BusinessRuleV2 rule, PropertyInfo<Nullable<T>> property) where T : struct
        {

            if (!rule.InputProperties.Contains(property))
            {
                rule.InputProperties.Add(property);
            }

            rule.ErrorCondition.Add((context) =>
            {
                var val = (Nullable<T>)context.InputPropertyValues[property];
                if (!val.HasValue)
                {
                    return $"Property {property.FriendlyName} is required";
                }
                return string.Empty;
            });

            return rule;
        }

        public static BusinessRuleV2 WhenEqual<T>(this BusinessRuleV2 rule, PropertyInfo<T> propertyInfo, T value) where T : IComparable
        {

            if (!rule.InputProperties.Contains(propertyInfo))
            {
                rule.InputProperties.Add(propertyInfo);
            }

            Func<RuleContext, bool> when = new Func<RuleContext, bool>((context) =>
            {

                T val = (T)context.InputPropertyValues[propertyInfo];

                if (val.Equals(value))
                {
                    return true;
                }

                return false;

            });

            rule.When.Add(when);

            return rule;

        }

        public static BusinessRuleV2 Sum(this BusinessRuleV2 rule, PropertyInfo<decimal?> propertyA, PropertyInfo<decimal?> propertyB)
        {
            if (rule.Set != null)
            {
                throw new Exception("Only one calculation allowed!");
            }

            if (!rule.InputProperties.Contains(propertyA))
            {
                rule.InputProperties.Add(propertyA);
            }

            if (!rule.InputProperties.Contains(propertyB))
            {
                rule.InputProperties.Add(propertyB);
            }

            rule.Set = (context) =>
            {
                decimal valueA = (decimal?)context.InputPropertyValues[propertyA] ?? 0;
                decimal valueB = (decimal?)context.InputPropertyValues[propertyB] ?? 0;

                context.AddOutValue(rule.PrimaryProperty, valueA + valueB);
            };

            return rule;
        }

        public static BusinessRuleV2 Calculate<T1, T2>(this BusinessRuleV2 rule, PropertyInfo<T1> propertyA, PropertyInfo<T2> propertyB, Func<T1, T2, object> calculate)
        {
            if (rule.Set != null)
            {
                throw new Exception("Only one calculation allowed!");
            }

            if (!rule.InputProperties.Contains(propertyA))
            {
                rule.InputProperties.Add(propertyA);
            }

            if (!rule.InputProperties.Contains(propertyB))
            {
                rule.InputProperties.Add(propertyB);
            }

            rule.Set = (context) =>
            {
                T1 valueA = (T1)context.InputPropertyValues[propertyA];
                T2 valueB = (T2)context.InputPropertyValues[propertyB];

                context.AddOutValue(calculate(valueA, valueB));

            };

            return rule;

        }

        public static BusinessRuleV2 ErrorCondition<T>(this BusinessRuleV2 rule, Func<T, string> errorCondition)
        {
            if (!rule.InputProperties.Contains(rule.PrimaryProperty))
            {
                rule.InputProperties.Add(rule.PrimaryProperty);
            }

            rule.ErrorCondition.Add((context) =>
            {

                T value = (T)context.InputPropertyValues[rule.PrimaryProperty];

                return errorCondition(value);

            });


            return rule;
        }

        public static BusinessRuleV2 ErrorCondition<T>(this BusinessRuleV2 rule, PropertyInfo<T> property, Func<T, string> errorCondition)
        {
            if (!rule.InputProperties.Contains(property))
            {
                rule.InputProperties.Add(property);
            }

            rule.ErrorCondition.Add((context) =>
            {

                T value = (T)context.InputPropertyValues[property];

                return errorCondition(value);

            });


            return rule;
        }
    }
}
