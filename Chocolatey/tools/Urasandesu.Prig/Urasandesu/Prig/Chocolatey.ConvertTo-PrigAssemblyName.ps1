# 
# File: Chocolatey.ConvertTo-PrigAssemblyName.ps1
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



function ConvertTo-PrigAssemblyName {

    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true, Position = 0)]
        $Info
    )

    switch ($Info)
    {
        { $_.psobject.TypeNames -contains 'System.Reflection.Assembly' } {  
            $name = $Info.GetName().Name
            $runtimeVer = $Info.ImageRuntimeVersion
            $ver = $Info.GetName().Version
            $procArch = ConvertTo-ProcessorArchitectureString $Info
        }
        { $_.psobject.TypeNames -contains $AssemblyNameExTypeName } {  
            $name = $Info.Name
            $runtimeVer = $Info.ImageRuntimeVersion
            $ver = $Info.Version
            $procArch = ConvertTo-ProcessorArchitectureString $Info
        }
        Default {
            throw New-Object System.ArgumentException ('Info(Type: {0}) is not supported.' -f $Info.GetType())
        }
    }

    '{0}.{1}.v{2}.{3}.Prig' -f $name, $runtimeVer, $ver, $procArch
}
