/* 
 * File: UntypedBehaviorPreparableImpl.cs
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
using Urasandesu.NAnonym;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace Urasandesu.Prig.Framework
{
    public abstract class UntypedBehaviorPreparableImpl : BehaviorPreparableImpl
    {
        protected UntypedBehaviorPreparableImpl(IndirectionStub stub, Type[] typeGenericArgs, Type[] methodGenericArgs) :
            base(stub, typeGenericArgs, methodGenericArgs)
        { }

        public virtual Work Body
        {
            get { return BodyCore; }
            set { BodyCore = value; }
        }

        public virtual Work BodyCore
        {
            get
            {
                var holder = LooseCrossDomainAccessorUntyped.GetOrRegister(IndirectionStub.GetIndirectionDelegate(TypeGenericArguments, MethodGenericArguments));
                return holder.GetOrDefault(Info);
            }
            set
            {
                var holder = LooseCrossDomainAccessorUntyped.GetOrRegister(IndirectionStub.GetIndirectionDelegate(TypeGenericArguments, MethodGenericArguments));
                if (value == null)
                    holder.Remove(Info);
                else
                    holder.AddOrUpdate(Info, value);
            }
        }

        public override void Prepare(IndirectionBehaviors defaultBehavior)
        {
            var behavior = IndirectionStub.CreateWorkOfDefaultBehavior(defaultBehavior, TypeGenericArguments, MethodGenericArguments);
            Body = behavior;
        }

        protected void SetTargetInstanceBody<TBehaviorPreparableImpl>(object target, Work value) where TBehaviorPreparableImpl : BehaviorPreparableImpl
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (value == null)
                throw new ArgumentNullException("value");

            RuntimeHelpers.PrepareDelegate(value);

            var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<TBehaviorPreparableImpl, Dictionary<object, TargetSettingValue<Work>>>>>();
            if (holder.Source.Value == null)
                holder.Source = TaggedBagFactory<TBehaviorPreparableImpl>.Make(new Dictionary<object, TargetSettingValue<Work>>());

            if (holder.Source.Value.Count == 0)
            {
                var behavior = BodyCore == null ?
                                    IndirectionStub.CreateWorkOfDefaultBehavior(IndirectionBehaviors.Fallthrough, TypeGenericArguments, MethodGenericArguments) :
                                    BodyCore;
                RuntimeHelpers.PrepareDelegate(behavior);
                holder.Source.Value[target] = new TargetSettingValue<Work>(behavior, value);
                {
                    // Prepare JIT
                    var original = holder.Source.Value[target].Original;
                    var indirection = holder.Source.Value[target].Indirection;
                }

                BodyCore = IndirectionStub.CreateWorkExecutingDefaultOr(behavior, holder.Source.Value, TypeGenericArguments, MethodGenericArguments);
            }
            else
            {
                var before = holder.Source.Value[target];
                holder.Source.Value[target] = new TargetSettingValue<Work>(before.Original, value);
            }
        }

        protected void RemoveTargetInstanceBody<TBehaviorPreparableImpl>(object target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<TBehaviorPreparableImpl, Dictionary<object, TargetSettingValue<Work>>>>>();
            if (holder.Source.Value == null)
                return;

            if (holder.Source.Value.Count == 0)
                return;

            var before = default(TargetSettingValue<Work>);
            if (holder.Source.Value.ContainsKey(target))
                before = holder.Source.Value[target];
            holder.Source.Value.Remove(target);
            if (holder.Source.Value.Count == 0)
                BodyCore = before.Original;
        }
    }
}
