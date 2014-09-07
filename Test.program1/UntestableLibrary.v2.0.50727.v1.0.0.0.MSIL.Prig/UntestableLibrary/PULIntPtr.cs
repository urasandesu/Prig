
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULIntPtr : PULIntPtrBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzConstructor Constructor() 
        {
            return new zzConstructor();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzConstructor : IBehaviorPreparable 
        {
            public IndirectionRefThisAction<UntestableLibrary.ULIntPtr, System.Int64> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisAction<UntestableLibrary.ULIntPtr, System.Int64>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisAction<UntestableLibrary.ULIntPtr, System.Int64>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionRefThisAction<UntestableLibrary.ULIntPtr, System.Int64>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfConstructor_long;
                    return info;
                }
            }
        }
 
        public static zzSizeGet SizeGet() 
        {
            return new zzSizeGet();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzSizeGet : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Int32>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfSizeGet;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULIntPtr).GetNestedTypes().
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
                    PULIntPtr.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULIntPtr.DefaultBehavior);
                }
            }
        }
    }
}
