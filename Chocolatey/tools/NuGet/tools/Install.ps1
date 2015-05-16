# 
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

$chocoLibPath = [IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'lib')
$chocoNet35LibPath = [IO.Path]::Combine($chocoLibPath, 'net35')
$chocoNet40LibPath = [IO.Path]::Combine($chocoLibPath, 'net40')
$chocoToolsPath = [IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools')


Import-Module ([System.IO.Path]::Combine($chocoToolsPath, 'Urasandesu.Prig'))


$here = Split-Path $MyInvocation.MyCommand.Path
$nugetLibPath = [IO.Path]::Combine([IO.Path]::GetDirectoryName($here), 'lib')
$nugetNet35LibPath = [IO.Path]::Combine($nugetLibPath, 'net35')
$nugetNet40LibPath = [IO.Path]::Combine($nugetLibPath, 'net40')
$nugetToolsPath = [IO.Path]::Combine($nugetPackageFolder, 'tools')
rmdir $nugetLibPath -Recurse -ErrorAction SilentlyContinue
$items = @($nugetNet35LibPath, $chocoNet35LibPath), @($nugetNet40LibPath, $chocoNet40LibPath)
foreach ($item in $items) {
    $nugetNetLibPath, $chocoNetLibPath = $item
    mkdir $nugetNetLibPath | Out-Null
    foreach ($dll in (dir $chocoNetLibPath)) {
        $target = [IO.Path]::Combine($nugetNetLibPath, $dll.Name)
        $source = $dll.FullName
        cmd /c ('" mklink "{0}" "{1}" "' -f $target, $source)
    }
}
