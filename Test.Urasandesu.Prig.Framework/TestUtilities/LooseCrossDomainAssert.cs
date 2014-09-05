/* 
 * File: LooseCrossDomainAssert.cs
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


using NUnit.Framework;
using System;
using Urasandesu.Prig.Framework;

namespace Test.Urasandesu.Prig.Framework.TestUtilities
{
    public class LooseCrossDomainAssert : NUnit.Framework.Assert
    {
        protected LooseCrossDomainAssert()
            : base()
        { }

        public new static TException Throws<TException>(TestDelegate code) where TException : Exception
        {
            return Throws<TException>(code, "");
        }

        public new static TException Throws<TException>(TestDelegate code, string message) where TException : Exception
        {
            return Throws<TException>(code, message, new object[0]);
        }

        public new static TException Throws<TException>(TestDelegate code, string message, params object[] args) where TException : Exception
        {
            try
            {
                code();
            }
            catch (Exception ex)
            {
                if (!LooseCrossDomainAccessor.IsTypeOf(ex, typeof(TException)))
                    return NUnit.Framework.Assert.Throws<TException>(code, message, args);

                return ex as TException;
            }

            throw new AssertionException("");  // avoid build failure(you will never get here).
        }
    }
}
