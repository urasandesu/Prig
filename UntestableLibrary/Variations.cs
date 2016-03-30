/* 
 * File: Variations.cs
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



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace UntestableLibrary
{
    // Variation_HASTHIS_GENERICMETHOD_RETTYPE_PARAM_PARAMTYPE_ARRAY_GENERICINST_MVAR_SZARRAY_VAR
    // 
    // HASTHIS:         None, HasThis, HasGenericThis
    // GENERICMETHOD:   Default, Generic
    // RETTYPE:         void, bool | char | sbyte | byte | short | ushort | int | uint | long | ulong | float | double | IntPtr | UIntPtr | string | object, CLASS, VALUETYPE, TYPESPEC
    // PARAM:           None, ByRef
    // PARAMTYPE:       bool | char | sbyte | byte | short | ushort | int | uint | long | ulong | float | double | IntPtr | UIntPtr | string | object, CLASS, VALUETYPE, TYPESPEC
    // ARRAY:           None, Array
    // GENERICINST:     None, GenericInst
    // MVAR:            None, MVar
    // SZARRAY:         None, SZArray
    // VAR:             None, Var
    // 
    // Against issue #65, add some generic parameter constraint variation.
    public class Variation_HasGenericThis_Default_bool_None_CLASS_None_None_None_None_None<T>
    {
        public bool Do(Regex r)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThisStructConstrained_Default_bool_None_CLASS_None_None_None_None_None<T> where T : struct
    {
        public bool Do(Regex r)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_TYPESPEC_ByRef_CLASS_Array_None_None_SZArray_Var<T>
    {
        public T[,][] Do<M>(Regex r)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThisClassConstrained_Generic_TYPESPEC_ByRef_CLASS_Array_None_None_SZArray_Var<T> where T : class
    {
        public T[,][] Do<M>(Regex r)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_GenericStructConstrained_TYPESPEC_ByRef_CLASS_Array_None_None_SZArray_Var<T>
    {
        public T[,][] Do<M>(Regex r) where M : struct
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Generic_VALUETYPE_ByRef_TYPESPEC_None_None_MVar_SZArray_None
    {
        public DateTime Do<M>(out M[] mArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_GenericClassConstrained_VALUETYPE_ByRef_TYPESPEC_None_None_MVar_SZArray_None
    {
        public DateTime Do<M>(out M[] mArr) where M : class
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Default_CLASS_None_TYPESPEC_Array_None_None_None_Var<T>
    {
        public Regex Do(T[,] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThisNewConstrained_Default_CLASS_None_TYPESPEC_Array_None_None_None_Var<T> where T : new()
    {
        public Regex Do(T[,] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Generic_TYPESPEC_None_VALUETYPE_Array_None_MVar_None_None
    {
        public static M[,] Do<M>(DateTime dt)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_GenericNewConstrained_TYPESPEC_None_VALUETYPE_Array_None_MVar_None_None
    {
        public static M[,] Do<M>(DateTime dt) where M : new()
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Default_TYPESPEC_None_bool_None_GenericInst_None_SZArray_None
    {
        public static DateTime?[] Do(bool b)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Generic_CLASS_ByRef_TYPESPEC_None_None_MVar_None_None
    {
        public Regex Do<M>(out M m)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_GenericBaseClassConstrained_CLASS_ByRef_TYPESPEC_None_None_MVar_None_None
    {
        public Regex Do<M>(out M m) where M : WebRequest
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Default_void_ByRef_VALUETYPE_None_None_None_None_None
    {
        public void Do(out DateTime dt)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_char_ByRef_TYPESPEC_Array_None_None_SZArray_Var<T>
    {
        public char Do<M>(out T[][,] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThisBaseClassConstrained_Generic_char_ByRef_TYPESPEC_Array_None_None_SZArray_Var<T> where T : WebRequest
    {
        public char Do<M>(out T[][,] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_GenericInterfaceConstrained_char_ByRef_TYPESPEC_Array_None_None_SZArray_Var<T>
    {
        public char Do<M>(out T[][,] tArr) where M : IWebProxy
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Generic_void_None_char_None_None_None_None_None
    {
        public void Do<M>(char c)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_GenericSuppliedArgumentConstrained_void_None_char_None_None_None_None_None
    {
        public void Do<M, N>(char c) where M : N
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Generic_TYPESPEC_ByRef_sbyte_Array_None_MVar_SZArray_None
    {
        public M[,][] Do<M>(sbyte i1)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Default_VALUETYPE_None_CLASS_None_None_None_None_None
    {
        public static DateTime Do(Regex r)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Generic_sbyte_ByRef_TYPESPEC_Array_None_MVar_SZArray_None
    {
        public sbyte Do<M>(out M[][,] mArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_VALUETYPE_ByRef_byte_None_None_None_None_None<T>
    {
        public DateTime Do<M>(out byte u1)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThisInterfaceConstrained_Generic_VALUETYPE_ByRef_byte_None_None_None_None_None<T> where T : IWebProxy
    {
        public DateTime Do<M>(out byte u1)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_None_SZArray_Var<T>
    {
        public void Do<M>(out T[,][] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThisSuppliedArgumentConstrained_Generic_void_ByRef_TYPESPEC_Array_None_None_SZArray_Var<T, U> where T : U
    {
        public void Do<M>(out T[,][] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Generic_VALUETYPE_ByRef_TYPESPEC_Array_GenericInst_None_None_None
    {
        public static DateTime Do<M>(out DateTime?[] dtArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Default_CLASS_ByRef_VALUETYPE_None_None_None_None_None<T>
    {
        public Regex Do(out DateTime dt)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_byte_None_TYPESPEC_None_GenericInst_None_SZArray_None<T>
    {
        public byte Do<M>(List<DateTime[]> list)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Generic_short_None_TYPESPEC_None_None_MVar_SZArray_None
    {
        public static short Do<M>(M[] mArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Default_CLASS_ByRef_short_None_None_None_None_None
    {
        public static Regex Do(out short i2)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_TYPESPEC_None_VALUETYPE_None_None_None_SZArray_Var<T>
    {
        public T[] Do<M>(DateTime dt)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Default_TYPESPEC_None_CLASS_None_GenericInst_None_SZArray_None
    {
        public List<decimal>[] Do(CultureInfo ci)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Default_CLASS_ByRef_TYPESPEC_Array_GenericInst_None_SZArray_None<T>
    {
        public CultureInfo Do(List<DateTime[]>[,] list)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Generic_void_ByRef_CLASS_None_None_None_None_None
    {
        public static void Do<M>(out CultureInfo ci)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Default_VALUETYPE_ByRef_TYPESPEC_None_None_None_SZArray_Var<T>
    {
        public decimal Do(out T[] tArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Default_ushort_None_VALUETYPE_None_None_None_None_None<T>
    {
        public ushort Do(decimal dec)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Default_void_None_TYPESPEC_Array_GenericInst_None_None_None
    {
        public void Do(List<DateTime>[] arr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_MVar_SZArray_None<T>
    {
        public void Do<M>(out M[,][] mArr)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_None_Generic_TYPESPEC_None_VALUETYPE_Array_GenericInst_None_SZArray_None
    {
        public static List<DateTime[,]>[] Do<M>(decimal dec)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasThis_Generic_TYPESPEC_ByRef_CLASS_None_None_MVar_SZArray_None
    {
        public decimal[][,] Do<M>(out Regex r)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
    public class Variation_HasGenericThis_Default_TYPESPEC_None_ushort_None_None_None_SZArray_Var<T>
    {
        public T[] Do(ushort u2)
        {
            throw new InvalidOperationException("We shouldn't get here!!");
        }
    }
}
