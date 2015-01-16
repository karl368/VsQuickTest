using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VsQuickTest.chap11_unittest.main;

namespace VsQuickTestTest.chap11_unittest.test
{
    [TestClass]
    public class SubscriptionTest
    {
        [TestMethod]
        public void CurrentStatus_NullPaidUpToDate_TemporaryStatus()
        {
            Subscription s = new Subscription();
            s.PaidUpTo = null;

            Subscription.Status actual = s.CurrentStatus;
            Assert.AreEqual(Subscription.Status.Temporary, actual, "Subscription status was not set correctly. ");

        }
    }
}
