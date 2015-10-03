# 
# File: Chocolatey.New-PrigProxyTypeIntroducersCs.ps1
# 
# Author: Akira Sugiura (urasandesu@gmail.com)
# 
# 
# Copyright (c) 2015 Akira Sugiura
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



function New-PrigProxyTypeIntroducersCs {
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
using Urasandesu.NAnonym;
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace $(ConcatIfNonEmpty $namespaceGrouped.Key '.')Prig
{
    public class OfPProxy$(ConvertTypeToClassName $declTypeGrouped.Key) : OfP$(ConvertTypeToClassName $declTypeGrouped.Key), IPrigProxyTypeIntroducer $(ConvertTypeToGenericParameterConstraints $declTypeGrouped.Key)
    {
        object m_target;
        
        void IPrigProxyTypeIntroducer.Initialize(object target)
        {
            m_target = target;
        }

"@ + $(foreach ($stub in $declTypeGrouped | ? { !$_.Target.IsStatic -and (IsSignaturePublic $_) -and ($_.Target -is [System.Reflection.MethodInfo]) }) {
        $hasAnyInstanceMember = $true
@"

        public override OfP$(ConvertTypeToClassName $declTypeGrouped.Key).ImplFor$(ConvertStubToClassName $stub) $(ConvertStubToClassName $stub)()
        {
            return new ImplFor$(ConvertStubToClassName $stub)(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class ImplFor$(ConvertStubToClassName $stub) : OfP$(ConvertTypeToClassName $declTypeGrouped.Key).ImplFor$(ConvertStubToClassName $stub) $(ConvertStubToGenericParameterConstraints $stub)
        {
            object m_target;

            public ImplFor$(StripGenericParameterCount $stub.Alias)(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<ImplFor$(ConvertStubToClassName $stub)>(m_target);
                    else
                        SetTargetInstanceBody<ImplFor$(ConvertStubToClassName $stub)>(m_target, value);
                }
            }
        }

"@}) + @"

"@ + $(foreach ($stub in $declTypeGrouped | ? { !$_.Target.IsStatic -and !(IsSignaturePublic $_) -and ($_.Target -is [System.Reflection.MethodInfo]) }) {
        $hasAnyInstanceMember = $true
@"

        public override OfP$(ConvertTypeToClassName $declTypeGrouped.Key).ImplFor$(ConvertStubToClassName $stub) $(ConvertStubToClassName $stub)()
        {
            return new ImplFor$(ConvertStubToClassName $stub)(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class ImplFor$(ConvertStubToClassName $stub) : OfP$(ConvertTypeToClassName $declTypeGrouped.Key).ImplFor$(ConvertStubToClassName $stub) $(ConvertStubToGenericParameterConstraints $stub)
        {
            object m_target;

            public ImplFor$(StripGenericParameterCount $stub.Alias)(object target)
                : base()
            {
                m_target = target;
            }

            public override Work Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<ImplFor$(ConvertStubToClassName $stub)>(m_target);
                    else
                        SetTargetInstanceBody<ImplFor$(ConvertStubToClassName $stub)>(m_target, value);
                }
            }
        }

"@}) + @"

    }
}
"@
            if (!$hasAnyInstanceMember) { continue }

            $result = 
                New-Object psobject | 
                    Add-Member NoteProperty 'Path' ([System.IO.Path]::Combine($WorkDirectory, "$(ConcatIfNonEmpty $dir '\')OfPProxy$(ConvertTypeToStubName $declTypeGrouped.Key).g.cs")) -PassThru | 
                    Add-Member NoteProperty 'Content' $content -PassThru
            [Void]$results.Add($result)
        }
    }

    ,$results
}
