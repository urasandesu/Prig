
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Globalization.Prig
{
    public class PPersianCalendar : PPersianCalendarBase 
    {
        public static zzCheckTicksRange CheckTicksRange() 
        {
            return new zzCheckTicksRange();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCheckTicksRange 
        {
            public IndirectionAction<System.Globalization.PersianCalendar, System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCheckTicksRange_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<System.Globalization.PersianCalendar, System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
