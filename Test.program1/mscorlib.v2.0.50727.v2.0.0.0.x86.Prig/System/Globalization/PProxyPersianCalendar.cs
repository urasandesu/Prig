
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PProxyPersianCalendar 
    {
        System.Globalization.PersianCalendar m_target;
        
        public PProxyPersianCalendar()
        {
            m_target = (System.Globalization.PersianCalendar)FormatterServices.GetUninitializedObject(typeof(System.Globalization.PersianCalendar));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzCheckTicksRange CheckTicksRange() 
        {
            return new zzCheckTicksRange(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCheckTicksRange : IBehaviorPreparable 
        {
            System.Globalization.PersianCalendar m_target;

            public zzCheckTicksRange(System.Globalization.PersianCalendar target)
            {
                m_target = target;
            }

            public IndirectionAction<System.Globalization.PersianCalendar, System.Int64> Body
            {
                get
                {
                    return PPersianCalendar.CheckTicksRange().Body;
                }
                set
                {
                    if (value == null)
                        PPersianCalendar.CheckTicksRange().RemoveTargetInstanceBody(m_target);
                    else
                        PPersianCalendar.CheckTicksRange().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<System.Globalization.PersianCalendar, System.Int64>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PPersianCalendar.CheckTicksRange().Info; }
            }
        }

        public static implicit operator System.Globalization.PersianCalendar(PProxyPersianCalendar @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyPersianCalendar).GetNestedTypes().
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
            private PProxyPersianCalendar m_this;

            public InstanceBehaviorSetting(PProxyPersianCalendar @this)
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
