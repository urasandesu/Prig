/* 
 * File: LifeInfo.cs
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
using UntestableLibrary;

namespace program1.MyLibrary
{
    public static class LifeInfo
    {
        public static bool IsNowLunchBreak()
        {
            var now = DateTime.Now;
            return 12 <= now.Hour && now.Hour < 13;
        }

        public static bool IsTodayHoliday()
        {
            var dayOfWeek = DateTime.Today.DayOfWeek;
            var holiday = ULConfigurationManager.GetProperty<DayOfWeek>("Holiday", DayOfWeek.Sunday);
            switch (holiday)
            {
                case DayOfWeek.Sunday:
                    return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
                case DayOfWeek.Monday:
                    return dayOfWeek == DayOfWeek.Sunday || dayOfWeek == DayOfWeek.Monday;
                case DayOfWeek.Tuesday:
                    return dayOfWeek == DayOfWeek.Monday || dayOfWeek == DayOfWeek.Tuesday;
                case DayOfWeek.Wednesday:
                    return dayOfWeek == DayOfWeek.Tuesday || dayOfWeek == DayOfWeek.Wednesday;
                case DayOfWeek.Thursday:
                    return dayOfWeek == DayOfWeek.Wednesday || dayOfWeek == DayOfWeek.Thursday;
                case DayOfWeek.Friday:
                    return dayOfWeek == DayOfWeek.Thursday || dayOfWeek == DayOfWeek.Friday;
                case DayOfWeek.Saturday:
                    return dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday;
                default:
                    return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
            }
        }
    }
}
