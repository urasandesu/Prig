# 
# File: NuGet.ConvertTo-ProcessorArchitectureString.ps1
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



function ConvertTo-ProcessorArchitectureString {

    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $True)]
        $Info
    )

    switch ($Info)
    {
        { $_.psobject.TypeNames -contains 'System.Reflection.Assembly' } {  
            $procArch = [string]$Info.GetName().ProcessorArchitecture 
        }
        { $_.psobject.TypeNames -contains 'System.Reflection.AssemblyName' } {  
            $procArch = [string]$Info.ProcessorArchitecture 
        }
        { $_.psobject.TypeNames -contains $AssemblyNameExTypeName } {  
            $procArch = $Info.ProcessorArchitecture 
        }
        { $_ -is [string] } { 
            $procArch = $Info 
        }
        Default { 
            throw New-Object System.ArgumentException ('Parameter $Info({0}) is not supported.' -f $Info.GetType()) 
        }
    }

    switch ($procArch)
    {
        'X86'                           { "x86"; break }
        { $_ -match '(Amd64)|(x64)' }   { "AMD64"; break }
        { $_ -match 'AnyCPU\|true' }    { "x86"; break }
        { $_ -match '(MSIL)|(AnyCPU)' } { "MSIL"; break }
        Default                         { "MSIL"; break }
    }
}
