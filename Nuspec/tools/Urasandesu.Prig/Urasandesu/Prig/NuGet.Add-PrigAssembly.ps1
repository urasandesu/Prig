# 
# File: NuGet.Add-PrigAssembly.ps1
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

function Add-PrigAssembly {
<#
    .SYNOPSIS
        Creates the stub settings file for the specified assembly on the Package Manager Console.

    .DESCRIPTION
        This command adds the stub setting file that sets up the indirection settings for specified assembly to a project on the Package Manager Console.
        Target project is the value that is selected as `Default project: ` on the Package Manager Console.
        The stub setting file is named like `<assembly name>.<runtime version>.v<assembly version>.prig`.
        After the file is added, you will get the confirmation message that the project has been modified externally, so reload the project.

    .PARAMETER  Assembly
        A display name recognizing uniquely an assembly. Use this parameter if adding the stub setting file for a GAC registered assembly.
        If it can't recognize uniquely, the error "Ambiguous match found" will be occurred. So, you have to specify a more detailed display name.

    .PARAMETER  AssemblyFrom
        A full path recognizing uniquely an assembly. Use this parameter if adding the stub setting file for a GAC unregistered assembly.
        Also, you have to change to the directory where specified assembly exists if using this parameter; otherwise referenced assemblies may not resolved.

    .EXAMPLE
        Add-PrigAssembly -Assembly "mscorlib, Version=4.0.0.0"
        
        DESCRIPTION
        -----------
        This command adds the stub setting file for the assembly `mscorlib` to the target project.

    .EXAMPLE
        padd -as mscorlib
        Ambiguous match found: 
        mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
        mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
        
        DESCRIPTION
        -----------
        This is example that "You try to add the stub setting file for the assembly `mscorlib`, but the error 'Ambiguous match found' is occurred".
        In this error, candidate assemblies are listed at the time. So, select again more detailed display name from them.

    .EXAMPLE
        C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug>dir


        Directory: C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug


        Mode                LastWriteTime     Length Name
        ----                -------------     ------ ----
        -a---        2014/10/10      9:31       4096 DependentLibrary1.dll
        -a---        2014/10/10      9:31       7680 DependentLibrary1.pdb
        -a---        2014/10/10      9:31       4608 DependentLibrary2.dll
        -a---        2014/10/10      9:31       7680 DependentLibrary2.pdb
        -a---        2014/10/10      9:31       7680 DemoLibrary.dll
        -a---        2014/10/10      9:31      28160 DemoLibrary.pdb


        C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug>padd -af (dir .\DemoLibrary.dll).FullName
        
        DESCRIPTION
        -----------
        This command adds the stub setting file for the GAC unregistered assembly `DemoLibrary` to the target project.
        If you specify `-AssemblyFrom` parameter, you have to execute this command in the directory that the target assembly exists like the example.

    .INPUTS
        System.String

    .OUTPUTS
        None

    .NOTES
        If you added the stub setting files once, it can only remove manually. Let's say you mistake the project that should be selected as `Default project: `.
        In this case, you have to remove the tags `Reference`, `None`, `PreBuildEvent` that are matched the regular expression `\.prig"`.

        Also, you can't set up the stub settings for multiple assemblies to one file. If you set up such setting, you will get the build error.
        
        You can also refer to the Add-PrigAssembly command by its built-in alias, "PAdd".

    .LINK
        Find-IndirectionTarget

    .LINK
        Get-IndirectionStubSetting

    .LINK
        Invoke-Prig

#>

    [CmdletBinding()]
    param (
        [Alias("as")]
        [Parameter(Mandatory = $true, ParameterSetName = 'Assembly')]
        [string]
        $Assembly, 

        [Alias("af")]
        [Parameter(Mandatory = $true, ParameterSetName = 'AssemblyFrom')]
        [string]
        $AssemblyFrom
    )

    $os = Get-WmiObject Win32_OperatingSystem
    [System.Reflection.ProcessorArchitecture]$osArch = 0
    $osArch = $(if ($os.OSArchitecture -match '64') { 'Amd64' } else { 'X86' })

    [void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Build')
    $msbProjCollection = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection
    $envProj = (Get-Project).Object.Project
    $allMsbProjs = $msbProjCollection.GetLoadedProjects($envProj.FullName).GetEnumerator()
    if(!$allMsbProjs.MoveNext()) {
        throw New-Object System.InvalidOperationException ('"{0}" has not been loaded.' -f $envProj.FullName)
    }
    $curMsbProj = $allMsbProjs.Current

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
            $1stCandidateArch = ToProcessorArchitectureString $conditionedProperty
            $1stCandidateArch = $(if ($1stCandidateArch -eq 'MSIL') { $osArch } else { $1stCandidateArch })
            $platformTargets[$conditionedProperty].Add($1stCandidateArch)
            $platformTargets[$conditionedProperty].Add('MSIL')
        }
    }

    $curMsbProj.MarkDirty()

    $candidateNames = $(if ([string]::IsNullOrEmpty($Assembly)) { GetAssemblyNameExsFrom $AssemblyFrom } else { GetAssemblyNameExs $Assembly })
    if ($candidateNames.Length -eq 0) {
        throw New-Object System.IO.FileNotFoundException ('Assembly ''{0}'' is not found.' -f $Assembly)
    }
    
    foreach ($platformTarget in $platformTargets.GetEnumerator()) {
        $actualNames = New-Object System.Collections.ArrayList
        foreach ($candidateName in $candidateNames) {
            if ($platformTarget.Value.Contains($candidateName.ProcessorArchitecture)) {
                [void]$actualNames.Add($candidateName)
            }
        }
        if ($actualNames.Count -eq 0) {
            throw New-Object System.BadImageFormatException ('Assembly ''{0}'' is mismatch to the specified platform ''{1}''.' -f $Assembly, $platformTarget.Key)
        }
        if (1 -lt $actualNames.Count) {
            throw New-Object System.BadImageFormatException ("Ambiguous match found: `r`n{0}" -f ([string]::Join("`r`n", ($actualNames | % { $_.FullName }))))
        }
        
        SetPrigAssemblyReferenceItem $curMsbProj $actualNames[0] $platformTarget.Key
        SetStubberPreBuildEventProperty $curMsbProj $actualNames[0] $platformTarget.Key $platformTarget.Value[0]
    }

    SetStubSettingNoneItem $curMsbProj $actualNames[0] $envProj.FullName

    $curMsbProj.Save()
}

