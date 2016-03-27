/* 
 * File: ULProcessMixinTest.cs
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


using Moq;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Prig;
using System.IO;
using System.Prig;
using Test.program1.TestUtilities.Mixins.System.Diagnostics.Prig;
using Test.program1.TestUtilities.Mixins.System.Prig;
using UntestableLibrary;
using Urasandesu.Moq.Prig;
using Urasandesu.Moq.Prig.Mixins.Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class ULProcessMixinTest
    {
        [Test]
        public void RestartCurrentProcess_should_start_new_process_and_close_current_process()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironment.GetCommandLineArgs().BodyBy(ms).Expect(_ => _(), Times.Once()).Returns(new[] { "file name" });
                PEnvironment.CurrentDirectoryGet().BodyBy(ms).Expect(_ => _(), Times.Once()).Returns("current directory");

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(curProcMainMod.FileNameGet).Expect(_ => _(curProcMainMod), Times.Once()).Returns("file path")
                    );
                
                var curProc = new PProxyProcess();
                curProc.StartInfoGet().BodyBy(ms).Expect(_ => _(curProc), Times.Once()).Returns(new ProcessStartInfo() { UserName = "urasandesu" });
                curProc.MainModuleGet().BodyBy(ms).Expect(_ => _(curProc), Times.Once()).Returns(curProcMainMod);
                curProc.CloseMainWindow().BodyBy(ms).Expect(_ => _(curProc), Times.Once()).Returns(true);

                PProcess.GetCurrentProcess().BodyBy(ms).Expect(_ => _(), Times.Once()).Returns(curProc);
                PProcess.StartProcessStartInfo().BodyBy(ms).Expect(_ => _(It.Is<ProcessStartInfo>(x => 
                    x.UserName == "urasandesu" && 
                    x.UseShellExecute == true && 
                    x.WorkingDirectory == "current directory" &&
                    x.FileName == "file path"
                )), Times.Once()).Returns(new PProxyProcess());


                // Act
                var result = ULProcessMixin.RestartCurrentProcess();


                // Assert
                Assert.IsTrue(result);
                ms.Verify();
            }
        }

        [Test]
        public void RestartCurrentProcess_should_propagate_command_line_arguments_if_they_are_specified()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironmentMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PEnvironment.GetCommandLineArgs).Setup(_ => _()).Returns(new[] { Guid.NewGuid().ToString(), "arg ument1", "argume nt2" })
                    );

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms);

                var curProc = new PProxyProcess();
                curProc.AutoBodyBy(ms);
                curProc.MainModuleGet().Body = @this => curProcMainMod;

                PProcessMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PProcess.StartProcessStartInfo).Setup(_ => _(It.Is<ProcessStartInfo>(x => 
                            x.Arguments == "\"arg ument1\" \"argume nt2\""
                        ))).Returns(new PProxyProcess())
                    );
                PProcess.GetCurrentProcess().Body = () => curProc;


                // Act
                var result = ULProcessMixin.RestartCurrentProcess();


                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void RestartCurrentProcess_should_organize_command_line_arguments_if_they_contain_double_quote()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironmentMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PEnvironment.GetCommandLineArgs).Setup(_ => _()).Returns(new[] { Guid.NewGuid().ToString(), "a a", "\"b\"bb", "c" })
                    );

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms);

                var curProc = new PProxyProcess();
                curProc.AutoBodyBy(ms);
                curProc.MainModuleGet().Body = @this => curProcMainMod;

                PProcessMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PProcess.StartProcessStartInfo).Setup(_ => _(It.Is<ProcessStartInfo>(x =>
                            x.Arguments == "\"a a\" \"\\\"b\\\"bb\" \"c\""
                        ))).Returns(new PProxyProcess())
                    );
                PProcess.GetCurrentProcess().Body = () => curProc;


                // Act
                var result = ULProcessMixin.RestartCurrentProcess();


                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void RestartCurrentProcess_should_return_false_if_user_cancelled_starting_process()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironmentMixin.AutoBodyBy(ms);

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms);

                var curProc = new PProxyProcess();
                curProc.AutoBodyBy(ms);
                curProc.MainModuleGet().Body = @this => curProcMainMod;

                PProcessMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PProcess.StartProcessStartInfo).Setup(_ => _(It.IsAny<ProcessStartInfo>())).Throws(new Win32Exception(1223))
                    );
                PProcess.GetCurrentProcess().Body = () => curProc;


                // Act
                var result = ULProcessMixin.RestartCurrentProcess();


                // Assert
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void RestartCurrentProcess_should_rethrow_Win32Exception_except_for_user_cancel()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironmentMixin.AutoBodyBy(ms);

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms);

                var curProc = new PProxyProcess();
                curProc.AutoBodyBy(ms);
                curProc.MainModuleGet().Body = @this => curProcMainMod;

                PProcessMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PProcess.StartProcessStartInfo).Setup(_ => _(It.IsAny<ProcessStartInfo>())).Throws(new Win32Exception(1224))
                    );
                PProcess.GetCurrentProcess().Body = () => curProc;


                // Act, Assert
                Assert.Throws<Win32Exception>(() => ULProcessMixin.RestartCurrentProcess());
            }
        }

        [Test]
        public void RestartCurrentProcess_should_throw_exception_as_it_is_except_for_Win32Exception()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironmentMixin.AutoBodyBy(ms);

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms);

                var curProc = new PProxyProcess();
                curProc.AutoBodyBy(ms);
                curProc.MainModuleGet().Body = @this => curProcMainMod;

                PProcessMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PProcess.StartProcessStartInfo).Setup(_ => _(It.IsAny<ProcessStartInfo>())).Throws(new FileNotFoundException())
                    );
                PProcess.GetCurrentProcess().Body = () => curProc;


                // Act, Assert
                Assert.Throws<FileNotFoundException>(() => ULProcessMixin.RestartCurrentProcess());
            }
        }

        
        
        [Test]
        public void RestartCurrentProcessWith_should_invoke_additionalSetup()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                var ms = new MockStorage(MockBehavior.Strict);
                PEnvironmentMixin.AutoBodyBy(ms);

                var curProcMainMod = new PProxyProcessModule();
                curProcMainMod.AutoBodyBy(ms);

                var procStartInfo = new ProcessStartInfo();
                var curProc = new PProxyProcess();
                curProc.AutoBodyBy(ms);
                curProc.StartInfoGet().Body = @this => procStartInfo;
                curProc.MainModuleGet().Body = @this => curProcMainMod;

                PProcessMixin.AutoBodyBy(ms).
                    Customize(m => m.
                        Do(PProcess.StartProcessStartInfo).Expect(_ => _(It.Is<ProcessStartInfo>(x =>
                            x.Verb == "runas"
                        )), Times.Once()).Returns(new PProxyProcess())
                    );
                PProcess.GetCurrentProcess().Body = () => curProc;

                var additionalSetup = ms.Create<Action<ProcessStartInfo>>().Object;
                Mock.Get(additionalSetup).Setup(_ => _(It.IsAny<ProcessStartInfo>())).Callback(() => procStartInfo.Verb = "runas");


                // Act
                var result = ULProcessMixin.RestartCurrentProcessWith(additionalSetup);


                // Assert
                Assert.IsTrue(result);
                ms.Verify();
            }
        }
    }
}
