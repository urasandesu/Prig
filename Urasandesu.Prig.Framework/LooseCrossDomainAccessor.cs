/* 
 * File: LooseCrossDomainAccessor.cs
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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Urasandesu.Prig.Framework
{
    public class LooseCrossDomainAccessor
    {
        protected LooseCrossDomainAccessor() { }

        static readonly HashSet<Action> ms_unloadMethods = new HashSet<Action>();

        public static void Register<T>() where T : InstanceHolder<T>
        {
            LooseCrossDomainAccessor<T>.Register();
            lock (ms_unloadMethods)
                using (InstanceGetters.DisableProcessing())
                    ms_unloadMethods.Add(LooseCrossDomainAccessor<T>.Unload);
        }

        public static void Unload<T>() where T : InstanceHolder<T>
        {
            LooseCrossDomainAccessor<T>.Unload();
        }

        public static T Get<T>() where T : InstanceHolder<T>
        {
            return LooseCrossDomainAccessor<T>.Holder;
        }

        public static T GetOrRegister<T>() where T : InstanceHolder<T>
        {
            var holder = LooseCrossDomainAccessor<T>.HolderOrRegistered;
            lock (ms_unloadMethods)
                using (InstanceGetters.DisableProcessing())
                    ms_unloadMethods.Add(LooseCrossDomainAccessor<T>.Unload);
            return holder;
        }

        public static bool TryGet<T>(out T holder) where T : InstanceHolder<T>
        {
            holder = LooseCrossDomainAccessor<T>.HolderOrDefault;
            if (holder == null && !InstanceGetters.IsDisabledProcessing())
                holder = GetOrRegister<T>();
            return holder != null;
        }

        public static void Clear()
        {
            lock (ms_unloadMethods)
            {
                using (InstanceGetters.DisableProcessing())
                {
                    foreach (var unloadMethod in ms_unloadMethods)
                        unloadMethod();
                }
            }
        }

        public static bool IsTypeOf(object obj, Type type)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            if (type == null)
                throw new ArgumentNullException("type");

            return ToLooseDomainIdentity(obj.GetType()) == ToLooseDomainIdentity(type);
        }

        public static bool IsInstanceOfType(object obj, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            var id = ToLooseDomainIdentity(type);
            return IsInstanceOfIdentity(obj, id);
        }

        public static bool IsInstanceOfIdentity(object obj, string id)
        {
            if (obj == null)
                return false;

            for (var _type = obj.GetType(); _type != null; _type = _type.BaseType)
                if (id == ToLooseDomainIdentity(_type))
                    return true;

            return false;
        }

        static string ToLooseDomainIdentity(Type type)
        {
            // Don't use AssemblyQualifiedName because the AppDomain may have different permission set from current AppDomain.
            // The property AssemblyQualifiedName tries load its assembly again, so FileLoadException will be thrown in this case.
            return type.FullName;
        }
    }

    public class LooseCrossDomainAccessor<T> where T : InstanceHolder<T>
    {
        static readonly object ms_lockObj = new object();
        static T ms_holder = null;
        static bool ms_ready = false;
        static readonly Type ms_t = typeof(T);
        static readonly string ms_key = ms_t.AssemblyQualifiedName;

        static LooseCrossDomainAccessor()
        {
            var all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var method in typeof(LooseCrossDomainAccessor<T>).GetMethods(all))
                RuntimeHelpers.PrepareMethod(method.MethodHandle);
        }

        protected LooseCrossDomainAccessor() { }

        public static void Register()
        {
            using (InstanceGetters.DisableProcessing())
            {
                var funcPtr = GetFunctionPointerCore(ms_t);
                InstanceGetters.TryAdd(ms_key, funcPtr);
            }
        }

        static IntPtr GetFunctionPointerCore(Type t)
        {
            var instance = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            var instanceGetter = instance.GetGetMethod();
            return instanceGetter.MethodHandle.GetFunctionPointer();
        }

        static T GetOrRegisterHolder()
        {
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
            var extractor = new DynamicMethod("Extractor", t, null, t.Module);
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
                    lock (ms_lockObj)
                    {
                        if (!ms_ready)
                        {
                            var holder = default(T);
                            if (!TryGetHolder(out holder))
                                using (InstanceGetters.DisableProcessing())
                                    throw new InvalidOperationException(string.Format("T({0}) has not been registered yet. Please call Register method.", typeof(T)));
                            ms_holder = holder;
                            using (InstanceGetters.DisableProcessing())
                                Thread.MemoryBarrier();
                            ms_ready = true;
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
                    lock (ms_lockObj)
                    {
                        if (!ms_ready)
                        {
                            var holder = default(T);
                            if (TryGetHolder(out holder))
                            {
                                ms_holder = holder;
                                using (InstanceGetters.DisableProcessing())
                                    Thread.MemoryBarrier();
                                ms_ready = true;
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
                    lock (ms_lockObj)
                    {
                        if (!ms_ready)
                        {
                            ms_holder = GetOrRegisterHolder();
                            using (InstanceGetters.DisableProcessing())
                                Thread.MemoryBarrier();
                            ms_ready = true;
                        }
                    }
                }
                return ms_holder;
            }
        }

        public static void Unload()
        {
            lock (ms_lockObj)
                if (ms_holder != null)
                    ms_holder.Dispose();
        }
    }

    public class LooseCrossDomainAccessorUntyped
    {
        class Definitions
        {
            public static readonly MethodInfo GetOrRegisterOfT = typeof(LooseCrossDomainAccessor).GetMethod("GetOrRegister");
        }

        public static IndirectionHolderUntyped GetOrRegister(MethodBase target, Type delegateType, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            var ti = IndirectionHolderUntyped.MakeTypedType(target, delegateType, typeGenericArgs, methodGenericArgs);
            var mi = Definitions.GetOrRegisterOfT.MakeGenericMethod(ti);
            return new IndirectionHolderUntyped(mi.Invoke(null, null));
        }
    }
}
