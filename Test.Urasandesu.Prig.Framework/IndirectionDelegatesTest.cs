/* 
 * File: IndirectionDelegatesTest.cs
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
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class IndirectionDelegatesTest
    {
        [Test]
        public void ExecuteOriginal_should_execute_specified_StaticIndirectionAction()
        {
            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Arrange
                var cache = default(IndirectionAction<int, int>);

                // Act
                HelperForIndirectionAction<int, int>.ExecuteOriginalOfStatic(() => cache, _cache => cache = _cache, typeof(TestClass), "Add", 10, 32);

                // Assert
                Assert.AreEqual(42, TestClass.StaticResult);
            });
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_InstanceIndirectionAction()
        {
            // Arrange
            var cache = default(IndirectionAction<TestClass, int, int>);
            var target = new TestClass();

            // Act
            HelperForIndirectionAction<TestClass, int, int>.ExecuteOriginalOfInstance(() => cache, _cache => cache = _cache, typeof(TestClass), "Minus", target, 52, 10);

            // Assert
            Assert.AreEqual(42, target.InstanceResult);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_StaticIndirectionOutAction()
        {
            // Arrange
            var cache = default(IndirectionOutAction<int, int, int>);

            // Act
            var actual = 0;
            HelperForIndirectionOutAction<int, int, int>.ExecuteOriginalOfStatic(() => cache, _cache => cache = _cache, typeof(TestClass), "Add", 10, 32, out actual);

            // Assert
            Assert.AreEqual(42, actual);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_InstanceIndirectionOutAction()
        {
            // Arrange
            var cache = default(IndirectionOutAction<TestClass, int, int, int>);
            var target = new TestClass();

            // Act
            var actual = 0;
            HelperForIndirectionOutAction<TestClass, int, int, int>.ExecuteOriginalOfInstance(() => cache, _cache => cache = _cache, typeof(TestClass), "Minus", target, 52, 10, out actual);

            // Assert
            Assert.AreEqual(42, actual);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_StaticIndirectionOutOutAction()
        {
            // Arrange
            var cache = default(IndirectionOutOutAction<int, int, int, int>);

            // Act
            var actual1 = 0;
            var actual2 = 0;
            HelperForIndirectionOutOutAction<int, int, int, int>.ExecuteOriginalOfStatic(() => cache, _cache => cache = _cache, typeof(TestClass), "Add", 10, 32, out actual1, out actual2);

            // Assert
            Assert.AreEqual(42, actual1);
            Assert.AreEqual(52, actual2);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_InstanceIndirectionFunc()
        {
            // Arrange
            var cache = default(IndirectionFunc<TestClass, double>);
            var target = new TestClass();

            // Act
            var actual = HelperForIndirectionFunc<TestClass, double>.ExecuteOriginalOfInstance(() => cache, _cache => cache = _cache, typeof(TestClass), "get_PI", target);

            // Assert
            Assert.LessOrEqual(Math.Abs(Math.PI - actual), 0.001);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_InstanceIndirectionRefRefFunc()
        {
            // Arrange
            var cache = default(IndirectionRefRefFunc<TestClass, int, int, int, int>);
            var target = new TestClass();

            // Act
            var result1 = 52;
            var result2 = 0;
            var actual = HelperForIndirectionRefRefFunc<TestClass, int, int, int, int>.ExecuteOriginalOfInstance(() => cache, _cache => cache = _cache, typeof(TestClass), "Minus", target, 10, ref result1, ref result2);

            // Assert
            Assert.AreEqual(42, actual);
            Assert.AreEqual(42, result1);
            Assert.AreEqual(52, result2);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_StaticIndirectionRefThisFunc()
        {
            // Arrange
            var cache = default(IndirectionRefThisFunc<TestStruct, double, double>);
            var target = new TestStruct(21.0);

            // Act
            var actual = HelperForIndirectionRefThisFunc<TestStruct, double, double>.ExecuteOriginalOfStatic(() => cache, _cache => cache = _cache, typeof(TestStruct), "Pow", ref target, 2.0);

            // Assert
            Assert.LessOrEqual(Math.Abs(42.0 - target.Value), 0.001);
            Assert.LessOrEqual(Math.Abs(42.0 - actual), 0.001);
        }


        [Test]
        public void ExecuteOriginal_should_execute_specified_StaticIndirectionRefThisRefRefFunc()
        {
            // Arrange
            var cache = default(IndirectionRefThisRefRefFunc<TestStruct, double, double, double>);
            var target = new TestStruct(21.0);

            // Act
            var result = Math.PI;
            var actual = HelperForIndirectionRefThisRefRefFunc<TestStruct, double, double, double>.ExecuteOriginalOfStatic(() => cache, _cache => cache = _cache, typeof(TestStruct), "Pow", ref target, 2.0, ref result);

            // Assert
            Assert.LessOrEqual(Math.Abs(Math.PI - target.Value), 0.001);
            Assert.LessOrEqual(Math.Abs(42.0 - actual), 0.001);
            Assert.LessOrEqual(Math.Abs(42.0 - result), 0.001);
        }



        class TestClass
        {
            public static int StaticResult { get; private set; }
            public int InstanceResult { get; private set; }

            static void Add(int x, int y)
            {
                StaticResult = x + y;
            }

            static void Add(int x, int y, out int result)
            {
                result = x + y;
            }

            static void Add(int x, int y, out int result1, out int result2)
            {
                result1 = x + y;
                result2 = result1 + 10;
            }

            void Minus(int x, int y)
            {
                InstanceResult = x - y;
            }

            void Minus(int x, int y, out int result)
            {
                result = x - y;
            }

            int Minus(int y, ref int result1, ref int result2)
            {
                result2 = result1;
                result1 = result1 - y;
                return result1;
            }

            double PI { get { return Math.PI; } }
        }

        struct TestStruct
        {
            double m_value;
            public TestStruct(double value)
            {
                m_value = value;
            }

            public double Value { get { return m_value; } }

            public static double Pow(ref TestStruct @this, double val)
            {
                @this.m_value = @this.m_value * val;
                return @this.m_value;
            }

            public static double Pow(ref TestStruct @this, double val, ref double result)
            {
                var tmp = @this.m_value;
                @this.m_value = result;
                result = tmp * val;
                return result;
            }
        }
    }
}
