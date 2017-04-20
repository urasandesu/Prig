# 
# File: Unregister-Prig.ps1
# 
# Author: Akira Sugiura (urasandesu@gmail.com)
# 
# 
# Copyright (c) 2016 Akira Sugiura
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
)

$procVars = New-Object hashtable ([System.Environment]::GetEnvironmentVariables())
foreach ($level in "Machine","User") {
    [System.Environment]::GetEnvironmentVariables($level).GetEnumerator() | % {
        if($_.Name -match 'Path$') { 
            $values = @{ }
            foreach ($value in $procVars[$_.Name] -split ';') {
                $values[$value] = $null
            }
            foreach ($value in $_.Value -split ';') {
                $values.Remove($value)
            }
            $procVars[$_.Name] = ($values.GetEnumerator() | % { $_.Name }) -join ';'
        } else {
            $procVars.Remove($_.Name)
        }
    }
}

& .\prig-vsix unregister

[System.Environment]::GetEnvironmentVariables().GetEnumerator() | % {
    [System.Environment]::SetEnvironmentVariable($_.Name, $null)
}

$procVars.GetEnumerator() | % {
    [System.Environment]::SetEnvironmentVariable($_.Name, $_.Value)
}

foreach ($level in "Machine","User") {
    [System.Environment]::GetEnvironmentVariables($level).GetEnumerator() | % {
        if($_.Name -match 'Path$' -and (Test-Path "Env:$($_.Name)")) { 
            $_.Value = ($((Get-Content "Env:$($_.Name)") + ";$($_.Value)") -split ';' | select -Unique) -join ';'
        }
        $_
    } | Set-Content -Path { "Env:$($_.Name)" }
}
