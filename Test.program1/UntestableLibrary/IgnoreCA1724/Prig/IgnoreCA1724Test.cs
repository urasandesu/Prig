/* 
 * File: IgnoreCA1724Test.cs
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



#if NUnit
using TestFixtureAttribute = NUnit.Framework.TestFixtureAttribute;
using TestAttribute = NUnit.Framework.TestAttribute;
#elif MsTest
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#endif
using System.Collections;
using System.Collections.Generic;
using Test.program1.TestUtilities;
using UntestableLibrary;
using UntestableLibrary.IgnoreCA1724.HasFullName.Prig;
using UntestableLibrary.IgnoreCA1724.IsGenericInstance.Prig;
using UntestableLibrary.IgnoreCA1724.IsGenericNestedParameter.Prig;
using UntestableLibrary.IgnoreCA1724.IsGenericParameter.Prig;
using UntestableLibrary.IgnoreCA1724.IsNested.Prig;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.IgnoreCA1724.Prig
{
    [TestFixture]
    public class HasFullNameTest
    {
        [Test]
        public void Hoge_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PHasFullName.Hoge().Body = @this => null;

                // Act, Assert
                ExceptionAssert.DoesNotThrow(() => new global::UntestableLibrary.IgnoreCA1724.HasFullName.HasFullName().Hoge());
            }
        }
    }

    [TestFixture]
    public class IsGenericInstanceTest
    {
        [Test]
        public void Hoge_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PIsGenericInstance<MyIsGenericInstance>.Hoge().Body = @this => { };

                // Act, Assert
                ExceptionAssert.DoesNotThrow(() => new global::UntestableLibrary.IgnoreCA1724.IsGenericInstance.IsGenericInstance<MyIsGenericInstance>().Hoge());
            }
        }
    }

    class MyIsGenericInstance : global::UntestableLibrary.IgnoreCA1724.IsGenericInstance.IsGenericInstance<MyIsGenericInstance>
    { }

    [TestFixture]
    public class IsGenericNestedParameterTest
    {
        [Test]
        public void Hoge_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PAB<int>.HogeListOfIsGenericNestedParameterArray().Body = (@this, arg) => { };

                // Act, Assert
                ExceptionAssert.DoesNotThrow(() => new global::UntestableLibrary.IgnoreCA1724.IsGenericNestedParameter.A.B<int>().Hoge(null));
            }
        }
    }

    [TestFixture]
    public class IsGenericParameterTest
    {
        [Test]
        public void Hoge_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PC<double>.FugaListOfIsGenericParameterRef().Body =
                    (global::UntestableLibrary.IgnoreCA1724.IsGenericParameter.C<double> @this, out global::System.Collections.Generic.List<double> result) =>
                    {
                        result = null;
                    };

                // Act, Assert
                {
                    var result = default(global::System.Collections.Generic.List<double>);
                    ExceptionAssert.DoesNotThrow(() => new global::UntestableLibrary.IgnoreCA1724.IsGenericParameter.C<double>().Fuga(out result));
                }
            }
        }
    }

    [TestFixture]
    public class IsNestedTest
    {
        [Test]
        public void Hoge_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDIsNested.Hoge().Body = @this => null;

                // Act, Assert
                ExceptionAssert.DoesNotThrow(() => new global::UntestableLibrary.IgnoreCA1724.IsNested.D.IsNested().Hoge());
            }
        }
    }
}
