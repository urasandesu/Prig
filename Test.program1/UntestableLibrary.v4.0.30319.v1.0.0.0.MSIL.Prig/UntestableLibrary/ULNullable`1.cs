
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULNullable<T> : PULNullableBase where T : struct
    {
        public static new class ToString
        {
            public static IndirectionRefFunc<ULNullable<T>, System.String> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfToString;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefFunc<ULNullable<T>, System.String>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
