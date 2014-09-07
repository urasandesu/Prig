
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULConfigurationManager : PULConfigurationManagerBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzGetProperty<T> GetProperty<T>() 
        {
            return new zzGetProperty<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetProperty<T> : IBehaviorPreparable 
        {
            public IndirectionFunc<System.String, T, T> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.String, T, T>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.String, T, T>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.String, T, T>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfGetProperty_T_string_T;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULConfigurationManager).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeGetProperty<T>() 
            {
                Include(PULConfigurationManager.GetProperty<T>());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PULConfigurationManager.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULConfigurationManager.DefaultBehavior);
                }
            }
        }
    }
}
