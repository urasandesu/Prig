/* 
 * File: StopwatchTest.cs
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


#define SOLUTION
#define ISSUE

#if _
#elif SOLUTION
using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.Prig.Framework;
using LooseStopwatch = 
    Urasandesu.Prig.Framework.LooseCrossDomainAccessor<
        Urasandesu.Prig.Framework.GenericHolder<System.Diagnostics.Stopwatch>>;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class StopwatchTest
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            LooseCrossDomainAccessor.Clear();
            LooseStopwatch.Unload();
            LooseStopwatch.Register();
            LooseStopwatch.Holder.Source = new Stopwatch();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            LooseStopwatch.Holder.Source = null;
            LooseStopwatch.Unload();
            LooseCrossDomainAccessor.Clear();
        }

        [Test]
        public void Test()
        {
            using (var sw = new StringWriter())
            {
                LooseStopwatch.Holder.Source.Restart();

                sw.WriteLine("Elapsed: {0} ms", LooseStopwatch.Holder.Source.ElapsedMilliseconds);

                AppDomain.CurrentDomain.RunAtIsolatedDomain<StringWriter>(sw_ =>
                {
                    sw_.WriteLine("Elapsed: {0} ms", LooseStopwatch.Holder.Source.ElapsedMilliseconds);
                }, sw);

                sw.WriteLine("Elapsed: {0} ms", LooseStopwatch.Holder.Source.ElapsedMilliseconds);

                Console.WriteLine(sw.ToString());
            }
        }
    }
}
#elif ISSUE
using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class StopwatchTest
    {
        [Test]
        public void Test()
        {
            using (var sw = new StringWriter())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Restart();

                sw.WriteLine("Elapsed: {0} ms", stopwatch.ElapsedMilliseconds);

                AppDomain.CurrentDomain.RunAtIsolatedDomain<StringWriter, Stopwatch>((sw_, stopwatch_) =>
                {
                    sw_.WriteLine("Elapsed: {0} ms", stopwatch_.ElapsedMilliseconds);
                }, sw, stopwatch);

                sw.WriteLine("Elapsed: {0} ms", stopwatch.ElapsedMilliseconds);

                Console.WriteLine(sw.ToString());
            }
        }
    }
}
#endif

namespace Test.Urasandesu.Prig.Framework
{
    public static class StopwatchMixin
    {
        public static void Restart(this Stopwatch s)
        {
            s.Reset();
            s.Start();
        }
    }
}
