/* 
 * File: ULConfigurationManager.cs
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
using System.Configuration;
using System.Linq.Expressions;
using System.Threading;

namespace UntestableLibrary
{
    public class ULConfigurationManager
    {
        public static T GetProperty<T>(string key, T defaultValue)
        {
            var value = ConfigurationManager.AppSettings[key];
            var impl = GetPropertyImpl<T>.CreateInstanceWithCache(GetPropertyImpl<T>.CreateInstance);
            if (impl == null)
                throw new NotSupportedException();
            return impl.ToPropertyWithCache(key, value, defaultValue);
        }


        protected abstract class GetPropertyImpl<T>
        {
            static readonly object ms_lockObj = new object();
            static GetPropertyImpl<T> ms_instance = null;
            static bool ms_ready = false;


            public static GetPropertyImpl<T> CreateSpecializedImplInstance<TSpecialized, TImpl>()
            {
                var t = typeof(T);
                if (t == typeof(TSpecialized))
                {
                    var implType = typeof(TImpl);
                    return Activator.CreateInstance(implType) as GetPropertyImpl<T>;
                }
                else
                {
                    return null;
                }
            }


            public static GetPropertyImpl<T> CreateInstance()
            {
                var impl = default(GetPropertyImpl<T>);
                if ((impl = CreateSpecializedImplInstance<DayOfWeek, GetPropertyImplForDayOfWeek>()) != null)
                {
                    return impl;
                }
                return impl;
            }


            public static GetPropertyImpl<T> CreateInstanceWithCache(Func<GetPropertyImpl<T>> instantiator)
            {
                if (!ms_ready)
                {
                    lock (ms_lockObj)
                    {
                        if (!ms_ready)
                        {
                            ms_instance = instantiator() as GetPropertyImpl<T>;
                            Thread.MemoryBarrier();
                            ms_ready = true;
                        }
                    }
                }
                return ms_instance;
            }


            public static void Reset()
            {
                if (ms_ready)
                {
                    lock (ms_lockObj)
                    {
                        if (ms_ready)
                        {
                            ms_instance = null;
                            Thread.MemoryBarrier();
                            ms_ready = false;
                        }
                    }
                }
            }


            Dictionary<string, T> m_properties = new Dictionary<string, T>();
            readonly object m_lockObj = new object();


            public T ToPropertyWithCache(string key, string value, T defaultValue)
            {
                lock (m_lockObj)
                {
                    var property = default(T);
                    if (m_properties.TryGetValue(key, out property))
                    {
                        return property;
                    }
                    else
                    {
                        property = ToPropertyCore(key, value, defaultValue);
                        m_properties.Add(key, property);
                        return property;
                    }
                }
            }


            protected abstract T ToPropertyCore(string key, string value, T defaultValue);
        }


        class GetPropertyImplForDayOfWeek : GetPropertyImpl<DayOfWeek>
        {
            protected override DayOfWeek ToPropertyCore(string key, string value, DayOfWeek defaultValue)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return defaultValue;
                }
                else if (Enum.IsDefined(typeof(DayOfWeek), value))
                {
                    return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), value);
                }
                else
                {
                    var fmt = "The value \"{0}\" linked by key \"{1}\" is invalid. " +
                              "It must take one of the following values: \r\n{2}.";
                    var msg = string.Format(fmt, value, key, string.Join(", ", Enum.GetNames(typeof(DayOfWeek))));
                    throw new ArgumentException(msg, "value");
                }
            }
        }


        protected ULConfigurationManager() { }
    }

    public static class ULNameHalper
    {
        public static string GetName<T>(Expression<Func<T>> exp)
        {
            throw new NotImplementedException();
        }
    }
}
