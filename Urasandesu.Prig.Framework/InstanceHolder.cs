/* 
 * File: InstanceHolder.cs
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
using System.Reflection;
using System.Runtime.CompilerServices;
using Urasandesu.NAnonym.Mixins.System;

namespace Urasandesu.Prig.Framework
{
    public abstract class InstanceHolder<T> : IDisposable where T : InstanceHolder<T>
    {
        static InstanceHolder()
        {
            var all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var method in typeof(InstanceHolder<T>).GetMethods(all).Where(_ => !_.IsAbstract))
                RuntimeHelpers.PrepareMethod(method.MethodHandle, new[] { typeof(T).TypeHandle });

            // Prepare JIT here if the type has been already known explicitly.
            new FallthroughException();
        }

        protected InstanceHolder() { }

        static T ms_instance = InstanceGetters.DisableProcessing().EnsureDisposalThen(_ => TypeMixin.ForciblyNew<T>());
        public static T Instance { get { return ms_instance; } }

        // Prepare JIT to transgress the boundary of AppDomain.
        // NOTE: You have to prohibit JIT and the type validation of CLR for any members astride between AppDomain. For example, the following procedures are applied to that: 
        //       * Instantiate a closure in any members of a generic type. It will be generated as a nested generic type automatically, 
        //         so the member will be not JIT till running at each AppDomain.
        //       * Define any generic methods. Generic methods are JITed against each generic method instance by CLR's design, 
        //         so the results will be mixed with the result of another AppDomain.
        //       * Cast a type to another type. Because CLR validates whether the AppDomain that the type belongs is correct in this timing. 
        //         Note that some standard APIs will perform it internally. For example, don't use the type of not domain-neutral assembly as the key of `System.Collections.Generic.Dictionary`.
        //         It finds the concrete type implementing the interface `System.Collections.Generic.IEqualityComparer<T>` that depends on the key 
        //         to calculate the container that houses the value. Of course the result has to cast to the interface.
        //         
        //         See also: 
        //           * Generics and Your Profiler - David Broman's CLR Profiling API Blog - Site Home - MSDN Blogs 
        //             http://blogs.msdn.com/b/davbr/archive/2010/01/28/generics-and-your-profiler.aspx
        //           * dictionary.cs - Microsoft Reference Source
        //             http://referencesource.microsoft.com/#mscorlib/system/collections/generic/dictionary.cs
        protected internal abstract void Prepare();

        public void Dispose()
        {
            Dispose(true);
            using (InstanceGetters.DisableProcessing())
                GC.SuppressFinalize(this);
        }

        // NOTE: You must NOT set the any members of a derived type to null if instantiating them in the constructor of the derived type.
        //       Because this instance will be reused usually under the control of LooseCrossDomainAccessor(This means that its constructor is never called again). 
        //       Also typical disposable pattern is not available. For example, the flag like `disposed` is never reverted to false if it is set to true once.
        protected abstract void Dispose(bool disposing);

        ~InstanceHolder()
        {
            Dispose(false);
        }
    }
}
