/* 
 * File: MockStorage.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
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


using Moq;
using System;
using System.Collections.Generic;
using Urasandesu.Prig.Framework;

namespace Urasandesu.Moq.Prig
{
    public class MockStorage : MockRepository
    {
        Dictionary<Delegate, Mock> m_storage = new Dictionary<Delegate, Mock>();

        public MockStorage(MockBehavior defaultBehavior)
            : base(defaultBehavior)
        { }

        public void Set<T>(Func<TypedBehaviorPreparable<T>> func, Mock<T> m) where T : class
        {
            m_storage[func] = m;
        }

        public MockStorage Customize(Action<MockStorage> exp)
        {
            exp(this);
            return this;
        }

        public Mock<T> Do<T>(Func<TypedBehaviorPreparable<T>> func) where T : class
        {
            return (Mock<T>)m_storage[func];
        }
    }
}
