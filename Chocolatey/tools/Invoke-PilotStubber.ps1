# 
# File: Invoke-PilotStubber.ps1
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
    [Parameter(Mandatory = $True)]
    $ReferenceFrom, 

    [string]
    $Assembly, 

    [string]
    $AssemblyFrom, 

    [string]
    $TargetFrameworkVersion,

    [string]
    $KeyFile,

    [string]
    $OutputPath, 
    
    [Parameter(Mandatory = $True)]
    [string]
    $Settings, 
    
    [switch]
    $WhatIf
)

trap {
    Write-Error ($Error[0] | Out-String)
    exit -1
}

Write-Verbose ('ReferenceFrom            : {0}(Type: {1})' -f $ReferenceFrom, ($ReferenceFrom.GetType()))
Write-Verbose ('Assembly                 : {0}' -f $Assembly)
Write-Verbose ('Target Framework Version : {0}' -f $TargetFrameworkVersion)
Write-Verbose ('Key File                 : {0}' -f $KeyFile)
Write-Verbose ('Output Path              : {0}' -f $OutputPath)
Write-Verbose ('Settings                 : {0}' -f $Settings)

$here = Split-Path $MyInvocation.MyCommand.Path
Write-Verbose ('Invocation From          : {0}' -f $here)
Import-Module ([System.IO.Path]::Combine($here, 'Urasandesu.Prig'))

Write-Verbose 'Load Settings ...'

if (![string]::IsNullOrEmpty($Assembly)) {
    $asmInfo = [System.Reflection.Assembly]::Load($Assembly)
} elseif (![string]::IsNullOrEmpty($AssemblyFrom)) {
    $asmInfo = [System.Reflection.Assembly]::LoadFrom($AssemblyFrom)
}
if ($null -eq $asmInfo) {
    Write-Error 'The parameter ''Assembly'' or ''AssemblyFrom'' is mandatory.'
    exit -403162398
}
 
$refAsmInfos = New-Object 'System.Collections.Generic.List[System.Reflection.Assembly]'
$refFroms = $ReferenceFrom
if ($refFroms -is [string]) {
    try
    {
        $refFroms = Invoke-Expression $refFroms
    }
    catch
    { }
}
foreach ($refFrom in $refFroms) {
    Write-Verbose ('    ReferenceFrom        : {0}' -f $refFrom)
    $refAsmInfos.Add([System.Reflection.Assembly]::LoadFrom($refFrom))
}
$refAsmInfos.Add($asmInfo)
foreach ($refAsmName in $asmInfo.GetReferencedAssemblies()) {
    try {
        $refAsmInfos.Add([System.Reflection.Assembly]::Load($refAsmName.FullName))
    }
    catch {
        if ([string]::IsNullOrEmpty($AssemblyFrom)) {
            $candidateDir = $OutputPath
        } else {
            $candidateDir = [System.IO.Path]::GetDirectoryName($AssemblyFrom)
        }
        $refAsmPathWithoutExtension = [System.IO.Path]::Combine($candidateDir, $refAsmName.Name)
        try {
            $refAsmInfos.Add([System.Reflection.Assembly]::LoadFrom($refAsmPathWithoutExtension + ".dll"))
        }
        catch {
            try {
                $refAsmInfos.Add([System.Reflection.Assembly]::LoadFrom($refAsmPathWithoutExtension + ".exe"))
            }
            catch {
                Write-Warning ($_ | Out-String)
            }
        }
    }
}

$confAsmInfo = [System.Reflection.Assembly]::LoadWithPartialName('System.Configuration')
if ($refAsmInfos -notcontains $confAsmInfo) {
    $refAsmInfos.Add($confAsmInfo)
}

$systemAsmInfo = [System.Reflection.Assembly]::LoadWithPartialName('System')
if ($refAsmInfos -notcontains $systemAsmInfo) {
    $refAsmInfos.Add($systemAsmInfo)
}


$onAsmInfoResolve = [System.ResolveEventHandler] {
    param($Sender, $E)
    foreach($curAsmInfo in [System.AppDomain]::CurrentDomain.GetAssemblies()) {
        if ($curAsmInfo.FullName -match $E.Name) {
            return $curAsmInfo
        }
    }
    return $null
}

[System.AppDomain]::CurrentDomain.add_AssemblyResolve($onAsmInfoResolve)

