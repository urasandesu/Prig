
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

        public zzGetYearInfoInt32Int32 GetYearInfoInt32Int32() 
        {
            return new zzGetYearInfoInt32Int32(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetYearInfoInt32Int32 : IBehaviorPreparable 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetYearInfoInt32Int32(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetYearInfoInt32Int32().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetYearInfoInt32Int32().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetYearInfoInt32Int32().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetYearInfoInt32Int32().Info; }
            }
        } 
        public zzGetEraDateTime GetEraDateTime() 
        {
            return new zzGetEraDateTime(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetEraDateTime : IBehaviorPreparable 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetEraDateTime(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetEraDateTime().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetEraDateTime().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetEraDateTime().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetEraDateTime().Info; }
            }
        } 
        public zzGetGregorianYearInt32Int32 GetGregorianYearInt32Int32() 
        {
            return new zzGetGregorianYearInt32Int32(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetGregorianYearInt32Int32 : IBehaviorPreparable 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetGregorianYearInt32Int32(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetGregorianYearInt32Int32().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetGregorianYearInt32Int32().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetGregorianYearInt32Int32().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetGregorianYearInt32Int32().Info; }
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
