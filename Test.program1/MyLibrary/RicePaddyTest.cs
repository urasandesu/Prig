/* 
 * File: RicePaddyTest.cs
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
using program1.MyLibrary;
using System;
using System.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.MyLibrary
{
    [TestFixture]
    public class RicePaddyTest
    {
        [Test]
        public void Constructor_should_be_initialized_by_null_if_number_that_is_divisible_by_10_is_passed()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var actualValue = 0;
                PRandom.Next().Body = @this => 10;
                PNullable<int>.ConstructorT().Body = (ref Nullable<int> @this, int value) =>
                {
                    actualValue = value;
                    @this = IndirectionsContext.ExecuteOriginal(() => new Nullable<int>(value));
                };


                // Act
                var paddy = new RicePaddy(1, new Random());

                
                // Assert
                Assert.AreEqual(0, actualValue);
            }
        }



        [Test]
        public void Constructor_should_be_initialized_by_non_null_if_number_that_is_not_divisible_by_10_is_passed()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var actualValue = 0;
                PRandom.Next().Body = @this => 9;
                PNullable<int>.ConstructorT().Body = (ref Nullable<int> @this, int value) =>
                {
                    actualValue = value;
                    @this = IndirectionsContext.ExecuteOriginal(() => new Nullable<int>(value));
                };

                
                // Act
                var paddy = new RicePaddy(1, new Random());

                
                // Assert
                Assert.AreEqual(9000, actualValue);
            }
        }
    }
}
