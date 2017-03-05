/* 
 * File: PArrayTest.cs
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
using Urasandesu.NAnonym.Collections.Generic;
using Urasandesu.Prig.Framework;

namespace Test.program1.System.Prig
{
    [TestFixture]
    public class PArrayTest
    {
        [Test]
        public void CreateInstance_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PArray.CreateInstanceTypeInt32ArrayInt32Array().Body = (elementType, lengths, lowerBounds) => new int[5, 5];

                // Act
                var actual = (int[,])Array.CreateInstance(typeof(int), new int[] { 3, 3 }, new int[] { 0, 0 });

                // Assert
                Assert.AreEqual(5, actual.GetLength(0));
                Assert.AreEqual(5, actual.GetLength(1));
            }
        }

        [Test]
        public void ExistsT_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PArray.ExistsOfTTArrayPredicateOfT<int>().Body = (array, match) => true;

                // Act
                var actual = Array.Exists(new int[] { 1, 2, 3 }, x => x == 42);

                // Assert
                Assert.IsTrue(actual);
            }
        }

        [Test]
        public void BinarySearch_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PArray.BinarySearchArrayInt32Int32ObjectIComparer().Body = (array, index, length, value, comparer) => 42;

                // Act
                var actual = Array.BinarySearch(new int[] { 1, 2, 3 }, 0, 3, (object)2, new LambdaComparer<int>((_1, _2) => _1 - _2));

                // Assert
                Assert.AreEqual(42, actual);
            }
        }



        [Test]
        public void ExistsT_should_throw_NotImplementedException_if_the_behavior_is_set_that()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PArray.
                    ExcludeGeneric().
                    IncludeExistsOfTTArrayPredicateOfT<int>().
                    DefaultBehavior = IndirectionBehaviors.NotImplemented;

                // Act, Assert
                ExceptionAssert.Throws<NotImplementedException>(() => Array.Exists(new int[] { 1, 2, 3 }, x => x == 42));
            }
        }

        [Test]
        public void ExistsT_should_return_default_value_if_the_behavior_is_set_that()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PArray.
                    ExcludeGeneric().
                    IncludeExistsOfTTArrayPredicateOfT<int>().
                    DefaultBehavior = IndirectionBehaviors.DefaultValue;

                // Act
                var result = Array.Exists(new int[] { 1, 2, 3 }, x => x == 2);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void ExistsT_should_behave_as_same_as_original_if_the_behavior_is_set_that()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PArray.
                    ExcludeGeneric().
                    IncludeExistsOfTTArrayPredicateOfT<int>().
                    DefaultBehavior = IndirectionBehaviors.Fallthrough;

                // Act
                var result = Array.Exists(new int[] { 1, 2, 3 }, x => x == 2);

                // Assert
                Assert.IsTrue(result);
            }
        }
    }
}
