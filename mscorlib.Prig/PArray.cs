/* 
 * File: PArray.cs
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


using System.Collections;
using System.Prig;
using Urasandesu.Prig.Framework;

[assembly: Indirectable(PArray.TokenOfCreateInstance_Func_Type_Int32Array_Int32Array_Array)]
[assembly: Indirectable(PArray.TokenOfExists_T_Func_TArray_Predicate_T_bool)]
[assembly: Indirectable(PArray.TokenOfBinarySearch_Func_Array_int_int_object_IComparer_int)]

namespace System.Prig
{
    public static class PArray
    {
#if _NET_3_5
        internal const int TokenOfCreateInstance_Func_Type_Int32Array_Int32Array_Array = 0x06000029;
        internal const int TokenOfExists_T_Func_TArray_Predicate_T_bool = 0x06000068;
        internal const int TokenOfBinarySearch_Func_Array_int_int_object_IComparer_int = 0x0600005F;
#else
        internal const int TokenOfCreateInstance_Func_Type_Int32Array_Int32Array_Array = 0x0600018C;
        internal const int TokenOfExists_T_Func_TArray_Predicate_T_bool = 0x060001D0;
        internal const int TokenOfBinarySearch_Func_Array_int_int_object_IComparer_int = 0x060001C7;
#endif

        public static class CreateInstance
        {
            public static IndirectionFunc<Type, int[], int[], Array> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Array).Assembly.FullName;
                    info.Token = TokenOfCreateInstance_Func_Type_Int32Array_Int32Array_Array;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<Type, int[], int[], Array>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class Exists<T>
        {
            public static IndirectionFunc<T[], Predicate<T>, bool> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Array).Assembly.FullName;
                    info.Token = TokenOfExists_T_Func_TArray_Predicate_T_bool;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], Predicate<T>, bool>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }

        public static class BinarySearch
        {
            public static IndirectionFunc<Array, int, int, object, IComparer, int> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(Array).Assembly.FullName;
                    info.Token = TokenOfBinarySearch_Func_Array_int_int_object_IComparer_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<Array, int, int, object, IComparer, int>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
