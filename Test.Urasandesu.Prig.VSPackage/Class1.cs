using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urasandesu.Prig.VSPackage;

namespace Test.Urasandesu.Prig.VSPackage
{
    public class Class1
    {
    }

    [TestFixture]
    public class PrigPackageControllerTest
    {
        //[Ignore("You should disable shadow copy.")]
        [Test]
        public void Hoge()
        {
            var controller = new PrigPackageController();

            controller.OnOpened(new PrigPackageViewModel());
        }
    }
    
}
