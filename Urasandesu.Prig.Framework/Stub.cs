/* 
 * File: Stub.cs
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

namespace Urasandesu.Prig.Framework
{
    public class Stub<OfPrigType> where OfPrigType : IPrigTypeIntroducer, new()
    {
        static OfPrigType ms_introducer = new OfPrigType();
        
        protected Stub()
        { }

        public static TypedBehaviorPreparable<TDelegate> Setup<TDelegate>(Func<OfPrigType, TypedBehaviorPreparableImpl> selectTarget) where TDelegate : class
        {
            if (selectTarget == null)
                throw new ArgumentNullException("selectTarget");

            var impl = selectTarget(ms_introducer);
            return new TypedBehaviorPreparable<TDelegate>(impl);
        }

        public static UntypedBehaviorPreparable Setup(Func<OfPrigType, UntypedBehaviorPreparableImpl> selectTarget)
        {
            if (selectTarget == null)
                throw new ArgumentNullException("selectTarget");

            var impl = selectTarget(ms_introducer);
            return new UntypedBehaviorPreparable(impl);
        }

        public static TBehaviorSetting ExcludeGeneric<TBehaviorSetting>(TBehaviorSetting setting) where TBehaviorSetting : BehaviorSetting
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            var preps = typeof(OfPrigType).GetNestedTypes().
                                           Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                           Where(_ => !_.IsGenericType).
                                           Where(_ => _.GetConstructor(Type.EmptyTypes) != null).
                                           Select(_ => Activator.CreateInstance(_)).
                                           Cast<IBehaviorPreparable>();
            foreach (var prep in preps)
                setting.Include(prep);
            return setting;
        }
    }
}
