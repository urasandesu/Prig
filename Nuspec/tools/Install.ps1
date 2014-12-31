﻿# 
# File: Install.ps1
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

[CmdletBinding()]
param (
    $InstallPath, 

    $ToolsPath, 

    $Package, 

    $Project
)

Write-Host ('Install Path: {0}' -f $InstallPath)
Write-Host ('Tolls Path: {0}' -f $ToolsPath)
Write-Host ('Package: {0}' -f $Package)
Write-Host ('Project: {0}' -f $Project)

[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Build')
$msbProjCollection = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection
if (@($msbProjCollection.LoadedProjects | ? { $msbProj = $_; 0 -lt @($msbProj.Items | ? { $_.UnevaluatedInclude -match 'Urasandesu\.Prig\.Framework' }).Length }).Length -eq 1) {
    regsvr32 /s /i ([System.IO.Path]::Combine($InstallPath, 'tools\x64\Urasandesu.Prig.dll'))
    regsvr32 /s /i ([System.IO.Path]::Combine($InstallPath, 'tools\x86\Urasandesu.Prig.dll'))

    cmd /c ('" "%VS120COMNTOOLS%VsDevCmd.bat" & gacutil /i "{0}" "' -f [System.IO.Path]::Combine($InstallPath, 'lib\net35\Urasandesu.NAnonym.dll'))
    cmd /c ('" "%VS120COMNTOOLS%VsDevCmd.bat" & gacutil /i "{0}" "' -f [System.IO.Path]::Combine($InstallPath, 'lib\net35\Urasandesu.Prig.Framework.dll'))
    cmd /c ('" "%VS120COMNTOOLS%VsDevCmd.bat" & gacutil /i "{0}" "' -f [System.IO.Path]::Combine($InstallPath, 'lib\net40\Urasandesu.NAnonym.dll'))
    cmd /c ('" "%VS120COMNTOOLS%VsDevCmd.bat" & gacutil /i "{0}" "' -f [System.IO.Path]::Combine($InstallPath, 'lib\net40\Urasandesu.Prig.Framework.dll'))

    Copy-Item ([System.IO.Path]::Combine($InstallPath, 'tools\x86\prig.exe')) $ToolsPath
}