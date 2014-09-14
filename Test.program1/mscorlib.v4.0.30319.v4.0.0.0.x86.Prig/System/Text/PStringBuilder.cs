
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Text.Prig
{
    public class PStringBuilder : PStringBuilderBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzInsertInt32StringInt32 InsertInt32StringInt32() 
        {
            return new zzInsertInt32StringInt32();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInsertInt32StringInt32 : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfInsertInt32StringInt32;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(StringBuilder target, IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzInsertInt32StringInt32, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzInsertInt32StringInt32>.Make(new Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(StringBuilder target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzInsertInt32StringInt32, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzReplaceCharCharInt32Int32 ReplaceCharCharInt32Int32() 
        {
            return new zzReplaceCharCharInt32Int32();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzReplaceCharCharInt32Int32 : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfReplaceCharCharInt32Int32;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(StringBuilder target, IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzReplaceCharCharInt32Int32, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzReplaceCharCharInt32Int32>.Make(new Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(StringBuilder target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzReplaceCharCharInt32Int32, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PStringBuilder).GetNestedTypes().
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
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PStringBuilder.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PStringBuilder.DefaultBehavior);
                }
            }
        }
    }
}
