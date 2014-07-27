/* 
 * File: PJapaneseLunisolarCalendarTest.cs
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
    public class PJapaneseLunisolarCalendarTest
    {
        [Test]
        public void GetYearInfo_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PJapaneseLunisolarCalendar.GetYearInfo().Body = (@this, lunarYear, index) => 41;

                // Act
                var calendar = new JapaneseLunisolarCalendar();
                var actual = calendar.GetLeapMonth(26, calendar.Eras[0]);

                // Assert
                // Before setting indirection: 平成 26 年 閏 9
                Assert.AreEqual(42, actual);
            }
        }



        [Test]
        public void GetYearInfo_should_be_callable_indirectly_against_only_specified_instance()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var calendar = new JapaneseLunisolarCalendar();

                var calendarProxy = new PProxyJapaneseLunisolarCalendar();
                calendarProxy.GetEra().Body = (@this, time) => calendar.Eras[0];
                calendarProxy.GetGregorianYear().Body = (@this, year, era) => 2000;
                calendarProxy.GetYearInfo().Body = (@this, lunarYear, index) => 41;
                var calendar_sut = (JapaneseLunisolarCalendar)calendarProxy;

                // Act
                var actual = calendar_sut.GetLeapMonth(26, calendar.Eras[0]);

                // Assert
                Assert.AreEqual(42, actual);
                Assert.AreNotEqual(calendar.GetLeapMonth(26, calendar.Eras[0]), actual);
            }
        }
    }
}
