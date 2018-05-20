using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.TphModel.DatabaseFirst
{
    public partial class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal? Turnover { get; set; }
        public string Discriminator { get; set; }
    }
}
