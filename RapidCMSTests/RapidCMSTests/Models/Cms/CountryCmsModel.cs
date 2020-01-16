using RapidCMS.Core.Abstractions.Data;
using RapidCMSTests.Entities;
using System.ComponentModel.DataAnnotations;

namespace RapidCMSTests.Models.Cms
{
    public class CountryCmsModel : IEntity
    {
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public static implicit operator Country(CountryCmsModel cms) => new Country
        {
            Id = int.TryParse(cms.Id, out var id) ? id : default,
            Name = cms.Name
        };

        public static implicit operator CountryCmsModel(Country db) => new CountryCmsModel
        {
            Id = db.Id.ToString(),
            Name = db.Name
        };
    }
}
