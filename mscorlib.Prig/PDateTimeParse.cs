/* 
 * File: PInt32.cs
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


using System.Globalization;
using System.Prig;
using Urasandesu.Prig.Framework;

[assembly: Indirectable(PDateTimeParse.TokenOfTryParse_OutFunc_string_DateTimeFormatInfo_DateTimeStyles_DateTime_bool)]

namespace System.Prig
{
    public static class PDateTimeParse
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfTryParse_OutFunc_string_DateTimeFormatInfo_DateTimeStyles_DateTime_bool = 0x0600245B;
#else
        internal const int TokenOfTryParse_OutFunc_string_DateTimeFormatInfo_DateTimeStyles_DateTime_bool = 0x060024AE;
#endif
#else
#if _M_IX86
        internal const int TokenOfTryParse_OutFunc_string_DateTimeFormatInfo_DateTimeStyles_DateTime_bool = 0x06003043;
#else
        internal const int TokenOfTryParse_OutFunc_string_DateTimeFormatInfo_DateTimeStyles_DateTime_bool = 0x06003040;
#endif
#endif

        public static class TryParse
        {
            public static IndirectionOutFunc<string, DateTimeFormatInfo, DateTimeStyles, DateTime, bool> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(DateTime).Assembly.FullName; // This class is commonly used from class DateTime.
                    info.Token = TokenOfTryParse_OutFunc_string_DateTimeFormatInfo_DateTimeStyles_DateTime_bool;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<string, DateTimeFormatInfo, DateTimeStyles, DateTime, bool>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
