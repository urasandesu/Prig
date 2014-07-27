
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.IO.Prig
{
    public class PProxyMemoryStream 
    {
        System.IO.MemoryStream m_target;
        
        public PProxyMemoryStream()
        {
            m_target = (System.IO.MemoryStream)FormatterServices.GetUninitializedObject(typeof(System.IO.MemoryStream));
        }

        public zzSeek Seek() 
        {
            return new zzSeek(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzSeek 
        {
            System.IO.MemoryStream m_target;

            public zzSeek(System.IO.MemoryStream target)
            {
                m_target = target;
            }

            class OriginalSeek
            {
                public static IndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64> Body;
            }

            IndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64> m_body;
            public IndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64> Body
            {
                set
                {
                    PMemoryStream.Seek().Body = (System.IO.MemoryStream arg1, System.Int64 arg2, System.IO.SeekOrigin arg3) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, arg3);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.IO.MemoryStream, System.Int64, System.IO.SeekOrigin, System.Int64>(ref OriginalSeek.Body, typeof(System.IO.MemoryStream), "Seek", arg1, arg2, arg3);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.IO.MemoryStream(PProxyMemoryStream @this)
        {
            return @this.m_target;
        }
    }
}
