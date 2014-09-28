
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULDictionaryOfTKeyOfTValueEnumerator<TKey, TValue> : PULDictionaryOfTKeyOfTValueEnumeratorBase 
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzCurrentGet CurrentGet() 
        {
            return new zzCurrentGet();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCurrentGet : IBehaviorPreparable 
        {
            public IndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Collections.Generic.KeyValuePair<TKey, TValue>> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Collections.Generic.KeyValuePair<TKey, TValue>>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Collections.Generic.KeyValuePair<TKey, TValue>>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Collections.Generic.KeyValuePair<TKey, TValue>>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfCurrentGet;
                    return info;
                }
            }
        }
 
        public static zzSystemCollectionsIEnumeratorCurrentGet SystemCollectionsIEnumeratorCurrentGet() 
        {
            return new zzSystemCollectionsIEnumeratorCurrentGet();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzSystemCollectionsIEnumeratorCurrentGet : IBehaviorPreparable 
        {
            public IndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Object> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Object>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Object>>>();
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
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionRefFunc<UntestableLibrary.ULDictionary<TKey, TValue>.Enumerator, System.Object>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfSystemCollectionsIEnumeratorCurrentGet;
                    return info;
                }
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PULDictionaryOfTKeyOfTValueEnumerator<TKey, TValue>).GetNestedTypes().
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
            public TypeBehaviorSetting IncludeCurrentGet() 
            {
                Include(PULDictionaryOfTKeyOfTValueEnumerator<TKey, TValue>.CurrentGet());
                return this;
            }
 
            public TypeBehaviorSetting IncludeSystemCollectionsIEnumeratorCurrentGet() 
            {
                Include(PULDictionaryOfTKeyOfTValueEnumerator<TKey, TValue>.SystemCollectionsIEnumeratorCurrentGet());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PULDictionaryOfTKeyOfTValueEnumerator<TKey, TValue>.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULDictionaryOfTKeyOfTValueEnumerator<TKey, TValue>.DefaultBehavior);
                }
            }
        }
    }
}
