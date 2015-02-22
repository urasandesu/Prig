# 
# File: Build.ps1
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

[CmdletBinding(DefaultParametersetName = 'Package')]
param (
    [Parameter(ParameterSetName = 'Package')]
    [Switch]
    $Package, 

    [ValidateSet("", "Clean", "Rebuild")] 
    [string]
    $BuildTarget
)

try {
    msbuild /ver | Out-Null
} catch [System.Management.Automation.CommandNotFoundException] {
    Write-Error "You have to run this script in the Developer Command Prompt for VS2013 as Administrator."
    exit 1
}


try {
    nuget | Out-Null
} catch [System.Management.Automation.CommandNotFoundException] {
    Write-Error "You have to install NuGet command line utility(nuget.exe). For more information, please see also README.md."
    exit 1
}


try {
    cpack | Out-Null
} catch [System.Management.Automation.CommandNotFoundException] {
    Write-Error "You have to install Chocolatey. For more information, please see also README.md."
    exit 1
}


if (![string]::IsNullOrEmpty($BuildTarget)) {
    $buildTarget_ = ":" + $BuildTarget
}

switch ($PsCmdlet.ParameterSetName) {
    'Package' { 
        $solution = "Prig.sln"
        $target = "/t:Urasandesu_Prig_Framework$buildTarget_;prig$buildTarget_;Urasandesu_Prig$buildTarget_;Urasandesu_Prig_VSPackage$buildTarget_"
        $configurations = "/p:Configuration=Release%28.NET 3.5%29", "/p:Configuration=Release%28.NET 4%29"
        $platforms = "/p:Platform=x86", "/p:Platform=x64"
        foreach ($configuration in $configurations) {
            foreach ($platform in $platforms) {
                Write-Verbose ("Solution: {0}" -f $solution)
                Write-Verbose ("Target: {0}" -f $target)
                Write-Verbose ("Configuration: {0}" -f $configuration)
                Write-Verbose ("Platform: {0}" -f $platform)
                msbuild $solution $target $configuration $platform /m
                if ($LASTEXITCODE -ne 0) {
                    exit $LASTEXITCODE
                }
            }
        }

        $curDir = $PWD.Path
        if ($BuildTarget -ne "Clean") {
            Set-Location ([System.IO.Path]::Combine($curDir, 'NuGet'))
            [System.Environment]::CurrentDirectory = $PWD
            nuget pack .\Prig.nuspec
            $src = (Resolve-Path *.nupkg).Path
            $dst = $src + '.zip'
            Move-Item $src $dst -Force
        }

        if ($BuildTarget -ne "Clean") {
            Set-Location ([System.IO.Path]::Combine($curDir, 'Chocolatey'))
            [System.Environment]::CurrentDirectory = $PWD
            cpack .\Prig.nuspec
        }
    }
}
