
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PPersianCalendar : PPersianCalendarBase
    {
        public static class CheckTicksRange
        {
            public static IndirectionAction<System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCheckTicksRange_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
