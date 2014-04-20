/* 
 * File: PException.cs
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

[assembly: Indirectable(PException.TokenOfInternalToString_Func_Exception_string)]

namespace System.Prig
{
    public static class PException
    {
#if _NET_3_5
#if _M_IX86
        internal const int TokenOfInternalToString_Func_Exception_string = 0x06000293;
#else
        internal const int TokenOfInternalToString_Func_Exception_string = 0x06000295;
#endif
#else
        internal const int TokenOfInternalToString_Func_Exception_string = 0x06000041;
#endif

        public static class InternalToString
        {
            public static IndirectionFunc<Exception, string> Body
            {
                set
                {
                    var t = typeof(Exception);
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Exception).Assembly.FullName;
                    info.Token = TokenOfInternalToString_Func_Exception_string;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<Exception, string>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
