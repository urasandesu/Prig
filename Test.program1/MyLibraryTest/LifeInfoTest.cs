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


using System;
using System.IO;
using System.Prig;
using NUnit.Framework;
using program1.MyLibrary;
using Urasandesu.Prig.Framework;

namespace Test.program1.MyLibraryTest
{
    // SET COR_ENABLE_PROFILING=1
    // SET COR_PROFILER={532C1F05-F8F3-4FBA-8724-699A31756ABD}
    // SET COMPLUS_ProfAPI_ProfilerCompatibilitySetting=EnableV2Profiler
    // program1.exe
    // 
    // "C:\Program Files (x86)\NUnit 2.5.10\bin\net-2.0\nunit-console-x86.exe" Test.program1.dll /domain=None
    [TestFixture]
    public class LifeInfoTest
    {
        [Test]
        public void LunchBreakTest01_NowIsLunchBreak()
        {
            using (new IndirectionsContext())
            using (new ConsoleContext())
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                PDateTime.NowGet.Body = () => new DateTime(2011, 12, 13, 12, 00, 00);
                LifeInfo.LunchBreak();
                Assert.AreEqual("時刻: 12\tお昼休みなう！" + sw.NewLine, sw.ToString());
            }
        }

        [Test]
        public void LunchBreakTest02_NowIsNotLunchBreak()
        {
            using (new IndirectionsContext())
            using (new ConsoleContext())
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                PDateTime.NowGet.Body = () => new DateTime(2011, 12, 13, 13, 00, 00);
                LifeInfo.LunchBreak();
                Assert.AreEqual("時刻: 13\tお仕事なう・・・" + sw.NewLine, sw.ToString());
            }
        }

        [Test]
        public void LunchBreakTest01_NowIsHoliday()
        {
            using (new IndirectionsContext())
            using (new ConsoleContext())
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                PDateTime.NowGet.Body = () => new DateTime(2011, 12, 18, 00, 00, 00);
                LifeInfo.Holiday();
                Assert.AreEqual("曜日: Sunday\t休日なう！" + sw.NewLine, sw.ToString());
            }
        }

        [Test]
        public void LunchBreakTest02_NowIsNotHoliday()
        {
            using (new IndirectionsContext())
            using (new ConsoleContext())
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                PDateTime.NowGet.Body = () => new DateTime(2011, 12, 19, 00, 00, 00);
                LifeInfo.Holiday();
                Assert.AreEqual("曜日: Monday\tお仕事なう・・・" + sw.NewLine, sw.ToString());
            }
        }
    }
}
