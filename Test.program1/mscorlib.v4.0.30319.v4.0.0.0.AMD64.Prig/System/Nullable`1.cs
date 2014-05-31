
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PNullable<T> : PNullableBase where T : struct
    {
        public static class Constructor
        {
            public static IndirectionRefThisAction<Nullable<T>, T> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfConstructor_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisAction<Nullable<T>, T>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
