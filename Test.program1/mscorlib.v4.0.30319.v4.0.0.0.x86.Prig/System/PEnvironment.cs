
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PEnvironment : PEnvironmentBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzGetFolderPathSpecialFolder GetFolderPathSpecialFolder() 
        {
            return new zzGetFolderPathSpecialFolder();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetFolderPathSpecialFolder : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Environment.SpecialFolder, System.String> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Environment.SpecialFolder, System.String>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Environment.SpecialFolder, System.String>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Environment.SpecialFolder, System.String>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetFolderPathSpecialFolder;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PEnvironment).GetNestedTypes().
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
                    PEnvironment.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PEnvironment.DefaultBehavior);
                }
            }
        }
    }
}
