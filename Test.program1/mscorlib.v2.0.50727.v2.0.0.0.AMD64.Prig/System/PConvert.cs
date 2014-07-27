
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PConvert : PConvertBase 
    {
        public static zzToInt32 ToInt32() 
        {
            return new zzToInt32();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzToInt32 
        {
            public IndirectionFunc<System.Double, System.Int32> Body
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
        public static zzToSByte ToSByte() 
        {
            return new zzToSByte();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzToSByte 
        {
            public IndirectionFunc<System.Char, System.SByte> Body
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
        public static zzToInt16 ToInt16() 
        {
            return new zzToInt16();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzToInt16 
        {
            public IndirectionFunc<System.Char, System.Int16> Body
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
        public static zzToInt64 ToInt64() 
        {
            return new zzToInt64();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzToInt64 
        {
            public IndirectionFunc<System.Double, System.Int64> Body
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
        public static zzToBoolean ToBoolean() 
        {
            return new zzToBoolean();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzToBoolean 
        {
            public IndirectionFunc<System.Single, System.Boolean> Body
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
