
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

        public zzInsertInt32StringInt32 InsertInt32StringInt32() 
        {
            return new zzInsertInt32StringInt32(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInsertInt32StringInt32 : IBehaviorPreparable 
        {
            System.Text.StringBuilder m_target;

            public zzInsertInt32StringInt32(System.Text.StringBuilder target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> Body
            {
                get
                {
                    return PStringBuilder.InsertInt32StringInt32().Body;
                }
                set
                {
                    if (value == null)
                        PStringBuilder.InsertInt32StringInt32().RemoveTargetInstanceBody(m_target);
                    else
                        PStringBuilder.InsertInt32StringInt32().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PStringBuilder.InsertInt32StringInt32().Info; }
            }
        } 
        public zzReplaceCharCharInt32Int32 ReplaceCharCharInt32Int32() 
        {
            return new zzReplaceCharCharInt32Int32(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzReplaceCharCharInt32Int32 : IBehaviorPreparable 
        {
            System.Text.StringBuilder m_target;

            public zzReplaceCharCharInt32Int32(System.Text.StringBuilder target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> Body
            {
                get
                {
                    return PStringBuilder.ReplaceCharCharInt32Int32().Body;
                }
                set
                {
                    if (value == null)
                        PStringBuilder.ReplaceCharCharInt32Int32().RemoveTargetInstanceBody(m_target);
                    else
                        PStringBuilder.ReplaceCharCharInt32Int32().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PStringBuilder.ReplaceCharCharInt32Int32().Info; }
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
