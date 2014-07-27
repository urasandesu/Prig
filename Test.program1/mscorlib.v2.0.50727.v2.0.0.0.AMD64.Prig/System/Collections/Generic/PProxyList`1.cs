
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Collections.Generic.Prig
{
    public class PProxyList<T> 
    {
        List<T> m_target;
        
        public PProxyList()
        {
            m_target = (List<T>)FormatterServices.GetUninitializedObject(typeof(List<T>));
        }

        public zzAdd Add() 
        {
            return new zzAdd(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAdd 
        {
            List<T> m_target;

            public zzAdd(List<T> target)
            {
                m_target = target;
            }

            class OriginalAdd
            {
                public static IndirectionAction<List<T>, T> Body;
            }

            IndirectionAction<List<T>, T> m_body;
            public IndirectionAction<List<T>, T> Body
            {
                set
                {
                    PList<T>.Add().Body = (List<T> arg1, T arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1, arg2);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<List<T>, T>(ref OriginalAdd.Body, typeof(List<T>), "Add", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator List<T>(PProxyList<T> @this)
        {
            return @this.m_target;
        }
    }
}
