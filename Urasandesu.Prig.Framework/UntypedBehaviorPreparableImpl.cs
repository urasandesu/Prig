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

        Work GetBodyOfDefaultBehavior()
        {
            return BodyCore == null ?
                        IndirectionStub.CreateWorkOfDefaultBehavior(IndirectionBehaviors.Fallthrough, TypeGenericArguments, MethodGenericArguments) :
                        BodyCore;
        }

        void SetBodyExecutingDefaultOr(Work defaultBehavior, Dictionary<object, TargetSettingValue<Work>> indirections)
        {
            BodyCore = IndirectionStub.CreateWorkExecutingDefaultOr(defaultBehavior, indirections, TypeGenericArguments, MethodGenericArguments);
        }

        protected void SetTargetInstanceBody<TBehaviorPreparableImpl>(object target, Work value) where TBehaviorPreparableImpl : BehaviorPreparableImpl
        {
            SetTargetInstanceBody<TBehaviorPreparableImpl, Work>(target, value, GetBodyOfDefaultBehavior, SetBodyExecutingDefaultOr);
        }

        void SetBodyToOriginal(Work original)
        {
            BodyCore = original;
        }

        protected void RemoveTargetInstanceBody<TBehaviorPreparableImpl>(object target) where TBehaviorPreparableImpl : BehaviorPreparableImpl
        {
            RemoveTargetInstanceBody<TBehaviorPreparableImpl, Work>(target, SetBodyToOriginal);
        }
    }
}
