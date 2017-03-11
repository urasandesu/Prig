/* 
 * File: AppDomainSomethingTest.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2017 Akira Sugiura
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



#if NUnit
using TestFixtureAttribute = NUnit.Framework.TestFixtureAttribute;
using TestAttribute = NUnit.Framework.TestAttribute;
#elif MsTest
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#elif Xunit
using TestAttribute = Xunit.FactAttribute;
#endif
using System;
using System.Prig;
using Test.program1.TestUtilities;
using Test.program1.TestUtilities.Mixins.System;
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
#if _NET_3_5
        // In CLR v2, new AppDomain is never created. It occurs ExecutionEngineException.
#else
    // When creating new AppDomain in each test case, you have to also create new IndirectionsContext in each AppDomain.
    [TestFixture]
    public class AppDomainSomethingTest
    {
        [Test]
        public void Something_that_is_cross_plural_AppDomains_should_not_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PAppDomainSomething.Do().Body = () => { };
            }

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                using (new IndirectionsContext())
                {
                    // Act, Assert
                    ExceptionAssert.Throws<InvalidOperationException>(() => AppDomainSomething.Do());
                }
            });
        }

        [Test]
        public void Something_that_is_cross_plural_AppDomains_should_be_handled_as_different_setup()
        {
            lock (AppDomainMixin.SyncRoot)
            {
                AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
                {
                    using (new IndirectionsContext())
                    {
                        // Arrange
                        PAppDomainSomething.Do().Body = () => { };
                    }
                });

                AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
                {
                    using (new IndirectionsContext())
                    {
                        // Act, Assert
                        ExceptionAssert.Throws<InvalidOperationException>(() => AppDomainSomething.Do());
                    }
                });
            }
        }

        [Test]
        public void Something_that_is_in_same_AppDomain_should_be_callable_indirectly()
        {
            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                using (new IndirectionsContext())
                {
                    // Arrange
                    PAppDomainSomething.Do().Body = () => { };

                    // Act, Assert
                    ExceptionAssert.DoesNotThrow(() => AppDomainSomething.Do());
                }
            });
        }

        [Test]
        public void MSCorLib_something_that_is_in_another_AppDomain_should_be_callable_indirectly()
        {
            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                using (new IndirectionsContext())
                {
                    // Arrange
                    PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 12, 00, 00);

                    // Act
                    var result = DateTime.Now;

                    // Assert
                    Assert.AreEqual(new DateTime(2013, 12, 13, 12, 00, 00), result);
                }
            });
        }
    }
#endif
}
