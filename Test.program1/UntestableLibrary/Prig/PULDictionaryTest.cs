/* 
 * File: PULDictionaryTest.cs
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
using System.Collections;
using System.Collections.Generic;
using UntestableLibrary;
using UntestableLibrary.Prig;
using Urasandesu.Prig.Framework;

namespace Test.program1.UntestableLibrary.Prig
{
    [TestFixture]
    public class PULDictionaryTest
    {
        [Test]
        public void IsCompatibleKey_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PULDictionary<int, string>.IsCompatibleKeyObject().Body = key => key is string;

                // Act
                var actual = ULDictionary<int, string>.IsCompatibleKey("aiueo");

                // Assert
                Assert.IsTrue(actual);
            }
        }
        
        [Test]
        public void EnumeratorCurrent_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PULDictionaryOfTKeyOfTValueEnumerator<int, string>.CurrentGet().Body = 
                    (ref ULDictionary<int, string>.Enumerator @this) => new KeyValuePair<int, string>(42, "にゃんぱすー");
                
                // Act
                var enumerator = new ULDictionary<int, string>.Enumerator();
                var actual = enumerator.Current;
                
                // Assert
                Assert.AreEqual(42, actual.Key);
                Assert.AreEqual("にゃんぱすー", actual.Value);
            }
        }

        [Test]
        public void IEnumeratorCurrent_should_be_callable_indirectly()
        {
            using (new IndirectionsContext())
            {
                // Arrange
                PULDictionaryOfTKeyOfTValueEnumerator<int, string>.SystemCollectionsIEnumeratorCurrentGet().Body =
                    (ref ULDictionary<int, string>.Enumerator @this) => new KeyValuePair<int, string>(42, "今回はここまで");

                // Act
                var enumerator = new ULDictionary<int, string>.Enumerator();
                var actual = (KeyValuePair<int, string>)((IEnumerator)enumerator).Current;

                // Assert
                Assert.AreEqual(42, actual.Key);
                Assert.AreEqual("今回はここまで", actual.Value);
            }
        }
    }
}
