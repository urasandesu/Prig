
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PArray : PArrayBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzCreateInstance CreateInstance() 
        {
            return new zzCreateInstance();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCreateInstance : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Type, System.Int32[], System.Int32[], System.Array>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCreateInstance_Type_intArray_intArray;
                    return info;
                }
            }
        }
 
        public static zzExists<T> Exists<T>() 
        {
            return new zzExists<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzExists<T> : IBehaviorPreparable 
        {
            public IndirectionFunc<T[], System.Predicate<T>, System.Boolean> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<T[], System.Predicate<T>, System.Boolean>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExists_T_TArray_Predicate_T;
                    return info;
                }
            }
        }
 
        public static zzBinarySearch BinarySearch() 
        {
            return new zzBinarySearch();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzBinarySearch : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Array, System.Int32, System.Int32, System.Object, System.Collections.IComparer, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfBinarySearch_Array_int_int_object_IComparer;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PArray).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeExists<T>() 
            {
                Include(PArray.Exists<T>());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PArray.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PArray.DefaultBehavior);
                }
            }
        }
    }
}
