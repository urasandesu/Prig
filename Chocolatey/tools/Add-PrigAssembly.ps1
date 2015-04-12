# 
# File: Add-PrigAssembly.ps1
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
    [string]
    $ProjectFullName, 

    [Parameter(Mandatory = $True)]
    [string]
    $TargetFrameworkVersion, 

    [string]
    $Assembly, 

    [string]
    $AssemblyFrom
)

Write-Verbose ('ProjectFullName          : {0}' -f $ProjectFullName)
Write-Verbose ('TargetFrameworkVersion   : {0}' -f $TargetFrameworkVersion)
Write-Verbose ('Assembly                 : {0}' -f $Assembly)
Write-Verbose ('AssemblyFrom             : {0}' -f $AssemblyFrom)



Import-Module ([System.IO.Path]::Combine((Split-Path $MyInvocation.MyCommand.Path), 'Urasandesu.Prig'))



function SetPrigAssemblyReferenceItem {

    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]
        $MSBuildProject,

        [Parameter(Mandatory = $true)]
        $AssemblyNameEx,

        [Parameter(Mandatory = $true)]
        [string]
        $Platform
    )

    $reference = $MSBuildProject.AddItem('Reference', (ConvertTo-PrigAssemblyName $AssemblyNameEx))
    if ($Platform -match 'AnyCPU') {
        $reference[0].Xml.Condition = "'`$(Platform)|`$(Prefer32Bit)' == '$Platform'"
    } else {
        $reference[0].Xml.Condition = "'`$(Platform)' == '$Platform'"
    }
}



function SetStubberPreBuildEventProperty {

    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]
        $MSBuildProject,

        [Parameter(Mandatory = $true)]
        [string]
        $TargetFrameworkVersion,

        [Parameter(Mandatory = $true)]
        $AssemblyNameEx,

        [Parameter(Mandatory = $true)]
        [string]
        $Platform, 

        [Parameter(Mandatory = $true)]
        [System.Reflection.ProcessorArchitecture]
        $ProcessorArchitecture
    )

    $toolsPath = [IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools')
    $libPath = [IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'lib')
    $powershell = $(if ($ProcessorArchitecture -eq 'Amd64') { '%windir%\SysNative\WindowsPowerShell\v1.0\powershell.exe' } else { '%windir%\system32\WindowsPowerShell\v1.0\powershell.exe' })
    $argFile = '-File "{0}\Invoke-PilotStubber.ps1"' -f $toolsPath
    $argAssembly = '-AssemblyFrom "{0}"' -f $AssemblyNameEx.Location
    $argTargetFrameworkVersion = '-TargetFrameworkVersion {0}' -f $TargetFrameworkVersion
    if ($TargetFrameworkVersion -eq 'v3.5') {
        $argOther = '-Version 2.0 -NoLogo -NoProfile'
        $argReferenceFrom = '-ReferenceFrom "@(''{0}\net35\Urasandesu.NAnonym.dll'',''{0}\net35\Urasandesu.Prig.Framework.dll'')"' -f $libPath
    } else {
        $argOther = '-NoLogo -NoProfile'
        $argReferenceFrom = '-ReferenceFrom "@(''{0}\net40\Urasandesu.NAnonym.dll'',''{0}\net40\Urasandesu.Prig.Framework.dll'')"' -f $libPath
    }
    $argKeyFile = '-KeyFile "{0}\Urasandesu.Prig.snk"' -f $toolsPath
    $argOutputPath = '-OutputPath "$(TargetDir)."'
    $argSettings = '-Settings "$(ProjectDir){0}.{1}.v{2}.prig"' -f $AssemblyNameEx.Name, $AssemblyNameEx.ImageRuntimeVersion, $AssemblyNameEx.Version
    $cmd = 'cmd /c " "%VS120COMNTOOLS%VsDevCmd.bat" & {0} {1} {2} {3} {4} {5} {6} {7} {8} "' -f 
                $powershell, 
                $argOther, 
                $argFile, 
                $argReferenceFrom, 
                $argAssembly, 
                $argTargetFrameworkVersion, 
                $argKeyFile, 
                $argOutputPath, 
                $argSettings
        

    if ($Platform -match 'AnyCPU') {
        $condition = "'`$(Platform)|`$(Prefer32Bit)' == '$Platform'"
    } else {
        $condition = "'`$(Platform)' == '$Platform'"
    }
    
    $preBuildEvents = 
        $MSBuildProject.Xml.Properties | 
            Where-Object { $_.Name -eq 'PreBuildEvent' } | 
            Where-Object { $_.Value -cmatch '\bPrig\b' } | 
            Where-Object { $_.Parent.Condition -eq $condition }
    
    if (0 -lt $preBuildEvents.Length) {
        $preBuildEvents[0].Value += "`r`n$cmd"
    } else {
        $propGroup = $MSBuildProject.Xml.CreatePropertyGroupElement()
        $MSBuildProject.Xml.InsertAfterChild($propGroup, @($MSBuildProject.Xml.Imports)[-1])
        $propGroup.Condition = $condition
        [void]$propGroup.AddProperty('PreBuildEvent', $cmd)
    }
}



