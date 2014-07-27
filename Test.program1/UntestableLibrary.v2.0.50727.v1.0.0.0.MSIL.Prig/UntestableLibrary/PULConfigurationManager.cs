
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULConfigurationManager : PULConfigurationManagerBase 
    {
        public static zzGetProperty<T> GetProperty<T>() 
        {
            return new zzGetProperty<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetProperty<T> 
        {
            public IndirectionFunc<System.String, T, T> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfGetProperty_T_string_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.String, T, T>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
