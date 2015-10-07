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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Test.Urasandesu.Prig.Framework.TestUtilities;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.Prig.Delegates;
using Urasandesu.Prig.Framework;
using Urasandesu.Prig.Framework.PilotStubberConfiguration;
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
            InstanceGetters.NewIndirectionAssemblyRepository = () => new MockIndirectionAssemblyRepository();
            InstanceGetters.NewAdditionalDelegatesAssemblyRepository = () => new MockAdditionalDelegatesAssemblyRepository();
        }

        [TearDown]
        public void TearDown()
        {
            InstanceGetters.NewAdditionalDelegatesAssemblyRepository = null;
            InstanceGetters.NewIndirectionAssemblyRepository = null;
            LooseCrossDomainAccessor.Clear();
        }



        [Test]
        public void IndirectionHolder_can_transport_delegates_to_different_AppDomain()
        {
            // Arrange
            PDateTime.NowGet().Body = () => new DateTime(2014, 1, 1);
            PList<int>.AddT().Body = (@this, item) => { @this.Add(item); @this.Add(item); };
            PArray.ExistsOfTTArrayPredicateOfT<int>().Body = (array, match) => match(array[0]);

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                // Act
                var get_now = PDateTime.NowGet().Body;
                var add = PList<int>.AddT().Body;
                var addResults = new List<int>();
                add(addResults, 10);
                var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


                // Assert
                Assert.AreEqual(new DateTime(2014, 1, 1), get_now());
                CollectionAssert.AreEqual(new int[] { 10, 10 }, addResults);
                Assert.IsFalse(exists(new int[] { 10, 42 }, _ => _ == 42));
            });
        }



        [Test]
        public void IndirectionHolder_can_transport_untyped_delegates_to_different_AppDomain()
        {
            // Arrange
            {
                PULColumns.ValidateStateULTableStatus().Body = args => { throw new NotSupportedException(); };
                #region Prepare JIT. This simulates the behavior during profiling. Actually, it is unnecessary to go that far with that.
                var validateStateULTableStatus = PULColumns.ValidateStateULTableStatus().Body;
                Assert.Throws<NotSupportedException>(() => validateStateULTableStatus(new object[] { new ULTableStatus() }));
                #endregion
            }

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                InstanceGetters.NewAdditionalDelegatesAssemblyRepository = () => new MockAdditionalDelegatesAssemblyRepository();

                // Act
                var validateStateULTableStatus = PULColumns.ValidateStateULTableStatus().Body;


                // Assert
                Assert.Throws<NotSupportedException>(() => validateStateULTableStatus(new object[] { new ULTableStatus() }));
            });
        }



        [Test]
        public void IndirectionsContextDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_globally()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.AddT()).
                Include(PArray.ExistsOfTTArrayPredicateOfT<int>()).
                DefaultBehavior = IndirectionBehaviors.NotImplemented;

            
            // Act
            var get_now = PDateTime.NowGet().Body;
            var add = PList<int>.AddT().Body;
            var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


            // Assert
            Assert.Throws<NotImplementedException>(() => get_now());
            Assert.Throws<NotImplementedException>(() => add(null, 0));
            Assert.Throws<NotImplementedException>(() => exists(null, null));
        }



        [Test]
        public void IndirectionsContextDefaultBehavior_can_apply_default_behavior_to_return_default_value_globally()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.AddT()).
                Include(PArray.ExistsOfTTArrayPredicateOfT<int>()).
                DefaultBehavior = IndirectionBehaviors.DefaultValue;


            // Act
            var get_now = PDateTime.NowGet().Body;
            var add = PList<int>.AddT().Body;
            var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


            // Assert
            Assert.AreEqual(default(DateTime), get_now());
            Assert.DoesNotThrow(() => add(null, 10));
            Assert.IsFalse(exists(null, null));
        }



        [Test]
        public void IndirectionsContextDefaultBehavior_can_apply_default_behavior_to_throw_FallthroughException_globally()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.AddT()).
                Include(PArray.ExistsOfTTArrayPredicateOfT<int>()).
                DefaultBehavior = IndirectionBehaviors.Fallthrough;


            // Act
            var get_now = PDateTime.NowGet().Body;
            var add = PList<int>.AddT().Body;
            var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


            // Assert
            Assert.Throws<FallthroughException>(() => get_now());
            Assert.Throws<FallthroughException>(() => add(null, 0));
            Assert.Throws<FallthroughException>(() => exists(null, null));
        }



        [Test]
        public void PArrayDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_against_one_type()
        {
            // Arrange
            PArray.
                ExcludeGeneric().
                IncludeExistsOfTTArrayPredicateOfT<int>().
                DefaultBehavior = IndirectionBehaviors.NotImplemented;


            // Act
            var get_now = PDateTime.NowGet().Body;
            var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


            // Assert
            Assert.IsNull(get_now);
            Assert.Throws<NotImplementedException>(() => exists(null, null));
        }



        [Test]
        public void PArrayDefaultBehavior_can_apply_default_behavior_to_return_default_value_against_one_type()
        {
            // Arrange
            PArray.
                ExcludeGeneric().
                IncludeExistsOfTTArrayPredicateOfT<int>().
                DefaultBehavior = IndirectionBehaviors.DefaultValue;


            // Act
            var get_now = PDateTime.NowGet().Body;
            var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


            // Assert
            Assert.IsNull(get_now);
            Assert.IsFalse(exists(null, null));
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
                IncludeExistsOfTTArrayPredicateOfT<int>().
                DefaultBehavior = IndirectionBehaviors.Fallthrough;


            // Act
            var get_now = PDateTime.NowGet().Body;
            var exists = PArray.ExistsOfTTArrayPredicateOfT<int>().Body;


            // Assert
            Assert.Throws<NotImplementedException>(() => get_now());
            Assert.Throws<FallthroughException>(() => exists(null, null));
        }



        [Test]
        public void PProxyListDefaultBehavior_can_apply_default_behavior_to_throw_NotImplementedException_against_one_instance()
        {
            // Arrange
            var proxy = new PProxyList<int>();
            proxy.
                ExcludeGeneric().
                IncludeAddT().
                DefaultBehavior = IndirectionBehaviors.NotImplemented;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source = proxy.AddT().Body;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source = (List<int>)proxy;


            // Act
            var addTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source;
            var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source;
            var addOther = PList<int>.AddT().Body;


            // Assert
            Assert.Throws<NotImplementedException>(() => addTarget(target, 0));
            Assert.Throws<FallthroughException>(() => addTarget(new List<int>(), 0));
            Assert.Throws<FallthroughException>(() => addOther(null, 0));
        }



        [Test]
        public void PProxyListDefaultBehavior_can_apply_default_behavior_to_return_default_value_against_one_instance()
        {
            // Arrange
            var proxy = new PProxyList<int>();
            proxy.
                ExcludeGeneric().
                IncludeAddT().
                DefaultBehavior = IndirectionBehaviors.DefaultValue;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source = proxy.AddT().Body;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source = (List<int>)proxy;


            // Act
            var addTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source;
            var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source;
            var addOther = PList<int>.AddT().Body;


            // Assert
            Assert.DoesNotThrow(() => addTarget(target, 0));
            Assert.Throws<FallthroughException>(() => addTarget(new List<int>(), 0));
            Assert.Throws<FallthroughException>(() => addOther(null, 0));
        }



        [Test]
        public void PProxyListDefaultBehavior_can_apply_default_behavior_to_throw_FallthroughException_against_one_instance()
        {
            // Arrange
            IndirectionsContext.
                ExcludeGeneric().
                Include(PList<int>.AddT()).
                DefaultBehavior = IndirectionBehaviors.NotImplemented;

            var proxy = new PProxyList<int>();
            proxy.
                ExcludeGeneric().
                IncludeAddT().
                DefaultBehavior = IndirectionBehaviors.Fallthrough;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source = proxy.AddT().Body;
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source = (List<int>)proxy;


            // Act
            var addTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<IndirectionAction<List<int>, int>>>().Source;
            var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<List<int>>>().Source;
            var addOther = PList<int>.AddT().Body;


            // Assert
            Assert.Throws<FallthroughException>(() => addTarget(target, 0));
            Assert.Throws<NotImplementedException>(() => addTarget(new List<int>(), 0));
            Assert.Throws<NotImplementedException>(() => addOther(null, 0));
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
                var bag = TaggedBagFactory<OfPJapaneseLunisolarCalendar.GetYearInfoInt32Int32Impl>.Make(proxy.GetYearInfoInt32Int32().Body);
                LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<OfPJapaneseLunisolarCalendar.GetYearInfoInt32Int32Impl, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source = bag;
            }
            {
                var bag = TaggedBagFactory<OfPJapaneseLunisolarCalendar.GetGregorianYearInt32Int32Impl>.Make(proxy.GetGregorianYearInt32Int32().Body);
                LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<OfPJapaneseLunisolarCalendar.GetGregorianYearInt32Int32Impl, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source = bag;
            }
            LooseCrossDomainAccessor.GetOrRegister<GenericHolder<JapaneseLunisolarCalendar>>().Source = (JapaneseLunisolarCalendar)proxy;


            // Act
            var getYearInfoTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<OfPJapaneseLunisolarCalendar.GetYearInfoInt32Int32Impl, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source.Value;
            var getGregorianYearTarget = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<TaggedBag<OfPJapaneseLunisolarCalendar.GetGregorianYearInt32Int32Impl, IndirectionFunc<JapaneseLunisolarCalendar, int, int, int>>>>().Source.Value;
            var target = LooseCrossDomainAccessor.GetOrRegister<GenericHolder<JapaneseLunisolarCalendar>>().Source;


            // Assert
            Assert.Throws<NotImplementedException>(() => getGregorianYearTarget(target, 0, 0));
            Assert.Throws<FallthroughException>(() => getGregorianYearTarget(new JapaneseLunisolarCalendar(), 0, 0));
            Assert.Throws<NotImplementedException>(() => getYearInfoTarget(target, 0, 0));
            Assert.Throws<FallthroughException>(() => getYearInfoTarget(new JapaneseLunisolarCalendar(), 0, 0));
        }
    }

    public abstract class PDateTimeBase
    {
        internal const int TokenOfNowGet = 0x060002D5;
    }

    public class OfPDateTime : PDateTimeBase, IPrigTypeIntroducer
    {
        public NowGetImpl NowGet() 
        {
            return new NowGetImpl();
        }

        static readonly IndirectionStub ms_stubNowGet = NewStubNowGet();
        static IndirectionStub NewStubNowGet()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""NowGet"" alias=""NowGet"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">get_Now</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.DateTime</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.DateTime get_Now()</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.DateTime get_Now()</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class NowGetImpl : TypedBehaviorPreparableImpl
        {
            public NowGetImpl()
                : base(ms_stubNowGet, new Type[] { }, new Type[] { })
            { }
        }

        public static Type Type
        {
            get { return ms_stubNowGet.GetDeclaringTypeInstance(new Type[] { }); }
            // When generating stub, this property returns the property that is the first indirection stub.
        }
    }

    public class PDateTime : PDateTimeBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static TypedBehaviorPreparable<IndirectionFunc<System.DateTime>> NowGet()
        {
            return Stub<OfPDateTime>.Setup<IndirectionFunc<System.DateTime>>(_ => _.NowGet());
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfPDateTime>.ExcludeGeneric(new TypeBehaviorSetting());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
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
        internal const int TokenOfAddT = 0x0600224E;
    }

    public class OfPList<T> : PListBase, IPrigTypeIntroducer
    {
        public virtual AddTImpl AddT()
        {
            return new AddTImpl();
        }

        static IndirectionStub ms_stubAddT = NewStubAddT();
        static IndirectionStub NewStubAddT()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""AddT"" alias=""AddT"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Add</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Collections.Generic.List`1</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Void Add(T)</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Void Add(!T)</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AddTImpl : TypedBehaviorPreparableImpl
        {
            public AddTImpl()
                : base(ms_stubAddT, new Type[] { typeof(T) }, new Type[] { })
            { }
        }

        public static Type Type
        {
            get { return ms_stubAddT.GetDeclaringTypeInstance(new Type[] { typeof(T) }); }
            // When generating stub, this property returns the property that is the first indirection stub.
        }
    }

    public class PList<T> : PListBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static TypedBehaviorPreparable<IndirectionAction<System.Collections.Generic.List<T>, T>> AddT()
        {
            return Stub<OfPList<T>>.Setup<IndirectionAction<System.Collections.Generic.List<T>, T>>(_ => _.AddT());
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfPList<T>>.ExcludeGeneric(new TypeBehaviorSetting());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class TypeBehaviorSetting : BehaviorSetting
        {
            public TypeBehaviorSetting IncludeAddT()
            {
                Include(PList<T>.AddT());
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

    public class OfPProxyList<T> : OfPList<T>, IPrigProxyTypeIntroducer
    {
        object m_target;

        Type IPrigProxyTypeIntroducer.Type
        {
            get { return Type; }
        }
        
        void IPrigProxyTypeIntroducer.Initialize(object target)
        {
            m_target = target;
        }

        public override OfPList<T>.AddTImpl AddT()
        {
            return new AddTImpl(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class AddTImpl : OfPList<T>.AddTImpl
        {
            object m_target;

            public AddTImpl(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<AddTImpl>(m_target);
                    else
                        SetTargetInstanceBody<AddTImpl>(m_target, value);
                }
            }
        }
    }

    public class PProxyList<T>
    {
        Proxy<OfPProxyList<T>> m_proxy = new Proxy<OfPProxyList<T>>();

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public TypedBehaviorPreparable<IndirectionAction<System.Collections.Generic.List<T>, T>> AddT()
        {
            return m_proxy.Setup<IndirectionAction<System.Collections.Generic.List<T>, T>>(_ => _.AddT());
        }

        public static implicit operator List<T>(PProxyList<T> @this)
        {
            return (List<T>)@this.m_proxy.Target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            return m_proxy.ExcludeGeneric(new InstanceBehaviorSetting(this));
        }

        public class InstanceBehaviorSetting : BehaviorSetting
        {
            private PProxyList<T> m_this;

            public InstanceBehaviorSetting(PProxyList<T> @this)
            {
                m_this = @this;
            }
            public InstanceBehaviorSetting IncludeAddT()
            {
                Include(m_this.AddT());
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
        internal const int TokenOfExistsOfTTArrayPredicateOfT = 0x060001D0; 
    }

    public class OfPArray : PArrayBase, IPrigTypeIntroducer
    {
        public ExistsOfTTArrayPredicateOfTImpl<T> ExistsOfTTArrayPredicateOfT<T>()
        {
            return new ExistsOfTTArrayPredicateOfTImpl<T>();
        }

        static readonly IndirectionStub ms_stubExistsOfTTArrayPredicateOfT = NewStubExistsOfTTArrayPredicateOfT();
        static IndirectionStub NewStubExistsOfTTArrayPredicateOfT()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""ExistsOfTTArrayPredicateOfT"" alias=""ExistsOfTTArrayPredicateOfT"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Exists</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Array</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Boolean Exists[T](T[], System.Predicate`1[T])</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Boolean Exists[T](!!T[], System.Predicate`1[T])</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ExistsOfTTArrayPredicateOfTImpl<T> : TypedBehaviorPreparableImpl
        {
            public ExistsOfTTArrayPredicateOfTImpl()
                : base(ms_stubExistsOfTTArrayPredicateOfT, new Type[] { }, new Type[] { typeof(T) })
            { }
        }

        public static Type Type
        {
            get { return ms_stubExistsOfTTArrayPredicateOfT.GetDeclaringTypeInstance(new Type[] { }); }
            // When generating stub, this property returns the property that is the first indirection stub.
        }
    }

    public class PArray : PArrayBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static TypedBehaviorPreparable<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>> ExistsOfTTArrayPredicateOfT<T>()
        {
            return Stub<OfPArray>.Setup<IndirectionFunc<T[], System.Predicate<T>, System.Boolean>>(_ => _.ExistsOfTTArrayPredicateOfT<T>());
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfPArray>.ExcludeGeneric(new TypeBehaviorSetting());
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public TypeBehaviorSetting IncludeExistsOfTTArrayPredicateOfT<T>()
            {
                Include(PArray.ExistsOfTTArrayPredicateOfT<T>());
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

    public class OfPJapaneseLunisolarCalendar : PJapaneseLunisolarCalendarBase, IPrigTypeIntroducer
    {
        public virtual GetYearInfoInt32Int32Impl GetYearInfoInt32Int32()
        {
            return new GetYearInfoInt32Int32Impl();
        }

        static readonly IndirectionStub ms_stubGetYearInfoInt32Int32 = NewStubGetYearInfoInt32Int32();
        static IndirectionStub NewStubGetYearInfoInt32Int32()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""GetYearInfoInt32Int32"" alias=""GetYearInfoInt32Int32"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">GetYearInfo</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Globalization.JapaneseLunisolarCalendar</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Int32 GetYearInfo(Int32, Int32)</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Int32 GetYearInfo(System.Int32, System.Int32)</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class GetYearInfoInt32Int32Impl : TypedBehaviorPreparableImpl
        {
            public GetYearInfoInt32Int32Impl()
                : base(ms_stubGetYearInfoInt32Int32, new Type[] { }, new Type[] { })
            { }
        }

        public virtual GetGregorianYearInt32Int32Impl GetGregorianYearInt32Int32()
        {
            return new GetGregorianYearInt32Int32Impl();
        }

        static readonly IndirectionStub ms_stubGetGregorianYearInt32Int32 = NewStubGetGregorianYearInt32Int32();
        static IndirectionStub NewStubGetGregorianYearInt32Int32()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""GetGregorianYearInt32Int32"" alias=""GetGregorianYearInt32Int32"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
          <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">GetGregorianYear</Name>
          <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</AssemblyName>
          <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Globalization.JapaneseLunisolarCalendar</ClassName>
          <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Int32 GetGregorianYear(Int32, Int32)</Signature>
          <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Int32 GetGregorianYear(System.Int32, System.Int32)</Signature2>
          <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
          <GenericArguments i:nil=""true"" xmlns="""" />
        </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class GetGregorianYearInt32Int32Impl : TypedBehaviorPreparableImpl
        {
            public GetGregorianYearInt32Int32Impl()
                : base(ms_stubGetGregorianYearInt32Int32, new Type[] { }, new Type[] { })
            { }
        }

        public static Type Type
        {
            get { return ms_stubGetYearInfoInt32Int32.GetDeclaringTypeInstance(new Type[] { }); }
            // When generating stub, this property returns the property that is the first indirection stub.
        }
    }

    public class PJapaneseLunisolarCalendar : PJapaneseLunisolarCalendarBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static TypedBehaviorPreparable<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>> GetYearInfoInt32Int32()
        {
            return Stub<OfPJapaneseLunisolarCalendar>.Setup<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(_ => _.GetYearInfoInt32Int32());
        }

        public static TypedBehaviorPreparable<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>> GetGregorianYearInt32Int32()
        {
            return Stub<OfPJapaneseLunisolarCalendar>.Setup<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(_ => _.GetGregorianYearInt32Int32());
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfPJapaneseLunisolarCalendar>.ExcludeGeneric(new TypeBehaviorSetting());
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

    public class OfPProxyJapaneseLunisolarCalendar : OfPJapaneseLunisolarCalendar, IPrigProxyTypeIntroducer
    {
        object m_target;

        Type IPrigProxyTypeIntroducer.Type
        {
            get { return Type; }
        }

        void IPrigProxyTypeIntroducer.Initialize(object target)
        {
            m_target = target;
        }

        public override OfPJapaneseLunisolarCalendar.GetYearInfoInt32Int32Impl GetYearInfoInt32Int32()
        {
            return new GetYearInfoInt32Int32Impl(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class GetYearInfoInt32Int32Impl : OfPJapaneseLunisolarCalendar.GetYearInfoInt32Int32Impl
        {
            object m_target;

            public GetYearInfoInt32Int32Impl(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<GetYearInfoInt32Int32Impl>(m_target);
                    else
                        SetTargetInstanceBody<GetYearInfoInt32Int32Impl>(m_target, value);
                }
            }
        }

        public override OfPJapaneseLunisolarCalendar.GetGregorianYearInt32Int32Impl GetGregorianYearInt32Int32()
        {
            return new GetGregorianYearInt32Int32Impl(m_target);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new class GetGregorianYearInt32Int32Impl : OfPJapaneseLunisolarCalendar.GetGregorianYearInt32Int32Impl
        {
            object m_target;

            public GetGregorianYearInt32Int32Impl(object target)
                : base()
            {
                m_target = target;
            }

            public override Delegate Body
            {
                get { return base.Body; }
                set
                {
                    if (value == null)
                        RemoveTargetInstanceBody<GetGregorianYearInt32Int32Impl>(m_target);
                    else
                        SetTargetInstanceBody<GetGregorianYearInt32Int32Impl>(m_target, value);
                }
            }
        }
    }

    public class PProxyJapaneseLunisolarCalendar
    {
        Proxy<OfPProxyJapaneseLunisolarCalendar> m_proxy = new Proxy<OfPProxyJapaneseLunisolarCalendar>();

        public IndirectionBehaviors DefaultBehavior { get; internal set; }

        public TypedBehaviorPreparable<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>> GetYearInfoInt32Int32()
        {
            return m_proxy.Setup<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(_ => _.GetYearInfoInt32Int32());
        }

        public TypedBehaviorPreparable<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>> GetGregorianYearInt32Int32()
        {
            return m_proxy.Setup<IndirectionFunc<System.Globalization.JapaneseLunisolarCalendar, System.Int32, System.Int32, System.Int32>>(_ => _.GetGregorianYearInt32Int32());
        }

        public static implicit operator System.Globalization.JapaneseLunisolarCalendar(PProxyJapaneseLunisolarCalendar @this)
        {
            return (System.Globalization.JapaneseLunisolarCalendar)@this.m_proxy.Target;
        }

        public InstanceBehaviorSetting ExcludeGeneric()
        {
            return m_proxy.ExcludeGeneric(new InstanceBehaviorSetting(this));
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

    class ULTableStatus
    {
        internal bool IsOpened = false;
        internal int RowsCount = 0;
    }

    public class ULColumns
    {
        static void ValidateState(ULTableStatus status)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class PULColumnsBase
    {
        internal const int TokenOfValidateStateULTableStatus = 0x06000009;
    }

    public class OfPULColumns : PULColumnsBase, IPrigTypeIntroducer
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public virtual ValidateStateULTableStatusImpl ValidateStateULTableStatus()
        {
            return new ValidateStateULTableStatusImpl();
        }

        static IndirectionStub ms_stubValidateStateULTableStatus = NewStubValidateStateULTableStatus();
        static IndirectionStub NewStubValidateStateULTableStatus()
        {
            var stubsXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<stubs>
  <add name=""ValidateStateULTableStatus"" alias=""ValidateStateULTableStatus"">
    <RuntimeMethodInfo xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:x=""http://www.w3.org/2001/XMLSchema"" z:Id=""1"" z:FactoryType=""MemberInfoSerializationHolder"" z:Type=""System.Reflection.MemberInfoSerializationHolder"" z:Assembly=""0"" xmlns:z=""http://schemas.microsoft.com/2003/10/Serialization/"" xmlns=""http://schemas.datacontract.org/2004/07/System.Reflection"">
      <Name z:Id=""2"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">ValidateState</Name>
      <AssemblyName z:Id=""3"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Test.Urasandesu.Prig.Framework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=acabb3ef0ebf69ce</AssemblyName>
      <ClassName z:Id=""4"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Test.Urasandesu.Prig.Framework.ULColumns</ClassName>
      <Signature z:Id=""5"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">Void ValidateState(Test.Urasandesu.Prig.Framework.ULTableStatus)</Signature>
      <Signature2 z:Id=""6"" z:Type=""System.String"" z:Assembly=""0"" xmlns="""">System.Void ValidateState(Test.Urasandesu.Prig.Framework.ULTableStatus)</Signature2>
      <MemberType z:Id=""7"" z:Type=""System.Int32"" z:Assembly=""0"" xmlns="""">8</MemberType>
      <GenericArguments i:nil=""true"" xmlns="""" />
    </RuntimeMethodInfo>
  </add>
</stubs>";
            var section = new PrigSection();
            section.DeserializeStubs(stubsXml);
            return section.Stubs.First();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public class ValidateStateULTableStatusImpl : UntypedBehaviorPreparableImpl
        {
            public ValidateStateULTableStatusImpl()
                : base(ms_stubValidateStateULTableStatus, new Type[] { }, new Type[] { })
            { }
        }

        public static Type Type
        {
            get { return ms_stubValidateStateULTableStatus.GetDeclaringTypeInstance(new Type[] { }); }
            // When generating stub, this property returns the property that is the first indirection stub.
        }
    }

    public class PULColumns : PULColumnsBase
    {
        public static IndirectionBehaviors DefaultBehavior { get; internal set; }

        public static UntypedBehaviorPreparable ValidateStateULTableStatus()
        {
            return Stub<OfPULColumns>.Setup(_ => _.ValidateStateULTableStatus());
        }

        public static TypeBehaviorSetting ExcludeGeneric()
        {
            return Stub<OfPULColumns>.ExcludeGeneric(new TypeBehaviorSetting());
        }

        public class TypeBehaviorSetting : BehaviorSetting
        {
            public override IndirectionBehaviors DefaultBehavior
            {
                set
                {
                    PULColumns.DefaultBehavior = value;
                    foreach (var preparable in Preparables)
                        preparable.Prepare(PULColumns.DefaultBehavior);
                }
            }
        }
    }
}
