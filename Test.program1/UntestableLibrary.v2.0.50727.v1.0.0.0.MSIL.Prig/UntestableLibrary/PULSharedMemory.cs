
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULSharedMemory : PULSharedMemoryBase 
    {
        public static zzGetMemory GetMemory() 
        {
            return new zzGetMemory();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetMemory 
        {
            public IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfGetMemory;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzDispose Dispose() 
        {
            return new zzDispose();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDispose 
        {
            public IndirectionAction<UntestableLibrary.ULSharedMemory> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfDispose;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULSharedMemory>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzAddOnDisposed AddOnDisposed() 
        {
            return new zzAddOnDisposed();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddOnDisposed 
        {
            public IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfAddOnDisposed;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
