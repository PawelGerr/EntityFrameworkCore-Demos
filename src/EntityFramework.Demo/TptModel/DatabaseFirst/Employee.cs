using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.TptModel.DatabaseFirst
{
    public partial class Employee
    {
        public Guid Id { get; set; }
        public decimal Turnover { get; set; }

        public Person Person { get; set; }
    }
}
