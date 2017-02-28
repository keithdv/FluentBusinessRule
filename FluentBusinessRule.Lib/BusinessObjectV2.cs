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
    [Serializable]
    public class BusinessObjectV2 : Csla.BusinessBase<BusinessObjectV2>
    {

        public BusinessObjectV2()
        {
            // This is key so that the changes
            // to the grandchildren will propagate up
            BusinessRules.CascadeOnDirtyProperties = true;
        }

        public static readonly PropertyInfo<decimal?> RootProperty = RegisterProperty<decimal?>(c => c.Root);
        public decimal? Root
        {
            get { return GetProperty(RootProperty); }
            private set { SetProperty(RootProperty, value); }
        }

        public static readonly PropertyInfo<decimal?> SumAProperty = RegisterProperty<decimal?>(c => c.SumA);
        public decimal? SumA
        {
            get { return GetProperty(SumAProperty); }
            private set { SetProperty(SumAProperty, value); }
        }

        public static readonly PropertyInfo<decimal?> ValueA1Property = RegisterProperty<decimal?>(c => c.ValueA1);
        public decimal? ValueA1
        {
            get { return GetProperty(ValueA1Property); }
            set { SetProperty(ValueA1Property, value); }
        }

        public static readonly PropertyInfo<decimal?> ValueA2Property = RegisterProperty<decimal?>(c => c.ValueA2);
        public decimal? ValueA2
        {
            get { return GetProperty(ValueA2Property); }
            set { SetProperty(ValueA2Property, value); }
        }

        public static readonly PropertyInfo<decimal?> CalculatedBProperty = RegisterProperty<decimal?>(c => c.CalculatedB);
        public decimal? CalculatedB
        {
            get { return GetProperty(CalculatedBProperty); }
            private set { SetProperty(CalculatedBProperty, value); }
        }

        public static readonly PropertyInfo<decimal?> ValueBProperty = RegisterProperty<decimal?>(c => c.ValueB);
        public decimal? ValueB
        {
            get { return GetProperty(ValueBProperty); }
            set { SetProperty(ValueBProperty, value); }
        }

        public static readonly PropertyInfo<decimal?> PercentageBProperty = RegisterProperty<decimal?>(c => c.PercentageB);
        public decimal? PercentageB
        {
            get { return GetProperty(PercentageBProperty); }
            set { SetProperty(PercentageBProperty, value); }
        }

        public static readonly PropertyInfo<bool> IgnoreBProperty = RegisterProperty<bool>(c => c.IgnoreB);
        public bool IgnoreB
        {
            get { return GetProperty(IgnoreBProperty); }
            set { SetProperty(IgnoreBProperty, value); }
        }

        protected override void AddBusinessRules()
        {

            base.AddBusinessRules();

            // DO NOT SET THIS HERE!!!
            // The value will only stay true for the FIRST business object created
            // then it will turn false again
            // Set it in the constructor!!
            //BusinessRules.CascadeOnDirtyProperties = true;

            // IgnoreB = false
            // Root = SumA + IsNull(CalculatedB, 0)
            BusinessRules.AddRule(BusinessRuleV2.NewRule(RootProperty)
                .WhenEqual(IgnoreBProperty, true)
                .Required(SumAProperty)
                .Calculate(SumAProperty, SumAProperty, (a, b) => a));

            // IgnoreB = true
            // Root = SumA
            BusinessRules.AddRule(BusinessRuleV2.NewRule(RootProperty)
                .WhenEqual(IgnoreBProperty, false)
                .Required(SumAProperty)
                .Sum(SumAProperty, CalculatedBProperty));

            // SumA = ValueA1 + ValueA2
            BusinessRules.AddRule(BusinessRuleV2.NewRule(SumAProperty)
               .Required(ValueA1Property)
               .Required(ValueA2Property)
               .Sum(ValueA1Property, ValueA2Property));

            // CalculatedB = ValueB * (PercentageB / 100)
            // CalculatedB is optional
            BusinessRules.AddRule(
                BusinessRuleV2.NewRule(CalculatedBProperty)
                .Calculate(
                    ValueBProperty, PercentageBProperty,
                    (v1, v2) =>
                    {
                        if (!v1.HasValue || !v2.HasValue)
                        {
                            return null;
                        }
                        return v1.Value * (v2.Value / 100);
                    }));

            // Percentage B between 0 and 100
            BusinessRules.AddRule(
                BusinessRuleV2.NewRule(PercentageBProperty)
                .ErrorCondition<decimal?>((perc) =>
                {
                    if (perc.HasValue)
                    {
                        if (!(perc.Value > 0 && perc.Value < 100))
                        {
                            return "Percentage must be between 0 and 100";
                        }
                    }
                    return string.Empty;
                }));

        }

        protected override void DataPortal_Create()
        {
            base.DataPortal_Create();
            LoadProperty(IgnoreBProperty, false);
        }

    }
}
