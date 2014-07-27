
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace System.Prig
{
    public class PProxyRandom 
    {
        System.Random m_target;
        
        public PProxyRandom()
        {
            m_target = (System.Random)FormatterServices.GetUninitializedObject(typeof(System.Random));
        }

        public zzNext Next() 
        {
            return new zzNext(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNext 
        {
            System.Random m_target;

            public zzNext(System.Random target)
            {
                m_target = target;
            }

            class OriginalNext
            {
                public static IndirectionFunc<System.Random, System.Int32> Body;
            }

            IndirectionFunc<System.Random, System.Int32> m_body;
            public IndirectionFunc<System.Random, System.Int32> Body
            {
                set
                {
                    PRandom.Next().Body = (System.Random arg1) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Random, System.Int32>(ref OriginalNext.Body, typeof(System.Random), "Next", arg1);
                    };
                    m_body = value;
                }
            }
        } 
        public zzNext_int Next_int() 
        {
            return new zzNext_int(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzNext_int 
        {
            System.Random m_target;

            public zzNext_int(System.Random target)
            {
                m_target = target;
            }

            class OriginalNext_int
            {
                public static IndirectionFunc<System.Random, System.Int32, System.Int32> Body;
            }

            IndirectionFunc<System.Random, System.Int32, System.Int32> m_body;
            public IndirectionFunc<System.Random, System.Int32, System.Int32> Body
            {
                set
                {
                    PRandom.Next_int().Body = (System.Random arg1, System.Int32 arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionFunc<System.Random, System.Int32, System.Int32>(ref OriginalNext_int.Body, typeof(System.Random), "Next", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator System.Random(PProxyRandom @this)
        {
            return @this.m_target;
        }
    }
}
