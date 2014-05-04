# Prig
## SYNOPSIS
Prig is a lightweight framework for test indirections in .NET Framework.



## DESCRIPTION
Prig(PRototyping jIG) is a framework that generates a Test Double like Microsoft Fakes/Typemock Isolator/Telerik JustMock based on Unmanaged Profiler APIs.
This framework enables that any methods are replaced with mocks. For example, a static property, a private method, a non-virtual member and so on.



## STATUS
As of May 4, 2014, Prig does not work completely. However, we steadily continue to develop at the private repository. This framework will come out within the year if everything goes well.



## QUICK TOUR
Let's say you want to test the following code: 
```cs
using System;

namespace program1.MyLibrary
{
    public static class LifeInfo
    {
        public static bool IsNowLunchBreak()
        {
            var now = DateTime.Now;
            return 12 <= now.Hour && now.Hour < 13;
        }
    }
}
```
You probably can't test this code, because ```DateTime.Now``` returns the value that depends on an external environment. For to make be testable, you should replace ```DateTime.Now``` to the Test Double that returns the fake information. If you use Prig, it will enable you to generate a Test Double by the following operation without any editing the product code:

First, you make the following stub into the assembly that named ```'dll name of the assembly that contains dependency' + '.' + 'runtime version string' + '.' + 'assembly version string' + '.' + 'processor architecture' + '.Prig.dll'```(e.g. DateTime.Now is in mscorlib.dll under .NET v2.0.50727 runtime, x86 processor architecture environment ==>> you should make the stub into mscorlib.v2.0.50727.v2.0.0.0.x86.Prig.dll).
```cs
using Urasandesu.Prig.Framework;

// You should grant the metadata token that you want to replace to the stub assembly with IndirectableAttribute.
// The getter method of DateTime.Now is the static method that named get_Now, 
// so you should specify its token 0x060002D2(*).
// [*] You can find the metadata token in the decompiled assembly, or by referring MethodInfo.MetadataToken. 
//     If using the ildasm, the menu View->MetaInfo->Show, or simply Ctrl+M shortcut is very convenience.
[assembly: Indirectable(PDateTime.TokenOfNowGet_Func_DateTime)]

namespace System.Prig
{
    public class PDateTime
    {
        internal const int TokenOfNowGet_Func_DateTime = 0x060002D2;

        public static class NowGet
        {
            // Please select the delegate that has same signature as target method.
            // You can select the delegate that have IndirectionDelegateAttribute(see also IndirectionDelegates.cs).
            public static IndirectionFunc<DateTime> Body
            {
                set
                {
                    // Register the delegate on the key that consists of the following: 
                    // - Full name of the assembly that contains target method.
                    // - Metadata token of the target method.
                    var info = new IndirectionInfo();
                    info.AssemblyName = typeof(DateTime).Assembly.FullName;
                    info.Token = TokenOfNowGet_Func_DateTime;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
```
Make a new class library for unit test, and add the stub dll reference to it.
In the test code, it become testable through the use of the stub and the replacement to Test Double that returns the fake information.
```cs
using NUnit.Framework;
using program1.MyLibrary;
using System;
using System.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.MyLibraryTest
{
    [TestFixture]
    public class LifeInfoTest
    {
        [Test]
        public void IsNowLunchBreak_should_return_true_when_12_oclock()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.NowGet.Body = () => new DateTime(2013, 12, 13, 12, 00, 00);

                // Act
                var result = LifeInfo.IsLunchBreak();

                // Assert
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void IsNowLunchBreak_should_return_false_when_13_oclock()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.NowGet.Body = () => new DateTime(2013, 12, 13, 13, 00, 00);

                // Act
                var result = LifeInfo.IsLunchBreak();

                // Assert
                Assert.IsFalse(result);
            }
        }
    }
}
```
To enable any profiler based mocking tool, you has to set the environment variables originally. Microsoft Fakes/Typemock Isolator/Telerik JustMock provides the small runner it required, it is true at Prig. So use prig.exe to run the test as follows: 
```dos
CMD x86>cd
C:\Users\User\Prig\Test.program1\bin\Release(.NET 3.5)\x86

CMD x86>"..\..\..\..\Release\x86\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe" -arguments "Test.program1.dll /domain=None"
NUnit-Console version 2.6.3.13283
Copyright (C) 2002-2012 Charlie Poole.
Copyright (C) 2002-2004 James W. Newkirk, Michael C. Two, Alexei A. Vorontsov.
Copyright (C) 2000-2002 Philip Craig.
All Rights Reserved.

Runtime Environment -
   OS Version: Microsoft Windows NT 6.2.9200.0
  CLR Version: 2.0.50727.8000 ( Net 3.5 )

ProcessModel: Default    DomainUsage: None
Execution Runtime: net-3.5
..........
Tests run: 10, Errors: 0, Failures: 0, Inconclusive: 0, Time: 1.44990184322627 seconds
  Not run: 0, Invalid: 0, Ignored: 0, Skipped: 0


CMD x86>
```
If tests have been created, you can refactor illimitably! Prig helps the test that depends on an untestable library get trig. Enjoy your trip!!



