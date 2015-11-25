/* 
 * File: PrigExecutorTest.cs
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
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.IO;
using Test.Urasandesu.Prig.VSPackage.Mixins.Ploeh.AutoFixture;
using Test.Urasandesu.Prig.VSPackage.Mixins.System;
using Test.Urasandesu.Prig.VSPackage.Mixins.System.IO;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
{
    [TestFixture]
    class PrigExecutorTest
    {
        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartInstalling_should_install_specified_source()
        {
            var prigConfigInfo = new FileInfo(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\Prig.config"));
            using (prigConfigInfo.BeginModifying())
            {
                try
                {
                    // Arrange
                    prigConfigInfo.Delete();
                    Environment.SetEnvironmentVariable("URASANDESU_PRIG_PACKAGE_FOLDER", AppDomain.CurrentDomain.BaseDirectory);

                    var fixture = new Fixture().Customize(new AutoMoqCustomization());
                    {
                        var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                        m.Setup(_ => _.GetPrigPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\prig.exe"));
                        fixture.Inject(m);
                    }

                    var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString("N"));
                    Directory.CreateDirectory(source);

                    var prigExecutor = fixture.NewPrigExecutor();
                    

                    // Act
                    var result = prigExecutor.StartInstalling("TestWindow", source);


                    // Assert
                    Assert.IsEmpty(result);
                    Assert.AreEqual(6, Directory.GetFiles(source).Length);
                }
                finally
                {
                    Environment.SetEnvironmentVariable("URASANDESU_PRIG_PACKAGE_FOLDER", null);
                }
            }
        }

        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartInstalling_should_do_nothing_if_it_will_be_executed_after_the_2nd_time()
        {
            var prigConfigInfo = new FileInfo(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\Prig.config"));
            using (prigConfigInfo.BeginModifying())
            {
                try
                {
                    // Arrange
                    prigConfigInfo.Delete();
                    Environment.SetEnvironmentVariable("URASANDESU_PRIG_PACKAGE_FOLDER", AppDomain.CurrentDomain.BaseDirectory);

                    var fixture = new Fixture().Customize(new AutoMoqCustomization());
                    {
                        var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                        m.Setup(_ => _.GetPrigPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\prig.exe"));
                        fixture.Inject(m);
                    }

                    var source = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString("N"));
                    Directory.CreateDirectory(source);

                    var prigExecutor = fixture.NewPrigExecutor();


                    // Act
                    prigExecutor.StartInstalling("TestWindow", source);
                    var result = prigExecutor.StartInstalling("TestWindow", source);


                    // Assert
                    Assert.That(result, Is.StringMatching(@"The specified source:""[^""]+"" has already installed\."));
                }
                finally
                {
                    Environment.SetEnvironmentVariable("URASANDESU_PRIG_PACKAGE_FOLDER", null);
                }
            }
        }
    }
}

namespace Test.Urasandesu.Prig.VSPackage.Mixins.Ploeh.AutoFixture
{
    static partial class IFixtureMixin
    {
        public static PrigExecutor NewPrigExecutor(this IFixture fixture)
        {
            var prigExecutor = new PrigExecutor();
            prigExecutor.EnvironmentRepository = fixture.Freeze<IEnvironmentRepository>();
            return prigExecutor;
        }
    }
}
