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
using Urasandesu.NAnonym.Mixins.System;

namespace Urasandesu.Prig.Framework
{
    public class LooseCrossDomainAccessor
    {
        protected LooseCrossDomainAccessor() { }

        static readonly HashSet<Action> ms_unloadMethods = InstanceGetters.DisableProcessing().EnsureDisposalThen(_ => new HashSet<Action>());

        public static void Register<T>() where T : InstanceHolder<T>
        {
            LooseCrossDomainAccessor<T>.Register();
            using (InstanceGetters.DisableProcessing())
                lock (ms_unloadMethods)
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
            if (holder != null)
                using (InstanceGetters.DisableProcessing())
                    lock (ms_unloadMethods)
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
            using (InstanceGetters.DisableProcessing())
            {
                lock (ms_unloadMethods)
                {
                    foreach (var unloadMethod in ms_unloadMethods)
                        unloadMethod();
                }
                InstanceGetters.Clear();
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

        public static TDelegate SafelyCast<TDelegate>(Delegate source) where TDelegate : class
        {
            if (source == null)
                return null;

            using (InstanceGetters.DisableProcessing())
                return source.Cast<TDelegate>(typeof(InstanceGetters).Module);
        }

        static string ToLooseDomainIdentity(Type type)
        {
            // Don't use AssemblyQualifiedName because the AppDomain may have different permission set from current AppDomain.
            // The property AssemblyQualifiedName tries load its assembly again, so FileLoadException will be thrown in this case.
            return type.FullName;
        }
    }
}