# INSTALLATION
## DEPENDENCY
To build this project needs the following dependencies: 
* [Visual Studio 2013(more than Professional Edition)](http://www.visualstudio.com/)
* [Boost 1.55.0](http://www.boost.org/)  
Extract to C:\boost_1_55_0, and will build with the following options(x86 and x64 libs are required):
```dos
CMD boost_1_55_0>cd
C:\boost_1_55_0

CMD boost_1_55_0>bootstrap.bat
Building Boost.Build engine

Bootstrapping is done. To build, run:

    .\b2

To adjust configuration, edit 'project-config.jam'.
Further information:
...

CMD boost_1_55_0>.\b2 link=static threading=multi variant=debug,release runtime-link=shared,static -j 4

Building the Boost C++ Libraries.

Performing configuration checks
...

CMD boost_1_55_0>.\b2 link=static threading=multi variant=debug,release runtime-link=shared,static -j 4 --stagedir=.\stage\x64 address-model=64

Building the Boost C++ Libraries.

Performing configuration checks
...
```
* [NUnit 2.6.3.13283](http://www.nunit.org/)  
Install using with the installer(NUnit-2.6.3.msi).
* [Google Test 1.6](https://code.google.com/p/googletest/)  
Extract to C:\gtest-1.6.0, and upgrade C:\gtest-1.6.0\msvc\gtest.sln to Visual Studio 2013. Choose the `Build` menu, and open `Configuration Manager...`. On `Configuration Manager` dialog box, in the `Active Solution Platform` drop-down list, select the `<New...>` option. After the `New Solution Platform` dialog box is opened, in the `Type or select the new platform` drop-down list, select a 64-bit platform. Then build all(Debug/Release) configurations.



## BUILD
1. Run Visual Studio as Administrator, and open Prig.sln(This sln contains some ATL projects, so the build process will modify registry).
2. According to the version of the product to use, change the solution configuration and the solution platform and build.
3. The results are outputted to ```$(SolutionDir)$(Configuration)\$(PlatformTarget)\```.



## REGISTRATION
Run Developer Command Prompt for VS2013 as Administrator, and register dlls that were outputted to ```$(SolutionDir)$(Configuration)\$(PlatformTarget)\``` to registry and GAC as follows(these are the examples for x86/.NET 3.5, but also another environments are in the same manner): 
```dos
CMD x86>cd
C:\Users\User\Prig\Release(.NET 3.5)\x86

CMD x86>regsvr32 /i Urasandesu.Prig.dll

CMD x86>gacutil /i Urasandesu.NAnonym.dll
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.

Assembly successfully added to the cache

CMD x86>gacutil /i Urasandesu.Prig.Framework.dll
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.

Assembly successfully added to the cache

CMD x86>
```



## UNREGISTRATION
Unregistration operation is similar in the registration. Run Developer Command Prompt for VS2013 as Administrator and execute the following commands: 
```dos
CMD x86>cd
C:\Users\User\Prig\Release(.NET 3.5)\x86

CMD x86>regsvr32 /u Urasandesu.Prig.dll

CMD x86>gacutil /u "Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=x86"
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.


Assembly: Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=x86
Uninstalled: Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=x86
Number of items uninstalled = 1
Number of failures = 0

CMD x86>gacutil /u "Urasandesu.NAnonym, Version=0.2.0.0, Culture=neutral, PublicKeyToken=ce9e95b04334d5fb, processorArchitecture=MSIL"
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.


Assembly: Urasandesu.NAnonym, Version=0.2.0.0, Culture=neutral, PublicKeyToken=ce9e95b04334d5fb, processorArchitecture=MSIL
Uninstalled: Urasandesu.NAnonym, Version=0.2.0.0, Culture=neutral, PublicKeyToken=ce9e95b04334d5fb, processorArchitecture=MSIL
Number of items uninstalled = 1
Number of failures = 0

CMD x86>
```



