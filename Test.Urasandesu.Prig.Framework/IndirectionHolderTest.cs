/* 
 * File: IndirectionHolderTest.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2014 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */


using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;
using Assert = Test.Urasandesu.Prig.Framework.TestUtilities.LooseCrossDomainAssert;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class IndirectionHolderTest
    {
        [SetUp]
        public void SetUp()
        {
            LooseCrossDomainAccessor.Clear();
            IndirectionsContext.NewAssemblyRepository = () => new MockIndirectionAssemblyRepository();
        }

        [TearDown]
        public void TearDown()
        {
            IndirectionsContext.NewAssemblyRepository = null;
            LooseCrossDomainAccessor.Clear();
        }



        [Test]
        public void IndirectionHolder_can_transport_delegates_to_different_AppDomain()
        {
            // Arrange
            PDateTime.NowGet().Body = () => new DateTime(2014, 1, 1);
            PList<int>.Add().Body = (@this, item) => { @this.Add(item); @this.Add(item); };
            PArray.Exists<int>().Body = (array, match) => match(array[0]);

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var add = PList<int>.Add().Body;
                var addResults = new List<int>();
                add(addResults, 10);
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.AreEqual(new DateTime(2014, 1, 1), get_now());
                CollectionAssert.AreEqual(new int[] { 10, 10 }, addResults);
                Assert.IsFalse(exists(new int[] { 10, 42 }, _ => _ == 42));
            });
        }



        [Test]
        public void IndirectionsContextDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_globally()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.Add()).
                Include(PArray.Exists<int>()).
                DefaultBehavior = IndirectionBehaviors.NotImplemented;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var add = PList<int>.Add().Body;
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.Throws<NotImplementedException>(() => get_now());
                Assert.Throws<NotImplementedException>(() => add(null, 0));
                Assert.Throws<NotImplementedException>(() => exists(null, null));
            });
        }



        [Test]
        public void IndirectionsContextDefaultBehavior_can_apply_default_behavior_to_return_default_value_globally()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.Add()).
                Include(PArray.Exists<int>()).
                DefaultBehavior = IndirectionBehaviors.DefaultValue;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var add = PList<int>.Add().Body;
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.AreEqual(default(DateTime), get_now());
                Assert.DoesNotThrow(() => add(null, 10));
                Assert.IsFalse(exists(null, null));
            });
        }



        [Test]
        public void IndirectionsContextDefaultBehavior_can_apply_default_behavior_to_throw_FallthroughException_globally()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.Add()).
                Include(PArray.Exists<int>()).
                DefaultBehavior = IndirectionBehaviors.Fallthrough;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var add = PList<int>.Add().Body;
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.Throws<FallthroughException>(() => get_now());
                Assert.Throws<FallthroughException>(() => add(null, 0));
                Assert.Throws<FallthroughException>(() => exists(null, null));
            });
        }



        [Test]
        public void PArrayDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_against_one_type()
        {
            // Arrange
            PArray.
                ExcludeGeneric().
                IncludeExists<int>().
                DefaultBehavior = IndirectionBehaviors.NotImplemented;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.IsNull(get_now);
                Assert.Throws<NotImplementedException>(() => exists(null, null));
            });
        }



        [Test]
        public void PArrayDefaultBehavior_can_apply_default_behavior_to_return_default_value_against_one_type()
        {
            // Arrange
            PArray.
                ExcludeGeneric().
                IncludeExists<int>().
                DefaultBehavior = IndirectionBehaviors.DefaultValue;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.IsNull(get_now);
                Assert.IsFalse(exists(null, null));
            });
        }



        [Test]
        public void PArrayDefaultBehavior_can_apply_default_behavior_to_throw_FallthroughException_against_one_type()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                DefaultBehavior = IndirectionBehaviors.NotImplemented;

            PArray.
                ExcludeGeneric().
                IncludeExists<int>().
                DefaultBehavior = IndirectionBehaviors.Fallthrough;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var exists = PArray.Exists<int>().Body;


                // Assert
                Assert.Throws<NotImplementedException>(() => get_now());
                Assert.Throws<FallthroughException>(() => exists(null, null));
            });
        }



        [Test]
        public void PProxyListDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_against_one_instance()
        {
            // Arrange
            var proxy = new PProxyList<int>();
            proxy.
                ExcludeGeneric().
                IncludeAdd().
                DefaultBehavior = IndirectionBehaviors.NotImplemented;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source = proxy.Add().Body;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source = (List<int>)proxy;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var addTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source;
                var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source;
                var addOther = PList<int>.Add().Body;


                // Assert
                Assert.Throws<NotImplementedException>(() => addTarget(target, 0));
                Assert.Throws<FallthroughException>(() => addTarget(new List<int>(), 0));
                Assert.Throws<FallthroughException>(() => addOther(null, 0));
            });
        }



        [Test]
        public void PProxyListDefaultBehavior_can_apply_default_behavior_to_return_default_value_against_one_instance()
        {
            // Arrange
            var proxy = new PProxyList<int>();
            proxy.
                ExcludeGeneric().
                IncludeAdd().
                DefaultBehavior = IndirectionBehaviors.DefaultValue;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source = proxy.Add().Body;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source = (List<int>)proxy;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var addTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source;
                var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source;
                var addOther = PList<int>.Add().Body;


                // Assert
                Assert.DoesNotThrow(() => addTarget(target, 0));
                Assert.Throws<FallthroughException>(() => addTarget(new List<int>(), 0));
                Assert.Throws<FallthroughException>(() => addOther(null, 0));
            });
        }



        [Test]
        public void PProxyListDefaultBehavior_can_apply_default_behavior_to_throw_FallthroughException_against_one_instance()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.Add()).
                DefaultBehavior = IndirectionBehaviors.NotImplemented;

            var proxy = new PProxyList<int>();
            proxy.
                ExcludeGeneric().
                IncludeAdd().
                DefaultBehavior = IndirectionBehaviors.Fallthrough;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source = proxy.Add().Body;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source = (List<int>)proxy;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var addTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source;
                var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source;
                var addOther = PList<int>.Add().Body;


                // Assert
                Assert.Throws<FallthroughException>(() => addTarget(target, 0));
                Assert.Throws<NotImplementedException>(() => addTarget(new List<int>(), 0));
                Assert.Throws<NotImplementedException>(() => addOther(null, 0));
            });
        }



        [Test]
        public void PProxyJapaneseLunisolarCalendarDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_against_one_instance()
        {
            // Arrange
            var proxy = new PProxyJapaneseLunisolarCalendar();
            proxy.
                ExcludeGeneric().
                DefaultBehavior = IndirectionBehaviors.NotImplemented;
            {
                var bag = TaggedBagFactory<PJapaneseLunisolarCalendar.zzGetGregorianYear>.Make(proxy.GetGregorianYear().Body);
                LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<PJapaneseLunisolarCalendar.zzGetGregorianYear, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source = bag;
            }
            {
                var bag = TaggedBagFactory<PJapaneseLunisolarCalendar.zzGetYearInfo>.Make(proxy.GetYearInfo().Body);
                LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<PJapaneseLunisolarCalendar.zzGetYearInfo, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source = bag;
            }
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<JapaneseLunisolarCalendar>>().Source = (JapaneseLunisolarCalendar)proxy;

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var getGregorianYearTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<PJapaneseLunisolarCalendar.zzGetGregorianYear, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source.Value;
                var getYearInfoTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<PJapaneseLunisolarCalendar.zzGetYearInfo, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source.Value;
                var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<JapaneseLunisolarCalendar>>().Source;


                // Assert
                Assert.Throws<NotImplementedException>(() => getGregorianYearTarget(target, 0, 0));
                Assert.Throws<FallthroughException>(() => getGregorianYearTarget(new JapaneseLunisolarCalendar(), 0, 0));
                Assert.Throws<NotImplementedException>(() => getYearInfoTarget(target, 0, 0));
                Assert.Throws<FallthroughException>(() => getYearInfoTarget(new JapaneseLunisolarCalendar(), 0, 0));
            });
        }

        class MockIndirectionAssemblyRepository : IndirectionAssemblyRepository
        {
            public override IEnumerable<Assembly> FindAll()
            {
                yield return Assembly.Load("Test.Urasandesu.Prig.Framework");
            }
        }
    }

    public abstract class PDateTimeBase
    {
        internal const int TokenOfNowGet = 0x060002D5;
    }

    public class PDateTime : PDateTimeBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzNowGet NowGet()
        {
            return new zzNowGet();
        }

        public class zzNowGet : IBehaviorPreparable
        {
            public IndirectionFunc<System.DateTime> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.DateTime>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.DateTime>>>();
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(Body);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionFunc<System.DateTime>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfNowGet;
                    return info;
                }
            }
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PDateTime).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>();
            var setting = new TypeBehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PDateTime.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PDateTime.DefaultBehavior);
                }
            }
        }
    }

    public abstract class PListBase
    {
        internal const int TokenOfAdd_T = 0x06001A4B;
    }

    public class PList<T> : PListBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzAdd Add()
        {
            return new zzAdd();
        }

        public class zzAdd : IBehaviorPreparable
        {
            public IndirectionAction<List<T>, T> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<List<T>, T>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionAction<List<T>, T>>>();
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(Body);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionAction<List<T>, T>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfAdd_T;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(List<T> target, IndirectionAction<List<T>, T> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAdd, Dictionary<List<T>, TargetSettingValue<IndirectionAction<List<T>, T>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzAdd>.Make(new Dictionary<List<T>, TargetSettingValue<IndirectionAction<List<T>, T>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? HelperForIndirectionAction<List<T>, T>.CreateDelegateOfDefaultBehavior(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<List<T>, T>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = HelperForIndirectionAction<List<T>, T>.CreateDelegateExecutingDefaultOr(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionAction<List<T>, T>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(List<T> target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzAdd, Dictionary<List<T>, TargetSettingValue<IndirectionAction<List<T>, T>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionAction<List<T>, T>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PList<T>).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>();
            var setting = new TypeBehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public TypeBehaviorSetting IncludeAdd()
            {
                Include(PList<T>.Add());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PList<T>.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PList<T>.DefaultBehavior);
                }
            }
        }
    }

    public class PProxyList<T>
    {
        List<T> m_target;

        public PProxyList()
        {
            m_target = (List<T>)FormatterServices.GetUninitializedObject(typeof(List<T>));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzAdd Add()
        {
            return new zzAdd(m_target);
        }

        public class zzAdd : IBehaviorPreparable
        {
            List<T> m_target;

            public zzAdd(List<T> target)
            {
                m_target = target;
            }

            public IndirectionAction<List<T>, T> Body
            {
                get
                {
                    return PList<T>.Add().Body;
                }
                set
                {
                    if (value == null)
                        PList<T>.Add().RemoveTargetInstanceBody(m_target);
                    else
                        PList<T>.Add().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionAction<List<T>, T>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PList<T>.Add().Info; }
            }
        }

        public static implicit operator List<T>(PProxyList<T> @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyList<T>).GetNestedTypes().
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
            private PProxyList<T> m_this;

            public InstanceBehaviorSetting(PProxyList<T> @this)
            {
                m_this = @this;
            }
            public InstanceBehaviorSetting IncludeAdd()
            {
                Include(m_this.Add());
                return this;
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

    public abstract class PArrayBase
    {
        internal const int TokenOfExists_T_TArray_Predicate_T = 0x06000068;
    }

    public class PArray : PArrayBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzExists<T> Exists<T>()
        {
            return new zzExists<T>();
        }

        public class zzExists<T> : IBehaviorPreparable
        {
            public IndirectionFunc<T[], System.Predicate<T>, System.Boolean> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>>();
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(Body);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionFunc<T[], System.Predicate<T>, System.Boolean>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfExists_T_TArray_Predicate_T;
                    return info;
                }
            }
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PArray).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>();
            var setting = new TypeBehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public TypeBehaviorSetting IncludeExists<T>()
            {
                Include(PArray.Exists<T>());
                return this;
            }

            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PArray.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PArray.DefaultBehavior);
                }
            }
        }
    }

    public abstract class PJapaneseLunisolarCalendarBase
    {
        internal const int TokenOfGetYearInfo_int_int = 0x06002669;
        internal const int TokenOfGetGregorianYear = 0x0600266B;
    }

    public class PJapaneseLunisolarCalendar : PJapaneseLunisolarCalendarBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static zzGetYearInfo GetYearInfo()
        {
            return new zzGetYearInfo();
        }

        public class zzGetYearInfo : IBehaviorPreparable
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(Body);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetYearInfo_int_int;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(JapaneseLunisolarCalendar target, IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetYearInfo, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzGetYearInfo>.Make(new Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateOfDefaultBehavior(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateExecutingDefaultOr(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(JapaneseLunisolarCalendar target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetYearInfo, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }

        public static zzGetGregorianYear GetGregorianYear()
        {
            return new zzGetGregorianYear();
        }

        public class zzGetGregorianYear : IBehaviorPreparable
        {
            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    return holder.GetOrDefault(Info);
                }
                set
                {
                    var holder = LooseCrossDomainAccessor.GetOrRegister<IndirectionHolder<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>();
                    if (value == null)
                    {
                        holder.Remove(Info);
                    }
                    else
                    {
                        holder.AddOrUpdate(Info, value);
                        RuntimeHelpers.PrepareDelegate(Body);
                    }
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get
                {
                    var info = new IndirectionInfo();
                    info.AssemblyName = "mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
                    info.Token = TokenOfGetGregorianYear;
                    return info;
                }
            }
            internal void SetTargetInstanceBody(JapaneseLunisolarCalendar target, IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> value)
            {
                RuntimeHelpers.PrepareDelegate(value);

                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetGregorianYear, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    holder.Source = TaggedBagFactory<zzGetGregorianYear>.Make(new Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>());

                if (holder.Source.Value.Count == 0)
                {
                    var behavior = Body == null ? HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateOfDefaultBehavior(IndirectionBehaviors.Fallthrough) : Body;
                    RuntimeHelpers.PrepareDelegate(behavior);
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(behavior, value);
                    {
                        // Prepare JIT
                        var original = holder.Source.Value[target].Original;
                        var indirection = holder.Source.Value[target].Indirection;
                    }
                    Body = HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateExecutingDefaultOr(behavior, holder.Source.Value);
                }
                else
                {
                    Debug.Assert(Body != null);
                    var before = holder.Source.Value[target];
                    holder.Source.Value[target] = new TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(before.Original, value);
                }
            }

            internal void RemoveTargetInstanceBody(JapaneseLunisolarCalendar target)
            {
                var holder = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<zzGetGregorianYear, Dictionary<JapaneseLunisolarCalendar, TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>>>>>();
                if (holder.Source.Value == null)
                    return;

                if (holder.Source.Value.Count == 0)
                    return;

                var before = default(TargetSettingValue<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>);
                if (holder.Source.Value.ContainsKey(target))
                    before = holder.Source.Value[target];
                holder.Source.Value.Remove(target);
                if (holder.Source.Value.Count == 0)
                    Body = before.Original;
            }
        }


        public static TypeBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PJapaneseLunisolarCalendar).GetNestedTypes().
                                          Where(_ => _.GetInterface(typeof(IBehaviorPreparable).FullName) != null).
                                          Where(_ => !_.IsGenericType).
                                          Select(_ => Activator.CreateInstance(_)).
                                          Cast<IBehaviorPreparable>();
            var setting = new TypeBehaviorSetting();
            foreach (var preparable in preparables)
                setting.Include(preparable);
            return setting;
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PJapaneseLunisolarCalendar.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PJapaneseLunisolarCalendar.DefaultBehavior);
                }
            }
        }
    }

    public class PProxyJapaneseLunisolarCalendar
    {
        System.Globalization.JapaneseLunisolarCalendar m_target;

        public PProxyJapaneseLunisolarCalendar()
        {
            m_target = (System.Globalization.JapaneseLunisolarCalendar)FormatterServices.GetUninitializedObject(typeof(System.Globalization.JapaneseLunisolarCalendar));
        }

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public zzGetYearInfo GetYearInfo()
        {
            return new zzGetYearInfo(m_target);
        }

        public class zzGetYearInfo : IBehaviorPreparable
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetYearInfo(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetYearInfo().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetYearInfo().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetYearInfo().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetYearInfo().Info; }
            }
        }

        public zzGetGregorianYear GetGregorianYear()
        {
            return new zzGetGregorianYear(m_target);
        }

        public class zzGetGregorianYear : IBehaviorPreparable
        {
            System.Globalization.JapaneseLunisolarCalendar m_target;

            public zzGetGregorianYear(System.Globalization.JapaneseLunisolarCalendar target)
            {
                m_target = target;
            }

            public IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32> Body
            {
                get
                {
                    return PJapaneseLunisolarCalendar.GetGregorianYear().Body;
                }
                set
                {
                    if (value == null)
                        PJapaneseLunisolarCalendar.GetGregorianYear().RemoveTargetInstanceBody(m_target);
                    else
                        PJapaneseLunisolarCalendar.GetGregorianYear().SetTargetInstanceBody(m_target, value);
                }
            }

            public void Prepare(IndirectionBehaviors defaultBehavior)
            {
                var behavior = HelperForIndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>.CreateDelegateOfDefaultBehavior(defaultBehavior);
                Body = behavior;
            }

            public IndirectionInfo Info
            {
                get { return PJapaneseLunisolarCalendar.GetGregorianYear().Info; }
            }
        }

        public static implicit operator System.Globalization.JapaneseLunisolarCalendar(PProxyJapaneseLunisolarCalendar @this)
        {
            return @this.m_target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            var preparables = typeof(PProxyJapaneseLunisolarCalendar).GetNestedTypes().
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
            private PProxyJapaneseLunisolarCalendar m_this;

            public InstanceBehaviorSetting(PProxyJapaneseLunisolarCalendar @this)
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
