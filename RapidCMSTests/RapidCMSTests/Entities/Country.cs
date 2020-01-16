using System.Collections.Generic;

namespace RapidCMSTests.Entities
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public ICollection<PersonCountry> People { get; set; } = default!;
    }
}
