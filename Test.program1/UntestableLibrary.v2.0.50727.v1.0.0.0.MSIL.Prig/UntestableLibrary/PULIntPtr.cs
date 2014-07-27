
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULIntPtr : PULIntPtrBase 
    {
        public static zzConstructor Constructor() 
        {
            return new zzConstructor();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzConstructor 
        {
            public IndirectionRefThisAction<UntestableLibrary.ULIntPtr, System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfConstructor_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisAction<UntestableLibrary.ULIntPtr, System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzSizeGet SizeGet() 
        {
            return new zzSizeGet();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzSizeGet 
        {
            public IndirectionFunc<System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfSizeGet;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
