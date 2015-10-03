/* 
 * File: IndirectionInfoTest.cs
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

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class IndirectionInfoTest
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
        public void SetInstantiation_should_set_Instantiation_property_with_the_generic_method_signature_of_generic_type()
        {
            // Arrange
            var name = "EchoOfT5OfT6OfT7OfT8OfTRetT4T8";
            var alias = "EchoOfT5OfT6OfT7OfT8OfTRetT4T8";
            var xml = string.Empty;
            var target = typeof(ULUnusedGeneric<,,,>).GetMethods().First();
            var stub = new IndirectionStub(name, alias, xml, target);

            var info = new IndirectionInfo();

            var typeGenericArgs = new[] { typeof(sbyte), typeof(byte), typeof(short), typeof(ushort) };
            var methodGenericArgs = new[] { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(decimal) };


            // Act
            info.SetInstantiation(target, stub.Signature, typeGenericArgs, methodGenericArgs);


            // Assert
            var expected = new[] 
            { 
                target.DeclaringType.MakeGenericType(typeGenericArgs[0], typeGenericArgs[1], typeGenericArgs[2], typeGenericArgs[3]), 
                typeGenericArgs[3], 
                methodGenericArgs[3], 
                methodGenericArgs[4] 
            }.Select(_ => _.ToString());
            CollectionAssert.AreEqual(expected, info.Instantiation);
        }
        
        [Test]
        public void SetInstantiation_should_set_Instantiation_property_with_the_generic_method_signature_that_contains_same_type_parameter()
        {
            // Arrange
            var name = "GetPropertyOfTStringT";
            var alias = "GetPropertyOfTStringT";
            var xml = string.Empty;
            var target = typeof(ULConfigurationManager).GetMethods().First();
            var stub = new IndirectionStub(name, alias, xml, target);

            var info = new IndirectionInfo();

            var typeGenericArgs = Type.EmptyTypes;
            var methodGenericArgs = new[] { typeof(DayOfWeek) };


            // Act
            info.SetInstantiation(target, stub.Signature, typeGenericArgs, methodGenericArgs);


            // Assert
            var expected = new[] 
            { 
                typeof(string), 
                methodGenericArgs[0], 
                methodGenericArgs[0] 
            }.Select(_ => _.ToString());
            CollectionAssert.AreEqual(expected, info.Instantiation);
        }

        
        
        class ULUnusedGeneric<T1, T2, T3, T4>
        {
            public TRet Echo<T5, T6, T7, T8, TRet>(T4 v4, T8 v8)
            {
                throw new NotImplementedException();
            }
        }

        class ULConfigurationManager
        {
            public static T GetProperty<T>(string key, T defaultValue)
            {
                throw new NotImplementedException();
            }
        }
    }
}
