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


using Urasandesu.Prig.Framework;

#if _M_IX86
[assembly: Indirectable(0x0600087E)]
#else
[assembly: Indirectable(0x06000880)]
#endif
[assembly: Indirectable(0x0600082D)]
[assembly: Indirectable(0x06000850)]
[assembly: Indirectable(0x060008A2)]
[assembly: Indirectable(0x06000813)]

namespace System.Prig
{
    public static class PConvert
    {
        public static class ToInt32
        {
            public static IndirectionFunc<double, int> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
#if _M_IX86
                    info.Token = 0x0600087E;
#else
                    info.Token = 0x06000880;
#endif
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
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = 0x0600082D;
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
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = 0x06000850;
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
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = 0x060008A2;
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
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = 0x06000813;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<float, bool>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
