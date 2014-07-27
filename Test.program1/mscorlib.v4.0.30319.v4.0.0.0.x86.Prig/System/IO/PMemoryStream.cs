
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.IO.Prig
{
    public class PMemoryStream : PMemoryStreamBase 
    {
        public static zzSeek Seek() 
        {
            return new zzSeek();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzSeek 
        {
            public IndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfSeek_long_SeekOrigin;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
