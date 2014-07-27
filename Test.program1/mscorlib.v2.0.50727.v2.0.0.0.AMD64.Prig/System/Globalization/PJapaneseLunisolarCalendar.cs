
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PJapaneseLunisolarCalendar : PJapaneseLunisolarCalendarBase 
    {
        public static zzGetYearInfo GetYearInfo() 
        {
            return new zzGetYearInfo();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetYearInfo 
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetYearInfo_int_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzGetEra GetEra() 
        {
            return new zzGetEra();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetEra 
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetEra;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.DateTime, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzGetGregorianYear GetGregorianYear() 
        {
            return new zzGetGregorianYear();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetGregorianYear 
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetGregorianYear;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
