
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PJapaneseLunisolarCalendar : PJapaneseLunisolarCalendarBase
    {
        public static class GetYearInfo
        {
            public static IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetYearInfo_int_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
