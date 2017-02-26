/* 
 * File: ULInstanceGettersTest.cs
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


#if NUnit
using TestFixtureAttribute = NUnit.Framework.TestFixtureAttribute;
using TestAttribute = NUnit.Framework.TestAttribute;
#elif MsTest
using TestFixtureAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute;
using TestAttribute = Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute;
#endif
using Microsoft.Win32;
using Microsoft.Win32.Prig;
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;
using Test.program1.TestUtilities;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class ULInstanceGettersTest
    {
        [Test]
        public void ULInstanceGetters_should_set_dll_directory_at_its_static_constructor()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var inprocServer32KeyMock = new PProxyRegistryKeyMock();
                inprocServer32KeyMock.SetupGetValueString("", @"C:\Users\urasa\Prig\Release\x64\Urasandesu.Prig.dll");
                var classesRootMock = new PRegistryKeyMock();
                classesRootMock.SetupOpenSubKeyString(@"CLSID\{532C1F05-F8F3-4FBA-8724-699A31756ABD}\InprocServer32", inprocServer32KeyMock);

                var instanceGetters = new PULInstanceGettersMock();
                instanceGetters.SetupSetDllDirectoryString(@"C:\Users\urasa\Prig\Release\x64");


                // Act
                // static constructor will be called when referencing any static members
                var _ = ULInstanceGetters.WeaverDirectory;


                // Assert
                inprocServer32KeyMock.VerifyGetValueString();
                classesRootMock.VerifyOpenSubKeyString();
                instanceGetters.VerifySetDllDirectoryString();
            }
        }

        

        [Test]
        public void WeaverDirectory_should_return_the_directory_path_of_the_profiler()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var inprocServer32KeyMock = new PProxyRegistryKeyMock();
                inprocServer32KeyMock.SetupGetValueStringAny(@"C:\Users\urasa\Prig\Release\x64\Urasandesu.Prig.dll");
                var classesRootMock = new PRegistryKeyMock();
                classesRootMock.SetupOpenSubKeyStringAny(inprocServer32KeyMock);

                var instanceGetters = new PULInstanceGettersMock();
                instanceGetters.SetupSetDllDirectoryStringAny();


                // Act
                var result = ULInstanceGetters.WeaverDirectory;


                // Assert
                Assert.AreEqual(result, @"C:\Users\urasa\Prig\Release\x64");
            }
        }


        
        class PProxyRegistryKeyMock : PProxyRegistryKey
        {
            string SetupGetValueString_expected_name { get; set; }
            string SetupGetValueString_actual_name { get; set; }

            public void SetupGetValueStringAny(object result)
            {
                GetValueString().Body =
                    (@this, name) =>
                    {
                        return result;
                    };
            }

            public void SetupGetValueString(string expected_name, object result)
            {
                SetupGetValueString_expected_name = expected_name;
                GetValueString().Body = 
                    (@this, name) => 
                    {
                        SetupGetValueString_actual_name = name;
                        return result;
                    };
            }

            public void VerifyGetValueString()
            {
                Assert.AreEqual(SetupGetValueString_expected_name, SetupGetValueString_actual_name);
            }
        }

        class PRegistryKeyMock : PRegistryKey
        {
            string SetupOpenSubKeyString_expected_name { get; set; }
            string SetupOpenSubKeyString_actual_name { get; set; }

            public void SetupOpenSubKeyStringAny(RegistryKey result)
            {
                OpenSubKeyString().Body =
                    (@this, name) =>
                    {
                        return result;
                    };
            }

            public void SetupOpenSubKeyString(string expected_name, RegistryKey result)
            {
                SetupOpenSubKeyString_expected_name = expected_name;
                OpenSubKeyString().Body = 
                    (@this, name) => 
                    {
                        Assert.AreEqual(Registry.ClassesRoot, @this);
                        SetupOpenSubKeyString_actual_name = name;
                        return result; 
                    };
            }

            public void VerifyOpenSubKeyString()
            {
                Assert.AreEqual(SetupOpenSubKeyString_expected_name, SetupOpenSubKeyString_actual_name);
            }
        }

        class PULInstanceGettersMock : PULInstanceGetters
        {
            string SetupSetDllDirectoryString_expected_lpPathName { get; set; }
            string SetupSetDllDirectoryString_actual_lpPathName { get; set; }

            public void SetupSetDllDirectoryStringAny()
            {
                SetDllDirectoryString().Body =
                    lpPathName =>
                    {
                        return true;
                    };
            }

            public void SetupSetDllDirectoryString(string expected_lpPathName)
            {
                SetupSetDllDirectoryString_expected_lpPathName = expected_lpPathName;
                SetDllDirectoryString().Body = 
                    lpPathName => 
                    {
                        SetupSetDllDirectoryString_actual_lpPathName = lpPathName;
                        return true;
                    };
            }

            public void VerifySetDllDirectoryString()
            {
                Assert.AreEqual(SetupSetDllDirectoryString_expected_lpPathName, SetupSetDllDirectoryString_actual_lpPathName);
            }

        }
    }
}
