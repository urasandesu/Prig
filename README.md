# Prig: Open Source Alternative to Microsoft Fakes
![Prig](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/PrigPreviewImage.png)

Prig(PRototyping jIG) is a framework that generates a [Test Double](http://martinfowler.com/bliki/TestDouble.html) like [Microsoft Fakes](http://msdn.microsoft.com/en-us/library/hh549175.aspx)/[Typemock Isolator](http://www.typemock.com/isolator-product-page)/[Telerik JustMock](http://www.telerik.com/products/mocking.aspx) based on Unmanaged Profiler APIs.
This framework enables that any methods are replaced with mocks. For example, a static property, a private method, a non-virtual member and so on.



## STATUS
As of Dec 20, 2015, Released V2.1.0.



## QUICK TOUR
Let's say you want to test the following code: 
```cs
using System;

namespace QuickTour
{
    public class LifeInfo
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


### Step 1: Install From Visual Studio Gallery
Run Visual Studio 2013(Community or more, 2015 is also supported) as Administrator and choose `TOOLS` - `Extensions and Updates...`.  
![Extensions and Updates... menu](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%201%20Install%20From%20Visual%20Studio%20Gallery%2001.png)

Now in the Extensions and Updates window, take the following steps:

1. On the left side, ensure `Visual Studio Gallery` is selected under `Online`.
2. In the search box in the upper right corner, type `prig`.
3. Select the `Prig` package, and click `Download`.

![Extensions and Updates dialog](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%201%20Install%20From%20Visual%20Studio%20Gallery%2002.png)

**NOTE:** Prig requires PowerShell v3.0+. If you want to use Prig in Windows 7, please install [Windows Management Framework 3.0+](https://www.microsoft.com/en-us/download/details.aspx?id=34595) beforehand. [See also this issue](https://github.com/urasandesu/Prig/issues/41).

Once restart Visual Studio, you can find `PRIG` in the menu. Choose `PRIG` - `Register Prig (Needs Restarting)`.  
![Register Prig menu](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%201%20Install%20From%20Visual%20Studio%20Gallery%2003.png)

Finally restart Visual Studio then you are now ready.


### Step 2: Add Stub Settings
Add test project(e.g. `QuickTourTest`). Then, right click `References` and choose `Add Prig Assembly for mscorlib`:  
![Add Stub Settings](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%202%20Add%20Stub%20Settings.png)


### Step 3: Modify Stub Settings
You can find the [Stub Settings File](https://github.com/urasandesu/Prig/wiki/Cheat-Sheet#stub_settings_file) `<assembly name>.<runtime version>.v<assembly version>.prig` in the project(in this case, it is `mscorlib.v4.0.30319.v4.0.0.0.prig`). So, right click the file and choose `Edit Prig Indirection Settings`:  
![Edit Prig Indirection Settings](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%203%20Modify%20Stub%20Settings%2001.png)


Then, [Prig Setup Session](https://github.com/urasandesu/Prig/wiki/Cheat-Sheet#prig_setup_session) will start:  
![Prig Setup Session](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%203%20Modify%20Stub%20Settings%2002.png)


```powershell
Welcome to Prig Setup Session!!


You can add the Stub Settings File from here. In this session, you can use `$ReferencedAssemblies` that contains all
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
PS> exit   # Then, paste the content on the clipboard to the Stub Settings File(e.g. `UntestableLibrary.v4.0.30319.v1.0.
0.0.prig`).



See also the command's help `padd`, `pfind` and `pget`.



Current Project: QuickTourTest
WARNING: Change the Current Project from `Default Project: ` on the Package Manager Console if it isn't what you want.


-EditorialInclude parameter is specified. You can also use the global variable $TargetReferencedAssembly in addition to
 $ReferencedAssemblies. Currently $TargetReferencedAssembly is:

FullName
--------
mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089




PS 01.QuickTour>

```

Now, we want to get the indirection setting for `DateTime.Now`. In this case, execute the following commands and copy it to the clipboard: 
```powershell
PS 01.QuickTour> $TargetReferencedAssembly.GetTypes() | ? { $_.Name -eq 'datetime' } | pfind -m 'get_Now' | pget | clip
PS 01.QuickTour> exit
```

Exit the [Prig Setup Session](https://github.com/urasandesu/Prig/wiki/Cheat-Sheet#prig_setup_session), and paste the copied information to the [Stub Settings File](https://github.com/urasandesu/Prig/wiki/Cheat-Sheet#stub_settings_file):  
![Indirection Setting File](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%203%20Modify%20Stub%20Settings%2003.png)


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
using QuickTour;
using System;
using System.Prig;
using Urasandesu.Prig.Framework;

namespace QuickTourTest
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
Before running tests in Visual Studio Test Explorer, you have to install a Test Adapter. Currently, Prig supports the following Test Adapters: NUnit, MSTest, [xUnit.net](https://www.nuget.org/packages/xunit.runner.visualstudio/). As the above described sample, let we use NUnit. Now, in the Package Manager Console, change the `Package source` to `Prig Source`, the `Default project` to `QuickTourTest` and execute the following command: 
```powershell
PM> Install-Package NUnitTestAdapterForPrig
```

**NOTE:** Unfortunately, you can't use official [NUnit Test Adapter](https://www.nuget.org/packages/NUnitTestAdapter/) because it doesn't support any configurations like prime NUnit which is supported, e.g. [NUnit Gui Runner's Settings](http://www.nunit.org/index.php?p=settingsDialog&r=2.6.4) and [NUnit-Console's Settings](http://www.nunit.org/index.php?p=consoleCommandLine&r=2.6.4).

After install, build the test project and choose the menu `TEST` - `Windows` - `Test Explorer`. Then, you can find runnable tests in the Test Explorer.  
![Install Test Adapter 01](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%205%20Install%20Test%20Adapter%2001.png)

When Test Adapter was installed successfully, you can also modify the `Test Settings`. As the following image, change `Default Processor Architecture` to `x64` and uncheck `Keep Test Execution Engine Running`:  
![Install Test Adapter 02](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%205%20Install%20Test%20Adapter%2002.png)


### Step 6: Run Tests
In fact, to enable any profiler based mocking tool, you have to set the environment variables. Therefore, such libraries - Microsoft Fakes/Typemock Isolator/Telerik JustMock provide small runner to satisfy the requisition, also it is true at Prig. Choose the menu `PRIG` - `Enable Test Adapter for ConsoleApplicationTest`:  
![Run Tests 01](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%206%20Run%20Tests%2001.png)  
_2015/12/16 The above picture should be changed to the newer_

Then, execute `TEST` - `Run` - `All Tests`, you can get test results in the Test Explorer.  
![Run Tests 02](https://cdn.rawgit.com/urasandesu/Prig/master/Urasandesu.Prig.VSPackage/Resources/Step%206%20Run%20Tests%2002.png)


### Final Step: Refactoring and Get Trig Back!
If tests have been created, you can refactor illimitably! For example, you probably can find the result of refactoring as follows: 
```cs
using System;

namespace QuickTour
{
    public class LifeInfo
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

For more information, see also [Prig's wiki](https://github.com/urasandesu/Prig/wiki).




# INSTALLATION FROM SOURCE CODE
## DEPENDENCY
To build this project needs the following dependencies: 
* [Visual Studio 2013(more than Professional Edition because it uses ATL. Also, you can use Community Edition)](http://www.visualstudio.com/)
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
Extract to C:\gtest-1.6.0, and upgrade C:\gtest-1.6.0\msvc\gtest.sln to Visual Studio 2013. Choose the `Build` menu, and open `Configuration Manager...`. On `Configuration Manager` dialog box, in the `Active Solution Platform` drop-down list, choose the `<New...>` option. After the `New Solution Platform` dialog box is opened, in the `Type or select the new platform` drop-down list, select a 64-bit platform. Then build all(Debug/Release) configurations.
* [NUnit 2.6.3.13283](http://www.nunit.org/)  
Install using with the installer(NUnit-2.6.3.msi).
* [Modeling SDK for Microsoft Visual Studio 2013](http://www.microsoft.com/en-us/download/details.aspx?id=40754)  
Install using with the installer(VS_VmSdk.exe).
* [Microsoft Visual Studio 2013 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=40758)  
Install using with the installer(vssdk_full.exe).
* [NAnt](http://nant.sourceforge.net/)  
You can also install in accordance with [the help](http://nant.sourceforge.net/release/latest/help/introduction/installation.html), but the easiest way is using Chocolatey: `choco install nant`.




## BUILD
### From PowerShell Script
Run Developer Command Prompt for VS2013 as Administrator, and execute the following commands: 
```dos
CMD Prig> cd
C:\Users\User\Prig

CMD Prig> powershell
Windows PowerShell
Copyright (C) 2014 Microsoft Corporation. All rights reserved.


PS Prig> .\Build.ps1
...

Chocolatey v0.9.9.1
Attempting to build package from 'Prig.nuspec'.
Successfully created package 'Prig.2.0.0-alpha04.nupkg'
PS Chocolatey>

```

**NOTE:** It takes somewhere round 30 minutes.



### From Visual Studio
After preparing all dependencies, you can build this project in the following steps:

1. Run Visual Studio as Administrator, and open Prig.sln(This sln contains some ATL projects, so the build process will modify registry).
2. According to the version of the product to use, change the solution configuration and the solution platform and build.
3. The results are output to `$(SolutionDir)$(Configuration)\$(PlatformTarget)\`.



## REGISTRATION
If you built Prig by PowerShell script, Prig nupkg will be output to `<top level directory you cloned>\Chocolatey` directory. So, run PowerShell as Administrator from the directory, and execute the following command: 
```powershell
PS Chocolatey> dir


    Directory: C:\Users\User\Prig\Chocolatey


Mode                LastWriteTime     Length Name
----                -------------     ------ ----
d----        2015/05/27     22:11            tools
-a---        2015/05/30     17:18    4143089 Prig.2.0.0-alpha04.nupkg
-a---        2015/05/27     22:11       5477 Prig.nuspec


PS Chocolatey> choco install prig -s "$pwd;https://chocolatey.org/api/v2/" -pre -y
Chocolatey v0.9.9.1
Installing the following packages:
prig
By installing you accept licenses for the packages.

Prig v2.0.0-alpha04
   Renaming 'C:\ProgramData\chocolatey\lib\Prig\tools\NUnitTestAdapterForPrig.2.0.0.nupkg.hedge' to 'C:\ProgramData\chocolatey\lib\Prig\tools\NUnitTestAdapterForPrig.2.0.0.nupkg'...
   Renaming 'C:\ProgramData\chocolatey\lib\Prig\tools\NuGet\Prig.nuspec.hedge' to 'C:\ProgramData\chocolatey\lib\Prig\tools\NuGet\Prig.nuspec'...
   Creating the nuget package 'Prig'...
   ...


 prig has been installed successfully.

Chocolatey installed 1/1 package(s). 0 package(s) failed.
 See the log for details.
PS Chocolatey>

```



## UNREGISTRATION
Unregistration operation is similar in the registration. Run PowerShell as Administrator and execute the following commands: 
```powershell
PS Chocolatey> choco uninstall prig -y -f
Chocolatey v0.9.9.1
Uninstalling the following packages:
prig

Prig v2.0.0-alpha04 (forced)
   Uninstalling Prig.vsix...
   Uninstalling the all package...
   Unregistering the profiler 'C:\ProgramData\chocolatey\lib\Prig\tools\x64\Urasandesu.Prig.dll' from Registry...
   Unregistering the profiler 'C:\ProgramData\chocolatey\lib\Prig\tools\x86\Urasandesu.Prig.dll' from Registry...
   ...

Chocolatey uninstalled 1/1 packages. 0 packages failed.
See the log for details.
PS Chocolatey>

```

**NOTE:** You have to run PowerShell as a different process from the registration time.



