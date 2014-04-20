/* 
 * File: PDateTime.cs
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


using System.Prig;
using Urasandesu.Prig.Framework;

[assembly: Indirectable(PDateTime.TokenOfNowGet_Func_DateTime)]
[assembly: Indirectable(PDateTime.TokenOfFromBinary_Func_long_DateTime)]
[assembly: Indirectable(PDateTime.TokenOfDoubleDateToTicks_Func_double_long)]
[assembly: Indirectable(PDateTime.TokenOfCompareTo_RefThisFunc_DateTime_object_int)]

namespace System.Prig
{
    public class PDateTime
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfNowGet_Func_DateTime = 0x060002D2;
        internal const int TokenOfFromBinary_Func_long_DateTime = 0x060002BC;
        internal const int TokenOfDoubleDateToTicks_Func_double_long = 0x060002B8;
        internal const int TokenOfCompareTo_RefThisFunc_DateTime_object_int = 0x060002B3;
#else
        internal const int TokenOfNowGet_Func_DateTime = 0x060002D4;
        internal const int TokenOfFromBinary_Func_long_DateTime = 0x060002BE;
        internal const int TokenOfDoubleDateToTicks_Func_double_long = 0x060002BA;
        internal const int TokenOfCompareTo_RefThisFunc_DateTime_object_int = 0x060002B5;
#endif
#else
        internal const int TokenOfNowGet_Func_DateTime = 0x0600047B;
        internal const int TokenOfFromBinary_Func_long_DateTime = 0x06000465;
        internal const int TokenOfDoubleDateToTicks_Func_double_long = 0x0600045F;
        internal const int TokenOfCompareTo_RefThisFunc_DateTime_object_int = 0x0600045C;
#endif

        public static class NowGet
        {
            public static IndirectionFunc<DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(DateTime).Assembly.FullName;
                    info.Token = TokenOfNowGet_Func_DateTime;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class FromBinary
        {
            public static IndirectionFunc<long, DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(DateTime).Assembly.FullName;
                    info.Token = TokenOfFromBinary_Func_long_DateTime;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<long, DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class DoubleDateToTicks
        {
            public static IndirectionFunc<double, long> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(DateTime).Assembly.FullName;
                    info.Token = TokenOfDoubleDateToTicks_Func_double_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<double, long>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class CompareTo
        {
            public static IndirectionRefThisFunc<DateTime, object, int> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(DateTime).Assembly.FullName;
                    info.Token = TokenOfCompareTo_RefThisFunc_DateTime_object_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisFunc<DateTime, object, int>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
