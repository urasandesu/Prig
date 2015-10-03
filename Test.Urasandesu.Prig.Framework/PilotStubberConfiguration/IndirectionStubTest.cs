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
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;
using Assert = Test.Urasandesu.Prig.Framework.TestUtilities.LooseCrossDomainAssert;
using Urasandesu.NAnonym.Mixins.System;

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



        [Repeat(50)]
        [Test]
        public void IndirectionDelegate_should_get_the_delegate_type_that_indicates_the_getter_of_static_class()
        {
            // Arrange
            var name = "NowGet";
            var alias = "NowGet";
            var xml = string.Empty;
            var target = typeof(DateTime).GetProperty("Now").GetGetMethod();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var indDlgt = stub.IndirectionDelegate;


            // Assert
            Assert.AreEqual("Urasandesu.Prig.Delegates.IndirectionFunc`1[System.DateTime]", indDlgt.ToString());
        }

        [Repeat(50)]
        [Test]
        public void IndirectionDelegate_should_get_the_delegate_type_that_indicates_the_ctor_of_generic_struct()
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

        [Repeat(50)]
        [Test]
        public void IndirectionDelegate_should_get_the_delegate_type_that_indicates_the_generic_method_of_a_class()
        {
            // Arrange
            var name = "GetPropertyOfTStringT";
            var alias = "GetPropertyOfTStringT";
            var xml = string.Empty;
            var target = typeof(ULConfigurationManager).GetMethods().First();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var indDlgt = stub.IndirectionDelegate;


            // Assert
            Assert.AreEqual("Urasandesu.Prig.Delegates.IndirectionFunc`3[System.String,T,T]", indDlgt.ToString());
        }

        [Repeat(50)]
        [Test]
        public void IndirectionDelegate_should_return_null_if_same_signature_delegate_is_not_found()
        {
            // Arrange
            var name = "GetContractDetailsStringBooleanRefDecimalRefDecimalRef";
            var alias = "GetContractDetailsStringBooleanRefDecimalRefDecimalRef";
            var xml = string.Empty;
            var target = typeof(ULHelpers).GetMethods().First();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var indDlgt = stub.IndirectionDelegate;


            // Assert
            Assert.IsNull(indDlgt);
        }



        [Test]
        public void CreateDelegateOfDefaultBehavior_should_return_the_default_behavior_that_indicates_the_getter_of_static_class()
        {
            // Arrange
            var name = "NowGet";
            var alias = "NowGet";
            var xml = string.Empty;
            var target = typeof(DateTime).GetProperty("Now").GetGetMethod();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var defaultBehavior = stub.CreateDelegateOfDefaultBehavior(IndirectionBehaviors.DefaultValue, new Type[] { }, new Type[] { });


            // Assert
            var get_Now = defaultBehavior.Cast<IndirectionFunc<DateTime>>(GetType().Module);
            var value = get_Now();
            Assert.AreEqual(default(DateTime), value);
        }
        
        [Test]
        public void CreateDelegateOfDefaultBehavior_should_return_the_default_behavior_that_indicates_the_ctor_of_generic_struct()
        {
            // Arrange
            var name = "ConstructorT";
            var alias = "ConstructorT";
            var xml = string.Empty;
            var target = typeof(Nullable<>).GetConstructors().First();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var defaultBehavior = stub.CreateDelegateOfDefaultBehavior(IndirectionBehaviors.DefaultValue, new Type[] { typeof(int) }, new Type[] { });


            // Assert
            var ctor = defaultBehavior.Cast<IndirectionRefThisAction<Nullable<int>, int>>(GetType().Module);
            var value = new int?(42);
            ctor(ref value, 42);
            Assert.AreEqual(default(int?), value);
        }

        [Test]
        public void CreateDelegateOfDefaultBehavior_should_return_the_default_behavior_that_indicates_the_generic_method_of_a_class()
        {
            // Arrange
            var name = "GetPropertyOfTStringT";
            var alias = "GetPropertyOfTStringT";
            var xml = string.Empty;
            var target = typeof(ULConfigurationManager).GetMethods().First();
            var stub = new IndirectionStub(name, alias, xml, target);


            // Act
            var defaultBehavior = stub.CreateDelegateOfDefaultBehavior(IndirectionBehaviors.DefaultValue, new Type[] { }, new Type[] { typeof(DayOfWeek) });


            // Assert
            var getProperty = defaultBehavior.Cast<IndirectionFunc<string, DayOfWeek, DayOfWeek>>(GetType().Module);
            var value = getProperty("aiueo", DayOfWeek.Wednesday);
            Assert.AreEqual(default(DayOfWeek), value);
        }

        class ULConfigurationManager
        {
            public static T GetProperty<T>(string key, T defaultValue)
            {
                throw new NotImplementedException();
            }
        }

        class ULHelpers
        {
            public void GetContractDetails(string s, ref bool b, ref decimal d1, ref decimal d2)
            {
                throw new NotSupportedException();
            }
        }
    }
}
