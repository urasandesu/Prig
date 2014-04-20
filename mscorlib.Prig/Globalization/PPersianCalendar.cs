/* 
 * File: PPersianCalendar.cs
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


using System.Globalization.Prig;
using Urasandesu.Prig.Framework;

[assembly: Indirectable(PPersianCalendar.TokenOfCheckTicksRange_Action_PersianCalendar_long)]

namespace System.Globalization.Prig
{
    public static class PPersianCalendar
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfCheckTicksRange_Action_PersianCalendar_long = 0x06002626;
#else
        internal const int TokenOfCheckTicksRange_Action_PersianCalendar_long = 0x06002679;
#endif
#else
#if _M_IX86
        internal const int TokenOfCheckTicksRange_Action_PersianCalendar_long = 0x0600326B;
#else
        internal const int TokenOfCheckTicksRange_Action_PersianCalendar_long = 0x06003268;
#endif
#endif

        public static class CheckTicksRange
        {
#if _NET_3_5
            public static IndirectionAction<PersianCalendar, long> Body
#else
            public static IndirectionAction<long> Body
#endif
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(PersianCalendar).Assembly.FullName;
                    info.Token = TokenOfCheckTicksRange_Action_PersianCalendar_long;
#if _NET_3_5
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<PersianCalendar, long>>>();
#else
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<long>>>();
#endif
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
