/* 
 * File: PrigPackageTest.cs
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



using EnvDTE;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
{
    [TestFixture]
    class PrigPackageTest
    {
        [Test]
        public void NewEnableTestAdapterCommand_should_bind_Text_property_to_CurrentProject_status()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var projName = fixture.Create<string>();
            {
                var m = fixture.Freeze<Mock<Project>>();
                m.Setup(_ => _.Name).Returns(projName);
            }

            var vm = new PrigViewModel();
            var menuCommand = PrigPackage.NewEnableTestAdapterCommand(vm);
            menuCommand.Text = null;


            // Act
            vm.CurrentProject.Value = null;
            var defaultText = menuCommand.Text;
            vm.CurrentProject.Value = fixture.Freeze<Project>();
            var projectSelectedText = menuCommand.Text;

            
            // Assert
            Assert.IsNotNull(defaultText);
            Assert.AreNotEqual(defaultText, projectSelectedText);
            Assert.That(projectSelectedText, Is.StringMatching(projName));
        }



        [Test]
        public void NewDisableTestAdapterCommand_should_bind_Text_property_to_CurrentProject_status()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var projName = fixture.Create<string>();
            {
                var m = fixture.Freeze<Mock<Project>>();
                m.Setup(_ => _.Name).Returns(projName);
            }

            var vm = new PrigViewModel();
            var menuCommand = PrigPackage.NewDisableTestAdapterCommand(vm);
            menuCommand.Text = null;


            // Act
            vm.CurrentProject.Value = null;
            var defaultText = menuCommand.Text;
            vm.CurrentProject.Value = fixture.Freeze<Project>();
            var projectSelectedText = menuCommand.Text;


            // Assert
            Assert.IsNotNull(defaultText);
            Assert.AreNotEqual(defaultText, projectSelectedText);
            Assert.That(projectSelectedText, Is.StringMatching(projName));
        }
    }
}
