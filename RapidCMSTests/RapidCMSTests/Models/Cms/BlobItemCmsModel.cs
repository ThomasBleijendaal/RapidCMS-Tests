using System;
using RapidCMS.Core.Abstractions.Data;

namespace RapidCMSTests.Models.Cms
{
    public class BlobContainerCmsModel : IEntity
    {
        public string Name { get; set; } = default!;

        public string? Id { get => Name; set => Name = value ?? throw new InvalidOperationException(); }
    }

    public class BlobItemCmsModel : IEntity
    {
        public string Name { get; set; } = default!;

        public string? Id { get => Name; set => Name = value ?? throw new InvalidOperationException(); }
    }

    public class BlockBlobCmsModel : BlobItemCmsModel
    {

    }

    public class BlobDirectoryCmsModel : BlobItemCmsModel
    {

    }
}
