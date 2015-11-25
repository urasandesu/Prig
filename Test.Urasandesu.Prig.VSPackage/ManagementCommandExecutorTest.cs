/* 
 * File: ManagementCommandExecutorTest.cs
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



using Moq;
using NUnit.Framework;
using System;
using System.Management.Automation;
using Test.Urasandesu.Prig.VSPackage.Mixins.Moq;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
{
    [TestFixture]
    class ManagementCommandExecutorTest
    {
        [Test]
        public void Execute_should_execute_specified_PowerShell_command()
        {
            // Arrange
            var command = @"
([System.Management.Automation.PSMemberTypes])
";
            var mci = new ManagementCommandInfo(command);
            var mcExecutor = new ManagementCommandExecutor();


            // Act
            var results = mcExecutor.Execute(mci);

            
            // Assert
            Assert.AreEqual(typeof(PSMemberTypes), results[0].BaseObject);
        }

        [Test]
        public void Execute_should_raise_the_event_to_execute_specified_PowerShell_command_before_and_after()
        {
            // Arrange
            var command = @"
([System.Management.Automation.PSMemberTypes])
";
            var mci = new ManagementCommandInfo(command);
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mci.CommandExecuting += mocks.InOrder<Action>(order, m => m.Setup(_ => _())).Object;
            mci.CommandExecuted += mocks.InOrder<Action>(order, m => m.Setup(_ => _())).Object;

            var mcExecutor = new ManagementCommandExecutor();

            
            // Act
            mcExecutor.Execute(mci);

            
            // Assert
            mocks.VerifyAll();
        }
    }
}
