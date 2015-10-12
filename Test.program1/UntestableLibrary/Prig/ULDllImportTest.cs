/* 
 * File: ULDllImportTest.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2015 Akira Sugiura
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
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class ULDllImportTest
    {
        [Test]
        public void FormatCurrentProcessId_on_execute_should_return_expected()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var expected = 3500;
                PULDllImport.GetCurrentProcessId().Body = () => expected;

                // Act
                var actual = new ULDllImport().FormatCurrentProcessId();

                // Assert
                Assert.AreEqual(string.Format("The current process ID is {0}", expected), actual);
            }
        }



        [Test]
        public void Start_on_execute_should_throw_NotImplementedException()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PULDllImport.ExcludeGeneric().DefaultBehavior = IndirectionBehaviors.NotImplemented;

                // Act, Assert
                Assert.Throws<NotImplementedException>(() => new ULDllImport().Start());
            }
        }



        [Test]
        public void Is64BitProcessMessage_on_64_bit_execute_should_return_expected()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PULDllImport.IsWow64ProcessIntPtrBooleanRef().Body = (IntPtr processHandle, out bool wow64Process) =>
                {
                    wow64Process = true;
                    return true;
                };

                // Act
                var actual = new ULDllImport().Is64BitProcessMessage();

                // Assert
                Assert.AreEqual("This is a 32 bit process!", actual);
            }
        }


        
        delegate bool GetThreadTimesFunc(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [Test]
        public void FormatCurrentThreadTimes_on_execute_should_return_expected()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                Stub<OfPULDllImport>.Setup<GetThreadTimesFunc>(_ => _.GetThreadTimesIntPtrInt64RefInt64RefInt64RefInt64Ref()).Body =
                    (IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime) =>
                    {
                        lpCreationTime = 0;
                        lpExitTime = 1;
                        lpKernelTime = 2;
                        lpUserTime = 3;
                        return true;
                    };


                // Act
                var actual = new ULDllImport().FormatCurrentThreadTimes();


                // Assert
                Assert.AreEqual("Creation Time: 0, Exit Time: 1, Kernel Time: 2, User Time: 3", actual);
            }
        }
    }
}
