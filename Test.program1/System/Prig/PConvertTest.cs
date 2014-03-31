/* 
 * File: PConvertTest.cs
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
using System.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.System.Prig
{
    [TestFixture]
    public class PConvertTest
    {
        [Test]
        public void ToInt32_ShouldBeCallableIndirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PConvert.ToInt32.Body = value => 42;

                // Act
                var actual = Convert.ToInt32(103.919);

                // Assert
                Assert.AreEqual(42, actual);
            }
        }

        [Test]
        public void ToSByte_ShouldBeCallableIndirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PConvert.ToSByte.Body = value => (sbyte)42;

                // Act
                var actual = Convert.ToSByte('a');

                // Assert
                Assert.AreEqual((sbyte)42, actual);
            }
        }

        [Test]
        public void ToInt16_ShouldBeCallableIndirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PConvert.ToInt16.Body = value => (Int16)42;

                // Act
                var actual = Convert.ToInt16('b');

                // Assert
                Assert.AreEqual((Int16)42, actual);
            }
        }

        [Test]
        public void ToInt64_ShouldBeCallableIndirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PConvert.ToInt64.Body = value => 42L;

                // Act
                var actual = Convert.ToInt64(9223372036854775807.5);

                // Assert
                Assert.AreEqual(42L, actual);
            }
        }
    }
}
