
using System.ComponentModel;
using System.Runtime.Serialization;
using Urasandesu.Prig.Framework;

namespace UntestableLibrary.Prig
{
    public class PProxyULWebClient 
    {
        UntestableLibrary.ULWebClient m_target;
        
        public PProxyULWebClient()
        {
            m_target = (UntestableLibrary.ULWebClient)FormatterServices.GetUninitializedObject(typeof(UntestableLibrary.ULWebClient));
        }

        public zzAddDownloadFileCompleted AddDownloadFileCompleted() 
        {
            return new zzAddDownloadFileCompleted(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddDownloadFileCompleted 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzAddDownloadFileCompleted(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            class OriginalAddDownloadFileCompleted
            {
                public static IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body;
            }

            IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> m_body;
            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                set
                {
                    PULWebClient.AddDownloadFileCompleted().Body = (UntestableLibrary.ULWebClient arg1, System.ComponentModel.AsyncCompletedEventHandler arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1, arg2);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(ref OriginalAddDownloadFileCompleted.Body, typeof(UntestableLibrary.ULWebClient), "add_DownloadFileCompleted", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        } 
        public zzRemoveDownloadFileCompleted RemoveDownloadFileCompleted() 
        {
            return new zzRemoveDownloadFileCompleted(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzRemoveDownloadFileCompleted 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzRemoveDownloadFileCompleted(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            class OriginalRemoveDownloadFileCompleted
            {
                public static IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body;
            }

            IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> m_body;
            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                set
                {
                    PULWebClient.RemoveDownloadFileCompleted().Body = (UntestableLibrary.ULWebClient arg1, System.ComponentModel.AsyncCompletedEventHandler arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1, arg2);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(ref OriginalRemoveDownloadFileCompleted.Body, typeof(UntestableLibrary.ULWebClient), "remove_DownloadFileCompleted", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        } 
        public zzDownloadFileAsync DownloadFileAsync() 
        {
            return new zzDownloadFileAsync(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDownloadFileAsync 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzDownloadFileAsync(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            class OriginalDownloadFileAsync
            {
                public static IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> Body;
            }

            IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> m_body;
            public IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> Body
            {
                set
                {
                    PULWebClient.DownloadFileAsync().Body = (UntestableLibrary.ULWebClient arg1, System.Uri arg2) =>
                    {
                        if (object.ReferenceEquals(arg1, m_target))
                            m_body(arg1, arg2);
                        else
                            IndirectionDelegates.ExecuteOriginalOfInstanceIndirectionAction<UntestableLibrary.ULWebClient, System.Uri>(ref OriginalDownloadFileAsync.Body, typeof(UntestableLibrary.ULWebClient), "DownloadFileAsync", arg1, arg2);
                    };
                    m_body = value;
                }
            }
        }

        public static implicit operator UntestableLibrary.ULWebClient(PProxyULWebClient @this)
        {
            return @this.m_target;
        }
    }
}
