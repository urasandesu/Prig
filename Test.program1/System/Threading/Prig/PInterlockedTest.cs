/* 
 * File: PInterlockedTest.cs
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.System.Threading.Prig
{
    [TestFixture]
    public class PInterlockedTest
    {
        [Test]
        public void ExchangeT_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PInterlocked.Exchange<MyData>().Body = (ref MyData location1, MyData value) =>
                {
                    location1 = value;
                    return new MyData(42);
                };

                // Act
                var data1 = new MyData(100);
                var data2 = new MyData(200);
                var actual = Interlocked.Exchange(ref data1, data2);

                // Assert
                Assert.AreEqual(200, data1.Value);
                Assert.AreEqual(42, actual.Value);
            }
        }

        class MyData
        {
            public MyData(int value)
            {
                Value = value;
            }

            public int Value { get; private set; }
        }
    }
}
