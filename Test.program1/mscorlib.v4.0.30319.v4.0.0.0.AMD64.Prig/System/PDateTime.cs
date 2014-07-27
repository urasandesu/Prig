
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PDateTime : PDateTimeBase 
    {
        public static zzNowGet NowGet() 
        {
            return new zzNowGet();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNowGet 
        {
            public IndirectionFunc<System.DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNowGet;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzTodayGet TodayGet() 
        {
            return new zzTodayGet();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzTodayGet 
        {
            public IndirectionFunc<System.DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfTodayGet;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzFromBinary FromBinary() 
        {
            return new zzFromBinary();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzFromBinary 
        {
            public IndirectionFunc<System.Int64, System.DateTime> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfFromBinary_long;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Int64, System.DateTime>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzDoubleDateToTicks DoubleDateToTicks() 
        {
            return new zzDoubleDateToTicks();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDoubleDateToTicks 
        {
            public IndirectionFunc<System.Double, System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfDoubleDateToTicks_double;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Double, System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzCompareTo CompareTo() 
        {
            return new zzCompareTo();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzCompareTo 
        {
            public IndirectionRefThisFunc<System.DateTime, System.Object, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfCompareTo_object;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionRefThisFunc<System.DateTime, System.Object, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
