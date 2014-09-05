
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Text.Prig
{
    public class PProxyStringBuilder 
    {
        System.Text.StringBuilder m_target;
        
        public PProxyStringBuilder()
        {
            m_target = (System.Text.StringBuilder)FormatterServices.GetUninitializedObject(typeof(System.Text.StringBuilder));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzInsert Insert() 
        {
            return new zzInsert(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInsert : IBehaviorPreparable 
        {
            System.Text.StringBuilder m_target;

            public zzInsert(System.Text.StringBuilder target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> Body
            {
                get
                {
                    return PStringBuilder.Insert().Body;
                }
                set
                {
                    if (value == null)
                        PStringBuilder.Insert().RemoveTargetInstanceBody(m_target);
                    else
                        PStringBuilder.Insert().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PStringBuilder.Insert().Info; }
            }
        } 
        public zzReplace Replace() 
        {
            return new zzReplace(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzReplace : IBehaviorPreparable 
        {
            System.Text.StringBuilder m_target;

            public zzReplace(System.Text.StringBuilder target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> Body
            {
                get
                {
                    return PStringBuilder.Replace().Body;
                }
                set
                {
                    if (value == null)
                        PStringBuilder.Replace().RemoveTargetInstanceBody(m_target);
                    else
                        PStringBuilder.Replace().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PStringBuilder.Replace().Info; }
            }
        }

        public static implicit operator System.Text.StringBuilder(PProxyStringBuilder @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyStringBuilder).GetNestedTypes().
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
            private PProxyStringBuilder m_this;

            public InstanceBehaviorSetting(PProxyStringBuilder @this)
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
