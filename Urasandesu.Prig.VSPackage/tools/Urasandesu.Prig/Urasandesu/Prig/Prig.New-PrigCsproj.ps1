# 
# File: Prig.New-PrigCsproj.ps1
# 
# Author: Akira Sugiura (urasandesu@gmail.com)
# 
# 
# Copyright (c) 2012 Akira Sugiura
#  
#  This software is MIT License.
#  
#  Permission is hereby granted, free of charge, to any person obtaining a copy
#  of this software and associated documentation files (the "Software"), to deal
#  in the Software without restriction, including without limitation the rights
#  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
#  copies of the Software, and to permit persons to whom the Software is
#  furnished to do so, subject to the following conditions:
#  
#  The above copyright notice and this permission notice shall be included in
#  all copies or substantial portions of the Software.
#  
#  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
#  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
#  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
#  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
#  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
#  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
#  THE SOFTWARE.
#



function New-PrigCsproj {
    param ($WorkDirectory, $AssemblyInfo, $ReferencedAssemblyInfos, $KeyFile, $TargetFrameworkVersion, $OutputPath)

    $rootNamespace = ToRootNamespace $AssemblyInfo
    $signAsm = ToSignAssembly $AssemblyInfo $KeyFile
    $defineConsts = ToDefineConstants $AssemblyInfo $TargetFrameworkVersion
    $platform = ToPlatformTarget $AssemblyInfo
    $asmName = ConvertTo-PrigAssemblyName $AssemblyInfo
    $refInc = ToReferenceInclude $ReferencedAssemblyInfos

    $csprojTemplate = [xml]@"
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
        <OutputType>Library</OutputType>
        <RootNamespace>$rootNamespace</RootNamespace>
        <FileAlignment>512</FileAlignment>
        <SignAssembly>$signAsm</SignAssembly>
        <AssemblyOriginatorKeyFile>$KeyFile</AssemblyOriginatorKeyFile>
        <OutputPath>$OutputPath</OutputPath>
        <DefineConstants>$defineConsts</DefineConstants>
        <PlatformTarget>$platform</PlatformTarget>
        <DebugType>pdbonly</DebugType>
        <Optimize>true</Optimize>
        <TargetFrameworkVersion>$TargetFrameworkVersion</TargetFrameworkVersion>
        <AssemblyName>$asmName</AssemblyName>
    </PropertyGroup>
    <ItemGroup>$refInc</ItemGroup>
    <ItemGroup>
        <Compile Include="**/*.cs" />
    </ItemGroup>
    <Import Project="`$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
"@

    New-Object psobject | 
        Add-Member NoteProperty 'Path' ([System.IO.Path]::Combine($WorkDirectory, "$rootNamespace.g.csproj")) -PassThru | 
        Add-Member NoteProperty 'XmlDocument' $csprojTemplate -PassThru
}
