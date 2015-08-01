# 
# File: Chocolatey.Enable-PrigTestAdapter.ps1
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

function Enable-PrigTestAdapter {

    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        $Project
    )

    $envProj = $Project.Object.Project

    try {
        $devenv = [System.Diagnostics.Process]::GetCurrentProcess()
        $devenvId = $devenv.Id
    }
    finally {
        if ($null -ne $devenv) {
            $devenv.Dispose()
        }
    }

    $executionEngineProcess = (Get-WmiObject Win32_Process | ? { $_.ParentProcessId -eq $devenvId -and $_.ProcessName -match 'vstest\.executionengine' })

    if ($null -ne $executionEngineProcess) {
        $executionengineId = $executionEngineProcess.ProcessId
        try {
            $executionengine = [System.Diagnostics.Process]::GetProcessById($executionengineId)
            if ($null -ne $executionengine) {
                $executionengine.Kill()
            }
        }
        finally {
            if ($null -ne $executionengine) {
                $executionengine.Dispose()
            }
        }
    }

    $projDir = [System.IO.Path]::GetDirectoryName($envProj.FullName)
    $outputPath = ($envProj.ConfigurationManager.ActiveConfiguration.Properties | ? { $_.Name -eq 'OutputPath' }).Value
    $targetDir = [System.IO.Path]::Combine($projDir, $outputPath)
    if ([string]::IsNullOrEmpty($targetDir)) {
        throw New-Object System.InvalidOperationException '"$(TargetDir)" has not been able to resolve.'
    }

    [System.Environment]::SetEnvironmentVariable($EnableProfilingKey, $EnableProfilingValueEnabled)
    [System.Environment]::SetEnvironmentVariable($ProfilerKey, $ProfilerValue)
    [System.Environment]::SetEnvironmentVariable($ProfilerCurrentDirectoryKey, $targetDir)
    [System.Environment]::SetEnvironmentVariable($ProfilerTargetProcessNameKey, $ProfilerTargetProcessNameValue)
}
