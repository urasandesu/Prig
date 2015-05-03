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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.NAnonym;
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;

namespace $(ConcatIfNonEmpty $namespaceGrouped.Key '.')Prig
{
    public class P$(ConvertTypeToClassName $declTypeGrouped.Key) : P$(ConvertTypeToBaseName $declTypeGrouped.Key) $(ConvertTypeToGenericParameterConstraints $declTypeGrouped.Key)
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

"@ + $(foreach ($stub in $declTypeGrouped | ? { IsSignaturePublic $_ }) {
@"

        public static zz$(ConvertStubToClassName $stub) $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return new zz$(ConvertStubToClassName $stub)();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zz$(ConvertStubToClassName $stub) : IBehaviorPreparable $(ConvertStubToGenericParameterConstraints $stub)
        {
            public $(ConvertTypeToClassName $stub.IndirectionDelegate) Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<$(ConvertTypeToClassName $stub.IndirectionDelegate)>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<$(ConvertTypeToClassName $stub.IndirectionDelegate)>>();
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(Body);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperFor$(ConvertTypeToClassName $stub.IndirectionDelegate).CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "$($AssemblyInfo.FullName)";
                    info.Token = TokenOf$($stub.Name);
                    return info;
                }
            }
"@ + $(if (!$stub.Target.IsStatic -and !$declTypeGrouped.Key.IsValueType) {
@"

            internal void SetTargetInstanceBody($(ConvertTypeToFullName $declTypeGrouped.Key) target, $(ConvertTypeToClassName $stub.IndirectionDelegate) value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zz$(ConvertStubToClassName $stub), Dictionary<$(ConvertTypeToFullName $declTypeGrouped.Key), TargetSettingValue<$(ConvertTypeToClassName $stub.IndirectionDelegate)>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zz$(ConvertStubToClassName $stub)>.Make(new Dictionary<$(ConvertTypeToFullName $declTypeGrouped.Key), TargetSettingValue<$(ConvertTypeToClassName $stub.IndirectionDelegate)>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? HelperFor$(ConvertTypeToClassName $stub.IndirectionDelegate).CreateDelegateOfDefaultBehavior(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<$(ConvertTypeToClassName $stub.IndirectionDelegate)>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = HelperFor$(ConvertTypeToClassName $stub.IndirectionDelegate).CreateDelegateExecutingDefaultOr(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<$(ConvertTypeToClassName $stub.IndirectionDelegate)>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody($(ConvertTypeToFullName $declTypeGrouped.Key) target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zz$(ConvertStubToClassName $stub), Dictionary<$(ConvertTypeToFullName $declTypeGrouped.Key), TargetSettingValue<$(ConvertTypeToClassName $stub.IndirectionDelegate)>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<$(ConvertTypeToClassName $stub.IndirectionDelegate)>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
"@}) + @"

        }

"@}) + @"

"@ + $(foreach ($stub in $declTypeGrouped | ? { !(IsSignaturePublic $_) }) {
@"

        public static zz$(ConvertStubToClassName $stub) $(ConvertStubToClassName $stub)() $(ConvertStubToGenericParameterConstraints $stub)
        {
            return new zz$(ConvertStubToClassName $stub)();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zz$(ConvertStubToClassName $stub) : IBehaviorPreparable $(ConvertStubToGenericParameterConstraints $stub)
        {
            public Work Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessorUntyped.GetOrRegister(Stub.Target, Stub.IndirectionDelegate, $(ConvertTypeToGenericParameterArray $declTypeGrouped.Key), $(ConvertStubToGenericParameterArray $stub));
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessorUntyped.GetOrRegister(Stub.Target, Stub.IndirectionDelegate, $(ConvertTypeToGenericParameterArray $declTypeGrouped.Key), $(ConvertStubToGenericParameterArray $stub));
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(value);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var indDlgt = IndirectionHolderUntyped.MakeGenericInstance(Stub.Target, Stub.IndirectionDelegate, $(ConvertTypeToGenericParameterArray $declTypeGrouped.Key), $(ConvertStubToGenericParameterArray $stub));
                var behavior = HelperForUntypedIndirectionDelegate.CreateDelegateOfDefaultBehavior(indDlgt, defaultBehavior);
                Body = behavior;
            }

            IndirectionStub m_stub;
            public IndirectionStub Stub
            {
                get
                {
                    if (m_stub == null)
                    {
                        var stubsXml = $(ConvertStubToStubsXml $stub);
                        var section = new PrigSection();
                        section.DeserializeStubs(stubsXml);
                        m_stub = section.Stubs.First();
                    }
                    return m_stub;
                }
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "$($AssemblyInfo.FullName)";
                    info.Token = TokenOf$($stub.Name);
                    return info;
                }
            }
"@ + $(if (!$stub.Target.IsStatic -and !$declTypeGrouped.Key.IsValueType) {
@"

            internal void SetTargetInstanceBody(object target, Work value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zz$(ConvertStubToClassName $stub), Dictionary<object, TargetSettingValue<Work>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zz$(ConvertStubToClassName $stub)>.Make(new Dictionary<object, TargetSettingValue<Work>>());

                if (holder.Source.Value.Count == 0)
                {
                    var indDlgt = IndirectionHolderUntyped.MakeGenericInstance(Stub.Target, Stub.IndirectionDelegate, $(ConvertTypeToGenericParameterArray $declTypeGrouped.Key), $(ConvertStubToGenericParameterArray $stub));
                    var behavior = Body == null ? HelperForUntypedIndirectionDelegate.CreateDelegateOfDefaultBehavior(indDlgt, IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<Work>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = HelperForUntypedIndirectionDelegate.CreateDelegateExecutingDefaultOr(indDlgt, behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<Work>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(object target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zz$(ConvertStubToClassName $stub), Dictionary<object, TargetSettingValue<Work>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<Work>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
"@}) + @"

        }

"@}) + @"


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(P$(ConvertTypeToClassName $declTypeGrouped.Key)).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>();
            var setting = new TypeBehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class TypeBehaviorSetting : BehaviorSetting
        {
"@ + $(foreach ($stub in $declTypeGrouped | ? { ($declTypeGrouped.Key.IsGenericType -or $_.Target.IsGenericMethod) -and (IsSignaturePublic $_) }) {
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
