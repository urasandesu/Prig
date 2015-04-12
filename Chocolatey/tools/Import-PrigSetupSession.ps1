# 
# File: Import-PrigSetupSession.ps1
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
    $ProjectName, 

    [Parameter(Mandatory = $True)]
    [string]
    $ProjectFullName, 

    [Parameter(Mandatory = $True)]
    [string]
    $TargetFrameworkVersion, 

    [Parameter(Mandatory = $True)]
    [string]
    $OutputPath, 

    [string]
    $ReferenceInclude, 

    [string]
    $ReferenceHintPath, 

    [string]
    $ProjectReferenceInclude, 

    [switch]
    $NoIntro, 

    [string]
    $AdditionalInclude, 

    [string]
    $EditorialInclude, 

    [string]
    $DeletionalInclude
)

Write-Verbose ('ProjectName              : {0}' -f $ProjectName)
Write-Verbose ('ProjectFullName          : {0}' -f $ProjectFullName)
Write-Verbose ('TargetFrameworkVersion   : {0}' -f $TargetFrameworkVersion)
Write-Verbose ('OutputPath               : {0}' -f $OutputPath)
Write-Verbose ('ReferenceInclude         : {0}' -f $ReferenceInclude)
Write-Verbose ('ReferenceHintPath        : {0}' -f $ReferenceHintPath)
Write-Verbose ('ProjectReferenceInclude  : {0}' -f $ProjectReferenceInclude)
Write-Verbose ('NoIntro                  : {0}' -f $NoIntro)
Write-Verbose ('AdditionalInclude        : {0}' -f $AdditionalInclude)
Write-Verbose ('EditorialInclude         : {0}' -f $EditorialInclude)
Write-Verbose ('DeletionalInclude        : {0}' -f $DeletionalInclude)

if (!$NoIntro) {
@'
Welcome to Prig setup session!!


You can add the indirection settings from here. In this session, you can use `$ReferencedAssemblies` that contains all referenced assemblies information of current project. For example, if you want to get the indirection settings for all members of the type `Foo` that belongs to the referenced assembly `UntestableLibrary`, the following commands will achieve it: 

PS> $ReferencedAssemblies

FullName
--------
mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
MyLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null


PS> padd -ra $ReferencedAssemblies[-1]
PS> $ReferencedAssemblies[-1].GetTypes() | ? { $_.Name -eq 'Foo' } | pfind | pget | clip
PS> exit   # Then, paste the content on the clipboard to the stub setting file(e.g. `UntestableLibrary.v4.0.30319.v1.0.0.0.prig`).



See also the command's help `padd`, `pfind` and `pget`.



'@
}

'Current Project: {0}' -f $ProjectName
Write-Warning 'Change the Current Project from `Default Project: ` on the Package Manager Console if it isn''t what you want.'
''
''

