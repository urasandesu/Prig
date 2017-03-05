/* 
 * File: LooseCrossDomainAccessor`1.cs
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
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using Urasandesu.NAnonym.Mixins.System;

namespace Urasandesu.Prig.Framework
{
    public class LooseCrossDomainAccessor<T> where T : InstanceHolder<T>
    {
        static readonly object ms_lockObj = new object();
        static T ms_holder = null;
        static bool ms_ready = false;
        static readonly Type ms_t = typeof(T);
        static readonly string ms_key = InstanceGetters.DisableProcessing().EnsureDisposalThen(_ => ms_t.AssemblyQualifiedName);

        static LooseCrossDomainAccessor()
        {
            using (InstanceGetters.DisableProcessing())
            {
                var all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
                foreach (var method in typeof(LooseCrossDomainAccessor<T>).GetMethods(all))
                    RuntimeHelpers.PrepareMethod(method.MethodHandle);
            }
        }

        protected LooseCrossDomainAccessor() { }

        public static void Register()
        {
            if (ms_key == null)
                return;

            var funcPtr = default(IntPtr);
            using (InstanceGetters.DisableProcessing())
                funcPtr = GetFunctionPointerCore(ms_t);
            InstanceGetters.TryAdd(ms_key, funcPtr);
        }

        static IntPtr GetFunctionPointerCore(Type t)
        {
            var instance = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var instanceGetter = instance.GetGetMethod();
            return instanceGetter.MethodHandle.GetFunctionPointer();
        }

        static T GetOrRegisterHolder()
        {
            if (ms_key == null)
                return default(T);

            var funcPtr = default(IntPtr);
            if (InstanceGetters.TryGet(ms_key, out funcPtr))
            {
                using (InstanceGetters.DisableProcessing())
                    return GetHolderCore(ms_t, funcPtr);
            }
            else
            {
                var funcPtrTmp = default(IntPtr);
                using (InstanceGetters.DisableProcessing())
                    funcPtrTmp = GetFunctionPointerCore(ms_t);

                while (!InstanceGetters.GetOrAdd(ms_key, funcPtrTmp, out funcPtr)) ;

                using (InstanceGetters.DisableProcessing())
                    return GetHolderCore(ms_t, funcPtr);
            }
        }

        static T GetHolderCore(Type t, IntPtr funcPtr)
        {
            var extractor = new DynamicMethod("Extractor", t, null, typeof(InstanceGetters).Module);
            var gen = extractor.GetILGenerator();
            if (IntPtr.Size == 4)
            {
                gen.Emit(OpCodes.Ldc_I4, funcPtr.ToInt32());
            }
            else if (IntPtr.Size == 8)
            {
                gen.Emit(OpCodes.Ldc_I8, funcPtr.ToInt64());
            }
            else
            {
                throw new NotSupportedException();
            }
            gen.EmitCalli(OpCodes.Calli, CallingConventions.Standard, t, null, null);
            gen.Emit(OpCodes.Ret);
            var holder = ((Func<T>)extractor.CreateDelegate(typeof(Func<T>)))();
            holder.Prepare();
            return holder;
        }

        static bool TryGetHolder(out T holder)
        {
            holder = default(T);
            if (ms_key == null)
                return false;

            var funcPtr = default(IntPtr);
            if (!InstanceGetters.TryGet(ms_key, out funcPtr))
            {
                holder = null;
                return false;
            }
            else
            {
                using (InstanceGetters.DisableProcessing())
                    holder = GetHolderCore(ms_t, funcPtr);
                return true;
            }
        }

        public static T Holder
        {
            get
            {
                if (!ms_ready)
                {
                    var holder = default(T);
                    if (!TryGetHolder(out holder))
                        using (InstanceGetters.DisableProcessing())
                            throw new InvalidOperationException(string.Format("T({0}) has not been registered yet. Please call Register method.", typeof(T)));

                    lock (ms_lockObj)
                    {
                        Thread.MemoryBarrier();
                        if (!ms_ready)
                        {
                            ms_holder = holder;
                            ms_ready = true;
                            Thread.MemoryBarrier();
                        }
                    }
                }
                return ms_holder;
            }
        }

        public static T HolderOrDefault
        {
            get
            {
                if (!ms_ready)
                {
                    var holder = default(T);
                    var existsHolder = TryGetHolder(out holder);

                    lock (ms_lockObj)
                    {
                        Thread.MemoryBarrier();
                        if (!ms_ready)
                        {
                            if (existsHolder)
                            {
                                ms_holder = holder;
                                ms_ready = true;
                                Thread.MemoryBarrier();
                            }
                        }
                    }
                }
                return ms_holder;
            }
        }

        public static T HolderOrRegistered
        {
            get
            {
                if (!ms_ready)
                {
                    var holder = GetOrRegisterHolder();

                    lock (ms_lockObj)
                    {
                        Thread.MemoryBarrier();
                        if (!ms_ready)
                        {
                            ms_holder = holder;
                            if (ms_holder != null)
                            {
                                ms_ready = true;
                                Thread.MemoryBarrier();
                            }
                        }
                    }
                }
                return ms_holder;
            }
        }

        public static void Unload()
        {
            var holder = ms_holder;
            if (holder != null)
                holder.Dispose();

            lock (ms_lockObj)
            {
                if (ms_holder != null)
                {
                    ms_holder = null;
                    ms_ready = false;
                    Thread.MemoryBarrier();
                }
            }
        }
    }
}
