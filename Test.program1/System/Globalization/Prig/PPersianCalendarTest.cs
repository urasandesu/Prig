/* 
 * File: PPersianCalendarTest.cs
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
using System.Globalization;
using System.Globalization.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.System.Globalization.Prig
{
    [TestFixture]
    public class PPersianCalendarTest
    {
        [Test]
        public void CheckTicksRange_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
#if _NET_3_5
                PPersianCalendar.CheckTicksRange().Body = (@this, ticks) => { };
#else
                PPersianCalendar.CheckTicksRange().Body = ticks => { };
#endif

                // Act
                var calendar = new PersianCalendar();
                var actual = calendar.GetEra(new DateTime(622, 3, 20));

                // Assert
                Assert.AreEqual(PersianCalendar.PersianEra, actual);
            }
        }
    }
}
