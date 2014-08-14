
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULGMTMaster : PULGMTMasterBase 
    {
        public static zzRefreshTimeZone RefreshTimeZone() 
        {
            return new zzRefreshTimeZone();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzRefreshTimeZone 
        {
            public IndirectionAction Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfRefreshTimeZone;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
