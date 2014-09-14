
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzGetMemoryInt32ByteArrayRef GetMemoryInt32ByteArrayRef() 
        {
            return new zzGetMemoryInt32ByteArrayRef(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetMemoryInt32ByteArrayRef : IBehaviorPreparable 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzGetMemoryInt32ByteArrayRef(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            public IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean> Body
            {
                get
                {
                    return PULSharedMemory.GetMemoryInt32ByteArrayRef().Body;
                }
                set
                {
                    if (value == null)
                        PULSharedMemory.GetMemoryInt32ByteArrayRef().RemoveTargetInstanceBody(m_target);
                    else
                        PULSharedMemory.GetMemoryInt32ByteArrayRef().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULSharedMemory.GetMemoryInt32ByteArrayRef().Info; }
            }
        } 
        public zzDispose Dispose() 
        {
            return new zzDispose(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzDispose : IBehaviorPreparable 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzDispose(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULSharedMemory> Body
            {
                get
                {
                    return PULSharedMemory.Dispose().Body;
                }
                set
                {
                    if (value == null)
                        PULSharedMemory.Dispose().RemoveTargetInstanceBody(m_target);
                    else
                        PULSharedMemory.Dispose().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULSharedMemory.Dispose().Info; }
            }
        } 
        public zzAddOnDisposedDisposedEventHandler AddOnDisposedDisposedEventHandler() 
        {
            return new zzAddOnDisposedDisposedEventHandler(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddOnDisposedDisposedEventHandler : IBehaviorPreparable 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzAddOnDisposedDisposedEventHandler(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> Body
            {
                get
                {
                    return PULSharedMemory.AddOnDisposedDisposedEventHandler().Body;
                }
                set
                {
                    if (value == null)
                        PULSharedMemory.AddOnDisposedDisposedEventHandler().RemoveTargetInstanceBody(m_target);
                    else
                        PULSharedMemory.AddOnDisposedDisposedEventHandler().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULSharedMemory.AddOnDisposedDisposedEventHandler().Info; }
            }
        }

        public static implicit operator UntestableLibrary.ULSharedMemory(PProxyULSharedMemory @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyULSharedMemory).GetNestedTypes().
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
            private PProxyULSharedMemory m_this;

            public InstanceBehaviorSetting(PProxyULSharedMemory @this)
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
