
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULDictionary<TKey, TValue> : PULDictionaryBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzIsCompatibleKey IsCompatibleKey() 
        {
            return new zzIsCompatibleKey();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzIsCompatibleKey : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Object, System.Boolean> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Object, System.Boolean>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Object, System.Boolean>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Object, System.Boolean>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfIsCompatibleKey_object;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULDictionary<TKey, TValue>).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeIsCompatibleKey() 
            {
                Include(PULDictionary<TKey, TValue>.IsCompatibleKey());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PULDictionary<TKey, TValue>.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULDictionary<TKey, TValue>.DefaultBehavior);
                }
            }
        }
    }
}
