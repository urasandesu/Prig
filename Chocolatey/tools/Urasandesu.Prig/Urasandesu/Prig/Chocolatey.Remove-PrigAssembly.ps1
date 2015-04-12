# 
# File: Chocolatey.Remove-PrigAssembly.ps1
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

function Remove-PrigAssembly {
<#
    .SYNOPSIS
        Removes the stub settings file for the specified assembly on the Package Manager Console or the Prig setup session.

    .DESCRIPTION
        This command removes the stub setting file that is added by `Add-PrigAssembly` from a project on the Package Manager Console or the Prig setup session.
        Target project is the value that is selected as `Default project: ` on the Package Manager Console. After the file is removed, you will get the confirmation message that the project has been modified externally, so reload the project.

    .PARAMETER  PrigAssembly
        A Prig assembly name.
        If it can't recognize uniquely, the error "Ambiguous match found" will be occurred. So, you have to specify a more detailed Prig assembly name.
        You can also refer to the PrigAssembly parameter by its built-in alias, "pa".

    .PARAMETER  Project
        A `EnvDTE.DTE` object that removes Prig assembly.
        This API supports the Prig infrastructure and is not intended to be used directly from your code.

    .EXAMPLE
        Remove-PrigAssembly -PrigAssembly mscorlib
        
        DESCRIPTION
        -----------
        This command removes the stub setting file for the assembly `mscorlib` from the target project.

    .INPUTS
        System.String

    .OUTPUTS
        None

    .NOTES
        You can also refer to the Remove-PrigAssembly command by its built-in alias, "PRemove".

    .LINK
        Add-PrigAssembly

    .LINK
        Start-PrigSetup

#>

    [CmdletBinding()]
    param (
        [Alias("pa")]
        [Parameter(Mandatory = $true)]
        [string]
        $PrigAssembly, 

        $Project
    )

    # $ProjectFullName are only enabled in Prig setup session. See also `Import-PrigSetupSession.ps1`.
    $projFullName = $ProjectFullName
    if ($null -eq $projFullName) {
        $envProj = $(if ($null -eq $Project) { (Get-Project).Object.Project } else { $Project.Object.Project })
        $projFullName = $envProj.FullName

        [void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Build')
        $msbProjCollection = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection
        $allMsbProjs = $msbProjCollection.GetLoadedProjects($projFullName).GetEnumerator()
        if(!$allMsbProjs.MoveNext()) {
            throw New-Object System.InvalidOperationException ('"{0}" has not been loaded.' -f $projFullName)
        }
        $curMsbProj = $allMsbProjs.Current
    }

    $powershell = 'powershell'

    $removePrigAssembly = [System.IO.Path]::Combine((Get-PackageToolsPath), 'Remove-PrigAssembly.ps1')
    $argList = '-NonInteractive', '-NoLogo', '-NoProfile', '-File', """$removePrigAssembly""", """$projFullName""", """$PrigAssembly"""
    if ($PSCmdlet.MyInvocation.BoundParameters["Verbose"].IsPresent) {
        $argList += '-Verbose'
    }

    $tmpFileName = [System.IO.Path]::GetTempFileName()

    Start-Process $powershell $argList -Wait -RedirectStandardError $tmpFileName -NoNewWindow
    $errors = Get-Content $tmpFileName
    if (0 -lt $errors.Length) {
        $Host.UI.WriteErrorLine($errors -join "`r`n")
    }

    Remove-Item $tmpFileName -ErrorAction SilentlyContinue
}

New-Alias PRemove Remove-PrigAssembly

