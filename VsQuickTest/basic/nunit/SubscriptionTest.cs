/*
 use unit to do the test 15 Jul 2015
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VsQuickTest.basic.nunit.main;

namespace VsQuickTest.basic.unit
{
    [TestFixture]
    public class SubScriptionTest
    {
        [Test]
        public void testByNunit()
        {
            Subscription s = new Subscription();
            s.PaidUpTo = null;

            Subscription.Status actual = s.CurrentStatus;
            Assert.AreEqual(Subscription.Status.Temporary, actual, "Subscription status was not set correctly in nunit test. ");

        }

    }
}
