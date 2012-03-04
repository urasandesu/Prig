//#define SOLUTION
//#define ISSUE_2
#define ISSUE_1

#if _
#elif SOLUTION
using System;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.Prig.Framework;

namespace Test.Urasandesu.Prig.Framework
{
    class LooseFunc<T1, T2, TResult> : LooseCrossDomainAccessor<GenericHolder<Func<T1, T2, TResult>>> { }

    [TestFixture]
    public class DelegateTest
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            LooseFunc<int, int, int>.Unload();
            LooseFunc<int, int, int>.Register();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            LooseFunc<int, int, int>.Holder.Source = null;
            LooseFunc<int, int, int>.Unload();
        }

        [SetUp]
        public void SetUp()
        {
            LooseFunc<int, int, int>.Holder.Source = null;
        }

        [TearDown]
        public void TearDown()
        {
            LooseFunc<int, int, int>.Holder.Source = null;
        }

        [Test]
        public void Test()
        {
            var adder = default(Func<int, int, int>);
            var z = 1;
            adder = (x, y) => x + y + z;    // To closure(capture local variable z)
            LooseFunc<int, int, int>.Holder.Source = adder;
            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                Assert.AreEqual(3, LooseFunc<int, int, int>.Holder.Source(1, 1));
            });
        }
    }
}
#elif ISSUE_2
using System;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class DelegateTest
    {
        [Test]
        public void Test()
        {
            var adder = default(Func<int, int, int>);
            var z = 1;
            adder = (x, y) => x + y + z;    // To closure(capture local variable z)
            AppDomain.CurrentDomain.RunAtIsolatedDomain<Func<int, int, int>>(adder_ =>
            {
                Assert.AreEqual(3, adder_(1, 1));
            }, adder);
        }
    }
}
#elif ISSUE_1
using System;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class DelegateTest
    {
        [Test]
        public void Test()
        {
            var adder = default(Func<int, int, int>);
            adder = (x, y) => x + y;    // Referencial transparent lambda
            AppDomain.CurrentDomain.RunAtIsolatedDomain<Func<int, int, int>>(adder_ =>
            {
                Assert.AreEqual(2, adder_(1, 1));
            }, adder);
        }
    }
}
#endif