
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Collections.Generic.Prig
{
    public class PProxyList<T> 
    {
        List<T> m_target;
        
        public PProxyList()
        {
            m_target = (List<T>)FormatterServices.GetUninitializedObject(typeof(List<T>));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzAdd Add() 
        {
            return new zzAdd(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAdd : IBehaviorPreparable 
        {
            List<T> m_target;

            public zzAdd(List<T> target)
            {
                m_target = target;
            }

            public IndirectionAction<List<T>, T> Body
            {
                get
                {
                    return PList<T>.Add().Body;
                }
                set
                {
                    if (value == null)
                        PList<T>.Add().RemoveTargetInstanceBody(m_target);
                    else
                        PList<T>.Add().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<List<T>, T>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PList<T>.Add().Info; }
            }
        }

        public static implicit operator List<T>(PProxyList<T> @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyList<T>).GetNestedTypes().
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
            private PProxyList<T> m_this;

            public InstanceBehaviorSetting(PProxyList<T> @this)
            {
                m_this = @this;
            }
            public InstanceBehaviorSetting IncludeAdd() 
            {
                Include(m_this.Add());
                return this;
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