$fileMap = New-Object System.Configuration.ExeConfigurationFileMap
$fileMap.ExeConfigFilename = $Settings
$config = [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration($fileMap, [System.Configuration.ConfigurationUserLevel]::None)
$section = $config.GetSection("prig")
$unintendedSettings = @($section.Stubs | ? { $_.Target.Module.Assembly -ne $asmInfo })
if (0 -lt $unintendedSettings.Length) {
    Write-Error ('Now the indirection settings for "{0}" is being analysed, but the settings for "{1}" was detected.' -f $asmInfo.FullName, $unintendedSettings[0].Target.Module.Assembly.FullName)
    exit -1944878741
}

$workDir = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($Settings), (ConvertTo-PrigAssemblyName $asmInfo))
Write-Verbose ('    Work Directory {0} ...' -f $workDir)
if (![string]::IsNullOrEmpty($workDir) -and [IO.Directory]::Exists($workDir)) {
    Remove-Item $workDir -Force -Recurse -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
}
New-Item $workDir -ItemType Directory -WhatIf:$WhatIf -ErrorAction Stop | Out-Null


Write-Verbose 'Generate Tokens.g.cs ...'
$tokensCsInfo = New-PrigTokensCs $workDir $asmInfo $section $TargetFrameworkVersion
$tokensCsDir = [System.IO.Path]::GetDirectoryName($tokensCsInfo.Path)
if (![string]::IsNullOrEmpty($tokensCsDir) -and ![IO.Directory]::Exists($tokensCsDir)) {
    New-Item $tokensCsDir -ItemType Directory -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
    Write-Verbose ('    Make Directory to {0} ...' -f $tokensCsDir)
}
$tokensCsInfo.Content | Out-File $tokensCsInfo.Path -Encoding utf8 -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
Write-Verbose ('    Output to {0} ...' -f $tokensCsInfo.Path)


$stubsCsInfos = New-PrigStubsCs $workDir $asmInfo $section $TargetFrameworkVersion
Write-Verbose ('Generate stubs *.g.cs(Count: {0}) ...' -f $stubsCsInfos.Count)
foreach ($stubsCsInfo in $stubsCsInfos) {
    $stubsCsDir = [System.IO.Path]::GetDirectoryName($stubsCsInfo.Path)
    Write-Verbose ('    Check Directory existence {0} ...' -f $stubsCsDir)
    if (![string]::IsNullOrEmpty($stubsCsDir) -and ![IO.Directory]::Exists($stubsCsDir)) {
        New-Item $stubsCsDir -ItemType Directory -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
        Write-Verbose ('    Make Directory to {0} ...' -f $stubsCsDir)
    }
    Write-Verbose ('    Check File existence {0} ...' -f $stubsCsInfo.Path)
    $stubsCsInfo.Content | Out-File $stubsCsInfo.Path -Encoding utf8 -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
    Write-Verbose ('    Output to {0} ...' -f $stubsCsInfo.Path)
}


$proxiesCsInfos = New-PrigProxiesCs $workDir $asmInfo $section $TargetFrameworkVersion
Write-Verbose ('Generate proxies *.g.cs(Count: {0}) ...' -f $proxiesCsInfos.Count)
foreach ($proxiesCsInfo in $proxiesCsInfos) {
    $proxiesCsDir = [System.IO.Path]::GetDirectoryName($proxiesCsInfo.Path)
    Write-Verbose ('    Check Directory existence {0} ...' -f $proxiesCsDir)
    if (![string]::IsNullOrEmpty($proxiesCsDir) -and ![IO.Directory]::Exists($proxiesCsDir)) {
        New-Item $proxiesCsDir -ItemType Directory -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
        Write-Verbose ('    Make Directory to {0} ...' -f $proxiesCsDir)
    }
    Write-Verbose ('    Check File existence {0} ...' -f $proxiesCsInfo.Path)
    $proxiesCsInfo.Content | Out-File $proxiesCsInfo.Path -Encoding utf8 -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
    Write-Verbose ('    Output to {0} ...' -f $proxiesCsInfo.Path)
}


Write-Verbose 'Generate *.g.csproj ...'
$csprojInfo = New-PrigCsproj $workDir $asmInfo $refAsmInfos $KeyFile $TargetFrameworkVersion $OutputPath
$csprojDir = [System.IO.Path]::GetDirectoryName($csprojInfo.Path)
if (![string]::IsNullOrEmpty($csprojDir) -and ![IO.Directory]::Exists($csprojDir)) {
    New-Item $csprojDir -ItemType Directory -WhatIf:$WhatIf -ErrorAction Stop | Out-Null
}
$csprojInfo.XmlDocument.Save($csprojInfo.Path)
Write-Verbose ('Output to {0} ...' -f $csprojInfo.Path)


Write-Verbose 'Build all *.cs files ...'
msbuild $csprojInfo.Path /t:rebuild
