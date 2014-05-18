
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULIdnMapping : PULIdnMappingBase
    {
        public static class basic
        {
            public static IndirectionFunc<System.UInt32, System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfbasic_uint;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.UInt32, System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
