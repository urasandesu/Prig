/* 
 * File: IndirectionDelegates.cs
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



namespace Urasandesu.Prig.Framework
{
    [IndirectionDelegate]
    public delegate void IndirectionAction();
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T>(T obj);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2>(T1 arg1, T2 arg2);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
    [IndirectionDelegate]
    public delegate void IndirectionAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<TOut>(out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T, TOut>(T obj, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, TOut>(T1 arg1, T2 arg2, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, TOut>(T1 arg1, T2 arg2, T3 arg3, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TOut>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, out TOut @out);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<TOut1, TOut2>(out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T, TOut1, TOut2>(T obj, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, TOut1, TOut2>(T1 arg1, T2 arg2, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionOutOutAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TOut1, TOut2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<TRef>(ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T, TRef>(T obj, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, TRef>(T1 arg1, T2 arg2, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, TRef>(T1 arg1, T2 arg2, T3 arg3, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TRef>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ref TRef @ref);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<TRef1, TRef2>(ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T, TRef1, TRef2>(T obj, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, TRef1, TRef2>(T1 arg1, T2 arg2, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate void IndirectionRefRefAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TRef1, TRef2>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<out TResult>();
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T, out TResult>(T arg);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);
    [IndirectionDelegate]
    public delegate TResult IndirectionFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<TOut, out TResult>(out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T, TOut, out TResult>(T arg, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, TOut, out TResult>(T1 arg1, T2 arg2, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TOut, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, out TOut @out);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<TOut1, TOut2, out TResult>(out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T, TOut1, TOut2, out TResult>(T arg, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionOutOutFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TOut1, TOut2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, out TOut1 out1, out TOut2 out2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<TRef, out TResult>(ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T, TRef, out TResult>(T arg, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, TRef, out TResult>(T1 arg1, T2 arg2, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TRef, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ref TRef @ref);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<TRef1, TRef2, out TResult>(ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T, TRef1, TRef2, out TResult>(T arg, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, ref TRef1 ref1, ref TRef2 ref2);
    [IndirectionDelegate]
    public delegate TResult IndirectionRefRefFunc<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, TRef1, TRef2, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, ref TRef1 ref1, ref TRef2 ref2);
}
