
using System.ComponentModel;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PULWebClient : PULWebClientBase 
    {
        public static zzAddDownloadFileCompleted AddDownloadFileCompleted() 
        {
            return new zzAddDownloadFileCompleted();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddDownloadFileCompleted 
        {
            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfAddDownloadFileCompleted;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzRemoveDownloadFileCompleted RemoveDownloadFileCompleted() 
        {
            return new zzRemoveDownloadFileCompleted();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzRemoveDownloadFileCompleted 
        {
            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfRemoveDownloadFileCompleted;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        } 
        public static zzDownloadFileAsync DownloadFileAsync() 
        {
            return new zzDownloadFileAsync();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDownloadFileAsync 
        {
            public IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> Body
            {
                set
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "UntestableLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                    info.Token = TokenOfDownloadFileAsync;
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<UntestableLibrary.ULWebClient, System.Uri>>>();
                    holder.AddOrUpdate(info, value);
                }
            }
        }
    }
}
