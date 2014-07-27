
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PDateTimeParse : PDateTimeParseBase 
    {
        public static zzTryParse TryParse() 
        {
            return new zzTryParse();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzTryParse 
        {
            public IndirectionOutFunc<System.String, System.Globalization.DateTimeFormatInfo, System.Globalization.DateTimeStyles, System.DateTime, System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfTryParse_string_DateTimeFormatInfo_DateTimeStyles_DateTimeRef;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<System.String, System.Globalization.DateTimeFormatInfo, System.Globalization.DateTimeStyles, System.DateTime, System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
