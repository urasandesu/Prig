﻿
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.IO.Prig
{
    public class PProxyMemoryStream 
    {
        System.IO.MemoryStream m_target;
        
        public PProxyMemoryStream()
        {
            m_target = (System.IO.MemoryStream)FormatterServices.GetUninitializedObject(typeof(System.IO.MemoryStream));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzSeek Seek() 
        {
            return new zzSeek(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzSeek : IBehaviorPreparable 
        {
            System.IO.MemoryStream m_target;

            public zzSeek(System.IO.MemoryStream target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64> Body
            {
                get
                {
                    return PMemoryStream.Seek().Body;
                }
                set
                {
                    if (value == null)
                        PMemoryStream.Seek().RemoveTargetInstanceBody(m_target);
                    else
                        PMemoryStream.Seek().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PMemoryStream.Seek().Info; }
            }
        }

        public static implicit operator System.IO.MemoryStream(PProxyMemoryStream @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyMemoryStream).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_, new object[] { m_target })).
                                          Cast<IBehaviorPreparable>();
            var setting = new InstanceBehaviorSetting(this);
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class InstanceBehaviorSetting : BehaviorSetting
        {
            private PProxyMemoryStream m_this;

            public InstanceBehaviorSetting(PProxyMemoryStream @this)
            {
                m_this = @this;
            }
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    m_this.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(m_this.DefaultBehavior);
                }
            }
        }
    }
}