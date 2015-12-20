/* 
 * File: Regsvr32ExecutorTest.cs
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



using Microsoft.Win32;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.Ploeh.AutoFixture;
using Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.System;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
{
    [TestFixture]
    class Regsvr32ExecutorTest
    {
        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartInstalling_should_install_x86_com_component()
        {
            using (var classesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32))
            {
                try
                {
                    // Arrange
                    var fixture = new Fixture().Customize(new AutoMoqCustomization());

                    var profPath = AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\x86\Urasandesu.Prig.dll");

                    var regsvr32Executor = fixture.NewRegsvr32Executor();


                    // Act
                    var result = regsvr32Executor.StartInstalling(profPath);


                    // Assert
                    using (var inprocServer32Key = classesRootKey.OpenSubKey(ProfilerLocation.InprocServer32Path))
                    {
                        Assert.AreEqual(inprocServer32Key.GetValue(null), profPath);
                    }
                }
                finally
                {
                    try
                    {
                        classesRootKey.DeleteSubKey(ProfilerLocation.InprocServer32Path);
                    }
                    catch
                    { }
                }
            }
        }

        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartInstalling_should_install_x64_com_component()
        {
            using (var classesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64))
            {
                try
                {
                    // Arrange
                    var fixture = new Fixture().Customize(new AutoMoqCustomization());

                    var profPath = AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\x64\Urasandesu.Prig.dll");

                    var regsvr32Executor = fixture.NewRegsvr32Executor();


                    // Act
                    var result = regsvr32Executor.StartInstalling(profPath);


                    // Assert
                    using (var inprocServer32Key = classesRootKey.OpenSubKey(ProfilerLocation.InprocServer32Path))
                    {
                        Assert.AreEqual(inprocServer32Key.GetValue(null), profPath);
                    }
                }
                finally
                {
                    try
                    {
                        classesRootKey.DeleteSubKey(ProfilerLocation.InprocServer32Path);
                    }
                    catch
                    { }
                }
            }
        }



        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartUninstalling_should_install_x86_com_component()
        {
            using (var classesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32))
            {
                try
                {
                    // Arrange
                    var fixture = new Fixture().Customize(new AutoMoqCustomization());

                    var profPath = AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\x86\Urasandesu.Prig.dll");

                    var regsvr32Executor = fixture.NewRegsvr32Executor();
                    regsvr32Executor.StartInstalling(profPath);


                    // Act
                    var result = regsvr32Executor.StartUninstalling(profPath);


                    // Assert
                    Assert.IsNull(classesRootKey.OpenSubKey(ProfilerLocation.InprocServer32Path));
                }
                finally
                {
                    try
                    {
                        classesRootKey.DeleteSubKey(ProfilerLocation.InprocServer32Path);
                    }
                    catch
                    { }
                }
            }
        }

        [Test]
        [Explicit("This test has the possibility that your machine environment is changed. You have to understand the content if you will run it.")]
        public void StartUninstalling_should_install_x64_com_component()
        {
            using (var classesRootKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64))
            {
                try
                {
                    // Arrange
                    var fixture = new Fixture().Customize(new AutoMoqCustomization());

                    var profPath = AppDomain.CurrentDomain.GetPathInBaseDirectory(@"tools\x64\Urasandesu.Prig.dll");

                    var regsvr32Executor = fixture.NewRegsvr32Executor();
                    regsvr32Executor.StartInstalling(profPath);


                    // Act
                    var result = regsvr32Executor.StartUninstalling(profPath);


                    // Assert
                    Assert.IsNull(classesRootKey.OpenSubKey(ProfilerLocation.InprocServer32Path));
                }
                finally
                {
                    try
                    {
                        classesRootKey.DeleteSubKey(ProfilerLocation.InprocServer32Path);
                    }
                    catch
                    { }
                }
            }
        }
    }
}