New-Alias PAdd Add-PrigAssembly



function GetAssemblyNameExs {
    param ($Assembly)
    $results = prig dasm -assembly $Assembly | ConvertFrom-Csv
    foreach ($result in $results) {
        $result.psobject.TypeNames.Insert(0, $AssemblyNameExTypeName)
        $result
    }
}



function GetAssemblyNameExsFrom {
    param ($AssemblyFrom)
    $results = prig dasm -assemblyfrom $AssemblyFrom | ConvertFrom-Csv
    foreach ($result in $results) {
        $result.psobject.TypeNames.Insert(0, $AssemblyNameExTypeName)
        $result
    }
}



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
        $AssemblyNameEx,

        [Parameter(Mandatory = $true)]
        [string]
        $Platform, 

        [Parameter(Mandatory = $true)]
        [System.Reflection.ProcessorArchitecture]
        $ProcessorArchitecture
    )

    $prigPkg = Get-Package Prig
    $prigPkgName = $prigPkg.Id + '.' + $prigPkg.Version
    $powershell = $(if ($ProcessorArchitecture -eq 'Amd64') { '%windir%\SysNative\WindowsPowerShell\v1.0\powershell.exe' } else { '%windir%\system32\WindowsPowerShell\v1.0\powershell.exe' })
    $argFile = '-File "$(SolutionDir)packages\{0}\tools\Invoke-PilotStubber.ps1"' -f $prigPkgName
    $argAssembly = '-AssemblyFrom "{0}"' -f $AssemblyNameEx.Location
    $argTargetFrameworkVersion = '-TargetFrameworkVersion {0}' -f $MSBuildProject.GetProperty('TargetFrameworkVersion').EvaluatedValue
    if ($MSBuildProject.GetProperty('TargetFrameworkVersion').EvaluatedValue -eq 'v3.5') {
        $argOther = '-Version 2.0 -NoLogo -NoProfile'
        $argReferenceFrom = '-ReferenceFrom "@(''$(SolutionDir)packages\{0}\lib\net35\Urasandesu.NAnonym.dll'',''$(SolutionDir)packages\{0}\lib\net35\Urasandesu.Prig.Framework.dll'')"' -f $prigPkgName
    } else {
        $argOther = '-NoLogo -NoProfile'
        $argReferenceFrom = '-ReferenceFrom "@(''$(SolutionDir)packages\{0}\lib\net40\Urasandesu.NAnonym.dll'',''$(SolutionDir)packages\{0}\lib\net40\Urasandesu.Prig.Framework.dll'')"' -f $prigPkgName
    }
    $argKeyFile = '-KeyFile "$(SolutionDir)packages\{0}\tools\Urasandesu.Prig.snk"' -f $prigPkgName
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
        $prigPkg = Get-Package Prig
        $prigPkgName = $prigPkg.Id + '.' + $prigPkg.Version
        $solutionDir = $MSBuildProject.ExpandString('$(SolutionDir)')
        $tools = $solutionDir + ('packages\{0}\tools' -f $prigPkgName)
        $stubSettingTemplate = [System.IO.Path]::Combine($tools, 'PilotStubber.prig')
        Copy-Item $stubSettingTemplate $stubSetting -ErrorAction Stop | Out-Null
    }
}
