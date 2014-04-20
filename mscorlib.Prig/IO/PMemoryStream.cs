/* 
 * File: PMemoryStream.cs
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


using System.IO.Prig;
using Urasandesu.Prig.Framework;

[assembly: Indirectable(PMemoryStream.TokenOfSeek_Func_MemoryStream_long_SeekOrigin_long)]

namespace System.IO.Prig
{
    public static class PMemoryStream
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfSeek_Func_MemoryStream_long_SeekOrigin_long = 0x060035FF;
#else
        internal const int TokenOfSeek_Func_MemoryStream_long_SeekOrigin_long = 0x06003652;
#endif
#else
#if _M_IX86
        internal const int TokenOfSeek_Func_MemoryStream_long_SeekOrigin_long = 0x06004755;
#else
        internal const int TokenOfSeek_Func_MemoryStream_long_SeekOrigin_long = 0x06004752;
#endif
#endif

        public static class Seek
        {
            public static IndirectionFunc<MemoryStream, long, SeekOrigin, long> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(MemoryStream).Assembly.FullName;
                    info.Token = TokenOfSeek_Func_MemoryStream_long_SeekOrigin_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<MemoryStream, long, SeekOrigin, long>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
