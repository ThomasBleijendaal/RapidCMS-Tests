namespace RapidCMSTests.Entities
{
    public class PersonCountry
    {
        public Person Person { get; set; } = default!;
        public int? PersonId { get; set; }

        public Country Country { get; set; } = default!;
        public int? CountryId { get; set; } 
    }
}
