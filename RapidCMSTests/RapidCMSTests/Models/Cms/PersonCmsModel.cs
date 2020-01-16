using RapidCMS.Core.Abstractions.Data;
using RapidCMSTests.Entities;
using System.ComponentModel.DataAnnotations;

namespace RapidCMSTests.Models.Cms
{
    public class PersonCmsModel : IEntity
    {
        public string? Id { get; set; }

        [Required]
        public string Name { get; set; } = default!;

        public string? ParentId { get; set; }

        public static implicit operator Person(PersonCmsModel cms) => new Person
        {
            Id = int.TryParse(cms.Id, out var id) ? id : default,
            Name = cms.Name,
            ParentId = int.TryParse(cms.ParentId, out var parentId) ? parentId : default(int?)
        };

        public static implicit operator PersonCmsModel(Person db) => new PersonCmsModel
        {
            Id = db.Id.ToString(),
            Name = db.Name,
            ParentId = db.ParentId?.ToString()
        };
    }
}
