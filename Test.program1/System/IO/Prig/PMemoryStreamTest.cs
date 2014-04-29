/* 
 * File: PMemoryStreamTest.cs
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
using System.IO;
using System.IO.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.System.IO.Prig
{
    [TestFixture]
    public class PMemoryStreamTest
    {
        [Test]
        public void Seek_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PMemoryStream.Seek.Body = (@this, offset, loc) => 42L;

                var buffer = new byte[256];
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = (byte)i;

                
                using (var ms = new MemoryStream(buffer))
                {
                    // Act
                    var actual = ms.Seek(128, SeekOrigin.Begin);

                    // Assert
                    Assert.AreEqual(42L, actual);
                }
            }
        }

        [Test]
        public void BeginRead_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PStream.BeginRead.Body = (@this, _buffer, offset, count, callback, state) => 
                    IndirectionsContext.ExecuteOriginal(() => @this.BeginRead(_buffer, offset, 42, callback, state));

                var buffer = new byte[256];
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = (byte)i;


                using (var ms = new MemoryStream(buffer))
                {
                    // Act
                    var _buffer = new byte[1024];
                    var ar = ms.BeginRead(_buffer, 0, _buffer.Length, null, null);
                    var actual = ms.EndRead(ar);

                    // Assert
                    Assert.AreEqual(42, actual);
                }
            }
        }
    }
}
