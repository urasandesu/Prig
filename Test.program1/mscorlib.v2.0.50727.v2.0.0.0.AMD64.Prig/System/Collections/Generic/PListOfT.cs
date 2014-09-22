
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Collections.Generic.Prig
{
    public class PList<T> : PListBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzAddT AddT() 
        {
            return new zzAddT();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddT : IBehaviorPreparable 
        {
            public IndirectionAction<System.Collections.Generic.List<T>, T> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<System.Collections.Generic.List<T>, T>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<System.Collections.Generic.List<T>, T>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<System.Collections.Generic.List<T>, T>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfAddT;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(System.Collections.Generic.List<T> target, IndirectionAction<System.Collections.Generic.List<T>, T> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAddT, Dictionary<System.Collections.Generic.List<T>, TargetSettingValue<IndirectionAction<System.Collections.Generic.List<T>, T>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzAddT>.Make(new Dictionary<System.Collections.Generic.List<T>, TargetSettingValue<IndirectionAction<System.Collections.Generic.List<T>, T>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<System.Collections.Generic.List<T>, T>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<System.Collections.Generic.List<T>, T>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionAction<System.Collections.Generic.List<T>, T>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<System.Collections.Generic.List<T>, T>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(System.Collections.Generic.List<T> target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAddT, Dictionary<System.Collections.Generic.List<T>, TargetSettingValue<IndirectionAction<System.Collections.Generic.List<T>, T>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<System.Collections.Generic.List<T>, T>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PList<T>).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeAddT() 
            {
                Include(PList<T>.AddT());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PList<T>.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PList<T>.DefaultBehavior);
                }
            }
        }
    }
}
