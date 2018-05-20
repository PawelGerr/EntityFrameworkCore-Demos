using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.TptModel.DatabaseFirst
{
    public partial class Customer
    {
        public Guid Id { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Person IdNavigation { get; set; }
    }
}
