using System;
using program1.ThirdPartyLibrary;

namespace program1.MyLibrary
{
    public static class LifeInfo
    {
        public static void LunchBreak()
        {
            var now = DateTime.Now;
            Console.WriteLine("時刻: " + now.Hour + "\t" +
                (12 <= now.Hour && now.Hour < 13 ? "お昼休みなう！" : "お仕事なう・・・"));
        }

        public static void Holiday()
        {
            var dayOfWeek = ConfigurationManager.GetProperty("Holiday", DayOfWeek.Sunday);
            var now = DateTime.Now;
            Console.WriteLine("曜日: " + now.DayOfWeek + "\t" +
                (now.DayOfWeek == dayOfWeek ? "休日なう！" : "お仕事なう・・・"));
        }
    }
}
