
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PProxyJapaneseLunisolarCalendar 
    {
        System.Globalization.JapaneseLunisolarCalendar m_target;
        
        public PProxyJapaneseLunisolarCalendar()
        {
            m_target = (System.Globalization.JapaneseLunisolarCalendar)FormatterServices.GetUninitializedObject(typeof(System.Globalization.JapaneseLunisolarCalendar));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzGetYearInfo GetYearInfo() 
        {
            return new zzGetYearInfo(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetYearInfo : IBehaviorPreparable 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetYearInfo(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetYearInfo().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetYearInfo().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetYearInfo().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetYearInfo().Info; }
            }
        } 
        public zzGetEra GetEra() 
        {
            return new zzGetEra(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetEra : IBehaviorPreparable 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetEra(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetEra().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetEra().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetEra().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetEra().Info; }
            }
        } 
        public zzGetGregorianYear GetGregorianYear() 
        {
            return new zzGetGregorianYear(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetGregorianYear : IBehaviorPreparable 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetGregorianYear(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetGregorianYear().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetGregorianYear().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetGregorianYear().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetGregorianYear().Info; }
            }
        }

        public static implicit operator System.Globalization.JapaneseLunisolarCalendar(PProxyJapaneseLunisolarCalendar @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyJapaneseLunisolarCalendar).GetNestedTypes().
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
            private PProxyJapaneseLunisolarCalendar m_this;

            public InstanceBehaviorSetting(PProxyJapaneseLunisolarCalendar @this)
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
