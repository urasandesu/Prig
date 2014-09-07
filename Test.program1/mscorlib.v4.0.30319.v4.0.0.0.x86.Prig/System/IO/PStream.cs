
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace System.IO.Prig
{
    public class PStream : PStreamBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzBeginRead BeginRead() 
        {
            return new zzBeginRead();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzBeginRead : IBehaviorPreparable 
        {
            public IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfBeginRead_byteArray_int_int_AsyncCallback_Object;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(Stream target, IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzBeginRead, Dictionary<Stream, TargetSettingValue<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzBeginRead>.Make(new Dictionary<Stream, TargetSettingValue<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(Stream target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzBeginRead, Dictionary<Stream, TargetSettingValue<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PStream).GetNestedTypes().
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
                    PStream.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PStream.DefaultBehavior);
                }
            }
        }
    }
}
