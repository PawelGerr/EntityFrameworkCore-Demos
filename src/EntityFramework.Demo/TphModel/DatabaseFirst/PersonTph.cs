using System;
using System.Collections.Generic;

namespace EntityFramework.Demo.TphModel.DatabaseFirst
{
    public class PersonTph
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Discriminator { get; set; }
    }
}
