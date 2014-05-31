
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PRandom : PRandomBase
    {
        public static class Constructor
        {
            public static IndirectionAction<System.Random, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfConstructor_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<System.Random, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class Next
        {
            public static IndirectionFunc<System.Random, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNext;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Random, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class Next_int
        {
            public static IndirectionFunc<System.Random, System.Int32, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNext_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Random, System.Int32, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
