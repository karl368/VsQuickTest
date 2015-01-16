using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VsQuickTest.chap11_unittest.main;

namespace VsQuickTest_Nunit
{
    [TestFixture]
    public class SubScriptionTest
    {
        [Test]
        public void CurrentStatus_NullPaidUpToDate_TemporaryStatus_NUnit()
        {
            Subscription s = new Subscription();
            s.PaidUpTo = null;

            Subscription.Status actual = s.CurrentStatus;
            Assert.AreEqual(Subscription.Status.Temporary, actual, "Subscription status was not set correctly in nunit test. ");

        }

    }
}
