# 
# File: Prig.New-PrigTypeIntroducersCs.ps1
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



function New-PrigTypeIntroducersCs {
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
    public class OfP$(ConvertTypeToClassName $declTypeGrouped.Key) : P$(ConvertTypeToBaseName $declTypeGrouped.Key), IPrigTypeIntroducer $(ConvertTypeToGenericParameterConstraints $declTypeGrouped.Key)
    {
"@ + $(foreach ($stub in $declTypeGrouped | ? { IsSignaturePublic $_ }) {
@"

        public virtual ImplFor$(ConvertStubToClassName $stub) $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return new ImplFor$(ConvertStubToClassName $stub)();
        }

        static IndirectionStub ms_stub$(StripGenericParameterCount $stub.Alias) = NewStub$(StripGenericParameterCount $stub.Alias)();
        static IndirectionStub NewStub$(StripGenericParameterCount $stub.Alias)()
        {
            var stubsXml = $(ConvertStubToStubsXml $stub);
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplFor$(ConvertStubToClassName $stub) : TypedBehaviorPreparableImpl $(ConvertStubToGenericParameterConstraints $stub)
        {
            public ImplFor$(StripGenericParameterCount $stub.Alias)()
                : base(ms_stub$(StripGenericParameterCount $stub.Alias), $(ConvertTypeToGenericParameterArray $declTypeGrouped.Key), $(ConvertStubToGenericParameterArray $stub))
            { }
        }

"@}) + @"

"@ + $(foreach ($stub in $declTypeGrouped | ? { !(IsSignaturePublic $_) }) {
@"

        public virtual ImplFor$(ConvertStubToClassName $stub) $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return new ImplFor$(ConvertStubToClassName $stub)();
        }

        static IndirectionStub ms_stub$(StripGenericParameterCount $stub.Alias) = NewStub$(StripGenericParameterCount $stub.Alias)();
        static IndirectionStub NewStub$(StripGenericParameterCount $stub.Alias)()
        {
            var stubsXml = $(ConvertStubToStubsXml $stub);
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ImplFor$(ConvertStubToClassName $stub) : UntypedBehaviorPreparableImpl $(ConvertStubToGenericParameterConstraints $stub)
        {
            public ImplFor$(StripGenericParameterCount $stub.Alias)()
                : base(ms_stub$(StripGenericParameterCount $stub.Alias), $(ConvertTypeToGenericParameterArray $declTypeGrouped.Key), $(ConvertStubToGenericParameterArray $stub))
            { }
        }

"@}) + @"


        public static Type Type
        {
"@ + $(foreach ($stub in $declTypeGrouped) {
@"

            get { return ms_stub$(StripGenericParameterCount $stub.Alias).GetDeclaringTypeInstance($(ConvertTypeToGenericParameterArray $declTypeGrouped.Key)); }

"@
                break
            }) + @"
        }
    }
}
"@
            $result = 
                New-Object psobject | 
                    Add-Member NoteProperty 'Path' ([System.IO.Path]::Combine($WorkDirectory, "$(ConcatIfNonEmpty $dir '\')OfP$(ConvertTypeToStubName $declTypeGrouped.Key).g.cs")) -PassThru | 
                    Add-Member NoteProperty 'Content' $content -PassThru
            [Void]$results.Add($result)
        }
    }

    ,$results
}
