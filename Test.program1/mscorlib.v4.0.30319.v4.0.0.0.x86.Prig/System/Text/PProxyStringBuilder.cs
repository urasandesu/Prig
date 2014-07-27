
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Text.Prig
{
    public class PProxyStringBuilder 
    {
        System.Text.StringBuilder m_target;
        
        public PProxyStringBuilder()
        {
            m_target = (System.Text.StringBuilder)FormatterServices.GetUninitializedObject(typeof(System.Text.StringBuilder));
        }

        public zzInsert Insert() 
        {
            return new zzInsert(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInsert 
        {
            System.Text.StringBuilder m_target;

            public zzInsert(System.Text.StringBuilder target)
            {
                m_target = target;
            }

            class OriginalInsert
            {
                public static IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> Body;
            }

            IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> m_body;
            public IndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder> Body
            {
                set
                {
                    PStringBuilder.Insert().Body = (System.Text.StringBuilder arg1, System.Int32 arg2, System.String arg3, System.Int32 arg4) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, arg3, arg4);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Text.StringBuilder, System.Int32, System.String, System.Int32, System.Text.StringBuilder>(ref OriginalInsert.Body, typeof(System.Text.StringBuilder), "Insert", arg1, arg2, arg3, arg4);
                    };
                    m_body = value;
                }
            }
        } 
        public zzReplace Replace() 
        {
            return new zzReplace(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzReplace 
        {
            System.Text.StringBuilder m_target;

            public zzReplace(System.Text.StringBuilder target)
            {
                m_target = target;
            }

            class OriginalReplace
            {
                public static IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> Body;
            }

            IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> m_body;
            public IndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder> Body
            {
                set
                {
                    PStringBuilder.Replace().Body = (System.Text.StringBuilder arg1, System.Char arg2, System.Char arg3, System.Int32 arg4, System.Int32 arg5) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, arg3, arg4, arg5);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Text.StringBuilder, System.Char, System.Char, System.Int32, System.Int32, System.Text.StringBuilder>(ref OriginalReplace.Body, typeof(System.Text.StringBuilder), "Replace", arg1, arg2, arg3, arg4, arg5);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.Text.StringBuilder(PProxyStringBuilder @this)
        {
            return @this.m_target;
        }
    }
}
