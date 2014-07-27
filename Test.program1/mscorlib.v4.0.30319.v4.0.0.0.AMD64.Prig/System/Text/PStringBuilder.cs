
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace System.Text.Prig
{
    public class PStringBuilder : PStringBuilderBase 
    {
        public static zzInsert Insert() 
        {
            return new zzInsert();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInsert 
        {
            public IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfInsert_int_string_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzReplace Replace() 
        {
            return new zzReplace();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzReplace 
        {
            public IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfReplace_char_char_int_int;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