function SetStubSettingNoneItem {

    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]
        $MSBuildProject,

        [Parameter(Mandatory = $true)]
        $AssemblyNameEx,

        [Parameter(Mandatory = $true)]
        [string]
        $ProjectFullName
    )

    $stubSettingName = "{0}.{1}.v{2}.prig" -f $AssemblyNameEx.Name, $AssemblyNameEx.ImageRuntimeVersion, $AssemblyNameEx.Version
    [void]$MSBuildProject.AddItem('None', $stubSettingName)
    $stubSetting = [System.IO.Path]::Combine([System.IO.Path]::GetDirectoryName($ProjectFullName), $stubSettingName)
    if (![System.IO.File]::Exists($stubSetting)) {
        $stubSettingTemplate = [System.IO.Path]::Combine((Get-PackageToolsPath), 'PilotStubber.prig')
        Copy-Item $stubSettingTemplate $stubSetting -ErrorAction Stop | Out-Null
    }
}



function HasAlreadyExistedStubSettingNoneItem {

    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]
        $MSBuildProject,

        [Parameter(Mandatory = $true)]
        $AssemblyNameEx
    )

    $prigAssembly = "{0}.{1}.v{2}" -f $AssemblyNameEx.Name, $AssemblyNameEx.ImageRuntimeVersion, $AssemblyNameEx.Version
    $escapedPrigAssembly = [regex]::Escape($prigAssembly)
    $items = 
        $MSBuildProject.ItemsIgnoringCondition | 
            Where-Object { $_.ItemType -eq "None" } | 
            Where-Object { $_.EvaluatedInclude -match "\.prig\b" } | 
            Where-Object { $_.EvaluatedInclude -match $escapedPrigAssembly }
    
    $items.Length -ne 0
}



$os = Get-WmiObject Win32_OperatingSystem
[System.Reflection.ProcessorArchitecture]$osArch = 0
$osArch = $(if ($os.OSArchitecture -match '64') { 'Amd64' } else { 'X86' })

[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Build')
$msbProjCollection = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection

$curMsbProj = $msbProjCollection.LoadProject($ProjectFullName)

$platformTargets = New-Object 'System.Collections.Generic.Dictionary[string, System.Collections.Generic.List[System.Reflection.ProcessorArchitecture]]'
if (!$curMsbProj.ConditionedProperties.ContainsKey('Platform')) {
    $platformTargets['AnyCPU'] = New-Object 'System.Collections.Generic.List[System.Reflection.ProcessorArchitecture]'
    $platformTargets['AnyCPU'].Add($osArch)
    $platformTargets['AnyCPU'].Add('MSIL')
} else {
    $conditionedProperties = New-Object 'System.Collections.Generic.List[string]'
    foreach ($conditionedProperty in $curMsbProj.ConditionedProperties['Platform']) {
        if ($conditionedProperty -eq 'AnyCPU') {
            $conditionedProperties.Add($conditionedProperty + '|true')
            $conditionedProperties.Add($conditionedProperty + '|false')
        } else {
            $conditionedProperties.Add($conditionedProperty)
        }
    }
    foreach ($conditionedProperty in $conditionedProperties) {
        $platformTargets[$conditionedProperty] = New-Object 'System.Collections.Generic.List[System.Reflection.ProcessorArchitecture]'
        [System.Reflection.ProcessorArchitecture]$1stCandidateArch = 0
        $1stCandidateArch = ConvertTo-ProcessorArchitectureString $conditionedProperty
        $1stCandidateArch = $(if ($1stCandidateArch -eq 'MSIL') { $osArch } else { $1stCandidateArch })
        $platformTargets[$conditionedProperty].Add($1stCandidateArch)
        $platformTargets[$conditionedProperty].Add('MSIL')
    }
}

$curMsbProj.MarkDirty()

$candidateNames = $(if ([string]::IsNullOrEmpty($Assembly)) { Get-AssemblyNameExsFrom $AssemblyFrom } else { Get-AssemblyNameExs $Assembly })
if ($candidateNames.Length -eq 0) {
    $identifier = $(if ([string]::IsNullOrEmpty($Assembly)) { $AssemblyFrom } else { $Assembly })
    $Host.UI.WriteErrorLine(('Assembly ''{0}'' is not found.' -f $identifier))
    exit 964834411
}

$isPlatformMatched = $false
foreach ($platformTarget in $platformTargets.GetEnumerator()) {
    $actualNames = New-Object System.Collections.ArrayList
    foreach ($candidateName in $candidateNames) {
        if ($platformTarget.Value.Contains($candidateName.ProcessorArchitecture)) {
            [void]$actualNames.Add($candidateName)
        }
    }
    if ($actualNames.Count -eq 0) {
        continue
    }
    if (1 -lt $actualNames.Count) {
        $Host.UI.WriteErrorLine(("Ambiguous match found: `r`n{0}" -f ([string]::Join("`r`n", ($actualNames | % { $_.FullName })))))
        exit -1786925265
    }
    if (!$isPlatformMatched -and (HasAlreadyExistedStubSettingNoneItem $curMsbProj $actualNames[0])) {
        $Host.UI.WriteErrorLine(("The Prig assembly for {0} has already been added." -f $actualNames[0].Name))
        exit -1863575335
    }
        
    SetPrigAssemblyReferenceItem $curMsbProj $actualNames[0] $platformTarget.Key
    SetStubberPreBuildEventProperty $curMsbProj $TargetFrameworkVersion $actualNames[0] $platformTarget.Key $platformTarget.Value[0]
    $isPlatformMatched = $true
}
if (!$isPlatformMatched) {
    $identifier = $(if ([string]::IsNullOrEmpty($Assembly)) { $AssemblyFrom } else { $Assembly })
    $Host.UI.WriteErrorLine(('Assembly ''{0}'' is mismatch to the any platforms ''{1}''.' -f $identifier, ($platformTargets.Keys -join ', ')))
    exit 1553812715
}

SetStubSettingNoneItem $curMsbProj $actualNames[0] $ProjectFullName

$curMsbProj.Save()
