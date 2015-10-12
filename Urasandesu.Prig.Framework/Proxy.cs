/* 
 * File: Proxy.cs
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
using System.Linq;
using System.Runtime.Serialization;

namespace Urasandesu.Prig.Framework
{
    public class Proxy<OfPrigProxyType> where OfPrigProxyType : IPrigProxyTypeIntroducer, new()
    {
        OfPrigProxyType m_introducer;

        public Proxy()
        {
            m_introducer = new OfPrigProxyType();
            Target = FormatterServices.GetUninitializedObject(m_introducer.Type);
            m_introducer.Initialize(Target);
        }

        public object Target { get; private set; }

        public TypedBehaviorPreparable<TDelegate> Setup<TDelegate>(Func<OfPrigProxyType, TypedBehaviorPreparableImpl> selectTarget) where TDelegate : class
        {
            if (selectTarget == null)
                throw new ArgumentNullException("selectTarget");

            var impl = selectTarget(m_introducer);
            return new TypedBehaviorPreparable<TDelegate>(impl);
        }

        public UntypedBehaviorPreparable Setup(Func<OfPrigProxyType, UntypedBehaviorPreparableImpl> selectTarget)
        {
            if (selectTarget == null)
                throw new ArgumentNullException("selectTarget");

            var impl = selectTarget(m_introducer);
            return new UntypedBehaviorPreparable(impl);
        }

        public TBehaviorSetting ExcludeGeneric<TBehaviorSetting>(TBehaviorSetting setting) where TBehaviorSetting : BehaviorSetting
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            var preps = typeof(OfPrigProxyType).GetNestedTypes().
                                                Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                                Where(_ => !_.IsGenericType).
                                                Where(_ => _.GetConstructor(new[] { typeof(object) }) != null).
                                                Select(_ => Activator.CreateInstance(_, new object[] { Target })).
                                                Cast<IBehaviorPreparable>();
            foreach (var prep in preps)
                setting.Include(prep);
            return setting;
        }
    }
}
