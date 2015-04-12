# 
# File: Chocolatey.Add-PrigAssembly.ps1
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
        Creates the stub settings file for the specified assembly on the Package Manager Console or the Prig setup session.

    .DESCRIPTION
        This command adds the stub setting file that sets up the indirection settings for specified assembly to a project on the Package Manager Console or the Prig setup session.
        Target project is the value that is selected as `Default project: ` on the Package Manager Console. The stub setting file is named like `<assembly name>.<runtime version>.v<assembly version>.prig`. After the file is added, you will get the confirmation message that the project has been modified externally, so reload the project.
        Also, you can't add the setting file for same assembly. If you want to add it again, execute `Remove-PrigAssembly` command once to remove the setting file.

    .PARAMETER  Assembly
        A display name recognizing uniquely an assembly. 
        Use this parameter if adding the stub setting file for a GAC registered assembly. If it can't recognize uniquely, the error "Ambiguous match found" will be occurred. So, you have to specify a more detailed display name.
        You can also refer to the Assembly parameter by its built-in alias, "as".

    .PARAMETER  AssemblyFrom
        A full path recognizing uniquely an assembly. 
        Use this parameter if adding the stub setting file for a GAC unregistered assembly. Also, you have to change to the directory where specified assembly exists if using this parameter; otherwise referenced assemblies may not resolved.
        You can also refer to the AssemblyFrom parameter by its built-in alias, "af".

    .PARAMETER  ReferencedAssembly
        A referenced assembly object. 
        You can use this parameter only in the Prig setup session. If you specify this parameter in another environment, the error "Invalid operation is detected" will be occurred. For more detail, please see the help for `Start-PrigSetup`.
        You can also refer to the ReferencedAssembly parameter by its built-in alias, "ra".

    .PARAMETER  Project
        A `EnvDTE.DTE` object that adds Prig assembly.
        This API supports the Prig infrastructure and is not intended to be used directly from your code.

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
        This is example that "You try to add the stub setting file for the assembly `mscorlib`, but the error 'Ambiguous match found' is occurred". In this error, candidate assemblies are listed at the time. So, select again more detailed display name from them.

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
        This command adds the stub setting file for the GAC unregistered assembly `DemoLibrary` to the target project. If you specify `-AssemblyFrom` parameter, you have to execute this command in the directory that the target assembly exists like the example.

    .EXAMPLE
        padd -ra $ReferencedAssemblies[0]
        
        DESCRIPTION
        -----------
        This command adds the stub setting file for the assembly `mscorlib` in the Prig setup session. In the session, you can confirm the referenced assemblies of current project through the global variable `$ReferencedAssemblies`, also you can pass it to `Add-PrigAssembly` command as it is. For more detail, please see the help for `Start-PrigSetup`.

    .INPUTS
        System.String, System.Reflection.Assembly

    .OUTPUTS
        None

    .NOTES
        You can also refer to the Add-PrigAssembly command by its built-in alias, "PAdd".

    .LINK
        Find-IndirectionTarget

    .LINK
        Get-IndirectionStubSetting

    .LINK
        Remove-PrigAssembly

    .LINK
        Start-PrigSetup

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
        $AssemblyFrom, 

        [Alias("ra")]
        [Parameter(Mandatory = $true, ParameterSetName = 'ReferencedAssembly')]
        [System.Reflection.Assembly]
        $ReferencedAssembly, 

        $Project
    )

    # $ProjectFullName and $TargetFrameworkVersion are only enabled in Prig setup session. See also `Import-PrigSetupSession.ps1`.
    $projFullName = $ProjectFullName
    $targetFrmwrkVer = $TargetFrameworkVersion
    if ($null -eq $projFullName) {
        if(![string]::IsNullOrEmpty($ReferencedAssembly)) {
            throw New-Object System.InvalidOperationException 'The parameter "ReferencedAssembly" can specify only in the Prig setup session. Please see the help "pstart" for more details.'
        }

        $envProj = $(if ($null -eq $Project) { (Get-Project).Object.Project } else { $Project.Object.Project })
        $projFullName = $envProj.FullName

        [void][System.Reflection.Assembly]::LoadWithPartialName('Microsoft.Build')
        $msbProjCollection = [Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection
        $allMsbProjs = $msbProjCollection.GetLoadedProjects($projFullName).GetEnumerator()
        if(!$allMsbProjs.MoveNext()) {
            throw New-Object System.InvalidOperationException ('"{0}" has not been loaded.' -f $projFullName)
        }
        $curMsbProj = $allMsbProjs.Current
        $targetFrmwrkVer = $curMsbProj.GetProperty('TargetFrameworkVersion').EvaluatedValue
    }

    if ($null -ne $ReferencedAssembly) {
        if ($ReferencedAssembly.GlobalAssemblyCache) {
            $Assembly = $ReferencedAssembly.FullName
        } else {
            $AssemblyFrom = $ReferencedAssembly.Location
        }
    }

    $powershell = 'powershell'

    $addPrigAssembly = [System.IO.Path]::Combine((Get-PackageToolsPath), 'Add-PrigAssembly.ps1')
    $argList = '-NonInteractive', '-NoLogo', '-NoProfile', '-File', """$addPrigAssembly""", """$projFullName""", """$targetFrmwrkVer""", """$Assembly""", """$AssemblyFrom"""
    if ($PSCmdlet.MyInvocation.BoundParameters["Verbose"].IsPresent) {
        $argList += '-Verbose'
    }

    $tmpFileName = [System.IO.Path]::GetTempFileName()

    Start-Process $powershell $argList -Wait -RedirectStandardError $tmpFileName -NoNewWindow
    $errors = Get-Content $tmpFileName
    Remove-Item $tmpFileName -ErrorAction SilentlyContinue
    if (0 -lt $errors.Length) {
        $Host.UI.WriteErrorLine($errors -join "`r`n")
    }
}

New-Alias PAdd Add-PrigAssembly

