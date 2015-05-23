# 
# File: Remove-PrigAssembly.ps1
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
    $PrigAssembly
)

Write-Verbose ('ProjectFullName          : {0}' -f $ProjectFullName)
Write-Verbose ('PrigAssembly             : {0}' -f $PrigAssembly)



Import-Module ([System.IO.Path]::Combine((Split-Path $MyInvocation.MyCommand.Path), 'Urasandesu.Prig'))



function EraseStubberPreBuildEventProperty {

    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]
        $MSBuildProject,

        [Parameter(Mandatory = $true)]
        [string]
        $EscapedPrigAssembly
    )
    
    $targetNames = 'BeforeBuild', 'BeforeRebuild'
    foreach ($targetName in $targetNames) {
        $targets = 
            $MSBuildProject.Xml.Targets | 
                Where-Object { $_.Name -eq $targetName }
        if ($targets.Length -eq 0) {
            continue;
        }

        $execs = 
            $targets[0].Children | 
                Where-Object { $_.Name -eq 'Exec' } | 
                Where-Object { $_.GetParameter('Command') -cmatch '\bPrig\b' }
        if ($execs.Length -eq 0) {
            continue;
        }

        for ($i = 0; $i -lt $execs.Length; $i++) {
            $command = $execs[$i].GetParameter('Command')
            $cmds = @($command -split "`r`n")
            $remainings = @($cmds | ? { $_ -notmatch $EscapedPrigAssembly })
            $command = $remainings -join "`r`n"
            if ([string]::IsNullOrEmpty($command)) {
                $targets[0].RemoveChild($execs[$i])
            } else {
                $execs[$i].SetParameter('Command', $command)
            }
        }

        if ($targets[0].Children.Count -eq 0) {
            $MSBuildProject.Xml.RemoveChild($targets[0])
        }
    }
}



function EraseStubItem {

    param (
        [Parameter(Mandatory = $true)]
        [Microsoft.Build.Evaluation.Project]
        $MSBuildProject,

        [Parameter(Mandatory = $true)]
        [string]
        $EscapedPrigAssembly
    )

    $items = $MSBuildProject.ItemsIgnoringCondition | ? { $_.EvaluatedInclude -match "\.prig\b" } | ? { $_.EvaluatedInclude -match $EscapedPrigAssembly }
    foreach ($item in $items) {
        [void]$MSBuildProject.RemoveItem($item)
    }
}



[void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Build')
$msbProjCollection = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection

$curMsbProj = $msbProjCollection.LoadProject($ProjectFullName)
$curMsbProj.MarkDirty()

$escapedPrigAssembly = [regex]::Escape($PrigAssembly)
$items = 
    $curMsbProj.ItemsIgnoringCondition | 
        Where-Object { $_.ItemType -eq "None" } | 
        Where-Object { $_.EvaluatedInclude -match "\.prig\b" } | 
        Where-Object { $_.EvaluatedInclude -match $escapedPrigAssembly }

if ($items.Length -eq 0) {
    $Host.UI.WriteErrorLine(('Prig Assembly ''{0}'' is not found.' -f $PrigAssembly))
    exit 737914319
}

if (1 -lt $items.Count) {
    $Host.UI.WriteErrorLine(("Ambiguous match found: `r`n{0}" -f ([string]::Join("`r`n", ($items | % { $_.EvaluatedInclude })))))
    exit -1786925265
}

EraseStubberPreBuildEventProperty $curMsbProj $escapedPrigAssembly
EraseStubItem $curMsbProj $escapedPrigAssembly

$curMsbProj.Save()
