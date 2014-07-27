
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PProxyException 
    {
        System.Exception m_target;
        
        public PProxyException()
        {
            m_target = (System.Exception)FormatterServices.GetUninitializedObject(typeof(System.Exception));
        }

        public zzInternalToString InternalToString() 
        {
            return new zzInternalToString(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzInternalToString 
        {
            System.Exception m_target;

            public zzInternalToString(System.Exception target)
            {
                m_target = target;
            }

            class OriginalInternalToString
            {
                public static IndirectionFunc<System.Exception, System.String> Body;
            }

            IndirectionFunc<System.Exception, System.String> m_body;
            public IndirectionFunc<System.Exception, System.String> Body
            {
                set
                {
                    PException.InternalToString().Body = (System.Exception arg1) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Exception, System.String>(ref OriginalInternalToString.Body, typeof(System.Exception), "InternalToString", arg1);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.Exception(PProxyException @this)
        {
            return @this.m_target;
        }
    }
}
