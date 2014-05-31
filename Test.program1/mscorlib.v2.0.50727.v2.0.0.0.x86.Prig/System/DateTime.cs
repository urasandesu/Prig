
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PDateTime : PDateTimeBase
    {
        public static class NowGet
        {
            public static IndirectionFunc<System.DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNowGet;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class TodayGet
        {
            public static IndirectionFunc<System.DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfTodayGet;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class FromBinary
        {
            public static IndirectionFunc<System.Int64, System.DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfFromBinary_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Int64, System.DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class DoubleDateToTicks
        {
            public static IndirectionFunc<System.Double, System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfDoubleDateToTicks_double;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Double, System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class CompareTo
        {
            public static IndirectionRefThisFunc<System.DateTime, System.Object, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCompareTo_object;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisFunc<System.DateTime, System.Object, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
