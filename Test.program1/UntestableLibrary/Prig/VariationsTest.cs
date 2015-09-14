/* 
 * File: VariationsTest.cs
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
using System.Globalization;
using System.Text.RegularExpressions;
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class Variation_HasGenericThis_Default_bool_None_CLASS_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_bool_None_CLASS_None_None_None_None_None<int>.DoRegex().Body = (@this, r) => true;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_bool_None_CLASS_None_None_None_None_None<int>().Do(new Regex("^$")));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_TYPESPEC_ByRef_CLASS_Array_None_None_SZArray_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_TYPESPEC_ByRef_CLASS_Array_None_None_SZArray_Var<int>.DoOfMRegex<double>().Body = (@this, r) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_TYPESPEC_ByRef_CLASS_Array_None_None_SZArray_Var<int>().Do<double>(new Regex("^$")));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Generic_VALUETYPE_ByRef_TYPESPEC_None_None_MVar_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Generic_VALUETYPE_ByRef_TYPESPEC_None_None_MVar_SZArray_None.DoOfMMArrayRef<double>().Body =
                    (Variation_HasThis_Generic_VALUETYPE_ByRef_TYPESPEC_None_None_MVar_SZArray_None @this, out double[] _mArr) => { _mArr = null; return DateTime.Now; };

                // Act, Assert
                var mArr = default(double[]);
                Assert.DoesNotThrow(() => new Variation_HasThis_Generic_VALUETYPE_ByRef_TYPESPEC_None_None_MVar_SZArray_None().Do<double>(out mArr));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Default_CLASS_None_TYPESPEC_Array_None_None_None_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_CLASS_None_TYPESPEC_Array_None_None_None_Var<int>.DoT2().Body = (@this, tArr) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_CLASS_None_TYPESPEC_Array_None_None_None_Var<int>().Do(new int[1, 1]));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Generic_TYPESPEC_None_VALUETYPE_Array_None_MVar_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Generic_TYPESPEC_None_VALUETYPE_Array_None_MVar_None_None.DoOfMDateTime<double>().Body = dt => null;

                // Act, Assert
                Assert.DoesNotThrow(() => Variation_None_Generic_TYPESPEC_None_VALUETYPE_Array_None_MVar_None_None.Do<double>(DateTime.Now));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Default_TYPESPEC_None_bool_None_GenericInst_None_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Default_TYPESPEC_None_bool_None_GenericInst_None_SZArray_None.DoBoolean().Body = b => null;

                // Act, Assert
                Assert.DoesNotThrow(() => Variation_None_Default_TYPESPEC_None_bool_None_GenericInst_None_SZArray_None.Do(true));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Generic_CLASS_ByRef_TYPESPEC_None_None_MVar_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Generic_CLASS_ByRef_TYPESPEC_None_None_MVar_None_None.DoOfMMRef<double>().Body =
                    (Variation_HasThis_Generic_CLASS_ByRef_TYPESPEC_None_None_MVar_None_None @this, out double _m) => { _m = 0f; return null; };

                // Act, Assert
                var m = default(double);
                Assert.DoesNotThrow(() => new Variation_HasThis_Generic_CLASS_ByRef_TYPESPEC_None_None_MVar_None_None().Do<double>(out m));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Default_void_ByRef_VALUETYPE_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Default_void_ByRef_VALUETYPE_None_None_None_None_None.DoDateTimeRef().Body =
                    (Variation_HasThis_Default_void_ByRef_VALUETYPE_None_None_None_None_None @this, out DateTime _dt) => { _dt = DateTime.Now; };

                // Act, Assert
                var dt = default(DateTime);
                Assert.DoesNotThrow(() => new Variation_HasThis_Default_void_ByRef_VALUETYPE_None_None_None_None_None().Do(out dt));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_char_ByRef_TYPESPEC_Array_None_None_SZArray_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_char_ByRef_TYPESPEC_Array_None_None_SZArray_Var<int>.DoOfMT2ArrayRef<double>().Body =
                    (Variation_HasGenericThis_Generic_char_ByRef_TYPESPEC_Array_None_None_SZArray_Var<int> @this, out int[][,] _tArr) => { _tArr = null; return '\0'; };

                // Act, Assert
                var tArr = default(int[][,]);
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_char_ByRef_TYPESPEC_Array_None_None_SZArray_Var<int>().Do<double>(out tArr));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Generic_void_None_char_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Generic_void_None_char_None_None_None_None_None.DoOfMChar<double>().Body = (@this, c) => { };

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasThis_Generic_void_None_char_None_None_None_None_None().Do<double>('\0'));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Generic_TYPESPEC_ByRef_sbyte_Array_None_MVar_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Generic_TYPESPEC_ByRef_sbyte_Array_None_MVar_SZArray_None.DoOfMSByte<double>().Body = (@this, i1) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasThis_Generic_TYPESPEC_ByRef_sbyte_Array_None_MVar_SZArray_None().Do<double>((sbyte)42));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Default_VALUETYPE_None_CLASS_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Default_VALUETYPE_None_CLASS_None_None_None_None_None.DoRegex().Body = r => DateTime.Now;

                // Act, Assert
                Assert.DoesNotThrow(() => Variation_None_Default_VALUETYPE_None_CLASS_None_None_None_None_None.Do(new Regex("^$")));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Generic_sbyte_ByRef_TYPESPEC_Array_None_MVar_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Generic_sbyte_ByRef_TYPESPEC_Array_None_MVar_SZArray_None.DoOfMM2ArrayRef<double>().Body =
                    (Variation_HasThis_Generic_sbyte_ByRef_TYPESPEC_Array_None_MVar_SZArray_None @this, out double[][,] _mArr) => { _mArr = null; return (sbyte)0; };

                // Act, Assert
                var mArr = default(double[][,]);
                Assert.DoesNotThrow(() => new Variation_HasThis_Generic_sbyte_ByRef_TYPESPEC_Array_None_MVar_SZArray_None().Do(out mArr));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_VALUETYPE_ByRef_byte_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_VALUETYPE_ByRef_byte_None_None_None_None_None<int>.DoOfMByteRef<double>().Body =
                    (Variation_HasGenericThis_Generic_VALUETYPE_ByRef_byte_None_None_None_None_None<int> @this, out byte _u1) => { _u1 = (byte)42; return DateTime.Now; };

                // Act, Assert
                var u1 = default(byte);
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_VALUETYPE_ByRef_byte_None_None_None_None_None<int>().Do<double>(out u1));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_None_SZArray_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_None_SZArray_Var<int>.DoOfMTArray2Ref<double>().Body =
                    (Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_None_SZArray_Var<int> @this, out int[,][] _tArr) => { _tArr = null; };

                // Act, Assert
                var tArr = default(int[,][]);
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_None_SZArray_Var<int>().Do<double>(out tArr));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Generic_VALUETYPE_ByRef_TYPESPEC_Array_GenericInst_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Generic_VALUETYPE_ByRef_TYPESPEC_Array_GenericInst_None_None_None.DoOfMNullableOfDateTimeArrayRef<double>().Body = (out DateTime?[] _dtArr) => { _dtArr = null; return DateTime.Now; };

                // Act, Assert
                var dtArr = default(DateTime?[]);
                Assert.DoesNotThrow(() => Variation_None_Generic_VALUETYPE_ByRef_TYPESPEC_Array_GenericInst_None_None_None.Do<double>(out dtArr));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Default_CLASS_ByRef_VALUETYPE_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_CLASS_ByRef_VALUETYPE_None_None_None_None_None<int>.DoDateTimeRef().Body =
                    (Variation_HasGenericThis_Default_CLASS_ByRef_VALUETYPE_None_None_None_None_None<int> @this, out DateTime _dt) => { _dt = DateTime.Now; return null; };

                // Act, Assert
                var dt = default(DateTime);
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_CLASS_ByRef_VALUETYPE_None_None_None_None_None<int>().Do(out dt));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_byte_None_TYPESPEC_None_GenericInst_None_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_byte_None_TYPESPEC_None_GenericInst_None_SZArray_None<int>.DoOfMListOfDateTimeArray<double>().Body = (@this, list) => (byte)42;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_byte_None_TYPESPEC_None_GenericInst_None_SZArray_None<int>().Do<double>(new List<DateTime[]>()));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Generic_short_None_TYPESPEC_None_None_MVar_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Generic_short_None_TYPESPEC_None_None_MVar_SZArray_None.DoOfMMArray<double>().Body = mArr => (short)42;

                // Act, Assert
                Assert.DoesNotThrow(() => Variation_None_Generic_short_None_TYPESPEC_None_None_MVar_SZArray_None.Do<double>(new double[10]));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Default_CLASS_ByRef_short_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Default_CLASS_ByRef_short_None_None_None_None_None.DoInt16Ref().Body = (out short _i2) => { _i2 = (short)42; return null; };

                // Act, Assert
                var i2 = default(short);
                Assert.DoesNotThrow(() => Variation_None_Default_CLASS_ByRef_short_None_None_None_None_None.Do(out i2));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_TYPESPEC_None_VALUETYPE_None_None_None_SZArray_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_TYPESPEC_None_VALUETYPE_None_None_None_SZArray_Var<int>.DoOfMDateTime<double>().Body = (@this, dt) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_TYPESPEC_None_VALUETYPE_None_None_None_SZArray_Var<int>().Do<double>(DateTime.Now));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Default_TYPESPEC_None_CLASS_None_GenericInst_None_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Default_TYPESPEC_None_CLASS_None_GenericInst_None_SZArray_None.DoCultureInfo().Body = (@this, ci) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasThis_Default_TYPESPEC_None_CLASS_None_GenericInst_None_SZArray_None().Do(CultureInfo.CurrentCulture));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Default_CLASS_ByRef_TYPESPEC_Array_GenericInst_None_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_CLASS_ByRef_TYPESPEC_Array_GenericInst_None_SZArray_None<int>.DoListOfDateTimeArray2().Body = (@this, list) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_CLASS_ByRef_TYPESPEC_Array_GenericInst_None_SZArray_None<int>().Do(new List<DateTime[]>[2, 2]));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Generic_void_ByRef_CLASS_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Generic_void_ByRef_CLASS_None_None_None_None_None.DoOfMCultureInfoRef<double>().Body = (out CultureInfo _ci) => { _ci = null; };

                // Act, Assert
                var ci = default(CultureInfo);
                Assert.DoesNotThrow(() => Variation_None_Generic_void_ByRef_CLASS_None_None_None_None_None.Do<double>(out ci));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Default_VALUETYPE_ByRef_TYPESPEC_None_None_None_SZArray_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_VALUETYPE_ByRef_TYPESPEC_None_None_None_SZArray_Var<int>.DoTArrayRef().Body =
                    (Variation_HasGenericThis_Default_VALUETYPE_ByRef_TYPESPEC_None_None_None_SZArray_Var<int> @this, out int[] _tArr) => { _tArr = null; return 42m; };

                // Act, Assert
                var tArr = default(int[]);
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_VALUETYPE_ByRef_TYPESPEC_None_None_None_SZArray_Var<int>().Do(out tArr));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Default_ushort_None_VALUETYPE_None_None_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_ushort_None_VALUETYPE_None_None_None_None_None<int>.DoDecimal().Body = (@this, dec) => (ushort)42;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_ushort_None_VALUETYPE_None_None_None_None_None<int>().Do(42m));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Default_void_None_TYPESPEC_Array_GenericInst_None_None_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Default_void_None_TYPESPEC_Array_GenericInst_None_None_None.DoListOfDateTimeArray().Body = (@this, arr) => { };

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasThis_Default_void_None_TYPESPEC_Array_GenericInst_None_None_None().Do(new List<DateTime>[2]));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_MVar_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_MVar_SZArray_None<int>.DoOfMMArray2Ref<double>().Body =
                    (Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_MVar_SZArray_None<int> @this, out double[,][] _mArr) => { _mArr = null; };

                // Act, Assert
                var mArr = default(double[,][]);
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Generic_void_ByRef_TYPESPEC_Array_None_MVar_SZArray_None<int>().Do(out mArr));
            }
        }
    }

    [TestFixture]
    public class Variation_None_Generic_TYPESPEC_None_VALUETYPE_Array_GenericInst_None_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_None_Generic_TYPESPEC_None_VALUETYPE_Array_GenericInst_None_SZArray_None.DoOfMDecimal<double>().Body = dec => null;

                // Act, Assert
                Assert.DoesNotThrow(() => Variation_None_Generic_TYPESPEC_None_VALUETYPE_Array_GenericInst_None_SZArray_None.Do<double>(42m));
            }
        }
    }

    [TestFixture]
    public class Variation_HasThis_Generic_TYPESPEC_ByRef_CLASS_None_None_MVar_SZArray_NoneTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasThis_Generic_TYPESPEC_ByRef_CLASS_None_None_MVar_SZArray_None.DoOfMRegexRef<double>().Body =
                    (Variation_HasThis_Generic_TYPESPEC_ByRef_CLASS_None_None_MVar_SZArray_None @this, out Regex _r) => { _r = null; return null; };

                // Act, Assert
                var r = default(Regex);
                Assert.DoesNotThrow(() => new Variation_HasThis_Generic_TYPESPEC_ByRef_CLASS_None_None_MVar_SZArray_None().Do<double>(out r));
            }
        }
    }

    [TestFixture]
    public class Variation_HasGenericThis_Default_TYPESPEC_None_ushort_None_None_None_SZArray_VarTest
    {
        [Test]
        public void Do_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PVariation_HasGenericThis_Default_TYPESPEC_None_ushort_None_None_None_SZArray_Var<int>.DoUInt16().Body = (@this, u2) => null;

                // Act, Assert
                Assert.DoesNotThrow(() => new Variation_HasGenericThis_Default_TYPESPEC_None_ushort_None_None_None_SZArray_Var<int>().Do((ushort)42));
            }
        }
    }
}
