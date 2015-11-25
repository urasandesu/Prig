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

trap {
    Write-Error ($Error[0] | Out-String)
    exit -1
}

try {
    msbuild /ver | Out-Null
} catch [System.Management.Automation.CommandNotFoundException] {
    Write-Error "You have to run this script in the Developer Command Prompt for VS2013 as Administrator."
    exit 392847384
}


try {
    nuget | Out-Null
} catch [System.Management.Automation.CommandNotFoundException] {
    Write-Error "You have to install NuGet command line utility(nuget.exe). For more information, please see also README.md."
    exit 1220074184
}


try {
    nant -help | Out-Null
} catch [System.Management.Automation.CommandNotFoundException] {
    Write-Error "You have to install NAnt. For more information, please see also README.md."
    exit 1444470369
}


if (![string]::IsNullOrEmpty($BuildTarget)) {
    $buildTarget_ = ":" + $BuildTarget
}

switch ($PsCmdlet.ParameterSetName) {
    'Package' { 
        $curDir = $PWD.Path
        if ($BuildTarget -ne "Clean") {
            $nuspecPath = [System.IO.Path]::Combine($curDir, 'Urasandesu.Prig.VSPackage\tools\NuGet\Prig.nuspec')
            $nuspec = [xml](Get-Content $nuspecPath)

            $resxPath = [System.IO.Path]::Combine($curDir, 'Urasandesu.Prig.VSPackage\Resources.resx')
            $resx = [xml](Get-Content $resxPath)

            ($resx.root.data | ? { $_.name -eq 'NuGetRootPackageId' }).value = $nuspec.package.metadata.id
            ($resx.root.data | ? { $_.name -eq 'NuGetRootPackageVersion' }).value = $nuspec.package.metadata.version
            $resx.Save($resxPath)

            $idlPath = [System.IO.Path]::Combine($curDir, 'Urasandesu.Prig\UrasandesuPrig.idl')
            $idl = Get-Content $idlPath

            $modifiedIdl = 
                $idl | 
                    ForEach-Object { 
                        if ($_ -match '    helpstring\("Prig Profiler [^\s]+ Type Library"\),') { 
                            '    helpstring("Prig Profiler {0} Type Library"),' -f $nuspec.package.metadata.version 
                        } else { 
                            $_ 
                        } 
                    }
            
            $isIdlChanged = @(Compare-Object $idl $modifiedIdl -SyncWindow 0).Length -ne 0
            if ($isIdlChanged) {
                $modifiedIdl | Out-File $idlPath -Encoding ascii
            }

            $rcPath = [System.IO.Path]::Combine($curDir, 'Urasandesu.Prig\Urasandesu.Prig.rc')
            $rc = Get-Content $rcPath

            $modifiedRc = 
                $rc | 
                    ForEach-Object { 
                        if ($_ -match '            VALUE "FileDescription", "Prig Profiler [^\s]+ Type Library"') { 
                            '            VALUE "FileDescription", "Prig Profiler {0} Type Library"' -f $nuspec.package.metadata.version 
                        } else { 
                            $_ 
                        } 
                    }
            
            $isRcChanged = @(Compare-Object $rc $modifiedRc -SyncWindow 0).Length -ne 0
            if ($isRcChanged) {
                $modifiedRc | Out-File $rcPath -Encoding unicode
            }
        }

        $solution = "Prig.sln"
        nuget restore $solution
        $target = "/t:Urasandesu_Prig_Framework$buildTarget_;prig$buildTarget_;Urasandesu_Prig$buildTarget_;Prig_Delegates\Urasandesu_Prig_Delegates$buildTarget_;Prig_Delegates\Urasandesu_Prig_Delegates_0404$buildTarget_;Prig_Delegates\Urasandesu_Prig_Delegates_0804$buildTarget_;Prig_Delegates\Urasandesu_Prig_Delegates_1205$buildTarget_"
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

        $solution = "NUnitTestAdapter\NUnitTestAdapter.sln"
        nuget restore $solution
        $target = "/t:NUnitTestAdapter$buildTarget_;NUnitTestAdapterInstall$buildTarget_"
        $configurations = , "/p:Configuration=Release"
        $platforms = , "/p:Platform=Any CPU"
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

        if ($BuildTarget -ne "Clean") {
            Set-Location ([System.IO.Path]::Combine($curDir, 'NUnitTestAdapter'))
            [System.Environment]::CurrentDirectory = $PWD
            nant package
            Set-Location 'package'
            [System.Environment]::CurrentDirectory = $PWD
            $nuspec = [xml]((Get-ChildItem NUnitVisualStudioTestAdapter-*.nuspec) | Get-Content)
            $nuspec.package.metadata.id = 'NUnitTestAdapterForPrig'
            $nuspec.Save('NUnitTestAdapterForPrig.nuspec')
            nuget pack .\NUnitTestAdapterForPrig.nuspec
        }

        if ($BuildTarget -ne "Clean") {
            Set-Location ([System.IO.Path]::Combine($curDir, 'Urasandesu.Prig.VSPackage'))
            [System.Environment]::CurrentDirectory = $PWD

            if (![IO.Directory]::Exists("lib")) {
                New-Item "lib" -ItemType Directory
            }
            if (![IO.Directory]::Exists("tools")) {
                New-Item "tools" -ItemType Directory
            }
            if (![IO.Directory]::Exists("tools\x64")) {
                New-Item "tools\x64" -ItemType Directory
            }
            if (![IO.Directory]::Exists("tools\x86")) {
                New-Item "tools\x86" -ItemType Directory
            }
            Copy-Item "..\Release\AnyCPU\Urasandesu.NAnonym.dll" "lib" -Force
            Copy-Item "..\Release\AnyCPU\Urasandesu.Prig.Delegates.0404.dll" "lib" -Force
            Copy-Item "..\Release\AnyCPU\Urasandesu.Prig.Delegates.0804.dll" "lib" -Force
            Copy-Item "..\Release\AnyCPU\Urasandesu.Prig.Delegates.1205.dll" "lib" -Force
            Copy-Item "..\Release\AnyCPU\Urasandesu.Prig.Delegates.dll" "lib" -Force
            Copy-Item "..\Release\AnyCPU\Urasandesu.Prig.Framework.dll" "lib" -Force
            Copy-Item "..\NUnitTestAdapter\package\NUnitTestAdapterForPrig.*.nupkg" "tools" -Force
            Copy-Item "..\Release\x64\Urasandesu.Prig.dll" "tools\x64" -Force
            Copy-Item "..\Release\x86\Urasandesu.Prig.dll" "tools\x86" -Force
            Copy-Item "..\Release\x86\prig.exe" "tools" -Force

            Set-Location $curDir
            [System.Environment]::CurrentDirectory = $PWD
        }
        $solution = "Prig.sln"
        $target = "/t:Urasandesu_Prig_VSPackage$buildTarget_"
        $configurations = @("/p:Configuration=Release%28.NET 4%29")
        $platforms = @("/p:Platform=x86")
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
    }
}
