
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PProxyRandom 
    {
        System.Random m_target;
        
        public PProxyRandom()
        {
            m_target = (System.Random)FormatterServices.GetUninitializedObject(typeof(System.Random));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzNext Next() 
        {
            return new zzNext(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNext : IBehaviorPreparable 
        {
            System.Random m_target;

            public zzNext(System.Random target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Random, System.Int32> Body
            {
                get
                {
                    return PRandom.Next().Body;
                }
                set
                {
                    if (value == null)
                        PRandom.Next().RemoveTargetInstanceBody(m_target);
                    else
                        PRandom.Next().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Random, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PRandom.Next().Info; }
            }
        } 
        public zzNext_int Next_int() 
        {
            return new zzNext_int(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNext_int : IBehaviorPreparable 
        {
            System.Random m_target;

            public zzNext_int(System.Random target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Random, System.Int32, System.Int32> Body
            {
                get
                {
                    return PRandom.Next_int().Body;
                }
                set
                {
                    if (value == null)
                        PRandom.Next_int().RemoveTargetInstanceBody(m_target);
                    else
                        PRandom.Next_int().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Random, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PRandom.Next_int().Info; }
            }
        }

        public static implicit operator System.Random(PProxyRandom @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyRandom).GetNestedTypes().
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
            private PProxyRandom m_this;

            public InstanceBehaviorSetting(PProxyRandom @this)
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
