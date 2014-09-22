
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULSharedMemory : PULSharedMemoryBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzGetMemoryInt32ByteArrayRef GetMemoryInt32ByteArrayRef() 
        {
            return new zzGetMemoryInt32ByteArrayRef();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetMemoryInt32ByteArrayRef : IBehaviorPreparable 
        {
            public IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfGetMemoryInt32ByteArrayRef;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(UntestableLibrary.ULSharedMemory target, IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetMemoryInt32ByteArrayRef, Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzGetMemoryInt32ByteArrayRef>.Make(new Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(UntestableLibrary.ULSharedMemory target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetMemoryInt32ByteArrayRef, Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, Byte[], System.Boolean>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzDispose Dispose() 
        {
            return new zzDispose();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDispose : IBehaviorPreparable 
        {
            public IndirectionAction<UntestableLibrary.ULSharedMemory> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULSharedMemory>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULSharedMemory>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfDispose;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(UntestableLibrary.ULSharedMemory target, IndirectionAction<UntestableLibrary.ULSharedMemory> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzDispose, Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzDispose>.Make(new Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionAction<UntestableLibrary.ULSharedMemory>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(UntestableLibrary.ULSharedMemory target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzDispose, Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzAddOnDisposedDisposedEventHandler AddOnDisposedDisposedEventHandler() 
        {
            return new zzAddOnDisposedDisposedEventHandler();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddOnDisposedDisposedEventHandler : IBehaviorPreparable 
        {
            public IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfAddOnDisposedDisposedEventHandler;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(UntestableLibrary.ULSharedMemory target, IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAddOnDisposedDisposedEventHandler, Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzAddOnDisposedDisposedEventHandler>.Make(new Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(UntestableLibrary.ULSharedMemory target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAddOnDisposedDisposedEventHandler, Dictionary<UntestableLibrary.ULSharedMemory, TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULSharedMemory).GetNestedTypes().
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
                    PULSharedMemory.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULSharedMemory.DefaultBehavior);
                }
            }
        }
    }
}
