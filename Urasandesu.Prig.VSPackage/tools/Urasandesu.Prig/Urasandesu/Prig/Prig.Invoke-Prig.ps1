# 
# File: Prig.Invoke-Prig.ps1
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



function Invoke-Prig {
<#
    .SYNOPSIS
        Executes `prig.exe` on the Package Manager Console.

    .DESCRIPTION
        This command is the wrapper to execute `prig.exe`.

        In NuGet, the path to the directory `tools` is automatically appended to $env:Path according to the explanation of the document [Creating And Publishing A Package](http://docs.nuget.org/docs/creating-packages/creating-and-publishing-a-package).
        However, it is enabled only at the installed time. If you restart the Visual Studio, the effect will be lost. Due to resolve the issue, it becomes the mechanism which detects the directory `tools` and executes the process `prig.exe` that is in it.

        The function of this command is same as `prig.exe`. For more details, see the help of `prig.exe` that is shown by the command `Invoke-Prig -Help`.

    .PARAMETER  Mode
        This is same as the subcommand of `prig.exe`. For more details, see also `Invoke-Prig -Help`.

    .PARAMETER  Help
        This is same as the help of `prig.exe`. For more details, see also `Invoke-Prig -Help`.

    .PARAMETER  Process
        This is same as the parameter `-process` which is passed to `run` subcommand in `prig.exe`. For more details, see also `Invoke-Prig run -Help`.

    .PARAMETER  Arguments
        This is same as the parameter `-arguments` which is passed to `run` subcommand in `prig.exe`. For more details, see also `Invoke-Prig run -Help`.

    .PARAMETER  Assembly
        This is same as the parameter `-assembly` which is passed to `dasm` subcommand in `prig.exe`. For more details, see also `Invoke-Prig dasm -Help`.

    .PARAMETER  AssemblyFrom
        This is same as the parameter `-assemblyfrom` which is passed to `dasm` subcommand in `prig.exe`. For more details, see also `Invoke-Prig dasm -Help`.

    .INPUTS
        System.String, System.Management.Automation.SwitchParameter

    .OUTPUTS
        None

    .NOTES
        You can also refer to the Invoke-Prig command by its built-in alias, "prig".

    .LINK
        Add-PrigAssembly

    .LINK
        Find-IndirectionTarget

    .LINK
        Get-IndirectionStubSetting

    .LINK
        Start-PrigSetup

#>

    [CmdletBinding(DefaultParametersetName = 'Runner')]
    param (
        [Parameter(Position = 0)]
        [ValidateSet("", "run", "dasm")] 
        [string]
        $Mode, 

        [switch]
        $Help, 

        [Alias("p")]
        [Parameter(ParameterSetName = 'Runner')]
        [string]
        $Process, 

        [Alias("a")]
        [Parameter(ParameterSetName = 'Runner')]
        [string]
        $Arguments, 

        [Parameter(ParameterSetName = 'DisassemblerWithAssembly')]
        [string]
        $Assembly, 

        [Parameter(ParameterSetName = 'DisassemblerWithAssemblyFrom')]
        [string]
        $AssemblyFrom
    )

    $prig = [System.IO.Path]::Combine((Get-PackageToolsPath), 'prig.exe')
    
    if ([string]::IsNullOrEmpty($Mode) -and $Help) {
        & $prig -help
    } elseif (![string]::IsNullOrEmpty($Mode) -and $Help) {
        & $prig $Mode -help
    } else {
        switch ($PsCmdlet.ParameterSetName) {
            'Runner' { 
                if ([string]::IsNullOrEmpty($Arguments)) {
                    & $prig run -process $Process
                } else {
                    & $prig run -process $Process -arguments $Arguments 
                }
            }
            'DisassemblerWithAssembly' { 
                & $prig dasm -assembly $Assembly
            }
            'DisassemblerWithAssemblyFrom' { 
                & $prig dasm -assemblyfrom $AssemblyFrom
            }
        }
    }
}

New-Alias prig Invoke-Prig
