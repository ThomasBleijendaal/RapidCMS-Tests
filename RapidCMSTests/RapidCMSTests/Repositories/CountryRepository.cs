using RapidCMS.Core.Abstractions.Data;
using RapidCMS.Core.Repositories;
using RapidCMSTests.EFCore;
using RapidCMSTests.Entities;
using RapidCMSTests.Models.Cms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using RapidCMS.Core.Abstractions.Forms;

namespace RapidCMSTests.Repositories
{
    public class CountryRepository : MappedBaseRepository<CountryCmsModel, Country>
    {
        private readonly TestDbContext _dbContext;

        public CountryRepository(TestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task DeleteAsync(string id, IParent? parent)
        {
            int.TryParse(id, out var entityId);

            var country = await _dbContext.Countries.FirstOrDefaultAsync(x => x.Id == entityId);
            _dbContext.Countries.Remove(country);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<CountryCmsModel>> GetAllAsync(IParent? parent, IQuery<Country> query)
        {
            var queryable = _dbContext.Countries.AsQueryable();

            if (query.SearchTerm != null)
            {
                queryable = queryable.Where(x => x.Name.Contains(query.SearchTerm));
            }

            if (query.DataViewExpression != null)
            {
                queryable = queryable.Where(query.DataViewExpression);
            }

            queryable = query.ApplyOrder(queryable).Skip(query.Skip).Take(query.Take + 1);

            var results = await queryable.ToListAsync();

            query.HasMoreData(results.Count > query.Take);

            return results.Take(query.Take).Select(x => (CountryCmsModel)x);
        }

        public override async Task<CountryCmsModel?> GetByIdAsync(string id, IParent? parent)
        {
            int.TryParse(id, out var entityId);

            return await _dbContext.Countries.FirstOrDefaultAsync(x => x.Id == entityId);
        }

        public override Task<CountryCmsModel> NewAsync(IParent? parent, Type? variantType = null)
        {
            return Task.FromResult(new CountryCmsModel());
        }

        public override async Task<CountryCmsModel?> InsertAsync(IEditContext<CountryCmsModel> editContext)
        {
            var country = (Country)editContext.Entity;

            var entry = _dbContext.Countries.Add(country);
            await _dbContext.SaveChangesAsync();

            return entry.Entity;
        }

        public override async Task UpdateAsync(IEditContext<CountryCmsModel> editContext)
        {
            var country = (Country)editContext.Entity;

            var entity = await _dbContext.Countries.FirstOrDefaultAsync(x => x.Id == country.Id);

            entity.Name = country.Name;

            _dbContext.Countries.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public override async Task<IEnumerable<CountryCmsModel>?> GetAllRelatedAsync(IRelated related, IQuery<Country> query)
        {
            if (related.Entity is PersonCmsModel person)
            {
                return await GetRelatedToGivenPersonAsync(query, person, true);
            }

            throw new InvalidOperationException();
        }

        public override async Task<IEnumerable<CountryCmsModel>?> GetAllNonRelatedAsync(IRelated related, IQuery<Country> query)
        {
            if (related.Entity is PersonCmsModel person)
            {
                return await GetRelatedToGivenPersonAsync(query, person, false);
            }

            throw new InvalidOperationException();
        }

        private async Task<IEnumerable<CountryCmsModel>> GetRelatedToGivenPersonAsync(IQuery<Country> query, PersonCmsModel person, bool related)
        {
            var personId = int.TryParse(person.Id, out var id) ? id : default(int?);

            var queryable = _dbContext.Countries.Where(x => x.People.Any(x => x.PersonId == personId) == related);

            if (query.SearchTerm != null)
            {
                queryable = queryable.Where(x => x.Name.Contains(query.SearchTerm));
            }

            if (query.DataViewExpression != null)
            {
                queryable = queryable.Where(query.DataViewExpression);
            }

            queryable = query.ApplyOrder(queryable).Skip(query.Skip).Take(query.Take + 1);

            var results = await queryable.ToListAsync();

            query.HasMoreData(results.Count > query.Take);

            return results.Take(query.Take).Select(x => (CountryCmsModel)x);
        }
    }
}
