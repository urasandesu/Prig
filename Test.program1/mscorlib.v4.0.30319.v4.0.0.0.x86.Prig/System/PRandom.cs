
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PRandom : PRandomBase 
    {
        public static zzConstructor Constructor() 
        {
            return new zzConstructor();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzConstructor 
        {
            public IndirectionAction<System.Random, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfConstructor_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<System.Random, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzNext Next() 
        {
            return new zzNext();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNext 
        {
            public IndirectionFunc<System.Random, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNext;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Random, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzNext_int Next_int() 
        {
            return new zzNext_int();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNext_int 
        {
            public IndirectionFunc<System.Random, System.Int32, System.Int32> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNext_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Random, System.Int32, System.Int32>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
