/* 
 * File: DelegateTest.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2014 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */


#define SOLUTION
#define ISSUE_2
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
            LooseCrossDomainAccessor.Clear();
            LooseFunc<int, int, int>.Unload();
            LooseFunc<int, int, int>.Register();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            LooseFunc<int, int, int>.Holder.Source = null;
            LooseFunc<int, int, int>.Unload();
            LooseCrossDomainAccessor.Clear();
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
