
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULDictionary<TKey, TValue> : PULDictionaryBase 
    {
        public static zzIsCompatibleKey IsCompatibleKey() 
        {
            return new zzIsCompatibleKey();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzIsCompatibleKey 
        {
            public IndirectionFunc<System.Object, System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfIsCompatibleKey_object;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Object, System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
