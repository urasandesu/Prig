/* 
 * File: Program.cs
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


#define PDateTime

using program1.MyLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using System.Prig;
using System.Reflection;
using System.Threading;
using Urasandesu.Prig.Framework;

namespace program1
{
    class Program
    {
        static void Main(string[] args)
        {
#if PDateTime
            //Thread.Sleep(10000);
            //Console.WriteLine("★入れ替え前: {0}", DateTime.Now);
            //PDateTime.NowGet.Body = () => new DateTime(2014, 1, 1);
            //Console.WriteLine("★入れ替え中: {0}", DateTime.Now);
            //PDateTime.NowGet.Body = null;
            //Console.WriteLine("★戻し:       {0}", DateTime.Now);
            Console.WriteLine("★入れ替え前: {0}", DateTime.Now);
            using (new IndirectionsContext())
            {
                PDateTime.NowGet.Body = () => new DateTime(2014, 1, 1);
                Console.WriteLine("★入れ替え中: {0}", DateTime.Now);
            }
            Console.WriteLine("★戻し:       {0}", DateTime.Now);
#elif PException
            Console.WriteLine("★入れ替え前: {0}", InternalToString());
            using (new IndirectionsContext())
            {
                PException.InternalToString.Body = () => "にゃんぱすー";
                Console.WriteLine("★入れ替え中: {0}", InternalToString());
            }
            Console.WriteLine("★戻し:       {0}", InternalToString());
#else
            Console.WriteLine("★入れ替え前: {0}", ParseOrDefault("10"));
            using (new IndirectionsContext())
            {
                PInt32.TryParse.Body = (string s, out int result) => { result = 42; return true; };
                Console.WriteLine("★入れ替え中: {0}", ParseOrDefault("10"));
            }
            Console.WriteLine("★戻し:       {0}", ParseOrDefault("10"));
#endif
        }

        static string InternalToString()
        {
            var e = new NotImplementedException();
            var internalToStringInfo = typeof(NotImplementedException).GetMethod("InternalToString", BindingFlags.Instance | BindingFlags.NonPublic);
            return (string)internalToStringInfo.Invoke(e, new object[0]);
        }

        static int ParseOrDefault(string s)
        {
            var result = default(int);
            return int.TryParse(s, out result) ? result : default(int);
        }
    }
}
