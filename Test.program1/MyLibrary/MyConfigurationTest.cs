/* 
 * File: MyConfigurationTest.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
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
#elif Xunit
using TestAttribute = Xunit.FactAttribute;
#endif
using program1.MyLibrary;
using System;
using System.Configuration;
using System.Configuration.Prig;
using System.Linq;
using System.Reflection;
using System.Reflection.Prig;
using Test.program1.TestUtilities;
using Urasandesu.Prig.Framework;

namespace Test.program1.MyLibrary
{
    [TestFixture]
    public class MyConfigurationTest
    {
        [Test]
        public void LoadFromSettings_should_return_valid_session_after_checking_specified_assembly_by_Assembly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var myConfiguration = new MyConfiguration();
                myConfiguration.Assembly = typeof(object).Assembly.FullName;
                myConfiguration.Settings = Guid.NewGuid().ToString("N") + ".my";

                var assemblyMock = new PAssemblyMock();
                assemblyMock.SetupLoadString(myConfiguration.Assembly);

                var configurationMock = new PProxyConfigurationMock();
                configurationMock.SetupGetSectionString("my");
                var configurationManagerMock = new PConfigurationManagerMock();
                configurationManagerMock.SetupOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel(myConfiguration.Settings, configurationMock);


                // Act
                var section = myConfiguration.LoadFromSettings();


                // Assert
                assemblyMock.VerifyLoadString();
                configurationMock.VerifyGetSectionString();
                configurationManagerMock.VerifyOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel();
                Assert.IsNotNull(section);
            }
        }

        [Test]
        public void LoadFromSettings_should_return_valid_session_after_checking_specified_assembly_by_AssemblyFrom()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var myConfiguration = new MyConfiguration();
                myConfiguration.AssemblyFrom = typeof(object).Assembly.Location;
                myConfiguration.Settings = Guid.NewGuid().ToString("N") + ".my";

                var assemblyMock = new PAssemblyMock();
                assemblyMock.SetupLoadFromString(myConfiguration.AssemblyFrom);

                var configurationMock = new PProxyConfigurationMock();
                configurationMock.SetupGetSectionString("my");
                var configurationManagerMock = new PConfigurationManagerMock();
                configurationManagerMock.SetupOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel(myConfiguration.Settings, configurationMock);


                // Act
                var section = myConfiguration.LoadFromSettings();


                // Assert
                assemblyMock.VerifyLoadString();
                configurationMock.VerifyGetSectionString();
                configurationManagerMock.VerifyOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel();
                Assert.IsNotNull(section);
            }
        }

        [Test]
        public void LoadFromSettings_should_throw_InvalidOperationException_if_Assembly_and_AssemblyFrom_are_not_specified()
        {
            // Arrange
            var myConfiguration = new MyConfiguration();

            // Act, Assert
            ExceptionAssert.Throws<InvalidOperationException>(() => myConfiguration.LoadFromSettings());
        }

        [Test]
        public void LoadFromSettings_should_throw_InvalidOperationException_if_configuration_contains_the_assembly_information_that_is_different_from_the_specified()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var myConfiguration = new MyConfiguration();
                myConfiguration.Assembly = typeof(object).Assembly.FullName;
                myConfiguration.Settings = Guid.NewGuid().ToString("N") + ".my";

                var configurationMock = new PProxyConfigurationMock();
                configurationMock.SetupGetSectionString("my", typeof(ConfigurationManager).GetMethods().First());
                var configurationManagerMock = new PConfigurationManagerMock();
                configurationManagerMock.SetupOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel(myConfiguration.Settings, configurationMock);


                // Act, Assert
                ExceptionAssert.Throws<InvalidOperationException>(() => myConfiguration.LoadFromSettings());
            }
        }

        
        
        [Test]
        public void LoadFromAppConfig_should_return_valid_session_after_checking_specified_assembly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var myConfiguration = new MyConfiguration();
                myConfiguration.Assembly = typeof(object).Assembly.FullName;

                var assemblyMock = new PAssemblyMock();
                assemblyMock.SetupLoadString(myConfiguration.Assembly);

                var configurationManagerMock = new PConfigurationManagerMock();
                configurationManagerMock.SetupGetSectionString("my");


                // Act
                var section = myConfiguration.LoadFromAppConfig();


                // Assert
                assemblyMock.VerifyLoadString();
                configurationManagerMock.VerifyGetSectionString();
                Assert.IsNotNull(section);
            }
        }


        
        class PAssemblyMock : PAssembly
        {
            string LoadString_expected_assemblyString { get; set; }
            string LoadString_actual_assemblyString { get; set; }

            public void SetupLoadString(string expected_assemblyString)
            {
                LoadString_expected_assemblyString = expected_assemblyString;
                LoadString().Body =
                    assemblyString =>
                    {
                        LoadString_actual_assemblyString = assemblyString;
                        return IndirectionsContext.ExecuteOriginal(() => Assembly.Load(assemblyString));
                    };
            }

            public void VerifyLoadString()
            {
                Assert.AreEqual(LoadString_expected_assemblyString, LoadString_actual_assemblyString);
            }


            
            string LoadFromString_expected_assemblyFile { get; set; }
            string LoadFromString_actual_assemblyFile { get; set; }

            public void SetupLoadFromString(string expected_assemblyFile)
            {
                LoadFromString_expected_assemblyFile = expected_assemblyFile;
                LoadFromString().Body =
                    assemblyFile =>
                    {
                        LoadFromString_actual_assemblyFile = assemblyFile;
                        return IndirectionsContext.ExecuteOriginal(() => Assembly.LoadFrom(assemblyFile));
                    };
            }

            public void VerifyLoadFromString()
            {
                Assert.AreEqual(LoadFromString_expected_assemblyFile, LoadFromString_actual_assemblyFile);
            }
        }

        class PProxyConfigurationMock : PProxyConfiguration
        {
            string GetSectionString_expected_sectionName { get; set; }
            string GetSectionString_actual_sectionName { get; set; }

            public void SetupGetSectionString(string expected_sectionName)
            {
                SetupGetSectionString(expected_sectionName, typeof(object).GetMethods().First());
            }

            public void SetupGetSectionString(string expected_sectionName, MethodBase target)
            {
                GetSectionString_expected_sectionName = expected_sectionName;
                GetSectionString().Body =
                    (@this, sectionName) =>
                    {
                        GetSectionString_actual_sectionName = sectionName;
                        var myElementCollection = new MyElementCollectionMock();
                        {
                            var myElement = new MyElementMock();
                            myElement.Target = target;
                            myElementCollection.Add(myElement);
                        }
                        return new MySectionMock() { MyElements = myElementCollection };
                    };
            }

            public void VerifyGetSectionString()
            {
                Assert.AreEqual(GetSectionString_expected_sectionName, GetSectionString_actual_sectionName);
            }
        }

        class PConfigurationManagerMock : PConfigurationManager
        {
            string OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel_expected_fileMap_ExeConfigFilename { get; set; }
            string OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel_actual_fileMap_ExeConfigFilename { get; set; }

            public void SetupOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel(string expected_fileMap_ExeConfigFilename, Configuration result)
            {
                OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel_expected_fileMap_ExeConfigFilename = expected_fileMap_ExeConfigFilename;
                OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel().Body =
                    (fileMap, userLevel) =>
                    {
                        Assert.IsNotNull(fileMap);
                        OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel_actual_fileMap_ExeConfigFilename = fileMap.ExeConfigFilename;
                        return result;
                    };
            }

            public void VerifyOpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel()
            {
                Assert.AreEqual(OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel_expected_fileMap_ExeConfigFilename, 
                                OpenMappedExeConfigurationExeConfigurationFileMapConfigurationUserLevel_actual_fileMap_ExeConfigFilename);
            }

            
            
            string GetSectionString_expected_sectionName { get; set; }
            string GetSectionString_actual_sectionName { get; set; }
            
            public void SetupGetSectionString(string expected_sectionName)
            {
                GetSectionString_expected_sectionName = expected_sectionName;
                GetSectionString().Body =
                    sectionName =>
                    {
                        GetSectionString_actual_sectionName = sectionName;
                        var myElementCollection = new MyElementCollectionMock();
                        {
                            var myElement = new MyElementMock();
                            myElement.Target = typeof(object).GetMethods().First();
                            myElementCollection.Add(myElement);
                        }
                        return new MySectionMock() { MyElements = myElementCollection };
                    };
            }

            public void VerifyGetSectionString()
            {
                Assert.AreEqual(GetSectionString_expected_sectionName, GetSectionString_actual_sectionName);
            }
        }

        class MySectionMock : MySection
        {
            [ConfigurationProperty("myElements", Options = ConfigurationPropertyOptions.IsRequired)]
            public new MyElementCollection MyElements
            {
                get { return base.MyElements; }
                set { base["myElements"] = value; }
            }
        }

        class MyElementCollectionMock : MyElementCollection
        {
            public void Add(MyElement myElement)
            {
                base.BaseAdd(myElement);
            }
        }

        class MyElementMock : MyElement
        {
            [ConfigurationProperty("name", IsRequired = true)]
            public new string Name
            {
                get { return base.Name; }
                set { base["name"] = value; }
            }

            [ConfigurationProperty("alias", IsRequired = true)]
            public new string Alias
            {
                get { return base.Alias; }
                set { base["alias"] = value; }
            }

            public new MethodBase Target 
            {
                get { return base.Target; }
                set { typeof(MyElement).GetProperty("Target").SetValue(this, value, null); }
            }
        }
    }
}
