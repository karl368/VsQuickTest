using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsQuickTest.chap11_unittest.main
{
    public class Subscription
    {
        public enum Status
        {
            Temporary,
            Financial,
            Unfinancial,
            Suspended
        }

        // take care that ? after the type means the field could be null - without any value
        public DateTime? PaidUpTo { get; set; }

        public Status CurrentStatus
        {
            get
            {
                if (this.PaidUpTo.HasValue == false)
                    return Status.Temporary;
                if (this.PaidUpTo > DateTime.Today)
                    return Status.Financial;
                else
                {
                    if (this.PaidUpTo >= DateTime.Today.AddMonths(-3))
                        return Status.Unfinancial;
                    else
                        return Status.Suspended;
                }
            }
        }        
    }
}
