/* 
 * File: IgnoreCA1724.cs
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

namespace UntestableLibrary.IgnoreCA1724
{
    namespace HasFullName
    {
        public class HasFullName
        {
            public HasFullName Hoge()
            {
                throw new InvalidOperationException("We shouldn't get here!!");
            }
        }
    }
    namespace IsGenericInstance
    {
        public class IsGenericInstance<T> where T : IsGenericInstance<T>
        {
            public void Hoge()
            {
                throw new InvalidOperationException("We shouldn't get here!!");
            }
        }
    }
    namespace IsGenericNestedParameter
    {
        public class A
        {
            public class B<IsGenericNestedParameter>
            {
                public void Hoge(List<IsGenericNestedParameter>[] arg)
                {
                    throw new InvalidOperationException("We shouldn't get here!!");
                }
            }
        }
    }
    namespace IsGenericParameter
    {
        public class C<IsGenericParameter>
        {
            public void Fuga(out List<IsGenericParameter> result)
            {
                throw new InvalidOperationException("We shouldn't get here!!");
            }
        }
    }
    namespace IsNested
    {
        public class D
        {
            public class IsNested
            {
                public IsNested[][] Hoge()
                {
                    throw new InvalidOperationException("We shouldn't get here!!");
                }
            }
        }
    }
}
