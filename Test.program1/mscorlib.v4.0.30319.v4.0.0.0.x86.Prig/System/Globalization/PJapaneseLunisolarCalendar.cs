
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PJapaneseLunisolarCalendar : PJapaneseLunisolarCalendarBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzGetYearInfo GetYearInfo() 
        {
            return new zzGetYearInfo();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetYearInfo : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetYearInfo_int_int;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(JapaneseLunisolarCalendar target, IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetYearInfo, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzGetYearInfo>.Make(new Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(JapaneseLunisolarCalendar target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetYearInfo, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzGetEra GetEra() 
        {
            return new zzGetEra();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetEra : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetEra;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(JapaneseLunisolarCalendar target, IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetEra, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzGetEra>.Make(new Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(JapaneseLunisolarCalendar target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetEra, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzGetGregorianYear GetGregorianYear() 
        {
            return new zzGetGregorianYear();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetGregorianYear : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetGregorianYear;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(JapaneseLunisolarCalendar target, IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetGregorianYear, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzGetGregorianYear>.Make(new Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(JapaneseLunisolarCalendar target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetGregorianYear, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PJapaneseLunisolarCalendar).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>();
            var setting = new TypeBehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PJapaneseLunisolarCalendar.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PJapaneseLunisolarCalendar.DefaultBehavior);
                }
            }
        }
    }
}
