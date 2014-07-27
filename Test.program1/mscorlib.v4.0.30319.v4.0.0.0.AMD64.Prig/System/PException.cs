
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PException : PExceptionBase 
    {
        public static zzInternalToString InternalToString() 
        {
            return new zzInternalToString();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInternalToString 
        {
            public IndirectionFunc<System.Exception, System.String> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfInternalToString;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Exception, System.String>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
