
using Urasandesu.Prig.Framework;

namespace System.Threading.Prig
{
    public class PInterlocked : PInterlockedBase
    {
        public static class Exchange<T>
        {
            public static IndirectionRefThisFunc<T, T, T> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExchange_T_TRef_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisFunc<T, T, T>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
