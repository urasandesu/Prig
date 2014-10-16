# 
# File: NuGet.Find-IndirectionTarget.ps1
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

function Find-IndirectionTarget {
<#
    .SYNOPSIS
        Finds the targets to create the indirection setting on the Package Console Manager or PowerShell.

    .DESCRIPTION
        This command finds the methods that are satisfied the search condition on the Package Manager Console or PowerShell.

        About the search way, first, it enumerates the members that are satisfied the following conditions from the specified parameter as `-InputObject`: 
        * a public member or a non-public member
        * a instance member or a static member
        * a only member declared at the level of the supplied type's hierarchy
        * a constructor(containing a type constructor) or a method
        * a non-abstract members
        
        From them, it narrow the search to the results that are matched to specified the parameter `-Method`.
        The string that is compared with `-Method` is same as the string that is listed as the results this command invoked.

    .PARAMETER  InputObject
        An array of type or an array of string that can be recognized as a type.
        "The string that can be recognized as a type" means that `Invoke-Expression ('[{0}]' -f $s)` - NOTE: $s is string - can evaluate to `Sytem.Type` object.
        Therefore, you can specify the parameter like `datetime`, `system.reflection.assembly`, `([int])` and so on.

    .PARAMETER  Method
        A search condition for the methods. You can use regular expression.

    .EXAMPLE
        Find-IndirectionTarget datetime '(today)|(now)'

        Method
        ------
        System.DateTime get_Now()
        System.DateTime get_UtcNow()
        System.DateTime get_Today()

        DESCRIPTION
        -----------
        This command finds the members that contain the string `today` or `now` from the type `System.DateTime`.

    .EXAMPLE
        PS C:\>$asmInfo = [System.Reflection.Assembly]::LoadWithPartialName("System.Web")
        PS C:\>$asmInfo.GetTypes() | ? { $_.Name -eq 'httpcontext' }
        
        IsPublic IsSerial Name                                     BaseType
        -------- -------- ----                                     --------
        True     False    HttpContext                              System.Object
        
        
        PS C:\>$asmInfo.GetTypes() | ? { $_.Name -eq 'httpcontext' } | pfind -m 'get_current\b'
        
        Method
        ------
        System.Web.HttpContext get_Current()
        
        DESCRIPTION
        -----------
        In this example, first, it finds the types that is named `httpcontext` from the assembly `System.Web`.
        Against the results, use this command, and find the members that are matched the regular expression `get_current\b`.

    .EXAMPLE
        C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug>ipmo ..\..\..\packages\Prig.0.0.0-alpha10\tools\Urasandesu.Prig
        C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug>$asmInfo = [System.Reflection.Assembly]::LoadFrom((dir .\DemoLibrary.dll).FullName)
        C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug>$asmInfo.GetTypes()
        
        IsPublic IsSerial Name                                     BaseType
        -------- -------- ----                                     --------
        True     False    Foo                                      System.Object
        
        
        C:\users\akira\documents\visual studio 2013\Projects\Demo\Demo\bin\Debug>$asmInfo.GetTypes() | pfind
        
        Method
        ------
        Void .ctor()
        
        DESCRIPTION
        -----------
        This is the example that doesn't use the Package Manager Console but use PowerShell.

        By the way, the Package Manager Console doesn't support nested prompt by its design. Therefore, there is the problem that it can never release the assemblies 
        if it loaded the assemblies to analyze indirection targets once. In the first place, the features that are used mundanely - such as autocompletion, 
        commands history and so on - are less functionality than PowerShell's.

        I recommend that you always use PowerShell when analyzing the indirection targets.

    .INPUTS
        System.String, System.String[], System.Type, System.Type[]

    .OUTPUTS
        None, System.Reflection.MethodBase, System.Reflection.MethodBase[]

    .NOTES
        You have to import the module `Urasandesu.Prig` explicitly if you use this command on PowerShell.
        The module `Urasandesu.Prig` is placed the directory `tools` of the package directory by NuGet when you installed Prig.
        So, execute `Import-Module` it if you work on PowerShell.
        
        You can also refer to the Find-IndirectionTarget command by its built-in alias, "PFind".

    .LINK
        Add-PrigAssembly

    .LINK
        Get-IndirectionStubSetting

    .LINK
        Invoke-Prig

#>

    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
        $InputObject, 
    
        [Parameter(Position = 1)]
        $Method 
    )

    begin {

    } process {
        if ($null -ne $InputObject) {
            foreach ($type in $InputObject) {
                $typeInfo = $type
                if ($typeInfo -is [string]) {
                    $typeInfo = $(try { Invoke-Expression $type } catch { })
                    if ($null -eq $typeInfo) {
                        $typeInfo = $(try { Invoke-Expression ('[{0}]' -f $type) } catch { })
                    }
                }
                if ($typeInfo -isnot [type]) {
                    throw New-Object System.ArgumentException '-Type option must be a type or a string that can parse to a type(e.g. -Type ([datetime]) or -Type datetime).'
                }

                $memberInfos = $typeInfo.GetMembers([System.Reflection.BindingFlags]'Public, NonPublic, Instance, Static, DeclaredOnly')
                $indirectlyCallables = $memberInfos | ? { $_ -is [System.Reflection.MethodBase] } | ? { !$_.IsAbstract }
                if ($null -ne $Method) {
                    foreach ($indirectlyCallable in $indirectlyCallables) {
                        if ($indirectlyCallable.ToString() -match $Method) {
                            $indirectlyCallable
                        }
                    }
                } else {
                    $indirectlyCallables
                }
            }
        }
    } end {

    }
}

New-Alias PFind Find-IndirectionTarget
