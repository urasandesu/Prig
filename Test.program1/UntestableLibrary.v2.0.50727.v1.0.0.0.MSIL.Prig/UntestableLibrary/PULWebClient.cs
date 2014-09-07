
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULWebClient : PULWebClientBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzAddDownloadFileCompleted AddDownloadFileCompleted() 
        {
            return new zzAddDownloadFileCompleted();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddDownloadFileCompleted : IBehaviorPreparable 
        {
            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfAddDownloadFileCompleted;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(ULWebClient target, IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAddDownloadFileCompleted, Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzAddDownloadFileCompleted>.Make(new Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(ULWebClient target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAddDownloadFileCompleted, Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzRemoveDownloadFileCompleted RemoveDownloadFileCompleted() 
        {
            return new zzRemoveDownloadFileCompleted();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzRemoveDownloadFileCompleted : IBehaviorPreparable 
        {
            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfRemoveDownloadFileCompleted;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(ULWebClient target, IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzRemoveDownloadFileCompleted, Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzRemoveDownloadFileCompleted>.Make(new Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(ULWebClient target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzRemoveDownloadFileCompleted, Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }
 
        public static zzDownloadFileAsync DownloadFileAsync() 
        {
            return new zzDownloadFileAsync();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDownloadFileAsync : IBehaviorPreparable 
        {
            public IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.Uri>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfDownloadFileAsync;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(ULWebClient target, IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzDownloadFileAsync, Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzDownloadFileAsync>.Make(new Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.Uri>(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = IndirectionDelegates.CreateDelegateExecutingDefaultOrIndirectionAction<UntestableLibrary.ULWebClient, System.Uri>(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(ULWebClient target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzDownloadFileAsync, Dictionary<ULWebClient, TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULWebClient).GetNestedTypes().
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
                    PULWebClient.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULWebClient.DefaultBehavior);
                }
            }
        }
    }
}
