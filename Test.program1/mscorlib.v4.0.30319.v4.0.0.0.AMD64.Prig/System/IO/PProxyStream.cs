
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.IO.Prig
{
    public class PProxyStream 
    {
        System.IO.Stream m_target;
        
        public PProxyStream()
        {
            m_target = (System.IO.Stream)FormatterServices.GetUninitializedObject(typeof(System.IO.Stream));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzBeginRead BeginRead() 
        {
            return new zzBeginRead(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzBeginRead : IBehaviorPreparable 
        {
            System.IO.Stream m_target;

            public zzBeginRead(System.IO.Stream target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult> Body
            {
                get
                {
                    return PStream.BeginRead().Body;
                }
                set
                {
                    if (value == null)
                        PStream.BeginRead().RemoveTargetInstanceBody(m_target);
                    else
                        PStream.BeginRead().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PStream.BeginRead().Info; }
            }
        }

        public static implicit operator System.IO.Stream(PProxyStream @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyStream).GetNestedTypes().
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
            private PProxyStream m_this;

            public InstanceBehaviorSetting(PProxyStream @this)
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
