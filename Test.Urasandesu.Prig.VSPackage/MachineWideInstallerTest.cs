/* 
 * File: MachineWideInstallerTest.cs
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
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Collections;
using System.Linq;
using Test.Urasandesu.Prig.VSPackage.Mixins.Moq;
using Test.Urasandesu.Prig.VSPackage.Mixins.Ploeh.AutoFixture;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
{
    [TestFixture]
    class MachineWideInstallerTest
    {
        static readonly RegistryKey DummyX86ClassesRootKey = Registry.Users;
        static readonly RegistryKey DummyX86InProcServer32Key = Registry.LocalMachine;
        static readonly RegistryKey DummyX64ClassesRootKey = Registry.CurrentUser;
        static readonly RegistryKey DummyX64InProcServer32Key = Registry.CurrentConfig;



        [TestCaseSource(typeof(TestSourceForHasBeenInstalled), "TestCases")]
        [Test]
        public bool HasBeenInstalled_(MachinePrerequisite machinePreq, IFixture fixture)
        {
            // Arrange
            var mwInstllr = fixture.NewMachineWideInstaller();

            // Act, Assert
            var result = mwInstllr.HasBeenInstalled(machinePreq);
            fixture.Freeze<Mock<IEnvironmentRepository>>().VerifyAll();
            return result;
        }

        class TestSourceForHasBeenInstalled
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return should_return_true_if_all_profilers_have_already_been_installed();
                    yield return should_return_false_if_profiler_locations_are_null();
                    yield return should_return_false_if_profiler_locations_are_empty();
                    yield return should_return_false_if_profiler_is_not_registered_to_regstry();
                    yield return should_return_false_if_profiler_is_not_found_in_the_location_that_is_specified_by_the_registry();
                    yield return should_return_false_if_the_file_description_of_the_profiler_is_mismatch_with_expected();
                    yield return should_return_false_if_there_is_mismatch_between_registered_profilers_x86();
                    yield return should_return_false_if_there_is_mismatch_between_registered_profilers_x64();
                }
            }

            #region static TestCaseData should_return_true_if_all_profilers_have_already_been_installed()
            static TestCaseData should_return_true_if_all_profilers_have_already_been_installed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs =
                        new[] 
                        { 
                            new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                            new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                        };
                    var @params =
                        new[] 
                        { 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[0].RegistryView, 
                                ClassesRootKey = DummyX86ClassesRootKey, 
                                InProcServer32Key = DummyX86InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x86\Urasandesu.Prig.dll", 
                                ProfilerExistence = true, 
                                ProfilerFileDescription = "Prig Profiler 2.0.0 Type Library" 
                            }, 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[1].RegistryView, 
                                ClassesRootKey = DummyX64ClassesRootKey, 
                                InProcServer32Key = DummyX64InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x64\Urasandesu.Prig.dll", 
                                ProfilerExistence = true, 
                                ProfilerFileDescription = "Prig Profiler 2.0.0 Type Library" 
                            } 
                        };
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(true).SetName("should_return_true_if_all_profilers_have_already_been_installed");
            }
            #endregion

            #region static TestCaseData should_return_false_if_profiler_locations_are_null()
            static TestCaseData should_return_false_if_profiler_locations_are_null()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs = default(ProfilerLocation[]);
                    var @params = new ParameterForHasBeenInstalled[0];
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_profiler_locations_are_null");
            }
            #endregion

            #region static TestCaseData should_return_false_if_profiler_locations_are_empty()
            static TestCaseData should_return_false_if_profiler_locations_are_empty()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs = new ProfilerLocation[0];
                    var @params = new ParameterForHasBeenInstalled[0];
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_profiler_locations_are_empty");
            }
            #endregion

            #region static TestCaseData should_return_false_if_profiler_is_not_registered_to_regstry()
            static TestCaseData should_return_false_if_profiler_is_not_registered_to_regstry()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs =
                        new[] 
                        { 
                            new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                            new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                        };
                    var DoesNotCall = ParameterForHasBeenInstalled.DoesNotCall;
                    var @params =
                        new[] 
                        { 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[0].RegistryView, 
                                ClassesRootKey = DummyX86ClassesRootKey, 
                                InProcServer32Key = null, 
                                SetupGetRegistryValue = DoesNotCall, 
                                SetupExistsFile = DoesNotCall, 
                                SetupGetFileDescription = DoesNotCall
                            }, 
                            new ParameterForHasBeenInstalled() 
                            { 
                                SetupOpenRegistryBaseKey = DoesNotCall,
                                SetupOpenRegistrySubKey = DoesNotCall,
                                SetupGetRegistryValue = DoesNotCall, 
                                SetupExistsFile = DoesNotCall, 
                                SetupGetFileDescription = DoesNotCall
                            } 
                        };
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_profiler_is_not_registered_to_regstry");
            }
            #endregion

            #region static TestCaseData should_return_false_if_profiler_is_not_found_in_the_location_that_is_specified_by_the_registry()
            static TestCaseData should_return_false_if_profiler_is_not_found_in_the_location_that_is_specified_by_the_registry()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs =
                        new[] 
                        { 
                            new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                            new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                        };
                    var DoesNotCall = ParameterForHasBeenInstalled.DoesNotCall;
                    var @params =
                        new[] 
                        { 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[0].RegistryView, 
                                ClassesRootKey = DummyX86ClassesRootKey, 
                                InProcServer32Key = DummyX86InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x86\Urasandesu.Prig.dll", 
                                ProfilerExistence = false, 
                                SetupGetFileDescription = DoesNotCall
                            }, 
                            new ParameterForHasBeenInstalled() 
                            { 
                                SetupOpenRegistryBaseKey = DoesNotCall, 
                                SetupOpenRegistrySubKey = DoesNotCall,
                                SetupGetRegistryValue = DoesNotCall,
                                SetupExistsFile = DoesNotCall, 
                                SetupGetFileDescription = DoesNotCall
                            } 
                        };
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_profiler_is_not_found_in_the_location_that_is_specified_by_the_registry");
            }
            #endregion

            #region static TestCaseData should_return_false_if_the_file_description_of_the_profiler_is_mismatch_with_expected()
            static TestCaseData should_return_false_if_the_file_description_of_the_profiler_is_mismatch_with_expected()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs =
                        new[] 
                        { 
                            new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                            new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                        };
                    var DoesNotCall = ParameterForHasBeenInstalled.DoesNotCall;
                    var @params =
                        new[] 
                        { 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[0].RegistryView, 
                                ClassesRootKey = DummyX86ClassesRootKey, 
                                InProcServer32Key = DummyX86InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x86\Urasandesu.Prig.dll", 
                                ProfilerExistence = true, 
                                ProfilerFileDescription = "Prig Profiler 1.0.0 Type Library" 
                            }, 
                            new ParameterForHasBeenInstalled() 
                            { 
                                SetupOpenRegistryBaseKey = DoesNotCall, 
                                SetupOpenRegistrySubKey = DoesNotCall, 
                                SetupGetRegistryValue = DoesNotCall, 
                                SetupExistsFile = DoesNotCall, 
                                SetupGetFileDescription = DoesNotCall
                            } 
                        };
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_the_file_description_of_the_profiler_is_mismatch_with_expected");
            }
            #endregion

            #region static TestCaseData should_return_false_if_there_is_mismatch_between_registered_profilers_x86()
            static TestCaseData should_return_false_if_there_is_mismatch_between_registered_profilers_x86()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs =
                        new[] 
                        { 
                            new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                            new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                        };
                    var DoesNotCall = ParameterForHasBeenInstalled.DoesNotCall;
                    var @params =
                        new[] 
                        { 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[0].RegistryView, 
                                ClassesRootKey = DummyX86ClassesRootKey, 
                                InProcServer32Key = DummyX86InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x86\Urasandesu.Prig.dll", 
                                ProfilerExistence = true, 
                                ProfilerFileDescription = "Prig Profiler 1.0.0 Type Library" 
                            }, 
                            new ParameterForHasBeenInstalled() 
                            { 
                                SetupOpenRegistryBaseKey = DoesNotCall, 
                                SetupOpenRegistrySubKey = DoesNotCall, 
                                SetupGetRegistryValue = DoesNotCall, 
                                SetupExistsFile = DoesNotCall, 
                                SetupGetFileDescription = DoesNotCall
                            } 
                        };
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_there_is_mismatch_between_registered_profilers_x86");
            }
            #endregion

            #region static TestCaseData should_return_false_if_there_is_mismatch_between_registered_profilers_x64()
            static TestCaseData should_return_false_if_there_is_mismatch_between_registered_profilers_x64()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var machinePreq = new MachinePrerequisite("2.0.0");
                {
                    var profLocs =
                        new[] 
                        { 
                            new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                            new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                        };
                    var @params =
                        new[] 
                        { 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[0].RegistryView, 
                                ClassesRootKey = DummyX86ClassesRootKey, 
                                InProcServer32Key = DummyX86InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x86\Urasandesu.Prig.dll", 
                                ProfilerExistence = true, 
                                ProfilerFileDescription = "Prig Profiler 2.0.0 Type Library" 
                            }, 
                            new ParameterForHasBeenInstalled() 
                            { 
                                RegistryView = profLocs[1].RegistryView, 
                                ClassesRootKey = DummyX64ClassesRootKey, 
                                InProcServer32Key = DummyX64InProcServer32Key, 
                                ProfilerPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\x64\Urasandesu.Prig.dll", 
                                ProfilerExistence = true, 
                                ProfilerFileDescription = "Prig Profiler 1.0.0 Type Library" 
                            } 
                        };
                    fixture.Inject(MockEnvironmentRepositoryForHasBeenInstalled(profLocs, @params));
                }
                return new TestCaseData(machinePreq, fixture).Returns(false).SetName("should_return_false_if_there_is_mismatch_between_registered_profilers_x64");
            }
            #endregion



            #region Helpers
            class ParameterForHasBeenInstalled
            {
                public RegistryView RegistryView;
                public RegistryKey ClassesRootKey;
                public RegistryKey InProcServer32Key;
                public string ProfilerPath;
                public bool ProfilerExistence;
                public string ProfilerFileDescription;

                public Action<Mock<IEnvironmentRepository>, ParameterForHasBeenInstalled> SetupOpenRegistryBaseKey =
                    (m, param) => m.Setup(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, param.RegistryView)).Returns(param.ClassesRootKey);
                public Action<Mock<IEnvironmentRepository>, ParameterForHasBeenInstalled> SetupOpenRegistrySubKey =
                    (m, param) => m.Setup(_ => _.OpenRegistrySubKey(param.ClassesRootKey, ProfilerLocation.InprocServer32Path)).Returns(param.InProcServer32Key);
                public Action<Mock<IEnvironmentRepository>, ParameterForHasBeenInstalled> SetupGetRegistryValue =
                    (m, param) => m.Setup(_ => _.GetRegistryValue(param.InProcServer32Key, null)).Returns(param.ProfilerPath);
                public Action<Mock<IEnvironmentRepository>, ParameterForHasBeenInstalled> SetupExistsFile =
                    (m, param) => m.Setup(_ => _.ExistsFile(param.ProfilerPath)).Returns(param.ProfilerExistence);
                public Action<Mock<IEnvironmentRepository>, ParameterForHasBeenInstalled> SetupGetFileDescription =
                    (m, param) => m.Setup(_ => _.GetFileDescription(param.ProfilerPath)).Returns(param.ProfilerFileDescription);

                public static readonly Action<Mock<IEnvironmentRepository>, ParameterForHasBeenInstalled> DoesNotCall = (m, param) => { };
            }

            static Mock<IEnvironmentRepository> MockEnvironmentRepositoryForHasBeenInstalled(ProfilerLocation[] profLocs, ParameterForHasBeenInstalled[] @params)
            {
                var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                foreach (var param in @params)
                {
                    param.SetupOpenRegistryBaseKey(m, param);
                    param.SetupOpenRegistrySubKey(m, param);
                    param.SetupGetRegistryValue(m, param);
                    param.SetupExistsFile(m, param);
                    param.SetupGetFileDescription(m, param);
                }
                return m;
            }
            #endregion
        }

        [Test]
        public void HasBeenInstalled_should_raise_the_checking_event_even_if_profiler_has_not_registered_yet()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var machinePreq = new MachinePrerequisite("2.0.0");
            var profLocs = new[] { new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()) };
            {
                var m = new Mock<IEnvironmentRepository>(MockBehavior.Strict);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                m.Setup(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLocs[0].RegistryView)).Returns(DummyX86ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(DummyX86ClassesRootKey, ProfilerLocation.InprocServer32Path)).Returns(default(RegistryKey));
                fixture.Inject(m);
            }
            {
                var m = new Mock<Action<ProfilerLocation>>(MockBehavior.Strict);
                m.Setup(_ => _(profLocs[0]));
                fixture.Inject(m);
                machinePreq.ProfilerStatusChecking += m.Object;
            }

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.HasBeenInstalled(machinePreq);


            // Assert
            fixture.Freeze<Mock<Action<ProfilerLocation>>>().VerifyAll();
        }

        [Test]
        public void HasBeenInstalled_should_throw_ArgumentNullException_if_null_is_passed()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var mwInstllr = fixture.NewMachineWideInstaller();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => mwInstllr.HasBeenInstalled(null));
        }



        [Test]
        public void Install_should_skip_installation_if_it_has_been_already_installed()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs = new[] { new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()) };
            {
                var m = new Mock<IEnvironmentRepository>();
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                m.Setup(_ => _.OpenRegistryBaseKey(It.IsAny<RegistryHive>(), It.IsAny<RegistryView>())).Returns(DummyX86ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(It.IsAny<RegistryKey>(), It.IsAny<string>())).Returns(DummyX86InProcServer32Key);
                m.Setup(_ => _.GetRegistryValue(It.IsAny<RegistryKey>(), It.IsAny<string>())).Returns(fixture.Create<string>());
                m.Setup(_ => _.ExistsFile(It.IsAny<string>())).Returns(true);
                m.Setup(_ => _.GetFileDescription(It.IsAny<string>())).Returns("Prig Profiler 2.0.0 Type Library");
                fixture.Inject(m);
            }
            fixture.Inject(new Mock<INuGetExecutor>(MockBehavior.Strict));
            fixture.Inject(new Mock<IRegsvr32Executor>(MockBehavior.Strict));
            fixture.Inject(new Mock<IPrigExecutor>(MockBehavior.Strict));

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            fixture.Freeze<Mock<IEnvironmentRepository>>().VerifyAll();
        }

        [Test]
        public void Install_should_create_Nupkg()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var toolsPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools";
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).Returns(toolsPath);
            }
            {
                var nuspec = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools\NuGet\Prig.nuspec";
                var outputDirectory = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools";
                var m = fixture.Freeze<Mock<INuGetExecutor>>();
                m.Setup(_ => _.StartPacking(nuspec, outputDirectory)).Returns(fixture.Create<string>());
            }

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            fixture.Freeze<Mock<INuGetExecutor>>().VerifyAll();
        }

        [Test]
        public void Install_should_raise_the_event_to_create_Nupkg_before_and_after()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
            }
            var stdout = fixture.Create<string>();
            {
                var m = fixture.Freeze<Mock<INuGetExecutor>>();
                m.Setup(_ => _.StartPacking(It.IsAny<string>(), It.IsAny<string>())).Returns(stdout);
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.NuGetPackageCreating += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _("Prig"))).Object;
            mwInstl.NuGetPackageCreated += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(stdout))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_register_NuGet_source()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var toolsPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools";
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).Returns(toolsPath);
            }
            {
                var name = @"Prig Source";
                var source = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools";
                var m = fixture.Freeze<Mock<INuGetExecutor>>();
                m.Setup(_ => _.StartSourcing(name, source)).Returns(fixture.Create<string>());
            }

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            fixture.Freeze<Mock<INuGetExecutor>>().VerifyAll();
        }

        [Test]
        public void Install_should_raise_the_event_to_register_NuGet_source_before_and_after()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var toolsPath = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld\tools";
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).Returns(toolsPath);
            }
            var stdout = fixture.Create<string>();
            {
                var m = fixture.Freeze<Mock<INuGetExecutor>>();
                m.Setup(_ => _.StartSourcing(It.IsAny<string>(), It.IsAny<string>())).Returns(stdout);
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.NuGetSourceRegistering += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _("Prig Source", toolsPath))).Object;
            mwInstl.NuGetSourceRegistered += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(stdout))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_register_environment_variables()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var pkgDir = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld";
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetPackageFolder()).Returns(pkgDir);
                m.Setup(_ => _.GetPackageFolderKey()).ReturnsUsingFixture(fixture);
                var variableValue = pkgDir;
                m.Setup(_ => _.SetPackageFolder(variableValue));
            }

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            fixture.Freeze<Mock<IEnvironmentRepository>>().VerifyAll();
        }

        [Test]
        public void Install_should_raise_the_event_to_register_environment_variables_before_and_after()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var pkgDir = @"C:\Users\Akira\AppData\Local\Microsoft\VisualStudio\12.0\Extensions\jvtxkz2r.sld";
            var variableName = "URASANDESU_PRIG_PACKAGE_FOLDER";
            var variableValue = pkgDir;
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetPackageFolder()).Returns(pkgDir);
                m.Setup(_ => _.GetPackageFolderKey()).Returns(variableName);
                m.Setup(_ => _.SetPackageFolder(variableValue));
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.EnvironmentVariableRegistering += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _(variableName, variableValue))).Object;
            mwInstl.EnvironmentVariableRegistered += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _(variableName, variableValue))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_register_profiler()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs =
                new[] 
                { 
                    new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                    new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                };
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                m.Setup(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLocs[0].RegistryView)).Returns(DummyX86ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(DummyX86ClassesRootKey, ProfilerLocation.InprocServer32Path)).Returns(DummyX86InProcServer32Key);
                m.Setup(_ => _.GetRegistryValue(DummyX86InProcServer32Key, null)).Returns(fixture.Create<string>());
            }
            {
                var m = fixture.Freeze<Mock<IRegsvr32Executor>>();
                m.Setup(_ => _.StartInstalling(profLocs[0].PathOfInstalling)).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.StartInstalling(profLocs[1].PathOfInstalling)).ReturnsUsingFixture(fixture);
            }

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            fixture.Freeze<Mock<IRegsvr32Executor>>().VerifyAll();
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Verify(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLocs[0].RegistryView), Times.Once());
                m.Verify(_ => _.OpenRegistrySubKey(DummyX86ClassesRootKey, ProfilerLocation.InprocServer32Path), Times.Once());
            }
        }

        [Test]
        public void Install_should_not_register_profiler_if_profiler_locations_are_null()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs = default(ProfilerLocation[]);
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
            }
            fixture.Inject(new Mock<IRegsvr32Executor>());

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            {
                var m = fixture.Freeze<Mock<IRegsvr32Executor>>();
                m.Verify(_ => _.StartInstalling(It.IsAny<string>()), Times.Never());
            }
        }

        [Test]
        public void Install_should_not_register_profiler_if_profiler_locations_are_empty()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs = new ProfilerLocation[0];
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
            }
            fixture.Inject(new Mock<IRegsvr32Executor>());

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            {
                var m = fixture.Freeze<Mock<IRegsvr32Executor>>();
                m.Verify(_ => _.StartInstalling(It.IsAny<string>()), Times.Never());
            }
        }

        [Test]
        public void Install_should_raise_the_event_to_register_profiler_before_and_after()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs =
                new[] 
                { 
                    new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()), 
                    new ProfilerLocation(RegistryView.Registry64, fixture.Create<string>()) 
                };
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                m.Setup(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLocs[0].RegistryView)).Returns(DummyX86ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(DummyX86ClassesRootKey, ProfilerLocation.InprocServer32Path)).Returns(DummyX86InProcServer32Key);
                m.Setup(_ => _.GetRegistryValue(DummyX86InProcServer32Key, null)).Returns(fixture.Create<string>());
                m.Setup(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLocs[1].RegistryView)).Returns(DummyX64ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(DummyX64ClassesRootKey, ProfilerLocation.InprocServer32Path)).Returns(DummyX64InProcServer32Key);
                m.Setup(_ => _.GetRegistryValue(DummyX64InProcServer32Key, null)).Returns(fixture.Create<string>());
            }
            var stdouts = fixture.CreateMany<string>(2).ToArray();
            {
                var m = fixture.Freeze<Mock<IRegsvr32Executor>>();
                m.Setup(_ => _.StartInstalling(profLocs[0].PathOfInstalling)).Returns(stdouts[0]);
                m.Setup(_ => _.StartInstalling(profLocs[1].PathOfInstalling)).Returns(stdouts[1]);
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order1 = new MockOrder();
            var order2 = new MockOrder();
            mwInstl.ProfilerRegistering += mocks.Create<Action<ProfilerLocation>>().
                                               InOrder(order1, m => m.Setup(_ => _(profLocs[0]))).
                                               InOrder(order2, m => m.Setup(_ => _(profLocs[1]))).Object;
            mwInstl.ProfilerRegistered += mocks.Create<Action<string>>().
                                              InOrder(order1, m => m.Setup(_ => _(stdouts[0]))).
                                              InOrder(order2, m => m.Setup(_ => _(stdouts[1]))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_install_the_newest_TestWindow_as_default_source()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
            }
            {
                var m = fixture.Freeze<Mock<IPrigExecutor>>();
                m.Setup(_ => _.StartInstalling("TestWindow", It.IsRegex(@"C:\\Program Files \(x86\)\\Microsoft Visual Studio \d+\.\d+\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow"))).ReturnsUsingFixture(fixture);
            }

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            fixture.Freeze<Mock<IPrigExecutor>>().VerifyAll();
        }

        [Test]
        public void Install_should_raise_the_event_to_install_the_newest_TestWindow_before_and_after()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
            }
            var stdout = fixture.Create<string>();
            {
                var m = fixture.Freeze<Mock<IPrigExecutor>>();
                m.Setup(_ => _.StartInstalling(It.IsAny<string>(), It.IsAny<string>())).Returns(stdout);
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.DefaultSourceInstalling += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _("TestWindow", It.IsRegex(@"C:\\Program Files \(x86\)\\Microsoft Visual Studio \d+\.\d+\\Common7\\IDE\\CommonExtensions\\Microsoft\\TestWindow")))).Object;
            mwInstl.DefaultSourceInstalled += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(stdout))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_call_installation_steps_by_a_fixed_sequence()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs = new[] { new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()) };
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                m.Setup(_ => _.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLocs[0].RegistryView)).Returns(DummyX86ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(DummyX86ClassesRootKey, ProfilerLocation.InprocServer32Path)).Returns(DummyX86InProcServer32Key);
                m.Setup(_ => _.GetRegistryValue(DummyX86InProcServer32Key, null)).Returns(fixture.Create<string>());
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.Preparing += mocks.InOrder<Action>(order, m => m.Setup(_ => _())).Object;
            mwInstl.ProfilerStatusChecking += mocks.InOrder<Action<ProfilerLocation>>(order, m => m.Setup(_ => _(It.IsAny<ProfilerLocation>()))).Object;
            mwInstl.NuGetPackageCreating += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(It.IsAny<string>()))).Object;
            mwInstl.NuGetPackageCreated += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(It.IsAny<string>()))).Object;
            mwInstl.NuGetSourceRegistering += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _(It.IsAny<string>(), It.IsAny<string>()))).Object;
            mwInstl.NuGetSourceRegistered += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(It.IsAny<string>()))).Object;
            mwInstl.EnvironmentVariableRegistering += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _(It.IsAny<string>(), It.IsAny<string>()))).Object;
            mwInstl.EnvironmentVariableRegistered += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _(It.IsAny<string>(), It.IsAny<string>()))).Object;
            mwInstl.ProfilerRegistering += mocks.InOrder<Action<ProfilerLocation>>(order, m => m.Setup(_ => _(It.IsAny<ProfilerLocation>()))).Object;
            mwInstl.ProfilerRegistered += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(It.IsAny<string>()))).Object;
            mwInstl.DefaultSourceInstalling += mocks.InOrder<Action<string, string>>(order, m => m.Setup(_ => _(It.IsAny<string>(), It.IsAny<string>()))).Object;
            mwInstl.DefaultSourceInstalled += mocks.InOrder<Action<string>>(order, m => m.Setup(_ => _(It.IsAny<string>()))).Object;
            mwInstl.Completed += mocks.InOrder<Action<MachineWideProcessResults>>(order, m => m.Setup(_ => _(It.IsAny<MachineWideProcessResults>()))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_raise_the_events_for_the_installation_steps_if_skipped()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            var profLocs = new[] { new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()) };
            {
                var m = new Mock<IEnvironmentRepository>();
                m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
                m.Setup(_ => _.OpenRegistryBaseKey(It.IsAny<RegistryHive>(), It.IsAny<RegistryView>())).Returns(DummyX86ClassesRootKey);
                m.Setup(_ => _.OpenRegistrySubKey(It.IsAny<RegistryKey>(), It.IsAny<string>())).Returns(DummyX86InProcServer32Key);
                m.Setup(_ => _.GetRegistryValue(It.IsAny<RegistryKey>(), It.IsAny<string>())).Returns(fixture.Create<string>());
                m.Setup(_ => _.ExistsFile(It.IsAny<string>())).Returns(true);
                m.Setup(_ => _.GetFileDescription(It.IsAny<string>())).Returns("Prig Profiler 2.0.0 Type Library");
                fixture.Inject(m);
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.Completed += mocks.InOrder<Action<MachineWideProcessResults>>(order, m => m.Setup(_ => _(MachineWideProcessResults.Skipped))).Object;


            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Install_should_raise_the_events_for_the_installation_steps_if_completed()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var mwInstl = new MachineWideInstallation("2.0.0");
            {
                var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
                m.Setup(_ => _.GetToolsPath()).ReturnsUsingFixture(fixture);
            }
            var mocks = new MockRepository(MockBehavior.Strict);
            var order = new MockOrder();
            mwInstl.Completed += mocks.InOrder<Action<MachineWideProcessResults>>(order, m => m.Setup(_ => _(MachineWideProcessResults.Completed))).Object;

            var mwInstllr = fixture.NewMachineWideInstaller();


            // Act
            mwInstllr.Install(mwInstl);


            // Assert
            mocks.VerifyAll();
        }
    }
}

namespace Test.Urasandesu.Prig.VSPackage.Mixins.Ploeh.AutoFixture
{
    static partial class IFixtureMixin
    {
        public static MachineWideInstaller NewMachineWideInstaller(this IFixture fixture)
        {
            var mwInstllr = new MachineWideInstaller();
            mwInstllr.EnvironmentRepository = fixture.Freeze<IEnvironmentRepository>();
            mwInstllr.NuGetExecutor = fixture.Freeze<INuGetExecutor>();
            mwInstllr.Regsvr32Executor = fixture.Freeze<IRegsvr32Executor>();
            mwInstllr.PrigExecutor = fixture.Freeze<IPrigExecutor>();
            return mwInstllr;
        }
    }
}

namespace Test.Urasandesu.Prig.VSPackage.Mixins.Moq
{
    // `MockSequence` does not support the combination with same type and different parameter like the following case. Also, it makes `VerifyAll` function unavailable.
    // See also, [c# - Using Moq to verify calls are made in the correct order - Stack Overflow](http://stackoverflow.com/questions/10602264/using-moq-to-verify-calls-are-made-in-the-correct-order).
    class MockOrder
    {
        public int Expected { get; internal set; }
        public int Actual { get; internal set; }
    }

    static class MockRepositoryMixin
    {
        public static Mock<TMock> InOrder<TMock>(this MockRepository repo, MockOrder order, Func<Mock<TMock>, ISetup<TMock>> setup) where TMock : class
        {
            return repo.Create<TMock>().InOrder(order, setup);
        }
    }

    static class MockMixin
    {
        public static Mock<TMock> InOrder<TMock>(this Mock<TMock> mock, MockOrder order, Func<Mock<TMock>, ISetup<TMock>> setup) where TMock : class
        {
            var expected = order.Expected++;
            setup(mock).Callback(() => Assert.AreEqual(expected, order.Actual++, "Check the call order."));
            return mock;
        }
    }
}