
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

        public static zzCreateInstanceTypeInt32ArrayInt32Array CreateInstanceTypeInt32ArrayInt32Array() 
        {
            return new zzCreateInstanceTypeInt32ArrayInt32Array();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCreateInstanceTypeInt32ArrayInt32Array : IBehaviorPreparable 
        {
            public IndirectionFunc<System.Type, Int32[], Int32[], System.Array> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Type, Int32[], Int32[], System.Array>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Type, Int32[], Int32[], System.Array>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Type, Int32[], Int32[], System.Array>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCreateInstanceTypeInt32ArrayInt32Array;
                    return info;
                }
            }
        }
 
        public static zzExistsOfTTArrayPredicateOfT<T> ExistsOfTTArrayPredicateOfT<T>() 
        {
            return new zzExistsOfTTArrayPredicateOfT<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzExistsOfTTArrayPredicateOfT<T> : IBehaviorPreparable 
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
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExistsOfTTArrayPredicateOfT;
                    return info;
                }
            }
        }
 
        public static zzBinarySearchArrayInt32Int32ObjectIComparer BinarySearchArrayInt32Int32ObjectIComparer() 
        {
            return new zzBinarySearchArrayInt32Int32ObjectIComparer();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzBinarySearchArrayInt32Int32ObjectIComparer : IBehaviorPreparable 
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
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfBinarySearchArrayInt32Int32ObjectIComparer;
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

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class TypeBehaviorSetting : BehaviorSetting
        {
            public TypeBehaviorSetting IncludeExistsOfTTArrayPredicateOfT<T>() 
            {
                Include(PArray.ExistsOfTTArrayPredicateOfT<T>());
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
