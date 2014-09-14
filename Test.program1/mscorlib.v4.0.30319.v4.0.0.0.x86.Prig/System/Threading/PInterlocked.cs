
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Threading.Prig
{
    public class PInterlocked : PInterlockedBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzExchangeOfTTRefT<T> ExchangeOfTTRefT<T>() where T : class
        {
            return new zzExchangeOfTTRefT<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzExchangeOfTTRefT<T> : IBehaviorPreparable where T : class
        {
            public IndirectionRefThisFunc<T, T, T> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisFunc<T, T, T>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisFunc<T, T, T>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionRefThisFunc<T, T, T>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExchangeOfTTRefT;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PInterlocked).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeExchangeOfTTRefT<T>() where T : class
            {
                Include(PInterlocked.ExchangeOfTTRefT<T>());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PInterlocked.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PInterlocked.DefaultBehavior);
                }
            }
        }
    }
}
