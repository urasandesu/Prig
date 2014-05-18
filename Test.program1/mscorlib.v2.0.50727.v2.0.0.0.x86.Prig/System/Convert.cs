
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PConvert : PConvertBase
    {
        public static class ToInt32
        {
            public static IndirectionFunc<System.Double, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfToInt32_double;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Double, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class ToSByte
        {
            public static IndirectionFunc<System.Char, System.SByte> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfToSByte_char;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Char, System.SByte>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class ToInt16
        {
            public static IndirectionFunc<System.Char, System.Int16> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfToInt16_char;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Char, System.Int16>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class ToInt64
        {
            public static IndirectionFunc<System.Double, System.Int64> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfToInt64_double;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Double, System.Int64>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static class ToBoolean
        {
            public static IndirectionFunc<System.Single, System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfToBoolean_float;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Single, System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
