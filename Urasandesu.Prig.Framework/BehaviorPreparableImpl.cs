/* 
 * File: BehaviorPreparableImpl.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2015 Akira Sugiura
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
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace Urasandesu.Prig.Framework
{
    public abstract class BehaviorPreparableImpl : IBehaviorPreparable
    {
        protected BehaviorPreparableImpl(IndirectionStub stub, Type[] typeGenericArgs, Type[] methodGenericArgs)
        {
            if (stub == null)
                throw new ArgumentNullException("stub");

            if (typeGenericArgs == null)
                throw new ArgumentNullException("typeGenericArgs");

            if (methodGenericArgs == null)
                throw new ArgumentNullException("methodGenericArgs");

            IndirectionStub = stub;
            TypeGenericArguments = typeGenericArgs;
            MethodGenericArguments = methodGenericArgs;
        }

        public IndirectionStub IndirectionStub { get; private set; }
        public Type[] TypeGenericArguments { get; private set; }
        public Type[] MethodGenericArguments { get; private set; }

        public abstract void Prepare(IndirectionBehaviors defaultBehavior);

        public IndirectionInfo Info
        {
            get
            {
                var info = new IndirectionInfo();
                info.AssemblyName = IndirectionStub.IndirectableAssemblyName;
                info.Token = IndirectionStub.IndirectableToken;
                info.SetInstantiation(IndirectionStub.Target, IndirectionStub.Signature, TypeGenericArguments, MethodGenericArguments);
                return info;
            }
        }

        protected void SetTargetInstanceBody<TBehaviorPreparableImpl, TBody>(
            object target, TBody value, Func<TBody> getBodyOfDefaultBehavior, Action<TBody, Dictionary<object, TargetSettingValue<TBody>>> setBodyExecutingDefaultOr) 
            where TBehaviorPreparableImpl : BehaviorPreparableImpl
            where TBody : class
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(TBody)))
                throw new ArgumentException("The generic parameter must be a delegate type.", "TBody");

            if (target == null)
                throw new ArgumentNullException("target");

            if (value == null)
                throw new ArgumentNullException("value");

            RuntimeHelpers.PrepareDelegate(value as Delegate);

            var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<TBehaviorPreparableImpl, Dictionary<object, TargetSettingValue<TBody>>>>>();
            if (holder.Source.Value == null)
                holder.Source = TaggedBagFactory<TBehaviorPreparableImpl>.Make(new Dictionary<object, TargetSettingValue<TBody>>());

            var dic = holder.Source.Value;
            if (!dic.ContainsKey(target))
            {
                var behavior = getBodyOfDefaultBehavior();
                RuntimeHelpers.PrepareDelegate(behavior as Delegate);
                dic[target] = new TargetSettingValue<TBody>(behavior, value);
                {
                    // Prepare JIT
                    var original = dic[target].Original;
                    var indirection = dic[target].Indirection;
                }

                setBodyExecutingDefaultOr(behavior, dic);
            }
            else
            {
                var before = dic[target];
                dic[target] = new TargetSettingValue<TBody>(before.Original, value);
            }
        }

        protected void RemoveTargetInstanceBody<TBehaviorPreparableImpl, TBody>(object target, Action<TBody> setBodyToOriginal) 
            where TBehaviorPreparableImpl : BehaviorPreparableImpl
            where TBody : class
        {
            if (!typeof(Delegate).IsAssignableFrom(typeof(TBody)))
                throw new ArgumentException("The generic parameter must be a delegate type.", "TBody");

            if (target == null)
                throw new ArgumentNullException("target");

            var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<TBehaviorPreparableImpl, Dictionary<object, TargetSettingValue<TBody>>>>>();
            if (holder.Source.Value == null)
                return;

            var dic = holder.Source.Value;
            if (dic.Count == 0)
                return;

            var before = default(TargetSettingValue<TBody>);
            if (dic.ContainsKey(target))
            {
                before = dic[target];
                dic.Remove(target);
            }
            if (dic.Count == 0)
                setBodyToOriginal(before.Original);
        }
    }
}
