/* 
 * File: PStringBuilder.cs
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


using System.Text.Prig;
using Urasandesu.Prig.Framework;

[assembly: Indirectable(PStringBuilder.TokenOfInsert_Func_StringBuilder_int_string_int_StringBuilder)]
[assembly: Indirectable(PStringBuilder.TokenOfReplace_Func_StringBuilder_char_char_int_int_StringBuilder)]

namespace System.Text.Prig
{
    public static class PStringBuilder
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfInsert_Func_StringBuilder_int_string_int_StringBuilder = 0x0600023B;
        internal const int TokenOfReplace_Func_StringBuilder_char_char_int_int_StringBuilder = 0x06000267;
#else
        internal const int TokenOfInsert_Func_StringBuilder_int_string_int_StringBuilder = 0x0600023D;
        internal const int TokenOfReplace_Func_StringBuilder_char_char_int_int_StringBuilder = 0x06000269;
#endif
#else
        internal const int TokenOfInsert_Func_StringBuilder_int_string_int_StringBuilder = 0x060003FF;
        internal const int TokenOfReplace_Func_StringBuilder_char_char_int_int_StringBuilder = 0x0600042A;
#endif

        public static class Insert
        {
            public static IndirectionFunc<StringBuilder, int, string, int, StringBuilder> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(StringBuilder).Assembly.FullName;
                    info.Token = TokenOfInsert_Func_StringBuilder_int_string_int_StringBuilder;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<StringBuilder, int, string, int, StringBuilder>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class Replace
        {
            public static IndirectionFunc<StringBuilder, char, char, int, int, StringBuilder> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(StringBuilder).Assembly.FullName;
                    info.Token = TokenOfReplace_Func_StringBuilder_char_char_int_int_StringBuilder;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<StringBuilder, char, char, int, int, StringBuilder>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
