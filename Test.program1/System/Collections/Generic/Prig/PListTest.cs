/* 
 * File: PListTest.cs
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


using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Generic.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.System.Collections.Generic.Prig
{
    [TestFixture]
    public class PListTest
    {
        [Test]
        public void Add_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var actual = default(int);
                PList<int>.AddT().Body = (@this, item) => actual = item;

                // Act
                var list = new List<int>();
                list.Add(42);

                // Assert
                Assert.AreEqual(0, list.Count);
                Assert.AreEqual(42, actual);
            }
        }

        [Test]
        public void Add_should_be_callable_indirectly_against_only_specified_instance()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var actual = default(int);
                var listProxy = new PProxyList<int>();
                listProxy.AddT().Body = (@this, item) => actual = item;
                var list_sut = (List<int>)listProxy;

                var list = new List<int>();

                // Act
                list_sut.Add(42);
                list.Add(10);

                // Assert
                Assert.AreEqual(42, actual);
            }
        }



        [Test]
        public void Add_should_throw_NotImplementedException_if_the_instance_behavior_is_set_that()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var listProxy = new PProxyList<int>();
                listProxy.
                    ExcludeGeneric().
                    IncludeAddT().
                    DefaultBehavior = IndirectionBehaviors.NotImplemented;
                var list_sut = (List<int>)listProxy;

                var list = new List<int>();

                // Act, Assert
                Assert.Throws<NotImplementedException>(() => list_sut.Add(42));
                Assert.DoesNotThrow(() => list.Add(10));
            }
        }

        [Test]
        public void Add_should_do_nothing_if_the_instance_behavior_is_set_that()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var listProxy = new PProxyList<int>();
                listProxy.
                    ExcludeGeneric().
                    IncludeAddT().
                    DefaultBehavior = IndirectionBehaviors.DefaultValue;
                var list_sut = (List<int>)listProxy;

                var list = new List<int>();

                // Act
                list_sut.Add(42);
                list.Add(10);

                // Assert
                CollectionAssert.IsEmpty(list_sut);
                CollectionAssert.AreEqual(new[] { 10 }, list);
            }
        }

        [Test]
        public void Add_should_behave_as_same_as_original_if_the_instance_behavior_is_set_that()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var listProxy = new PProxyList<int>();
                listProxy.
                    ExcludeGeneric().
                    IncludeAddT().
                    DefaultBehavior = IndirectionBehaviors.Fallthrough;
                var list_sut = (List<int>)listProxy;
                // You have to call the constructor of the type that is original for a proxy.
                // This isn't easy usage, but I believe that proxy is usually set indirection settings explicitly.
                typeof(List<int>).GetConstructor(Type.EmptyTypes).Invoke(list_sut, new object[0]);

                var list = new List<int>();

                // Act
                list_sut.Add(42);
                list.Add(10);

                // Assert
                CollectionAssert.AreEqual(new[] { 42 }, list_sut);
                CollectionAssert.AreEqual(new[] { 10 }, list);
            }
        }
    }
}
