
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Collections.Generic.Prig
{
    public class PList<T> : PListBase 
    {
        public static zzAdd Add() 
        {
            return new zzAdd();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAdd 
        {
            public IndirectionAction<List<T>, T> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfAdd_T;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<List<T>, T>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
