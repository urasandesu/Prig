/* 
 * File: PConvert.cs
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

[assembly: Indirectable(PConvert.TokenOfToInt32_Func_double_int)]
[assembly: Indirectable(PConvert.TokenOfToSByte_Func_char_sbyte)]
[assembly: Indirectable(PConvert.TokenOfToInt16_Func_char_Int16)]
[assembly: Indirectable(PConvert.TokenOfToInt64_Func_double_long)]
[assembly: Indirectable(PConvert.TokenOfToBoolean_Func_float_bool)]

namespace System.Prig
{
    public static class PConvert
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfToInt32_Func_double_int = 0x0600087E;
        internal const int TokenOfToSByte_Func_char_sbyte = 0x0600082D;
        internal const int TokenOfToInt16_Func_char_Int16 = 0x06000850;
        internal const int TokenOfToInt64_Func_double_long = 0x060008A2;
        internal const int TokenOfToBoolean_Func_float_bool = 0x06000813;
#else
        internal const int TokenOfToInt32_Func_double_int = 0x06000880;
        internal const int TokenOfToSByte_Func_char_sbyte = 0x0600082F;
        internal const int TokenOfToInt16_Func_char_Int16 = 0x06000852;
        internal const int TokenOfToInt64_Func_double_long = 0x060008A4;
        internal const int TokenOfToBoolean_Func_float_bool = 0x06000815;
#endif
#else
        internal const int TokenOfToInt32_Func_double_int = 0x06000A6B;
        internal const int TokenOfToSByte_Func_char_sbyte = 0x06000A1A;
        internal const int TokenOfToInt16_Func_char_Int16 = 0x06000A3D;
        internal const int TokenOfToInt64_Func_double_long = 0x06000A8F;
        internal const int TokenOfToBoolean_Func_float_bool = 0x06000A00;
#endif

        public static class ToInt32
        {
            public static IndirectionFunc<double, int> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Convert).Assembly.FullName;
                    info.Token = TokenOfToInt32_Func_double_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<double, int>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class ToSByte
        {
            public static IndirectionFunc<char, sbyte> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Convert).Assembly.FullName;
                    info.Token = TokenOfToSByte_Func_char_sbyte;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<char, sbyte>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class ToInt16
        {
            public static IndirectionFunc<char, Int16> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Convert).Assembly.FullName;
                    info.Token = TokenOfToInt16_Func_char_Int16;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<char, Int16>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class ToInt64
        {
            public static IndirectionFunc<double, long> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Convert).Assembly.FullName;
                    info.Token = TokenOfToInt64_Func_double_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<double, long>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class ToBoolean
        {
            public static IndirectionFunc<float, bool> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Convert).Assembly.FullName;
                    info.Token = TokenOfToBoolean_Func_float_bool;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<float, bool>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
