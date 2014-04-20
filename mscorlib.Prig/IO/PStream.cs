/* 
 * File: PStream.cs
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

[assembly: Indirectable(PStream.TokenOfBeginRead_IndirectionFunc_Stream_ByteArray_int_int_AsyncCallback_object_IAsyncResult)]

namespace System.IO.Prig
{
    public static class PStream
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfBeginRead_IndirectionFunc_Stream_ByteArray_int_int_AsyncCallback_object_IAsyncResult = 0x06003400;
#else
        internal const int TokenOfBeginRead_IndirectionFunc_Stream_ByteArray_int_int_AsyncCallback_object_IAsyncResult = 0x06003453;
#endif
#else
#if _M_IX86
        internal const int TokenOfBeginRead_IndirectionFunc_Stream_ByteArray_int_int_AsyncCallback_object_IAsyncResult = 0x06004488;
#else
        internal const int TokenOfBeginRead_IndirectionFunc_Stream_ByteArray_int_int_AsyncCallback_object_IAsyncResult = 0x06004485;
#endif
#endif

        public static class BeginRead
        {
            public static IndirectionFunc<Stream, byte[], int, int, AsyncCallback, object, IAsyncResult> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Stream).Assembly.FullName;
                    info.Token = TokenOfBeginRead_IndirectionFunc_Stream_ByteArray_int_int_AsyncCallback_object_IAsyncResult;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<Stream, byte[], int, int, AsyncCallback, object, IAsyncResult>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
