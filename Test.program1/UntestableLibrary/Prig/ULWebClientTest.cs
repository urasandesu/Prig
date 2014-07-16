/* 
 * File: ULWebClientTest.cs
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
using System.ComponentModel;
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class ULWebClientTest
    {
        [Test]
        public void DownloadFileAsync_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var called = false;
                var notCalled = true;
                var handler = default(AsyncCompletedEventHandler);
                handler = (sender, e) => called = true;
                PULWebClient.AddDownloadFileCompleted.Body = (@this, value) => handler += value;
                PULWebClient.RemoveDownloadFileCompleted.Body = (@this, value) => handler -= value;
                PULWebClient.DownloadFileAsync.Body = (@this, address) =>
                {
                    var e = new AsyncCompletedEventArgs(null, false, null);
                    handler(@this, e);
                };

                
                // Act
                var client = new ULWebClient();
                client.DownloadFileAsync(new Uri("http://google.co.jp/"));
                client.DownloadFileCompleted += (sender, e) => notCalled = false;

                
                // Assert
                Assert.IsTrue(called);
                Assert.IsTrue(notCalled);
            }
        }
    }
}
