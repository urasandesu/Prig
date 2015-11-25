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
using Test.Urasandesu.Prig.VSPackage.Mixins.Ploeh.AutoFixture;
using Test.Urasandesu.Prig.VSPackage.Mixins.System;
using Test.Urasandesu.Prig.VSPackage.Mixins.System.IO;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
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
            var nugetConfigInfo = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config"));
            using (nugetConfigInfo.BeginModifying())
            {
                // Arrange
                nugetConfigInfo.Delete();

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
            var nugetConfigInfo = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config"));
            using (nugetConfigInfo.BeginModifying())
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
                using (var fs = nugetConfigInfo.Open(FileMode.Create))
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
            var nugetConfigInfo = new FileInfo(Path.Combine(Environment.ExpandEnvironmentVariables("%APPDATA%"), @"NuGet\NuGet.Config"));
            using (nugetConfigInfo.BeginModifying())
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
                using (var fs = nugetConfigInfo.Open(FileMode.Create))
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
    }
}

namespace Test.Urasandesu.Prig.VSPackage.Mixins.Ploeh.AutoFixture
{
    static partial class IFixtureMixin
    {
        public static NuGetExecutor NewNuGetExecutor(this IFixture fixture)
        {
            var nugetExecutor = new NuGetExecutor();
            nugetExecutor.EnvironmentRepository = fixture.Freeze<IEnvironmentRepository>();
            return nugetExecutor;
        }
    }
}

namespace Test.Urasandesu.Prig.VSPackage.Mixins.System.IO
{
    public static class FileInfoMixin
    {
        public static FileInfoModifyingBegun BeginModifying(this FileInfo orgInfo)
        {
            var id = Guid.NewGuid().ToString("N");
            var bakPath = orgInfo.FullName + "." + id + ".bak";
            var modPath = orgInfo.FullName + "." + id + ".mod";
            try
            {
                File.Delete(bakPath);
            }
            catch
            { }
            if (orgInfo.Exists)
                orgInfo.CopyTo(bakPath, true);
            return new FileInfoModifyingBegun(orgInfo, bakPath, modPath);
        }
    }

    public struct FileInfoModifyingBegun : IDisposable
    {
        readonly FileInfo m_orgInfo;
        readonly string m_bakPath;
        readonly string m_modPath;

        public FileInfoModifyingBegun(FileInfo orgInfo, string bakPath, string modPath)
        {
            m_orgInfo = orgInfo;
            m_bakPath = bakPath;
            m_modPath = modPath;
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(m_modPath))
                    File.Delete(m_modPath);
                if (m_orgInfo.Exists)
                {
                    m_orgInfo.CopyTo(m_modPath, true);
                    m_orgInfo.Delete();
                }
            }
            catch
            { }

            try
            {
                if (File.Exists(m_bakPath))
                    File.Copy(m_bakPath, m_orgInfo.FullName, true);
            }
            catch
            { }
        }
    }
}