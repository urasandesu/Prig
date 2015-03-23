# Prig
## SYNOPSIS
Prig is a lightweight framework for test indirections in .NET Framework.



## DESCRIPTION
Prig(PRototyping jIG) is a framework that generates a [Test Double](http://martinfowler.com/bliki/TestDouble.html) like [Microsoft Fakes](http://msdn.microsoft.com/en-us/library/hh549175.aspx)/[Typemock Isolator](http://www.typemock.com/isolator-product-page)/[Telerik JustMock](http://www.telerik.com/products/mocking.aspx) based on Unmanaged Profiler APIs.
This framework enables that any methods are replaced with mocks. For example, a static property, a private method, a non-virtual member and so on.



## STATUS
As of MM dd, yyyy, Released V2.0.0.



## QUICK TOUR
Let's say you want to test the following code: 
```cs
using System;

namespace ConsoleApplication
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
You probably can't test this code, because `DateTime.Now` returns the value that depends on an external environment. To make be testable, you should replace `DateTime.Now` to the Test Double that returns the fake information. If you use Prig, it will enable you to generate a Test Double by the following steps without any editing the product code:


### Step 1: Install From Chocolatey
Install Chocolatey in accordance with [the top page](https://chocolatey.org/). Then, run Developer Command Prompt for VS2013 as Administrator, execute the following command: 
```dos
CMD C:\> choco install prig -Pre -y
```


### Step 2: Add Stub Settings
Run Visual Studio 2013(Community or more) as Administrator, add test project(e.g. `ConsoleApplicationTest`). Then, right click `References` and select `Add Prig Assembly for mscorlib`:  
![Add Stub Settings](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%202%20Add%20Stub%20Settings.png)


### Step 3: Modify Stub Settings
You can find the indirection stub setting file `<assembly name>.<runtime version>.v<assembly version>.prig` in the project(in this case, it is `mscorlib.v4.0.30319.v4.0.0.0.prig`). So, right click the file and select `Edit Prig Indirection Settings`:  
![Edit Prig Indirection Settings](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%203%20Modify%20Stub%20Settings%2001.png)


Then, Prig setup session will start:  
![Prig Setup Session](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%203%20Modify%20Stub%20Settings%2002.png)


```powershell
Welcome to Prig setup session!!


You can add the indirection settings from here. In this session, you can use `$ReferencedAssemblies` that contains all
referenced assemblies information of current project. For example, if you want to get the indirection settings for all
members of the type `Foo` that belongs to the referenced assembly `UntestableLibrary`, the following commands will achi
eve it:

PS> $ReferencedAssemblies

FullName
--------
mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
MyLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null


PS> padd -ra $ReferencedAssemblies[-1]
PS> $ReferencedAssemblies[-1].GetTypes() | ? { $_.Name -eq 'Foo' } | pfind | pget | clip
PS> exit   # Then, paste the content on the clipboard to the stub setting file(e.g. `UntestableLibrary.v4.0.30319.v1.0.
0.0.prig`).



See also the command's help `padd`, `pfind` and `pget`.



Current Project: ConsoleApplicationTest
WARNING: Change the Current Project from `Default Project: ` on the Package Manager Console if it isn't what you want.


-EditorialInclude parameter is specified. You can also use the global variable $TargetReferencedAssembly in addition to
 $ReferencedAssemblies. Currently $TargetReferencedAssembly is:

FullName
--------
mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089




PS ConsoleApplication>

```

Now, we want to get the indirection setting for `DateTime.Now`. In this case, execute the following commands and copy it to the clipboard: 
```powershell
PS ConsoleApplication> $TargetReferencedAssembly.GetTypes() | ? { $_.Name -eq 'datetime' } | pfind -m 'get_Now' | pget | clip
PS ConsoleApplication> exit
```

Exit the Prig setup session, and paste the copied information to the setting file:  
![Indirection Setting File](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%203%20Modify%20Stub%20Settings%2003.png)


```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  
  <configSections>
    <section name="prig" type="Urasandesu.Prig.Framework.PilotStubberConfiguration.PrigSection, Urasandesu.Prig.Framework" />
  </configSections>

  <prig>

    <stubs>
      <!-- PASTE HERE -->
      <add name="NowGet" alias="NowGet">
        <RuntimeMethodInfo xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns:x="http://www.w3.org/2001/XMLSchema" z:Id="1" z:FactoryType="MemberInfoSerializationHolder" z:Type="System.Reflection.MemberInfoSerializationHolder" z:Assembly="0" xmlns:z="http://schemas.microsoft.com/2003/10/Serialization/" xmlns="http://schemas.datacontract.org/2004/07/System.Reflection">
          <Name z:Id="2" z:Type="System.String" z:Assembly="0" xmlns="">get_Now</Name>
          <AssemblyName z:Id="3" z:Type="System.String" z:Assembly="0" xmlns="">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id="4" z:Type="System.String" z:Assembly="0" xmlns="">System.DateTime</ClassName>
          <Signature z:Id="5" z:Type="System.String" z:Assembly="0" xmlns="">System.DateTime get_Now()</Signature>
          <Signature2 z:Id="6" z:Type="System.String" z:Assembly="0" xmlns="">System.DateTime get_Now()</Signature2>
          <MemberType z:Id="7" z:Type="System.Int32" z:Assembly="0" xmlns="">8</MemberType>
          <GenericArguments i:nil="true" xmlns="" />
        </RuntimeMethodInfo>
      </add>
      <!-- PASTE HERE -->
    </stubs>
    
  </prig>

</configuration>
```

Were you able to build successfully? OK, now you're ready to test them.


### Step 4: Make Tests
In the test code, it becomes testable through the use of the stub and the replacement to Test Double that returns the fake information: 
```cs
using NUnit.Framework;
using ConsoleApplication;
using System;
using System.Prig;
using Urasandesu.Prig.Framework;

namespace ConsoleApplicationTest
{
    [TestFixture]
    public class LifeInfoTest
    {
        [Test]
        public void IsNowLunchBreak_should_return_false_when_11_oclock()
        {
            // `IndirectionsContext` can minimize the influence of the API replacement.
            using (new IndirectionsContext())
            {
                // Arrange
                // Replace `DateTime.Now` body. Hereafter, `DateTime.Now` will return only `2013/12/13 11:00:00`.
                PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 11, 00, 00);

                // Act
                var result = LifeInfo.IsNowLunchBreak();

                // Assert
                Assert.IsFalse(result);
            }
        }

        // In the same way, add the test case to cover other branches...
        [Test]
        public void IsNowLunchBreak_should_return_true_when_12_oclock()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 12, 00, 00);

                // Act
                var result = LifeInfo.IsNowLunchBreak();

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
                PDateTime.NowGet().Body = () => new DateTime(2013, 12, 13, 13, 00, 00);

                // Act
                var result = LifeInfo.IsNowLunchBreak();

                // Assert
                Assert.IsFalse(result);
            }
        }
    }
}
```


### Step 5: Install Test Adapter
Before running tests in Visual Studio Test Explorer, you have to install a Test Adapter. Currently, Prig supports the following Test Adapters: NUnit, MSTest, [xUnit.net](https://www.nuget.org/packages/xunit.runner.visualstudio/2.0.0-rc4-build1049). As the above described sample, let we use NUnit. Now, in the Package Manager Console, change the `Package source` to `Prig Source`, the `Default project` to `ConsoleApplicationTest` and execute the following command: 
```powershell
PM> Install-Package NUnitTestAdapterForPrig
```

**NOTE:** Unfortunately, you can't use official [NUnit Test Adapter](https://www.nuget.org/packages/NUnitTestAdapter/) because it doesn't support any configurations like prime NUnit which is supported, e.g. [NUnit Gui Runner's Settings](http://www.nunit.org/index.php?p=settingsDialog&r=2.6.3) and [NUnit-Console's Settings](http://www.nunit.org/index.php?p=consoleCommandLine&r=2.6.3).

After install, build the test project and select the menu `TEST` - `Windows` - `Test Explorer`. Then, you can find runnable tests in the Test Explorer.  
![Install Test Adapter 01](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%205%20Install%20Test%20Adapter%2001.png)

When Test Adapter was installed successfully, you can also modify the `Test Settings`. As the following image, change `Default Processor Architecture` to `x64` and uncheck `Keep Test Execution Engine Running`:  
![Install Test Adapter 02](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%205%20Install%20Test%20Adapter%2002.png)


### Step 6: Run Tests
In fact, to enable any profiler based mocking tool, you have to set the environment variables. Therefore, such libraries - Microsoft Fakes/Typemock Isolator/Telerik JustMock provide small runner to satisfy the requisition, also it is true at Prig. Select the menu `PRIG` - `Enable Test Adapter for ConsoleApplicationTest`:  
![Run Tests 01](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%206%20Run%20Tests%2001.png)

Then, execute `TEST` - `Run` - `All Tests`, you can get test results in the Test Explorer.  
![Run Tests 02](https://github.com/urasandesu/Prig.V2Docs/blob/master/Urasandesu.Prig.VSPackage/Resources/Step%206%20Run%20Tests%2002.png)


### Final Step: Refactoring and Get Trig Back!
If tests have been created, you can refactor illimitably! For example, you probably can find the result of refactoring as follows: 
```cs
using System;

namespace ConsoleApplication
{
    public static class LifeInfo
    {
        public static bool IsNowLunchBreak()
        {
            // 1. Add overload to isolate from external environment then call it from original method.
            return IsNowLunchBreak(DateTime.Now);
        }

        public static bool IsNowLunchBreak(DateTime now)
        {
            // 2. Also, I think the expression '12 <= now.Hour && now.Hour < 13' is too complex.
            //    Better this way, isn't it?
            return now.Hour == 12;
        }
        // 3. After refactoring, no longer need to use Prig, because you can test this overload.
    }
}
```
As just described, Prig helps the code that depends on an untestable library gets trig back. I guarantee you will enjoy your development again!!

For more information, see also [Prig's wiki](https://github.com/urasandesu/Prig.V2Docs/wiki).




# INSTALLATION FROM SOURCE CODE
## DEPENDENCY
To build this project needs the following dependencies: 
* [Visual Studio 2013(more than Professional Edition because it uses ATL)](http://www.visualstudio.com/)
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
* [Google Test 1.6](https://code.google.com/p/googletest/)  
Extract to C:\gtest-1.6.0, and upgrade C:\gtest-1.6.0\msvc\gtest.sln to Visual Studio 2013. Choose the `Build` menu, and open `Configuration Manager...`. On `Configuration Manager` dialog box, in the `Active Solution Platform` drop-down list, select the `<New...>` option. After the `New Solution Platform` dialog box is opened, in the `Type or select the new platform` drop-down list, select a 64-bit platform. Then build all(Debug/Release) configurations.
* [NUnit 2.6.3.13283](http://www.nunit.org/)  
Install using with the installer(NUnit-2.6.3.msi).
* [Modeling SDK for Microsoft Visual Studio 2013](http://www.microsoft.com/en-us/download/details.aspx?id=40754)  
Install using with the installer(VS_VmSdk.exe).
* [Microsoft Visual Studio 2013 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=40758)  
Install using with the installer(vssdk_full.exe).
* [NAnt](http://nant.sourceforge.net/)  
You can also install in accordance with [the help](http://nant.sourceforge.net/release/latest/help/introduction/installation.html), but the easiest way is using Chocolatey: `choco install nant`.



## BUILD
After preparing all dependencies, you can build this project in the following steps:

1. Run Visual Studio as Administrator, and open Prig.sln(This sln contains some ATL projects, so the build process will modify registry).
2. According to the version of the product to use, change the solution configuration and the solution platform and build.
3. The results are output to `$(SolutionDir)$(Configuration)\$(PlatformTarget)\`.



## REGISTRATION
Run Developer Command Prompt for VS2013 as Administrator, and register dlls that were output to `$(SolutionDir)$(Configuration)\$(PlatformTarget)\` to registry and GAC as follows(these are the examples for x86/.NET 3.5, but also another environments are in the same manner): 
```dos
CMD x86>cd
C:\Prig\Release\x86

CMD x86>regsvr32 /i Urasandesu.Prig.dll

CMD x86>cd "..\..\Release(.NET 3.5)\AnyCPU"

CMD AnyCPU>cd
C:\Prig\Release(.NET 3.5)\AnyCPU

CMD AnyCPU>gacutil /i Urasandesu.NAnonym.dll
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.

Assembly successfully added to the cache

CMD AnyCPU>gacutil /i Urasandesu.Prig.Framework.dll
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.

Assembly successfully added to the cache

CMD AnyCPU>
```



## UNREGISTRATION
Unregistration operation is similar in the registration. Run Developer Command Prompt for VS2013 as Administrator and execute the following commands: 
```dos
CMD x86>cd
C:\Prig\Release\x86

CMD x86>regsvr32 /u Urasandesu.Prig.dll

CMD x86>cd "..\..\Release(.NET 3.5)\AnyCPU"

CMD AnyCPU>cd
C:\Prig\Release(.NET 3.5)\AnyCPU

CMD AnyCPU>gacutil /u "Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=MSIL"
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.


Assembly: Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=MSIL
Uninstalled: Urasandesu.Prig.Framework, Version=0.1.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce, processorArchitecture=MSIL
Number of items uninstalled = 1
Number of failures = 0

CMD AnyCPU>gacutil /u "Urasandesu.NAnonym, Version=0.2.0.0, Culture=neutral, PublicKeyToken=ce9e95b04334d5fb, processorArchitecture=MSIL"
Microsoft (R) .NET Global Assembly Cache Utility.  Version 4.0.30319.33440
Copyright (c) Microsoft Corporation.  All rights reserved.


Assembly: Urasandesu.NAnonym, Version=0.2.0.0, Culture=neutral, PublicKeyToken=ce9e95b04334d5fb, processorArchitecture=MSIL
Uninstalled: Urasandesu.NAnonym, Version=0.2.0.0, Culture=neutral, PublicKeyToken=ce9e95b04334d5fb, processorArchitecture=MSIL
Number of items uninstalled = 1
Number of failures = 0

CMD AnyCPU>
```



