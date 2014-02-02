/* 
 * File: ConsoleTest.cs
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
#define ISSUE_2
#define ISSUE_1

#if _
#elif SOLUTION
using System;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;
using Urasandesu.Prig.Framework;
using LooseConsole =
    Urasandesu.Prig.Framework.LooseCrossDomainAccessor<
        Urasandesu.Prig.Framework.GenericHolder<System.IO.TextWriter>>;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class ConsoleTest
    {
        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            LooseCrossDomainAccessor.Clear();
            LooseConsole.Unload();
            LooseConsole.Register();
            LooseConsole.Holder.Source = Console.Out;

            // Pre-call to run the action that was registered in this AppDomain, 
            // not in other AppDomain but in this AppDomain.
            // Because the event loop that is managed by NUnit GUI - contains calling 
            // Write or WriteLine method - runs in other thread. 
            Console.Write(string.Empty);
            Console.Out.Flush();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            LooseConsole.Holder.Source = null;
            LooseConsole.Unload();
            LooseCrossDomainAccessor.Clear();
        }

        [Test]
        public void Test()
        {
            LooseConsole.Holder.Source.WriteLine("AppDomain: {0}",
                                                   AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                LooseConsole.Holder.Source.WriteLine("AppDomain: {0}",
                                                   AppDomain.CurrentDomain.FriendlyName);
            });

            LooseConsole.Holder.Source.WriteLine("AppDomain: {0}",
                                                   AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
#elif ISSUE_2
using System;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class ConsoleTest
    {
        [Test]
        public void Test()
        {
            Console.WriteLine("AppDomain: {0}", AppDomain.CurrentDomain.FriendlyName);

            AppDomain.CurrentDomain.RunAtIsolatedDomain(() =>
            {
                Console.WriteLine("AppDomain: {0}", AppDomain.CurrentDomain.FriendlyName);
            });

            Console.WriteLine("AppDomain: {0}", AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
#elif ISSUE_1
using System;
using NUnit.Framework;

namespace Test.Urasandesu.Prig.Framework
{
    [TestFixture]
    public class ConsoleTest
    {
        [Test]
        public void Test()
        {
            Console.WriteLine("AppDomain: {0}", AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
#endif
