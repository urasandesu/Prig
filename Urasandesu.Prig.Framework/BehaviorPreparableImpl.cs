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
    }
}
