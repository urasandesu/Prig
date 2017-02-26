/* 
 * File: .cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2017 Akira Sugiura
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test.program1.TestUtilities
{
    class Class1
    {
    }

#if NUnit
    class Assert : NUnit.Framework.Assert { }
    class ExceptionAssert : NUnit.Framework.Assert { }
    class CollectionAssert : NUnit.Framework.CollectionAssert { }
#elif MsTest
    class Assert
    {
        public static void AreEqual<T>(T expected, T actual)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actual);
        }

        public static void AreNotEqual<T>(T notExpected, T actual)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreNotEqual(notExpected, actual);
        }

        public static void IsFalse(bool condition)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(condition);
        }

        public static void IsTrue(bool condition)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(condition);
        }

        public static void IsNotNull(object value)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(value);
        }
    }

    class ExceptionAssert
    {
        public static void DoesNotThrow(Action action)
        {
            action();
        }

        public static void Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(string.Format("This action should throw '{0}' but it didn't.", typeof(TException).Name));
            }
            catch (TException)
            { }
        }
    }

    class CollectionAssert
    {
        public static void AreEqual(IEnumerable expected, IEnumerable actual)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert.AreEqual(expected.OfType<object>().ToArray(), actual.OfType<object>().ToArray());
        }

        public static void AreNotEqual(IEnumerable notExpected, IEnumerable actual)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert.AreNotEqual(notExpected.OfType<object>().ToArray(), actual.OfType<object>().ToArray());
        }

        public static void IsEmpty(IEnumerable actual)
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(actual.OfType<object>().Any(), "Expected result is empty but actual result has some elements.");
        }
    }
#elif Xunit
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    sealed class TestFixtureAttribute : Attribute { }
#endif
}
