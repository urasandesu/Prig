/* 
 * File: AssemblyInfo.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2014 Akira Sugiura
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


using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Test.program1")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Toshiba")]
[assembly: AssemblyProduct("Test.program1")]
[assembly: AssemblyCopyright("Copyright © Toshiba 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、その型はこのアセンブリ内で COM コンポーネントから 
// 参照不可能になります。COM からこのアセンブリ内の型にアクセスする場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

// 次の GUID は、このプロジェクトが COM に公開される場合の、typelib の ID です
[assembly: Guid("79b40bde-45d3-4b78-90a1-5f29412f6379")]

// アセンブリのバージョン情報は、以下の 4 つの値で構成されています:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// すべての値を指定するか、下のように '*' を使ってビルドおよびリビジョン番号を 
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]


// regsvr32 /i .\x86\Urasandesu.Prig.dll
// regsvr32 /i .\x64\Urasandesu.Prig.dll

// SET URASANDESU_CPPANONYM_LOGGING_SEVERITY=0
// SET URASANDESU_PRIG_DEBUGGING_BREAK=10000
//  System.Environment.SetEnvironmentVariable("URASANDESU_PRIG_DEBUGGING_BREAK", "-1");

// "..\..\..\..\Debug\x86\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe" -arguments "Test.program1.dll /domain=None"
// "..\..\..\..\Debug\x64\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console.exe" -arguments "Test.program1.dll /domain=None"

// "..\..\..\..\Release\x86\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe" -arguments "Test.program1.dll /domain=None"
// "..\..\..\..\Release\x64\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console.exe" -arguments "Test.program1.dll /domain=None"

// "..\..\..\..\Debug\x86\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe" -arguments "Test.program1.dll /domain=None /framework=v4.0"
// "..\..\..\..\Debug\x64\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console.exe" -arguments "Test.program1.dll /domain=None /framework=v4.0"

// "..\..\..\..\Release\x86\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console-x86.exe" -arguments "Test.program1.dll /domain=None /framework=v4.0"
// "..\..\..\..\Release\x64\prig.exe" run -process "C:\Program Files (x86)\NUnit 2.6.3\bin\nunit-console.exe" -arguments "Test.program1.dll /domain=None /framework=v4.0"
