
using Urasandesu.Prig.Framework;

namespace System.Collections.Generic.Prig
{
    public class PList<T> : PListBase
    {
        public static class Add
        {
            public static IndirectionAction<List<T>, T> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfAdd_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<List<T>, T>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
