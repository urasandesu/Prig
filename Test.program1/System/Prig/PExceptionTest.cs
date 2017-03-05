/* 
 * File: PExceptionTest.cs
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



#if NUnit
using TestFixtureAttribute = NUnit.Framework.TestFixtureAttribute;
using TestAttribute = NUnit.Framework.TestAttribute;
#elif MsTest
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#elif Xunit
using TestAttribute = Xunit.FactAttribute;
#endif
using System;
using System.Prig;
using System.Reflection;
using Urasandesu.Prig.Framework;
using Test.program1.TestUtilities;

namespace Test.program1.System.Prig
{
    [TestFixture]
    public class PExceptionTest
    {
        [Test]
        public void InternalToString_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PException.InternalToString().Body = _ => "にゃんぱすー";

                // Act
                var actual = new NotImplementedException().InternalToString();

                // Assert
                Assert.AreEqual("にゃんぱすー", actual);
            }
        }
    }

    public static class ExceptionMixin
    {
        public static string InternalToString(this Exception source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var internalToStringInfo = source.GetType().GetMethod("InternalToString", BindingFlags.Instance | BindingFlags.NonPublic);
            return (string)internalToStringInfo.Invoke(source, new object[0]);
        }
    }
}
