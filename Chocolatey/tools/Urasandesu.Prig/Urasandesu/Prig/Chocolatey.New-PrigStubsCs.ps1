# 
# File: Chocolatey.New-PrigStubsCs.ps1
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



function New-PrigStubsCs {
    param ($WorkDirectory, $AssemblyInfo, $Section, $TargetFrameworkVersion)

    $results = New-Object System.Collections.ArrayList
    
    foreach ($namespaceGrouped in $Section.GroupedStubs) {
        $dir = $namespaceGrouped.Key -replace '\.', '\'

        foreach ($declTypeGrouped in $namespaceGrouped) {
            $content = @"

using System;
using System.ComponentModel;
using System.Linq;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace $(ConcatIfNonEmpty $namespaceGrouped.Key '.')Prig
{
    public class P$(ConvertTypeToClassName $declTypeGrouped.Key) : P$(ConvertTypeToBaseName $declTypeGrouped.Key) $(ConvertTypeToGenericParameterConstraints $declTypeGrouped.Key)
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

"@ + $(foreach ($stub in $declTypeGrouped | ? { (IsSignaturePublic $_) -and (ExistsIndirectionDelegate $_) }) {
@"

        public static TypedBehaviorPreparable<$(ConvertTypeToFullName $stub.IndirectionDelegate)> $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return Stub<OfP$(ConvertTypeToClassName $declTypeGrouped.Key)>.Setup<$(ConvertTypeToFullName $stub.IndirectionDelegate)>(_ => _.$(ConvertStubToClassName $stub)());
        }

"@}) + @"

"@ + $(foreach ($stub in $declTypeGrouped | ? { !(IsSignaturePublic $_) -and (ExistsIndirectionDelegate $_) }) {
@"

        public static UntypedBehaviorPreparable $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return Stub<OfP$(ConvertTypeToClassName $declTypeGrouped.Key)>.Setup(_ => _.$(ConvertStubToClassName $stub)());
        }

"@}) + @"


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfP$(ConvertTypeToClassName $declTypeGrouped.Key)>.ExcludeGeneric(new TypeBehaviorSetting());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class TypeBehaviorSetting : BehaviorSetting
        {
"@ + $(foreach ($stub in $declTypeGrouped | ? { ($declTypeGrouped.Key.IsGenericType -or $_.Target.IsGenericMethod) -and (IsSignaturePublic $_) -and (ExistsIndirectionDelegate $_) }) {
@"

            public TypeBehaviorSetting Include$(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
            {
                Include(P$(ConvertTypeToClassName $declTypeGrouped.Key).$(ConvertStubToClassName $stub)());
                return this;
            }

"@}) + @"

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    P$(ConvertTypeToClassName $declTypeGrouped.Key).DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(P$(ConvertTypeToClassName $declTypeGrouped.Key).DefaultBehavior);
                }
            }
        }
    }
}
"@
            $result = 
                New-Object psobject | 
                    Add-Member NoteProperty 'Path' ([System.IO.Path]::Combine($WorkDirectory, "$(ConcatIfNonEmpty $dir '\')P$(ConvertTypeToStubName $declTypeGrouped.Key).g.cs")) -PassThru | 
                    Add-Member NoteProperty 'Content' $content -PassThru
            [Void]$results.Add($result)
        }
    }

    ,$results
}
