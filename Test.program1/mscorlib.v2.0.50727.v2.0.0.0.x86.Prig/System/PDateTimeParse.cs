
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PDateTimeParse : PDateTimeParseBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzTryParse TryParse() 
        {
            return new zzTryParse();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzTryParse : IBehaviorPreparable 
        {
            public IndirectionOutFunc<System.String, System.Globalization.DateTimeFormatInfo, System.Globalization.DateTimeStyles, System.DateTime, System.Boolean> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<System.String, System.Globalization.DateTimeFormatInfo, System.Globalization.DateTimeStyles, System.DateTime, System.Boolean>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<System.String, System.Globalization.DateTimeFormatInfo, System.Globalization.DateTimeStyles, System.DateTime, System.Boolean>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionOutFunc<System.String, System.Globalization.DateTimeFormatInfo, System.Globalization.DateTimeStyles, System.DateTime, System.Boolean>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfTryParse_string_DateTimeFormatInfo_DateTimeStyles_DateTimeRef;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PDateTimeParse).GetNestedTypes().
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
                    PDateTimeParse.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PDateTimeParse.DefaultBehavior);
                }
            }
        }
    }
}
