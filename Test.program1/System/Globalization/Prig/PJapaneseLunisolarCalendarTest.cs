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



#if NUnit
using TestFixtureAttribute = NUnit.Framework.TestFixtureAttribute;
using TestAttribute = NUnit.Framework.TestAttribute;
#elif MsTest
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#endif
using System;
using System.Collections;
using System.Globalization;
using System.Globalization.Prig;
using System.Reflection;
using Urasandesu.Prig.Framework;
using Test.program1.TestUtilities;

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
                PJapaneseLunisolarCalendar.GetYearInfoInt32Int32().Body = (@this, lunarYear, index) => 41;

                // Act
                var calendar = new JapaneseLunisolarCalendar();
                var actual = calendar.GetLeapMonth(26, calendar.Eras[0]);

                // Assert
                // Before setting indirection: 平成 26 年 閏 9
                Assert.AreEqual(42, actual);
            }
        }

        [Test]
        public void CalEraInfo_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var japaneseLunisolarCalendar_get_CalEraInfo = default(MethodInfo);
                var expected = default(Array);
                MakeCalEraInfoTestData(out japaneseLunisolarCalendar_get_CalEraInfo, out expected);

                PJapaneseLunisolarCalendar.CalEraInfoGet().Body = args => expected;


                // Act
                var calendar = new JapaneseLunisolarCalendar();
                var actual = japaneseLunisolarCalendar_get_CalEraInfo.Invoke(calendar, null) as IEnumerable;


                // Assert
                CollectionAssert.AreEqual(expected, actual);
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
                calendarProxy.GetEraDateTime().Body = (@this, time) => calendar.Eras[0];
                calendarProxy.GetGregorianYearInt32Int32().Body = (@this, year, era) => 2000;
                calendarProxy.GetYearInfoInt32Int32().Body = (@this, lunarYear, index) => 41;
                var calendar_sut = (JapaneseLunisolarCalendar)calendarProxy;

                // Act
                var actual = calendar_sut.GetLeapMonth(26, calendar.Eras[0]);

                // Assert
                Assert.AreEqual(42, actual);
                Assert.AreNotEqual(calendar.GetLeapMonth(26, calendar.Eras[0]), actual);
            }
        }

        [Test]
        public void CalEraInfo_should_be_callable_indirectly_against_only_specified_instance()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var calendar = new JapaneseLunisolarCalendar();

                var calendarProxy = new PProxyJapaneseLunisolarCalendar();
                var japaneseLunisolarCalendar_get_CalEraInfo = default(MethodInfo);
                var expected = default(Array);
                MakeCalEraInfoTestData(out japaneseLunisolarCalendar_get_CalEraInfo, out expected);
                calendarProxy.CalEraInfoGet().Body = args => expected;
                var calendar_sut = (JapaneseLunisolarCalendar)calendarProxy;


                // Act
                var actual = japaneseLunisolarCalendar_get_CalEraInfo.Invoke(calendar_sut, null) as IEnumerable;


                // Assert
                CollectionAssert.AreEqual(expected, actual);
                CollectionAssert.AreNotEqual(japaneseLunisolarCalendar_get_CalEraInfo.Invoke(calendar, null) as IEnumerable, actual);
            }
        }



        static void MakeCalEraInfoTestData(out MethodInfo japaneseLunisolarCalendar_get_CalEraInfo, out Array expected)
        {
            var japaneseLunisolarCalendar = typeof(JapaneseLunisolarCalendar);
            japaneseLunisolarCalendar_get_CalEraInfo = japaneseLunisolarCalendar.GetMethod("get_CalEraInfo",
                                                                                           BindingFlags.NonPublic |
                                                                                           BindingFlags.Instance);
            var eraInfoArr = japaneseLunisolarCalendar_get_CalEraInfo.ReturnType;
            var eraInfo = eraInfoArr.GetElementType();
            var eraInfo_ctor = eraInfo.GetConstructor(BindingFlags.NonPublic |
                                                      BindingFlags.Instance,
                                                      null,
#if _NET_3_5
                                                      new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) },
#else
                                                      new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) },
#endif
                                                      null);
#if _NET_3_5
            var expectedElem = eraInfo_ctor.Invoke(new object[] { 42, 1900, 1, 1, 20 });
#else
            var expectedElem = eraInfo_ctor.Invoke(new object[] { 42, 1900, 1, 1, 20, 1, 13 });
#endif
            expected = Array.CreateInstance(eraInfo, 1);
            expected.SetValue(expectedElem, 0);
        }
    }
}
