
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

        public zzAddDownloadFileCompleted AddDownloadFileCompleted() 
        {
            return new zzAddDownloadFileCompleted(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddDownloadFileCompleted : IBehaviorPreparable 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzAddDownloadFileCompleted(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                get
                {
                    return PULWebClient.AddDownloadFileCompleted().Body;
                }
                set
                {
                    if (value == null)
                        PULWebClient.AddDownloadFileCompleted().RemoveTargetInstanceBody(m_target);
                    else
                        PULWebClient.AddDownloadFileCompleted().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULWebClient.AddDownloadFileCompleted().Info; }
            }
        } 
        public zzRemoveDownloadFileCompleted RemoveDownloadFileCompleted() 
        {
            return new zzRemoveDownloadFileCompleted(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzRemoveDownloadFileCompleted : IBehaviorPreparable 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzRemoveDownloadFileCompleted(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler> Body
            {
                get
                {
                    return PULWebClient.RemoveDownloadFileCompleted().Body;
                }
                set
                {
                    if (value == null)
                        PULWebClient.RemoveDownloadFileCompleted().RemoveTargetInstanceBody(m_target);
                    else
                        PULWebClient.RemoveDownloadFileCompleted().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.ComponentModel.AsyncCompletedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULWebClient.RemoveDownloadFileCompleted().Info; }
            }
        } 
        public zzDownloadFileAsync DownloadFileAsync() 
        {
            return new zzDownloadFileAsync(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDownloadFileAsync : IBehaviorPreparable 
        {
            UntestableLibrary.ULWebClient m_target;

            public zzDownloadFileAsync(UntestableLibrary.ULWebClient target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULWebClient, System.Uri> Body
            {
                get
                {
                    return PULWebClient.DownloadFileAsync().Body;
                }
                set
                {
                    if (value == null)
                        PULWebClient.DownloadFileAsync().RemoveTargetInstanceBody(m_target);
                    else
                        PULWebClient.DownloadFileAsync().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULWebClient, System.Uri>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULWebClient.DownloadFileAsync().Info; }
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
