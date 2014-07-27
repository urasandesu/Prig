
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PProxyULSharedMemory 
    {
        UntestableLibrary.ULSharedMemory m_target;
        
        public PProxyULSharedMemory()
        {
            m_target = (UntestableLibrary.ULSharedMemory)FormatterServices.GetUninitializedObject(typeof(UntestableLibrary.ULSharedMemory));
        }

        public zzGetMemory GetMemory() 
        {
            return new zzGetMemory(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetMemory 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzGetMemory(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            class OriginalGetMemory
            {
                public static IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean> Body;
            }

            IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean> m_body;
            public IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean> Body
            {
                set
                {
                    PULSharedMemory.GetMemory().Body = (UntestableLibrary.ULSharedMemory arg1, System.Int32 arg2, out System.Byte[] out1) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            return m_body(arg1, arg2, out out1);
                        else
                            return IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean>(ref OriginalGetMemory.Body, typeof(UntestableLibrary.ULSharedMemory), "GetMemory", arg1, arg2, out out1);
                    };
                    m_body = value;
                }
            }
        } 
        public zzDispose Dispose() 
        {
            return new zzDispose(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDispose 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzDispose(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            class OriginalDispose
            {
                public static IndirectionAction<UntestableLibrary.ULSharedMemory> Body;
            }

            IndirectionAction<UntestableLibrary.ULSharedMemory> m_body;
            public IndirectionAction<UntestableLibrary.ULSharedMemory> Body
            {
                set
                {
                    PULSharedMemory.Dispose().Body = (UntestableLibrary.ULSharedMemory arg1) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<UntestableLibrary.ULSharedMemory>(ref OriginalDispose.Body, typeof(UntestableLibrary.ULSharedMemory), "Dispose", arg1);
                    };
                    m_body = value;
                }
            }
        } 
        public zzAddOnDisposed AddOnDisposed() 
        {
            return new zzAddOnDisposed(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddOnDisposed 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzAddOnDisposed(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            class OriginalAddOnDisposed
            {
                public static IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> Body;
            }

            IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> m_body;
            public IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> Body
            {
                set
                {
                    PULSharedMemory.AddOnDisposed().Body = (UntestableLibrary.ULSharedMemory arg1, UntestableLibrary.ULSharedMemory.DisposedEventHandler arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1, arg2);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>(ref OriginalAddOnDisposed.Body, typeof(UntestableLibrary.ULSharedMemory), "add_OnDisposed", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator UntestableLibrary.ULSharedMemory(PProxyULSharedMemory @this)
        {
            return @this.m_target;
        }
    }
}
