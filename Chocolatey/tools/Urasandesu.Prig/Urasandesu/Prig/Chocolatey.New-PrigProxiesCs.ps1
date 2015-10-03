# 
# File: Chocolatey.New-PrigProxiesCs.ps1
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



function New-PrigProxiesCs {
    param ($WorkDirectory, $AssemblyInfo, $Section, $TargetFrameworkVersion)

    $results = New-Object System.Collections.ArrayList
    
    foreach ($namespaceGrouped in $Section.GroupedStubs) {
        $dir = $namespaceGrouped.Key -replace '\.', '\'

        foreach ($declTypeGrouped in $namespaceGrouped) {
            if (!(IsPublic $declTypeGrouped.Key) -or $declTypeGrouped.Key.IsValueType) { continue }
            $hasAnyInstanceMember = $false
            $content = @"

using System;
using System.ComponentModel;
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace $(ConcatIfNonEmpty $namespaceGrouped.Key '.')Prig
{
    public class PProxy$(ConvertTypeToClassName $declTypeGrouped.Key) $(ConvertTypeToGenericParameterConstraints $declTypeGrouped.Key)
    {
        Proxy<OfPProxy$(ConvertTypeToClassName $declTypeGrouped.Key)> m_proxy = new Proxy<OfPProxy$(ConvertTypeToClassName $declTypeGrouped.Key)>(OfPProxy$(ConvertTypeToClassName $declTypeGrouped.Key).Type);

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

"@ + $(foreach ($stub in $declTypeGrouped | ? { !$_.Target.IsStatic -and (IsSignaturePublic $_) -and ($_.Target -is [System.Reflection.MethodInfo]) -and (ExistsIndirectionDelegate $_) }) {
        $hasAnyInstanceMember = $true
@"

        public TypedBehaviorPreparable<$(ConvertTypeToClassName $stub.IndirectionDelegate)> $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return m_proxy.Setup<$(ConvertTypeToClassName $stub.IndirectionDelegate)>(_ => _.$(ConvertStubToClassName $stub)());
        }

"@}) + @"

"@ + $(foreach ($stub in $declTypeGrouped | ? { !$_.Target.IsStatic -and !(IsSignaturePublic $_) -and ($_.Target -is [System.Reflection.MethodInfo]) -and (ExistsIndirectionDelegate $_) }) {
        $hasAnyInstanceMember = $true
@"

        public UntypedBehaviorPreparable $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return m_proxy.Setup(_ => _.$(ConvertStubToClassName $stub)());
        }

"@}) + @"


        public static implicit operator $(ConvertTypeToFullName $declTypeGrouped.Key)(PProxy$(ConvertTypeToClassName $declTypeGrouped.Key) @this)
        {
            return ($(ConvertTypeToFullName $declTypeGrouped.Key))@this.m_proxy.Target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            return m_proxy.ExcludeGeneric(new InstanceBehaviorSetting(this));
        }

        public class InstanceBehaviorSetting : BehaviorSetting
        {
            private PProxy$(ConvertTypeToClassName $declTypeGrouped.Key) m_this;

            public InstanceBehaviorSetting(PProxy$(ConvertTypeToClassName $declTypeGrouped.Key) @this)
            {
                m_this = @this;
            }
"@ + $(foreach ($stub in $declTypeGrouped | ? { ($declTypeGrouped.Key.IsGenericType -or $_.Target.IsGenericMethod) -and (IsSignaturePublic $_) -and (ExistsIndirectionDelegate $_) }) {
@"

            public InstanceBehaviorSetting Include$(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
            {
                Include(m_this.$(ConvertStubToClassName $stub)());
                return this;
            }

"@}) + @"

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    m_this.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(m_this.DefaultBehavior);
                }
            }
        }
    }
}
"@
            if (!$hasAnyInstanceMember) { continue }

            $result = 
                New-Object psobject | 
                    Add-Member NoteProperty 'Path' ([System.IO.Path]::Combine($WorkDirectory, "$(ConcatIfNonEmpty $dir '\')PProxy$(ConvertTypeToStubName $declTypeGrouped.Key).g.cs")) -PassThru | 
                    Add-Member NoteProperty 'Content' $content -PassThru
            [Void]$results.Add($result)
        }
    }

    ,$results
}
