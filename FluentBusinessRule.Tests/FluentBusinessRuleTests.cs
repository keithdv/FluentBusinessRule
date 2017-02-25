using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla;
using FluentBusinessRule.Lib;
using Csla.Core;

namespace FluentBusinessRule.Tests
{
    [TestClass]
    public class FluentBusinessRuleTests
    {

        // SumA = ValueA1 + ValueA2
        // CalculatedB = ValueB * (PercentageB / 100)
        // Root = SumA + IsNull(CalculatedB, 0)

        [TestMethod]
        public void OnlySetValueA()
        {
            var bo = DataPortal.Create<BusinessObject>();

            bo.ValueA1 = 1.1m;

            Assert.IsFalse(bo.IsValid);

            bo.ValueA2 = 2.2m;

            Assert.IsTrue(bo.IsValid);

            Assert.AreEqual(3.3m, bo.Root);
            Assert.AreEqual(3.3m, bo.SumA);

        }

        private BusinessObject _SetAll()
        {
            BusinessObject bo;

            bo = DataPortal.Create<BusinessObject>();

            bo.ValueA1 = 1.1m;
            bo.ValueA2 = 2.2m;

            bo.ValueB = 1000m;
            bo.PercentageB = 75m;

            return bo;
        }

        [TestMethod]
        public void SetAll()
        {

            var bo = _SetAll();

            Assert.AreEqual(3.3m, bo.SumA);
            Assert.AreEqual(750m, bo.CalculatedB);
            Assert.AreEqual(753.3m, bo.Root);

        }

        [TestMethod]
        public void ChangePercentage()
        {
            var bo = _SetAll();

            bo.PercentageB = 55m;

            Assert.AreEqual(553.3m, bo.Root);

        }

        [TestMethod]
        public void ChangeValueA2()
        {
            var bo = _SetAll();

            bo.ValueA2 = 5.5m;
            Assert.AreEqual(756.6m, bo.Root);

        }

        [TestMethod]
        public void InvalidPercentageB()
        {
            var bo = _SetAll();

            bo.PercentageB = 110m;

            Assert.IsFalse(bo.IsValid);
        }

        [TestMethod]
        public void NullValueA2()
        {
            var bo = _SetAll();

            bo.ValueA2 = null;

            Assert.IsFalse(bo.IsValid);

        }

        [TestMethod]
        public void RemoveValueB_0()
        {
            var bo = _SetAll();

            bo.ValueB = 0m;

            Assert.IsTrue(bo.IsValid);

            Assert.AreEqual(3.3m, bo.Root);
        }

        [TestMethod]
        public void RemoveValueB_Null()
        {
            var bo = _SetAll();

            bo.ValueB = null;

            Assert.IsTrue(bo.IsValid);

            // Doesn't work...and I think it should!
            Assert.AreEqual(3.3m, bo.Root);
        }

    }
}
