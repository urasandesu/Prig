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
