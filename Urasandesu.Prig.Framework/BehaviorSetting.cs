/* 
 * File: BehaviorSetting.cs
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



using System.Collections.Generic;
using Urasandesu.NAnonym.Collections.Generic;

namespace Urasandesu.Prig.Framework
{
    public class BehaviorSetting
    {
        HashSet<IBehaviorPreparable> m_preparables = new HashSet<IBehaviorPreparable>(new LambdaEqualityComparer<IBehaviorPreparable>(_ => _.Info));
        protected IEnumerable<IBehaviorPreparable> Preparables { get { return m_preparables; } }

        public BehaviorSetting()
        { }

        public BehaviorSetting Include(IBehaviorPreparable preparable)
        {
            m_preparables.Add(preparable);
            return this;
        }

        public BehaviorSetting Exclude(IBehaviorPreparable preparable)
        {
            m_preparables.Remove(preparable);
            return this;
        }

        public virtual IndirectionBehaviors DefaultBehavior
        {
            set
            {
                IndirectionsContext.DefaultBehavior = value;
                foreach (var preparable in Preparables)
                    preparable.Prepare(IndirectionsContext.DefaultBehavior);
            }
        }
    }
}
