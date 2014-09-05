
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PProxyException 
    {
        System.Exception m_target;
        
        public PProxyException()
        {
            m_target = (System.Exception)FormatterServices.GetUninitializedObject(typeof(System.Exception));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzInternalToString InternalToString() 
        {
            return new zzInternalToString(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInternalToString : IBehaviorPreparable 
        {
            System.Exception m_target;

            public zzInternalToString(System.Exception target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Exception, System.String> Body
            {
                get
                {
                    return PException.InternalToString().Body;
                }
                set
                {
                    if (value == null)
                        PException.InternalToString().RemoveTargetInstanceBody(m_target);
                    else
                        PException.InternalToString().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Exception, System.String>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PException.InternalToString().Info; }
            }
        }

        public static implicit operator System.Exception(PProxyException @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyException).GetNestedTypes().
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
            private PProxyException m_this;

            public InstanceBehaviorSetting(PProxyException @this)
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