$ImportPrigSetupSessionDetail = 'Import-PrigSetupSessionDetail' | New-Module {

    function ResolveAssemblyByReference {
        
        param (
            [string]
            $ProjectDirectory, 

            [string]
            $OutputPath, 

            [string]
            $ReferenceInclude, 

            [string]
            $ReferenceHintPath
        )

        Set-Location $OutputPath
        [System.Environment]::CurrentDirectory = $PWD

        $refAsmInfo = [System.Reflection.Assembly]::LoadWithPartialName($ReferenceInclude)
        if ($null -eq $refAsmInfo) {
            if (![string]::IsNullOrEmpty($ReferenceHintPath)) {
                $refAsmPath = [System.IO.Path]::Combine($ProjectDirectory, $ReferenceHintPath)

                Set-Location ([System.IO.Path]::GetDirectoryName($refAsmPath))
                [System.Environment]::CurrentDirectory = $PWD
                    
                try {
                    $refAsmInfo = [System.Reflection.Assembly]::LoadFrom($refAsmPath)
                }
                catch {
                    Write-Warning $_.Exception.Message
                }
            } else {
                $refAsmPathWithoutExtension = [System.IO.Path]::Combine($PWD.Path, ($ReferenceInclude -replace ',.*$', ''))
                $refAsmPath = $refAsmPathWithoutExtension + ".dll"
                try {
                    $refAsmInfo = [System.Reflection.Assembly]::LoadFrom($refAsmPath)
                }
                catch {
                    Write-Warning $_.Exception.Message

                    try {
                        $refAsmPath = $refAsmPathWithoutExtension + ".exe"
                        $refAsmInfo = [System.Reflection.Assembly]::LoadFrom($refAsmPath)
                    }
                    catch {
                        Write-Warning $_.Exception.Message
                    }
                }
            }
        }

        $refAsmInfo
    }



    function ResolveAssemblyByProjectReference {
        
        param (
            [string]
            $ProjectReferenceInclude
        )

        $outPath = [System.IO.Path]::GetDirectoryName($ProjectReferenceInclude)
        Set-Location $outPath
        [System.Environment]::CurrentDirectory = $PWD

        $refAsmInfo = $null
        try {
            $refAsmInfo = [System.Reflection.Assembly]::LoadFrom($ProjectReferenceInclude)
        }
        catch {
            Write-Warning $_.Exception.Message
        }

        $refAsmInfo
    }



    function Main {

        param (
            [string]
            $Here
        )

        $orgPath = $PWD.Path

        $projDir = [System.IO.Path]::GetDirectoryName($ProjectFullName)
        $outPathResolved = [System.IO.Path]::Combine($projDir, $OutputPath)

        
        $refAsmInfos = New-Object 'System.Collections.Generic.List[System.Reflection.Assembly]'

        $refAsmInfo = [System.Reflection.Assembly]::LoadWithPartialName('mscorlib')
        [void]$refAsmInfos.Add($refAsmInfo)

        if (![string]::IsNullOrEmpty($ReferenceInclude) -and ![string]::IsNullOrEmpty($ReferenceHintPath)) {
            $refIncludes = $ReferenceInclude -split ';'
            $refHints =  $ReferenceHintPath -split ';'
            for ($i = 0; $i -lt $refIncludes.Length; $i++) {
                $refInclude = $refIncludes[$i]
                $refHint = $refHints[$i]
                if ($refInclude -cmatch '\bUrasandesu\b' -or $refInclude -cmatch '\.Prig$') {
                    continue
                }
            
                $refAsmInfo = ResolveAssemblyByReference $projDir $outPathResolved $refInclude $refHint
                if ($null -ne $refAsmInfo) {
                    [void]$refAsmInfos.Add($refAsmInfo)
                }
            }
        }

        if (![string]::IsNullOrEmpty($ProjectReferenceInclude)) {
            $projRefIncludes = $ProjectReferenceInclude -split ';'
            for ($i = 0; $i -lt $projRefIncludes.Length; $i++) {
                $projRefInclude = $projRefIncludes[$i]
            
                $refAsmInfo = ResolveAssemblyByProjectReference $projRefInclude
                if ($null -ne  $refAsmInfo) {
                    [void]$refAsmInfos.Add($refAsmInfo)
                }
            }
        }


        Set-Variable ReferencedAssemblies $refAsmInfos.ToArray() -Scope Global -Option ReadOnly

        Set-Location $orgPath
    }
} | Import-Module -PassThru



Import-Module ([System.IO.Path]::Combine((Split-Path $MyInvocation.MyCommand.Path), 'Urasandesu.Prig'))
& $ImportPrigSetupSessionDetail { Main $args[0] } (Split-Path $MyInvocation.MyCommand.Path)
Remove-Module $ImportPrigSetupSessionDetail

if (![string]::IsNullOrEmpty($DeletionalInclude)) {
    Remove-PrigAssembly -pa $DeletionalInclude
}

if (![string]::IsNullOrEmpty($AdditionalInclude)) {
    foreach ($refAsm in $ReferencedAssemblies | ? { $_.GetName().Name -eq $AdditionalInclude }) {
        Add-PrigAssembly -ra $refAsm
    }
}

if (![string]::IsNullOrEmpty($EditorialInclude)) {
'-EditorialInclude parameter is specified. You can also use the global variable $TargetReferencedAssembly in addition to $ReferencedAssemblies. Currently $TargetReferencedAssembly is: '
    foreach ($refAsm in $ReferencedAssemblies | ? { $_.GetName().Name -eq $EditorialInclude }) {
        Set-Variable TargetReferencedAssembly $refAsm -Scope Global -Option ReadOnly
        break
    }
    $TargetReferencedAssembly
''
''
}
