/* 
 * File: NuGetExecutorTest.cs
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
using System.Text.RegularExpressions;
using Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.Ploeh.AutoFixture;
using Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.System;
using Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.System.IO;
using Urasandesu.Prig.VSPackage.Models;

namespace Test.Urasandesu.Prig.VSPackage.Models
{
    [TestFixture]
    class NuGetExecutorTest
    {
        [Test]
        public void StartPacking_should_create_nupkg()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            {
                var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                m.Setup(_ => _.GetNuGetPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet.exe"));
                fixture.Inject(m);
            }

            var nugetExecutor = fixture.NewNuGetExecutor();


            // Act
            var result = nugetExecutor.StartPacking(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet\Prig.nuspec"), AppDomain.CurrentDomain.BaseDirectory);


            // Assert
            var lines = result.Split(new[] { "\r\n" }, StringSplitOptions.None);
            Assert.LessOrEqual(2, lines.Length);
            var match = Regex.Match(lines[1], "'([^']+)'");
            Assert.IsTrue(match.Success);
            var nupkgPath = match.Groups[1].Value;
            Assert.IsTrue(File.Exists(nupkgPath));
            Assert.GreaterOrEqual(TimeSpan.FromSeconds(1), DateTime.Now - File.GetLastWriteTime(nupkgPath));
        }



        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartSourcing_should_add_the_specified_source_if_it_has_not_existed_yet()
        {
            using (var nugetConfig = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config")).BeginModifying())
            {
                // Arrange
                nugetConfig.Info.Delete();

                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                    m.Setup(_ => _.GetNuGetPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet.exe"));
                    fixture.Inject(m);
                }

                var nugetExecutor = fixture.NewNuGetExecutor();


                // Act
                var result = nugetExecutor.StartSourcing("Prig Source", AppDomain.CurrentDomain.BaseDirectory);


                // Assert
                Assert.That(result, Is.StringMatching("Prig Source"));
            }
        }



        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartSourcing_should_update_the_specified_source_if_it_has_already_existed()
        {
            using (var nugetConfig = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config")).BeginModifying())
            {
                // Arrange
                var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""https://www.nuget.org/api/v2/"" value=""https://www.nuget.org/api/v2/"" />
    <add key=""Prig Source"" value=""C:\Users\Akira\Prig\Test.Urasandesu.Prig.VSPackage\bin\Debug\ "" />
  </packageSources>
  <disabledPackageSources />
</configuration>
";
                using (var fs = nugetConfig.Info.Open(FileMode.Create))
                using (var sw = new StreamWriter(fs))
                    sw.Write(xml);

                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                    m.Setup(_ => _.GetNuGetPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet.exe"));
                    fixture.Inject(m);
                }

                var nugetExecutor = fixture.NewNuGetExecutor();


                // Act
                var result = nugetExecutor.StartSourcing("Prig Source", Environment.ExpandEnvironmentVariables("%TEMP%"));


                // Assert
                Assert.That(result, Is.StringMatching("Prig Source"));
            }
        }



        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartSourcing_should_update_the_specified_source_if_only_letter_case_is_unmatched()
        {
            using (var nugetConfig = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config")).BeginModifying())
            {
                // Arrange
                var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""https://www.nuget.org/api/v2/"" value=""https://www.nuget.org/api/v2/"" />
    <add key=""pRiG sOurCe"" value=""C:\Users\Akira\Prig\Test.Urasandesu.Prig.VSPackage\bin\Debug\ "" />
  </packageSources>
  <disabledPackageSources />
</configuration>
";
                using (var fs = nugetConfig.Info.Open(FileMode.Create))
                using (var sw = new StreamWriter(fs))
                    sw.Write(xml);

                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                    m.Setup(_ => _.GetNuGetPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet.exe"));
                    fixture.Inject(m);
                }

                var nugetExecutor = fixture.NewNuGetExecutor();


                // Act
                var result = nugetExecutor.StartSourcing("Prig Source", Environment.ExpandEnvironmentVariables("%TEMP%"));


                // Assert
                Assert.That(result, Is.StringMatching("Prig Source"));
            }
        }



        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartUnsourcing_should_remove_the_specified_source_if_it_has_already_existed()
        {
            using (var nugetConfig = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config")).BeginModifying())
            {
                // Arrange
                nugetConfig.Info.Delete();

                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                    m.Setup(_ => _.GetNuGetPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet.exe"));
                    fixture.Inject(m);
                }

                var nugetExecutor = fixture.NewNuGetExecutor();
                nugetExecutor.StartSourcing("Prig Source", AppDomain.CurrentDomain.BaseDirectory);


                // Act
                var result = nugetExecutor.StartUnsourcing("Prig Source");


                // Assert
                Assert.That(result, Is.StringMatching("Prig Source"));
            }
        }

        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartUnsourcing_should_do_nothing_if_it_has_already_removed()
        {
            using (var nugetConfig = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config")).BeginModifying())
            {
                // Arrange
                nugetConfig.Info.Delete();

                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                    m.Setup(_ => _.GetNuGetPath()).Returns(AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\NuGet.exe"));
                    fixture.Inject(m);
                }

                var nugetExecutor = fixture.NewNuGetExecutor();


                // Act
                var result = nugetExecutor.StartUnsourcing("Prig Source");


                // Assert
                Assert.That(result, Is.StringMatching("Prig Source"));
            }
        }
    }
}
