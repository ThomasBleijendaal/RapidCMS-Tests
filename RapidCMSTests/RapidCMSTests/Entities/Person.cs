using System.Collections.Generic;

namespace RapidCMSTests.Entities
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public Person? Parent { get; set; } = default!;
        public int? ParentId { get; set; }

        public ICollection<PersonCountry> Countries { get; set; } = default!;
    }
}
