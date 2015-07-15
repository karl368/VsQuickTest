/*
 test by build test framework 15 Jul 2015
 */
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VsQuickTest.basic.nunit.main;

namespace VsQuickTest.basic.nunit.test
{
    [TestClass]
    public class SubscriptionBuildInTest
    {
        [TestMethod]
        [TestCategory("Financial")]
        public void testByBuildInTestFramework()
        {
            Subscription s = new Subscription();
            s.PaidUpTo = null;

            Subscription.Status actual = s.CurrentStatus;
            Assert.AreEqual(Subscription.Status.Temporary, actual, "Subscription status was not set correctly. ");

            // this case not yet completed
            //Assert.Inconclusive();

        }


    }
}
