/* 
 * File: IndirectionStubTest.cs
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
using System.Linq;
using Test.Urasandesu.Prig.Framework.TestUtilities;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;
using Assert = Test.Urasandesu.Prig.Framework.TestUtilities.LooseCrossDomainAssert;

namespace Test.Urasandesu.Prig.Framework.PilotStubberConfiguration
{
    [TestFixture]
    public class IndirectionStubTest
    {
        [SetUp]
        public void SetUp()
        {
            InstanceGetters.NewAdditionalDelegatesAssemblyRepository = () => new MockAdditionalDelegatesAssemblyRepository();
        }

        [TearDown]
        public void TearDown()
        {
            InstanceGetters.NewAdditionalDelegatesAssemblyRepository = null;
        }



        [Test]
        public void IndirectionDelegate_should_get_the_delegate_type_that_contains_the_generic_type_that_is_instantiated_by_its_generic_parameter()
        {
            // Arrange
            var name = "ConstructorT";
            var alias = "ConstructorT";
            var xml = string.Empty;
            var target = typeof(Nullable<>).GetConstructors().First();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var indDlgt = stub.IndirectionDelegate;


            // Assert
            Assert.AreEqual("Urasandesu.Prig.Delegates.IndirectionRefThisAction`2[System.Nullable`1[T],T]", indDlgt.ToString());
            var genericType = indDlgt.GetMethod("Invoke").GetParameters()[0].ParameterType.GetElementType();
            Assert.AreEqual("System.Nullable`1[T]", genericType.ToString());
            Assert.AreNotEqual(typeof(Nullable<>), genericType);
        }
    }
}
