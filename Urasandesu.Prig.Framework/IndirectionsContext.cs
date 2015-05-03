/* 
 * File: IndirectionsContext.cs
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
using System.Linq;

namespace Urasandesu.Prig.Framework
{
    public class IndirectionsContext : IDisposable
    {
        public IndirectionsContext()
        {
            DefaultBehavior = IndirectionBehaviors.Fallthrough;
        }

        bool m_disposed;

        public static void ExecuteOriginal(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            using (InstanceGetters.DisableProcessing())
                action();
        }

        public static T ExecuteOriginal<T>(Func<T> func)
        {
            if (func == null)
                throw new ArgumentNullException("func");

            using (InstanceGetters.DisableProcessing())
                return func();
        }

        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static BehaviorSetting ExcludeGeneric()
        {
            var repository = InstanceGetters.NewIndirectionAssemblyRepository();
            var indAsmInfos = repository.FindAll();
            var preparables = indAsmInfos.SelectMany(_ => _.GetTypes()).
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Where(_ => _.GetConstructor(Type.EmptyTypes) != null).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>(); ;
            var setting = new BehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    DefaultBehavior = IndirectionBehaviors.Fallthrough;
                    LooseCrossDomainAccessor.Clear();
                }
            }
            m_disposed = true;
        }

        ~IndirectionsContext()
        {
            Dispose(false);
        }
    }
}
