
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.IO.Prig
{
    public class PProxyStream 
    {
        System.IO.Stream m_target;
        
        public PProxyStream()
        {
            m_target = (System.IO.Stream)FormatterServices.GetUninitializedObject(typeof(System.IO.Stream));
        }

        public zzBeginRead BeginRead() 
        {
            return new zzBeginRead(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzBeginRead 
        {
            System.IO.Stream m_target;

            public zzBeginRead(System.IO.Stream target)
            {
                m_target = target;
            }

            class OriginalBeginRead
            {
                public static IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult> Body;
            }

            IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult> m_body;
            public IndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult> Body
            {
                set
                {
                    PStream.BeginRead().Body = (System.IO.Stream arg1, System.Byte[] arg2, System.Int32 arg3, System.Int32 arg4, System.AsyncCallback arg5, System.Object arg6) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, arg3, arg4, arg5, arg6);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.IO.Stream, System.Byte[], System.Int32, System.Int32, System.AsyncCallback, System.Object, System.IAsyncResult>(ref OriginalBeginRead.Body, typeof(System.IO.Stream), "BeginRead", arg1, arg2, arg3, arg4, arg5, arg6);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.IO.Stream(PProxyStream @this)
        {
            return @this.m_target;
        }
    }
}
