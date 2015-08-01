/* 
 * File: LifeInfoTest.cs
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
using program1.MyLibrary;
using System;
using System.Collections;
using System.Prig;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.MyLibrary
{
    [TestFixture]
    public class LifeInfoTest
    {
        [Test]
        public void IsNowLunchBreak_should_return_false_when_11_oclock()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 11, 00, 00);
                
                // Act
                var result = LifeInfo.IsNowLunchBreak();

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void IsNowLunchBreak_should_return_true_when_12_oclock()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 12, 00, 00);

                // Act
                var result = LifeInfo.IsNowLunchBreak();

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void IsNowLunchBreak_should_return_false_when_13_oclock()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 13, 00, 00);

                // Act
                var result = LifeInfo.IsNowLunchBreak();

                // Assert
                Assert.IsFalse(result);
            }
        }



        [Test]
        [TestCaseSource(typeof(IsTodayHolidayTestSource), "TestCases")]
        public bool IsTodayHoliday_should_consider_a_set_day_and_the_previous_day_as_holiday(DateTime today, DayOfWeek holiday)
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.TodayGet().Body = () => today;
                PULConfigurationManager.GetPropertyOfTStringT<DayOfWeek>().Body = (key, defaultValue) => holiday;

                // Act, Assert
                return LifeInfo.IsTodayHoliday();
            }
        }

        class IsTodayHolidayTestSource
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return new TestCaseData(new DateTime(2013, 11, 16), DayOfWeek.Sunday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 17), DayOfWeek.Sunday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 18), DayOfWeek.Sunday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 17), DayOfWeek.Monday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 18), DayOfWeek.Monday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 19), DayOfWeek.Monday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 18), DayOfWeek.Tuesday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 19), DayOfWeek.Tuesday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 20), DayOfWeek.Tuesday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 19), DayOfWeek.Wednesday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 20), DayOfWeek.Wednesday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 21), DayOfWeek.Wednesday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 20), DayOfWeek.Thursday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 21), DayOfWeek.Thursday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 22), DayOfWeek.Thursday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 21), DayOfWeek.Friday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 22), DayOfWeek.Friday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 23), DayOfWeek.Friday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 22), DayOfWeek.Saturday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 23), DayOfWeek.Saturday).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 24), DayOfWeek.Saturday).Returns(false);

                    yield return new TestCaseData(new DateTime(2013, 11, 23), (DayOfWeek)99).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 24), (DayOfWeek)99).Returns(true);
                    yield return new TestCaseData(new DateTime(2013, 11, 25), (DayOfWeek)99).Returns(false);
                }
            }
        }



        [Test]
        public void IsNowLunchBreak_should_rethrow_any_exception_if_an_exception_is_thrown_internally()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                IndirectionsContext.
                    ExcludeGeneric().
                    DefaultBehavior = IndirectionBehaviors.NotImplemented;

                // Act, Assert
                Assert.Throws<NotImplementedException>(() => LifeInfo.IsNowLunchBreak());
            }
        }

        [Test]
        public void IsNowLunchBreak_should_return_false_if_default_value_is_returned_internally()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                IndirectionsContext.
                    ExcludeGeneric().
                    DefaultBehavior = IndirectionBehaviors.DefaultValue;

                // Act
                var result = LifeInfo.IsNowLunchBreak();

                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void IsNowLunchBreak_should_return_indeterminate_result_if_original_behavior_is_performed_internally()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                IndirectionsContext.
                    ExcludeGeneric().
                    DefaultBehavior = IndirectionBehaviors.Fallthrough;

                // Act, Assert
                Assert.DoesNotThrow(() => LifeInfo.IsNowLunchBreak());
            }
        }
    }    
}
