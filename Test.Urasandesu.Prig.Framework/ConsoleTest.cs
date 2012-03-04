//#define SOLUTION
//#define ISSUE_2
#define ISSUE_1

#if _
#elif SOLUTION
using System;
using NUnit.Framework;
using Urasandesu.NAnonym.Mixins.System;
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