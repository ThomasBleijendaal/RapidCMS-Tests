using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Abstractions.Forms;
using RapidCMS.Core.Repositories;
using RapidCMSTests.Extensions;
using RapidCMSTests.Models.Cms;

namespace RapidCMSTests.Repositories
{
    public class BlobRepository : BaseRepository<BlobItemCmsModel>
    {
        private readonly CloudBlobClient _client;

        public BlobRepository(CloudStorageAccount account)
        {
            _client = account.CreateCloudBlobClient();
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            if (!(parent.GetRootParent()?.Entity is BlobContainerCmsModel container))
            {
                throw new InvalidOperationException();
            }

            await _client.GetContainerReference(container.Name).GetBlockBlobReference(id).DeleteIfExistsAsync();
        }

        public override Task<IEnumerable<BlobItemCmsModel>> GetAllAsync(IParent? parent, IQuery<BlobItemCmsModel> query)
        {
            if (!(parent.GetRootParent()?.Entity is BlobContainerCmsModel container))
            {
                throw new InvalidOperationException();
            }

            var parentBlob = parent?.Entity as BlobItemCmsModel;

            var blobs = _client.GetContainerReference(container.Name).ListBlobsSegmented(parentBlob?.Name, false, BlobListingDetails.None, default, default, default, default);

            var models = blobs.Results.Skip(query.Skip).Take(query.Take).Select(x => x switch
            {
                CloudBlockBlob blob => (BlobItemCmsModel)new BlobItemCmsModel { Name = blob.Name },
                _ => (BlobItemCmsModel)new BlobItemCmsModel { Name = x.Uri.AbsolutePath }
            }).ToList();

            return Task.FromResult(models.AsEnumerable());
        }

        public override Task<BlobItemCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            if (!(parent.GetRootParent()?.Entity is BlobContainerCmsModel container))
            {
                throw new InvalidOperationException();
            }

            var blob = _client.GetContainerReference(container.Name).GetBlockBlobReference(id);

            var model = (BlobItemCmsModel?)(blob == null ? default : new BlockBlobCmsModel { Name = blob.Name });

            return Task.FromResult(model);
        }

        public override async Task<BlobItemCmsModel?> InsertAsync(IEditContext<BlobItemCmsModel> editContext)
        {
            if (!(editContext.Parent?.GetRootParent()?.Entity is BlobContainerCmsModel container))
            {
                throw new InvalidOperationException();
            }

            var blob = _client.GetContainerReference(container.Name).GetBlockBlobReference(editContext.Entity.Id);

            await blob.UploadTextAsync("123");

            return new BlobItemCmsModel { Name = editContext.Entity.Id! };
        }

        public override Task<BlobItemCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new BlobItemCmsModel());
        }

        public override Task UpdateAsync(IEditContext<BlobItemCmsModel> editContext)
        {
            throw new InvalidOperationException();
        }
    }

    public class ContainerRepository : BaseRepository<BlobContainerCmsModel>
    {
        private readonly CloudBlobClient _client;

        public ContainerRepository(CloudStorageAccount account)
        {
            _client = account.CreateCloudBlobClient();
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            var container = _client.GetContainerReference(id);
            await container.DeleteIfExistsAsync();
        }

        public override Task<IEnumerable<BlobContainerCmsModel>> GetAllAsync(IParent? parent, IQuery<BlobContainerCmsModel> query)
        {
            var containers = _client.ListContainers(query.SearchTerm);
            return Task.FromResult(containers.Skip(query.Skip).Take(query.Take).Select(x => new BlobContainerCmsModel { Name = x.Name }));
        }

        public override async Task<BlobContainerCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            var container = _client.GetContainerReference(id);
            if (await container.ExistsAsync())
            {
                return new BlobContainerCmsModel { Name = container.Name };
            }
            else
            {
                return default;
            }
        }

        public override async Task<BlobContainerCmsModel?> InsertAsync(IEditContext<BlobContainerCmsModel> editContext)
        {
            await _client.GetContainerReference(editContext.Entity.Name).CreateAsync();
            return new BlobContainerCmsModel { Name = editContext.Entity.Name };
        }

        public override Task<BlobContainerCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new BlobContainerCmsModel());
        }

        public override Task UpdateAsync(IEditContext<BlobContainerCmsModel> editContext)
        {
            throw new InvalidOperationException();
        }
    }
}
