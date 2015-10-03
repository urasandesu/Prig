/* 
 * File: TypedBehaviorPreparable.cs
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
using System.Runtime.CompilerServices;

namespace Urasandesu.Prig.Framework
{
    public class TypedBehaviorPreparable<TDelegate> : IBehaviorPreparable where TDelegate : class
    {
        TypedBehaviorPreparableImpl m_impl;

        public TypedBehaviorPreparable(TypedBehaviorPreparableImpl impl)
        {
            if (impl == null)
                throw new ArgumentNullException("impl");

            m_impl = impl;
        }

        public TDelegate Body
        {
            get { return LooseCrossDomainAccessor.SafelyCast<TDelegate>(m_impl.Body); }
            set
            {
                m_impl.Body = value as Delegate;
                var body = Body as Delegate;
                if (body != null)
                    RuntimeHelpers.PrepareDelegate(body);
            }
        }

        public void Prepare(IndirectionBehaviors defaultBehavior)
        {
            m_impl.Prepare(defaultBehavior);
        }

        public IndirectionInfo Info
        {
            get { return m_impl.Info; }
        }
    }
}
