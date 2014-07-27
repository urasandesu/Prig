
using System.ComponentModel;
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

        public zzGetYearInfo GetYearInfo() 
        {
            return new zzGetYearInfo(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetYearInfo 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetYearInfo(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            class OriginalGetYearInfo
            {
                public static IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body;
            }

            IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> m_body;
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                set
                {
                    PJapaneseLunisolarCalendar.GetYearInfo().Body = (System.Globalization.JapaneseLunisolarCalendar arg1, System.Int32 arg2, System.Int32 arg3) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, arg3);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(ref OriginalGetYearInfo.Body, typeof(System.Globalization.JapaneseLunisolarCalendar), "GetYearInfo", arg1, arg2, arg3);
                    };
                    m_body = value;
                }
            }
        } 
        public zzGetEra GetEra() 
        {
            return new zzGetEra(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetEra 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetEra(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            class OriginalGetEra
            {
                public static IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> Body;
            }

            IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> m_body;
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> Body
            {
                set
                {
                    PJapaneseLunisolarCalendar.GetEra().Body = (System.Globalization.JapaneseLunisolarCalendar arg1, System.DateTime arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>(ref OriginalGetEra.Body, typeof(System.Globalization.JapaneseLunisolarCalendar), "GetEra", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        } 
        public zzGetGregorianYear GetGregorianYear() 
        {
            return new zzGetGregorianYear(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetGregorianYear 
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetGregorianYear(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            class OriginalGetGregorianYear
            {
                public static IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body;
            }

            IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> m_body;
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                set
                {
                    PJapaneseLunisolarCalendar.GetGregorianYear().Body = (System.Globalization.JapaneseLunisolarCalendar arg1, System.Int32 arg2, System.Int32 arg3) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, arg3);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>(ref OriginalGetGregorianYear.Body, typeof(System.Globalization.JapaneseLunisolarCalendar), "GetGregorianYear", arg1, arg2, arg3);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.Globalization.JapaneseLunisolarCalendar(PProxyJapaneseLunisolarCalendar @this)
        {
            return @this.m_target;
        }
    }
}
