
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

        public zzGetMemory GetMemory() 
        {
            return new zzGetMemory(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzGetMemory : IBehaviorPreparable 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzGetMemory(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            public IndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean> Body
            {
                get
                {
                    return PULSharedMemory.GetMemory().Body;
                }
                set
                {
                    if (value == null)
                        PULSharedMemory.GetMemory().RemoveTargetInstanceBody(m_target);
                    else
                        PULSharedMemory.GetMemory().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionOutFunc<UntestableLibrary.ULSharedMemory, System.Int32, System.Byte[], System.Boolean>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULSharedMemory.GetMemory().Info; }
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
        public zzAddOnDisposed AddOnDisposed() 
        {
            return new zzAddOnDisposed(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class zzAddOnDisposed : IBehaviorPreparable 
        {
            UntestableLibrary.ULSharedMemory m_target;

            public zzAddOnDisposed(UntestableLibrary.ULSharedMemory target)
            {
                m_target = target;
            }

            public IndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler> Body
            {
                get
                {
                    return PULSharedMemory.AddOnDisposed().Body;
                }
                set
                {
                    if (value == null)
                        PULSharedMemory.AddOnDisposed().RemoveTargetInstanceBody(m_target);
                    else
                        PULSharedMemory.AddOnDisposed().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = IndirectionDelegates.CreateDelegateOfDefaultBehaviorIndirectionAction<UntestableLibrary.ULSharedMemory, UntestableLibrary.ULSharedMemory.DisposedEventHandler>(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PULSharedMemory.AddOnDisposed().Info; }
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
