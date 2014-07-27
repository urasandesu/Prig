
using System.ComponentModel;
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

        public zzCheckTicksRange CheckTicksRange() 
        {
            return new zzCheckTicksRange(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCheckTicksRange 
        {
            System.Globalization.PersianCalendar m_target;

            public zzCheckTicksRange(System.Globalization.PersianCalendar target)
            {
                m_target = target;
            }

            class OriginalCheckTicksRange
            {
                public static IndirectionAction<System.Globalization.PersianCalendar, System.Int64> Body;
            }

            IndirectionAction<System.Globalization.PersianCalendar, System.Int64> m_body;
            public IndirectionAction<System.Globalization.PersianCalendar, System.Int64> Body
            {
                set
                {
                    PPersianCalendar.CheckTicksRange().Body = (System.Globalization.PersianCalendar arg1, System.Int64 arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1, arg2);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<System.Globalization.PersianCalendar, System.Int64>(ref OriginalCheckTicksRange.Body, typeof(System.Globalization.PersianCalendar), "CheckTicksRange", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.Globalization.PersianCalendar(PProxyPersianCalendar @this)
        {
            return @this.m_target;
        }
    }
}
