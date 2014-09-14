
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzAddDownloadFileCompletedAsyncCompletedEventHandler AddDownloadFileCompletedAsyncCompletedEventHandler() 
        {
            return new zzAddDownloadFileCompletedAsyncCompletedEventHandler(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddDownloadFileCompletedAsyncCompletedEventHandler : IBehaviorPreparable 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzAddDownloadFileCompletedAsyncCompletedEventHandler(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                get
                {
                    return PULWebClient.AddDownloadFileCompletedAsyncCompletedEventHandler().Body;
                }
                set
                {
                    if (value == null)
                        PULWebClient.AddDownloadFileCompletedAsyncCompletedEventHandler().RemoveTargetInstanceBody(m_target);
                    else
                        PULWebClient.AddDownloadFileCompletedAsyncCompletedEventHandler().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULWebClient.AddDownloadFileCompletedAsyncCompletedEventHandler().Info; }
            }
        } 
        public zzRemoveDownloadFileCompletedAsyncCompletedEventHandler RemoveDownloadFileCompletedAsyncCompletedEventHandler() 
        {
            return new zzRemoveDownloadFileCompletedAsyncCompletedEventHandler(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzRemoveDownloadFileCompletedAsyncCompletedEventHandler : IBehaviorPreparable 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzRemoveDownloadFileCompletedAsyncCompletedEventHandler(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                get
                {
                    return PULWebClient.RemoveDownloadFileCompletedAsyncCompletedEventHandler().Body;
                }
                set
                {
                    if (value == null)
                        PULWebClient.RemoveDownloadFileCompletedAsyncCompletedEventHandler().RemoveTargetInstanceBody(m_target);
                    else
                        PULWebClient.RemoveDownloadFileCompletedAsyncCompletedEventHandler().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULWebClient.RemoveDownloadFileCompletedAsyncCompletedEventHandler().Info; }
            }
        } 
        public zzDownloadFileAsyncUri DownloadFileAsyncUri() 
        {
            return new zzDownloadFileAsyncUri(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDownloadFileAsyncUri : IBehaviorPreparable 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzDownloadFileAsyncUri(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> Body
            {
                get
                {
                    return PULWebClient.DownloadFileAsyncUri().Body;
                }
                set
                {
                    if (value == null)
                        PULWebClient.DownloadFileAsyncUri().RemoveTargetInstanceBody(m_target);
                    else
                        PULWebClient.DownloadFileAsyncUri().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.Uri>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULWebClient.DownloadFileAsyncUri().Info; }
            }
        }

        public static implicit operator UntestableLibrary.ULWebClient(PProxyULWebClient @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyULWebClient).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_, new object[] { m_target })).
                                          Cast<IBehaviorPreparable>();
            var setting = new InstanceBehaviorSetting(this);
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class InstanceBehaviorSetting : BehaviorSetting
        {
            private PProxyULWebClient m_this;

            public InstanceBehaviorSetting(PProxyULWebClient @this)
            {
                m_this = @this;
            }
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    m_this.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(m_this.DefaultBehavior);
                }
            }
        }
    }
}
