using System;
using System.IO;
using System.Prig;
using NUnit.Framework;
using program1.MyLibrary;

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
            using (new PDateTimeContext.NowGet())
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
            using (new PDateTimeContext.NowGet())
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
            using (new PDateTimeContext.NowGet())
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
            using (new PDateTimeContext.NowGet())
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
