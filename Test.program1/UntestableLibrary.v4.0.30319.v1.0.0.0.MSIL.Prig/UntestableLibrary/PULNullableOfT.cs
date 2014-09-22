
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULNullable<T> : PULNullableBase where T : struct
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzToString ToString() 
        {
            return new zzToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzToString : IBehaviorPreparable 
        {
            public IndirectionRefFunc<UntestableLibrary.ULNullable<T>, System.String> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<UntestableLibrary.ULNullable<T>, System.String>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<UntestableLibrary.ULNullable<T>, System.String>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionRefFunc<UntestableLibrary.ULNullable<T>, System.String>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfToString;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULNullable<T>).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeToString() 
            {
                Include(PULNullable<T>.ToString());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PULNullable<T>.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULNullable<T>.DefaultBehavior);
                }
            }
        }
    }
}
