
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

        public static zzInsert Insert() 
        {
            return new zzInsert();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInsert : IBehaviorPreparable 
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
                    info.Token = TokenOfInsert_int_string_int;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(StringBuilder target, IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzInsert, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzInsert>.Make(new Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>());

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
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzInsert, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>>>>();
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
 
        public static zzReplace Replace() 
        {
            return new zzReplace();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzReplace : IBehaviorPreparable 
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
                    info.Token = TokenOfReplace_char_char_int_int;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(StringBuilder target, IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzReplace, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzReplace>.Make(new Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>());

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
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzReplace, Dictionary<StringBuilder, TargetSettingValue<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>>>>();
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
