/* 
 * File: PULSharedMemoryTest.cs
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
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class PULSharedMemoryTest
    {
        [Test]
        public void GetMemory_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PULSharedMemory.GetMemoryInt32ByteArrayRef().Body = (ULSharedMemory @this, int size, out byte[] buf) =>
                {
                    buf = new byte[42];
                    for (int i = 0; i < buf.Length; i++)
                        buf[i] = 42;
                    return true;
                };

                using (var sm = new ULSharedMemory())
                {
                    // Act
                    var size = 1024;
                    var buf = default(byte[]);
                    var actual = sm.GetMemory(size, out buf);

                    // Assert
                    Assert.IsTrue(actual);
                    Assert.AreEqual(42, buf.Length);
                    Assert.AreEqual((byte)42, buf[0]);
                    Assert.AreEqual((byte)42, buf[buf.Length - 1]);
                }
            }
        }



        [Test]
        public void GetMemory_should_be_callable_indirectly_against_only_specified_instance()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var smProxy = new PProxyULSharedMemory();
                smProxy.GetMemoryInt32ByteArrayRef().Body = (ULSharedMemory @this, int size, out byte[] buf) =>
                {
                    buf = new byte[42];
                    for (int i = 0; i < buf.Length; i++)
                        buf[i] = 42;
                    return true;
                };
                var sm_sut = (ULSharedMemory)smProxy;


                using (var sm = new ULSharedMemory())
                {
                    // Act
                    var size = 1024;

                    var buf_sut = default(byte[]);
                    var actual_sut = sm_sut.GetMemory(size, out buf_sut);

                    var buf = default(byte[]);
                    var actual = sm.GetMemory(size, out buf);

                    // Assert
                    Assert.IsTrue(actual_sut);
                    Assert.AreEqual(42, buf_sut.Length);
                    Assert.AreEqual((byte)42, buf_sut[0]);
                    Assert.AreEqual((byte)42, buf_sut[buf_sut.Length - 1]);

                    Assert.AreNotEqual(buf.Length, buf_sut.Length); // constant value is just Length :)
                }
            }
        }



        [Test]
        public void AddOnDisposed_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var handler = default(ULSharedMemory.DisposedEventHandler);

                PULSharedMemory.AddOnDisposedDisposedEventHandler().Body = (@this, value) => handler += value;
                PULSharedMemory.Dispose().Body = @this =>
                {
                    handler(true);
                    IndirectionsContext.ExecuteOriginal(() => @this.Dispose());
                };

                var called = false;

                using (var sm = new ULSharedMemory())
                {
                    // Act
                    sm.OnDisposed += disposing => called = disposing;
                }

                
                // Assert
                Assert.IsTrue(called);
            }
        }
    }
}
